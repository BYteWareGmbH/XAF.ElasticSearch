namespace BYteWare.XAF.ElasticSearch
{
    using DevExpress.ExpressApp.Utils;
    using Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// ElasticSearch Mapping Helper class
    /// </summary>
    public class TypeMappingWriter
    {
        private readonly Type _type;

        /// <summary>
        /// ElasticSearch Type Name for the base class
        /// </summary>
        public string BaseTypeName
        {
            get;
            set;
        }

        private int MaxRecursion
        {
            get;
            set;
        }

        private string TypeName
        {
            get;
            set;
        }

        private ConcurrentDictionary<Type, int> SeenTypes
        {
            get;
            set;
        }

        private bool OptOut
        {
            get;
            set;
        }

        /// <summary>
        /// Initalizes a new instance of the <see cref="TypeMappingWriter"/> class.
        /// </summary>
        /// <param name="type">Type to create the mapping for</param>
        /// <param name="typeName">The ElasticSearch Type Name</param>
        /// <param name="maxRecursion">Maximum number of recursions for nested types</param>
        /// <param name="optOut">Was the type marked as opting out of ElasticSearch indexing</param>
        public TypeMappingWriter(Type type, string typeName, int maxRecursion, bool optOut)
        {
            _type = type;

            TypeName = typeName;
            BaseTypeName = typeName;
            MaxRecursion = maxRecursion;
            OptOut = optOut;

            SeenTypes = new ConcurrentDictionary<Type, int>();
            SeenTypes.TryAdd(type, 0);
        }

        /// <summary>
        /// Returns a json string with mapping settings for all properties of the type
        /// </summary>
        /// <returns>ElasticSearch json mapping string</returns>
        public string MapFromAttributes()
        {
            var sb = new StringBuilder();
            StringWriter sw = null;
            try
            {
                var strw = sw = new StringWriter(sb, CultureInfo.InvariantCulture);
                using (var jsonWriter = new JsonTextWriter(sw))
                {
                    sw = null;
                    jsonWriter.Formatting = Formatting.Indented;
                    jsonWriter.WriteStartObject();
                    {
                        jsonWriter.WritePropertyName(TypeName);
                        jsonWriter.WriteStartObject();
                        {
                            jsonWriter.WritePropertyName("properties");
                            jsonWriter.WriteStartObject();
                            {
                                WriteProperties(jsonWriter);
                            }
                            jsonWriter.WriteEnd();
                        }
                        jsonWriter.WriteEnd();
                    }
                    jsonWriter.WriteEndObject();
                    jsonWriter.Flush();
                    return strw.ToString();
                }
            }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                }
            }
        }

        /// <summary>
        /// Initalizes a new instance of the <see cref="TypeMappingWriter"/> class.
        /// internal constructor by TypeMappingWriter itself when it recurses, passes seenTypes as safeguard against maxRecursion
        /// </summary>
        /// <param name="t">Type to create the mapping for</param>
        /// <param name="typeName">The ElasticSearch Type Name</param>
        /// <param name="maxRecursion">Maximum number of recursions for nested types</param>
        /// <param name="optOut">Was the type marked as opting out of ElasticSearch indexing</param>
        /// <param name="baseTypeName">ElasticSearch Type Name for the base class</param>
        /// <param name="seenTypes">Already visited types</param>
        internal TypeMappingWriter(Type t, string typeName, int maxRecursion, bool optOut, string baseTypeName, ConcurrentDictionary<Type, int> seenTypes)
        {
            _type = BYteWareTypeInfo.GetUnderlyingType(t);

            TypeName = typeName;
            MaxRecursion = maxRecursion;
            OptOut = optOut;
            BaseTypeName = baseTypeName;
            SeenTypes = seenTypes;
        }

        /// <summary>
        /// Returns a JObject ElasticSearch mapping for a nested/object type
        /// </summary>
        /// <returns>JObject with the ElasticSearch mapping</returns>
        internal JObject MapPropertiesFromAttributes()
        {
            int seen;
            if (SeenTypes.TryGetValue(_type, out seen) && seen > MaxRecursion)
            {
                return JObject.Parse("{}");
            }

            var sb = new StringBuilder();
            StringWriter sw = null;
            try
            {
                var strw = sw = new StringWriter(sb, CultureInfo.InvariantCulture);
                using (var jsonWriter = new JsonTextWriter(sw))
                {
                    sw = null;
                    jsonWriter.WriteStartObject();
                    {
                        WriteProperties(jsonWriter);
                    }
                    jsonWriter.WriteEnd();
                    jsonWriter.Flush();
                    return JObject.Parse(strw.ToString());
                }
            }
            finally
            {
                if (sw != null)
                {
                    sw.Dispose();
                }
            }
        }

        /// <summary>
        /// Writes into jsonWriter mapping settings for all to be indexed properties
        /// </summary>
        /// <param name="jsonWriter">A JsonWriter instance</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = nameof(ElasticSearch))]
        internal void WriteProperties(JsonWriter jsonWriter)
        {
            var bti = BYteWareTypeInfo.GetBYteWareTypeInfo(_type);
            foreach (var p in bti.GetTopPropertyInfos)
            {
                var props = bti.ESProperties(p.Name);
                var type = ElasticSearchClient.GetElasticSearchType(props, p.PropertyType);
                var propertyName = ElasticSearchClient.FieldName(string.IsNullOrEmpty(props?.Name) ? p.Name : props.Name);
                if ((props != null && props.OptOut) || (props == null && OptOut) || (type == null))
                {
                    continue;
                }

                jsonWriter.WritePropertyName(propertyName);
                jsonWriter.WriteStartObject();
                {
                    if (props == null)
                    {
                        // properties that follow can not be inferred from the CLR.
#pragma warning disable CC0021 // Use nameof
                        jsonWriter.WritePropertyName("type");
#pragma warning restore CC0021 // Use nameof
                        jsonWriter.WriteValue(type);

                        // jsonWriter.WriteEnd();
                    }
                    else
                    {
                        WriteProperties(jsonWriter, bti, p, props, type);
                        var multiFields = BYteWareTypeInfo.Model != null ? (props as IModelMemberElasticSearchField)?.Fields : Attribute.GetCustomAttributes(p, typeof(ElasticMultiFieldAttribute), true).OfType<IElasticSearchFieldProperties>();
                        if (multiFields != null && multiFields.Any())
                        {
                            jsonWriter.WritePropertyName("fields");
                            jsonWriter.WriteStartObject();
                            foreach (var ga in multiFields.GroupBy(t => t.Name))
                            {
                                var a = ga.First();
                                jsonWriter.WritePropertyName(ElasticSearchClient.FieldName(ga.Key));
                                jsonWriter.WriteStartObject();
                                WriteProperties(jsonWriter, bti, p, a, ElasticSearchClient.GetElasticSearchType(a, p.PropertyType));
                                jsonWriter.WriteEndObject();
                            }
                            jsonWriter.WriteEndObject();
                        }
                    }
                    if (type == "object" || type == "nested")
                    {
                        var deepType = p.PropertyType;
                        var dbti = BYteWareTypeInfo.GetBYteWareTypeInfo(deepType);
                        var typeName = dbti.ESTypeName;
                        if (string.IsNullOrWhiteSpace(typeName))
                        {
                            typeName = deepType.Name.ToLowerInvariant();
                        }
                        var seenTypes = new ConcurrentDictionary<Type, int>(SeenTypes);
                        seenTypes.AddOrUpdate(deepType, 0, (t, i) => ++i);

                        var newTypeMappingWriter = new TypeMappingWriter(deepType, typeName, MaxRecursion, OptOut, BaseTypeName, seenTypes);
                        var nestedProperties = newTypeMappingWriter.MapPropertiesFromAttributes();

                        jsonWriter.WritePropertyName("properties");
                        nestedProperties.WriteTo(jsonWriter);
                    }
                }
                jsonWriter.WriteEnd();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = nameof(ElasticSearch))]
        private static void WriteProperties(JsonWriter jsonWriter, BYteWareTypeInfo bti, PropertyInfo propInfo, IElasticSearchFieldProperties props, string type)
        {
#pragma warning disable CC0021 // Use nameof
            var fieldProps = props as IElasticProperties;
            jsonWriter.WritePropertyName("type");
            jsonWriter.WriteValue(type);
            if (!string.IsNullOrEmpty(props.Analyzer))
            {
                jsonWriter.WritePropertyName("analyzer");
                jsonWriter.WriteValue(props.Analyzer);
            }
            if (props.Boost.HasValue)
            {
                jsonWriter.WritePropertyName("boost");
                jsonWriter.WriteValue(props.Boost.Value);
            }
            if (props.DocValues.HasValue)
            {
                jsonWriter.WritePropertyName("doc_values");
                jsonWriter.WriteValue(props.DocValues.Value);
            }
            if (props.EagerGlobalOrdinals.HasValue)
            {
                jsonWriter.WritePropertyName("eager_global_ordinals");
                jsonWriter.WriteValue(props.EagerGlobalOrdinals.Value);
            }
            if (props.FieldData.HasValue)
            {
                jsonWriter.WritePropertyName("fielddata");
                jsonWriter.WriteValue(props.FieldData.Value);
            }
            if (props.ESIndex.HasValue)
            {
                jsonWriter.WritePropertyName("index");
                jsonWriter.WriteValue(props.ESIndex.Value);
            }
            if (props.IndexOptions.HasValue)
            {
                jsonWriter.WritePropertyName("index_options");
                jsonWriter.WriteValue(Enum.GetName(typeof(IndexOptions), props.IndexOptions.Value));
            }
            if (props.Norms.HasValue)
            {
                jsonWriter.WritePropertyName("norms");
                jsonWriter.WriteValue(props.Norms.Value);
            }
            if (props.PositionOffsetGap.HasValue)
            {
                jsonWriter.WritePropertyName("position_offset_gap");
                jsonWriter.WriteValue(props.PositionOffsetGap.Value);
            }
            if (props.Store.HasValue)
            {
                jsonWriter.WritePropertyName("store");
                jsonWriter.WriteValue(props.Store.Value);
            }
            if (!string.IsNullOrEmpty(props.SearchAnalyzer))
            {
                jsonWriter.WritePropertyName("search_analyzer");
                jsonWriter.WriteValue(props.SearchAnalyzer);
            }
            if (!string.IsNullOrEmpty(props.SearchQuoteAnalyzer))
            {
                jsonWriter.WritePropertyName("search_quote_analyzer");
                jsonWriter.WriteValue(props.SearchQuoteAnalyzer);
            }
            if (!string.IsNullOrEmpty(props.Similarity))
            {
                jsonWriter.WritePropertyName("similarity");
                jsonWriter.WriteValue(props.Similarity);
            }
            if (props.TermVector.HasValue)
            {
                jsonWriter.WritePropertyName("term_vector");
                jsonWriter.WriteValue(Enum.GetName(typeof(TermVectorOption), props.TermVector.Value));
            }
            if (props.IgnoreAbove.HasValue)
            {
                jsonWriter.WritePropertyName("ignore_above");
                jsonWriter.WriteValue(props.IgnoreAbove.Value);
            }
            if (!string.IsNullOrEmpty(props.NullValue))
            {
                jsonWriter.WritePropertyName("null_value");
                jsonWriter.WriteValue(props.NullValue);
            }
            if (props.Coerce.HasValue)
            {
                jsonWriter.WritePropertyName("coerce");
                jsonWriter.WriteValue(props.Coerce.Value);
            }
            if (props.IgnoreMalformed.HasValue)
            {
                jsonWriter.WritePropertyName("ignore_malformed");
                jsonWriter.WriteValue(props.IgnoreMalformed.Value);
            }
            if (props.ScalingFactor.HasValue)
            {
                jsonWriter.WritePropertyName("scaling_factor");
                jsonWriter.WriteValue(props.ScalingFactor.Value);
            }
            if (!string.IsNullOrEmpty(props.DateFormat))
            {
                jsonWriter.WritePropertyName("format");
                jsonWriter.WriteValue(props.DateFormat);
            }
            if (!string.IsNullOrEmpty(props.Locale))
            {
                jsonWriter.WritePropertyName("locale");
                jsonWriter.WriteValue(props.Locale);
            }
            if (fieldProps != null && fieldProps.IncludeInAll.HasValue)
            {
                jsonWriter.WritePropertyName("include_in_all");
                jsonWriter.WriteValue(fieldProps.IncludeInAll.Value);
            }
            if (props.PreserveSeparators.HasValue)
            {
                jsonWriter.WritePropertyName("preserve_separators");
                jsonWriter.WriteValue(props.PreserveSeparators.Value);
            }
            if (props.PreservePositionIncrements.HasValue)
            {
                jsonWriter.WritePropertyName("preserve_position_increments");
                jsonWriter.WriteValue(props.PreservePositionIncrements.Value);
            }
            if (props.MaxInputLength.HasValue)
            {
                jsonWriter.WritePropertyName("max_input_length");
                jsonWriter.WriteValue(props.MaxInputLength.Value);
            }
            if (fieldProps != null && !string.IsNullOrEmpty(fieldProps.CopyTo))
            {
                jsonWriter.WritePropertyName("copy_to");
                jsonWriter.WriteStartArray();
                foreach (var item in fieldProps.CopyTo.Split(";, ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries))
                {
                    jsonWriter.WriteValue(item);
                }
                jsonWriter.WriteEndArray();
            }
            if (type == ElasticSearchClient.GetElasticSearchTypeFromFieldType(FieldType.completion))
            {
                var defType = ElasticSearchClient.GetFieldTypeFromType(propInfo.PropertyType);
                if (defType != FieldType.text || defType != FieldType.keyword)
                {
                    throw new ElasticIndexException(CaptionHelper.GetLocalizedText(ElasticSearchClient.IndexExceptionGroup, "SuggestNoString"));
                }
                var sf = bti.ESSuggestFields.FirstOrDefault(t => t.FieldName == ElasticSearchClient.FieldName(!string.IsNullOrWhiteSpace(props.Name) ? props.Name : propInfo.Name));
                jsonWriter.WritePropertyName("contexts");
                jsonWriter.WriteStartArray();

                jsonWriter.WriteStartObject();
                jsonWriter.WritePropertyName("name");
                jsonWriter.WriteValue(ElasticSearchClient.TypeContext);
                jsonWriter.WritePropertyName("type");
                jsonWriter.WriteValue(Enum.GetName(typeof(SuggestContextType), SuggestContextType.category));
                jsonWriter.WritePropertyName("path");
                jsonWriter.WriteValue(ElasticSearchClient.TypeContext);
                jsonWriter.WriteEndObject();

                foreach (var ca in sf.ContextSettings)
                {
                    jsonWriter.WriteStartObject();
                    jsonWriter.WritePropertyName("name");
                    jsonWriter.WriteValue(ca.ContextName);
                    jsonWriter.WritePropertyName("type");
                    jsonWriter.WriteValue(Enum.GetName(typeof(SuggestContextType), ca.ContextType));
                    if (!string.IsNullOrEmpty(ca.PathField))
                    {
                        jsonWriter.WritePropertyName("path");
                        jsonWriter.WriteValue(ca.PathField.ToLowerInvariant());
                    }
                    if (!string.IsNullOrEmpty(ca.Precision))
                    {
                        jsonWriter.WritePropertyName("precision");
                        jsonWriter.WriteValue(ca.Precision);
                    }
                    jsonWriter.WriteEndObject();
                }
                jsonWriter.WriteEndArray();
            }
#pragma warning restore CC0021 // Use nameof
        }
    }
}