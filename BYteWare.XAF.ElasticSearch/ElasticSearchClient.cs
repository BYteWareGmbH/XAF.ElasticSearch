namespace BYteWare.XAF.ElasticSearch
{
    using BusinessObjects;
    using BYteWare.Utils;
    using BYteWare.Utils.Extension;
    using DevExpress.Data.Filtering;
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.DC;
    using DevExpress.ExpressApp.Model;
    using DevExpress.ExpressApp.Security;
    using DevExpress.ExpressApp.Security.Strategy;
    using DevExpress.ExpressApp.Utils;
    using DevExpress.ExpressApp.Xpo;
    using DevExpress.Persistent.Base;
    using DevExpress.Xpo;
    using DevExpress.Xpo.DB.Exceptions;
    using DevExpress.Xpo.Metadata;
    using DevExpress.Xpo.Metadata.Helpers;
    using Elasticsearch.Net;
    using Fasterflect;
    using Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using Response;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// ElasticSearch XAF/XPO Client
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = nameof(ElasticSearch))]
    public class ElasticSearchClient
    {
        /// <summary>
        /// Default Value for MinimumShouldMatch
        /// </summary>
        public const string DefaultMinimumShouldMatch = "3<66%";

        /// <summary>
        /// Name of the Localization Group in the Model for Messages
        /// </summary>
        public const string MessageGroup = @"Messages\ElasticSearch";

        /// <summary>
        /// Name of the Localization Group in the Model for ElasticIndexExceptions
        /// </summary>
        public const string IndexExceptionGroup = @"Exceptions\ElasticIndexException";

        /// <summary>
        /// Name for the Lock on Checking for a necessary Reindex
        /// </summary>
        public const string ElasticSearchCheckNecessary = "ElasticSearchCheckNecessary";

        /// <summary>
        /// Name for the Lock on Reindexing
        /// </summary>
        public const string ElasticSearchRefresh = "ElasticSearchRefresh";

        /// <summary>
        /// Maximum Size for a Bulk Message
        /// </summary>
        public const int BulkSize = 1000000;

        /// <summary>
        /// Maximum number of attempts to refresh the status of an index
        /// </summary>
        public const int MaxAttemptsCounter = 7;

        private readonly ObservableCollection<Uri> elasticSearchNodes = new ObservableCollection<Uri>();
        private readonly Dictionary<string, string> parameters = new Dictionary<string, string>();
        private readonly Dictionary<XPObjectSpace, Changes> _RegisteredObjectSpaces = new Dictionary<XPObjectSpace, Changes>();
        private readonly Dictionary<UnitOfWork, Changes> _RegisteredUnitOfWork = new Dictionary<UnitOfWork, Changes>();

        private readonly Type elasticSearchIndexPersistentType;
        private readonly Type elasticSearchIndexRefreshPersistentType;
        private JsonSerializerSettings jsonSerializerSettings;
        private JsonSerializerSettings jsonDeserializerSettings;
        private ElasticLowLevelClient elasticClient;

        private class Changes
        {
            private readonly List<XPBaseObject> _ModifiedObjects;

            public Changes(IEnumerable<XPBaseObject> modified)
            {
                _ModifiedObjects = modified.ToList();
            }

            public ReadOnlyCollection<XPBaseObject> ModifiedObjects
            {
                get
                {
                    return _ModifiedObjects.AsReadOnly();
                }
            }
        }

        /// <summary>
        /// Default ElasticSearchClient Instance
        /// </summary>
        public static ElasticSearchClient Instance
        {
            get;
            set;
        }

        /// <summary>
        /// Returns the Field Name for ElasticSearch
        /// </summary>
        /// <param name="name">Name to convert</param>
        /// <returns>Elasticsearch Field Name</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = nameof(ElasticSearch))]
        public static string FieldName(string name)
        {
            return name?.ToLowerInvariant();
        }

        /// <summary>
        /// Infers the FieldType from the type of the property.
        /// </summary>
        /// <param name="propertyType">Type of the property</param>
        /// <returns>FieldType or null if can not be inferred</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = nameof(ElasticSearch))]
        public static FieldType GetFieldTypeFromType(Type propertyType)
        {
            propertyType = BYteWareTypeInfo.GetUnderlyingType(propertyType);

            if (propertyType == typeof(string))
            {
                return FieldType.keyword;
            }

            if (propertyType.IsValueType)
            {
                if (propertyType.IsEnum)
                {
                    return FieldType.keyword;
                }
                else
                {
                    switch (propertyType.Name)
                    {
                        case "Int32":
                        case "UInt16":
                            return FieldType.integer_type;
                        case "Int16":
                        case "Byte":
                            return FieldType.short_type;
                        case "SByte":
                            return FieldType.byte_type;
                        case "Int64":
                        case "UInt32":
                        case "TimeSpan":
                            return FieldType.long_type;
                        case "Single":
                            return FieldType.float_type;
                        case "Decimal":
                        case "Double":
                        case "UInt64":
                            return FieldType.double_type;
                        case "DateTime":
                        case "DateTimeOffset":
                            return FieldType.date_type;
                        case "Boolean":
                            return FieldType.boolean_type;
                        case "Char":
                        case "Guid":
                            return FieldType.keyword;
                    }
                }
            }
            else
            {
                return FieldType.object_type;
            }

            return FieldType.keyword;
        }

        /// <summary>
        /// Get the Elastic Search Field Type Related.
        /// </summary>
        /// <param name="props">ElasticSearch Field Properties</param>
        /// <param name="type">Property Type</param>
        /// <returns>String with the type name or null if can not be inferred</returns>
        public static string GetElasticSearchType(IElasticSearchFieldProperties props, Type type)
        {
            FieldType? fieldType = null;
            if (props != null)
            {
                fieldType = props.FieldType;
            }

            if (fieldType == null)
            {
                fieldType = GetFieldTypeFromType(type);
            }

            return GetElasticSearchTypeFromFieldType(fieldType);
        }

        /// <summary>
        /// Get the ElasticSearch Field Type from a FieldType.
        /// </summary>
        /// <param name="fieldType">FieldType value</param>
        /// <returns>String with the type name or null if can not be inferred</returns>
        public static string GetElasticSearchTypeFromFieldType(FieldType? fieldType)
        {
            if (fieldType.HasValue)
            {
                var s = Enum.GetName(typeof(FieldType), fieldType.Value);
#pragma warning disable CC0021 // Use nameof
                var posA = s.IndexOf("_type", StringComparison.CurrentCulture);
#pragma warning restore CC0021 // Use nameof
                if (posA == -1)
                {
                    return s;
                }
                return s.Substring(0, posA);
            }
            return null;
        }

        /// <summary>
        /// Does the type identified by xci partake in any ElasticSearch index
        /// </summary>
        /// <param name="xci">A XPClassInfo instance of the type to be tested</param>
        /// <returns>True if the type is part of any ElasticSearch index; otherwise False</returns>
        public static bool IsPartOfAnyESIndex(XPClassInfo xci)
        {
            if (xci == null)
            {
                throw new ArgumentNullException(nameof(xci));
            }
            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(xci.ClassType);
            return ci.IsPartOfAnyESIndex;
        }

        /// <summary>
        /// Returns all possible ElasticSearch Suggest Fields for the typeInfo
        /// </summary>
        /// <param name="typeInfo">The Type Info to search for all possible suggest fields</param>
        /// <returns>List of all names of possible ElasticSearch Suggest Fields</returns>
        [CLSCompliant(false)]
        public static IEnumerable<string> ElasticSearchSuggestFields(ITypeInfo typeInfo)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }
            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(typeInfo.Type);
            if (ci.ESSuggestFields != null)
            {
                return ci.ESSuggestFields.Select(t => t.FieldName);
            }
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Returns all possible Contexts for the Suggest Field of the typeInfo
        /// </summary>
        /// <param name="typeInfo">The Type Info to search for all possible contexts of the Suggest Field</param>
        /// <param name="field">The Suggest Field name</param>
        /// <returns>List of all names of possible contexts of the Suggest Field</returns>
        [CLSCompliant(false)]
        public static IEnumerable<string> ElasticSearchSuggestFieldContexts(ITypeInfo typeInfo, string field)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }
            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(typeInfo.Type);
            if (ci.ESSuggestFields != null && !string.IsNullOrEmpty(field))
            {
                var suggestField = ci.ESSuggestFields.FirstOrDefault(t => t.FieldName == field);
                if (suggestField != null && suggestField.ContextSettings != null)
                {
                    return new List<string>(suggestField.ContextSettings.Select(t => t.ContextName));
                }
            }
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Returns the parameter for the Suggest field's context of the typeInfo
        /// </summary>
        /// <param name="typeInfo">The Type Info to search for all possible contexts of the Suggest Field</param>
        /// <param name="field">The Suggest Field name</param>
        /// <param name="context">The Suggest Context name</param>
        /// <returns>The Parameter name of the Suggest field's context</returns>
        [CLSCompliant(false)]
        public static string ElasticSearchSuggestFieldContextParameter(ITypeInfo typeInfo, string field, string context)
        {
            if (typeInfo == null)
            {
                throw new ArgumentNullException(nameof(typeInfo));
            }
            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(typeInfo.Type);
            if (ci.ESSuggestFields != null && !string.IsNullOrEmpty(field))
            {
                var suggestField = ci.ESSuggestFields.FirstOrDefault(t => t.FieldName == field);
                if (suggestField != null && suggestField.ContextSettings != null)
                {
                    return suggestField.ContextSettings.Where(t => t.ContextName == context).Select(t => t.QueryParameter).FirstOrDefault();
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the json query string for the suggest API
        /// </summary>
        /// <param name="text">The text to search completion suggestions for</param>
        /// <param name="field">The field name in which to search</param>
        /// <param name="results">The maximum number of results to return</param>
        /// <param name="fuzzy">Are typos allowed</param>
        /// <param name="contexts">Context names and values to filter the results</param>
        /// <returns>a json query suggest string</returns>
        public static string SuggestCompletion(string text, string field, int results, bool fuzzy, IEnumerable<KeyValuePair<string, string>> contexts)
        {
#pragma warning disable CC0021 // Use nameof
            StringWriter sw = null;
            try
            {
                var strw = sw = new StringWriter(CultureInfo.InvariantCulture);
                using (var writer = new JsonTextWriter(sw))
                {
                    sw = null;
                    writer.WriteStartObject();
                    writer.WritePropertyName("text");
                    writer.WriteValue(text);

                    writer.WritePropertyName("completion");
                    writer.WriteStartObject();
                    writer.WritePropertyName("field");
                    writer.WriteValue(field);
                    writer.WritePropertyName("size");
                    writer.WriteValue(results);
                    if (fuzzy)
                    {
                        writer.WritePropertyName("fuzzy");
                        writer.WriteValue(true);
                    }
                    if (contexts != null && contexts.Any())
                    {
                        writer.WritePropertyName("contexts");
                        writer.WriteStartObject();
                        foreach (var context in contexts)
                        {
                            writer.WritePropertyName(context.Key);
                            writer.WriteRawValue(context.Value);
                        }
                        writer.WriteEndObject();
                    }
                    writer.WriteEndObject();

                    writer.WriteEndObject();
                    writer.Flush();
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
#pragma warning restore CC0021 // Use nameof
        }

        /// <summary>
        /// Returns the filter string defined through the Security System to use for the user and type
        /// </summary>
        /// <param name="user">The XAF Security System User</param>
        /// <param name="type">The XPO BusinessClass Type</param>
        /// <returns>The ElasticSearch Filter string</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = nameof(ElasticSearch))]
        public static string SecurityFilter(object user, Type type)
        {
            if (user is ISecurityUserWithRoles u && type != null)
            {
                var noFilter = false;
                var filters = new HashSet<string>();
                if (SecuritySystem.Instance is IRoleTypeProvider security)
                {
                    if (typeof(SecuritySystemRole).IsAssignableFrom(security.RoleType))
                    {
                        foreach (var role in u.Roles.OfType<SecuritySystemRole>().SelectMany(t => t.DescendantsAndSelf(r => r.ChildRoles)))
                        {
                            var typePermission = role.FindTypePermissionObject(type);
                            if (role.IsAdministrative || (typePermission != null && typePermission.AllowRead))
                            {
                                noFilter = true;
                                break;
                            }
                            else
                            {
                                if (typePermission != null)
                                {
                                    foreach (var objectPermission in typePermission.ObjectPermissions.Where(t => t.AllowRead))
                                    {
                                        var esFilter = objectPermission as IObjectPermissionElasticSearchFilter;
                                        if (esFilter == null || string.IsNullOrWhiteSpace(esFilter.ElasticSearchFilter))
                                        {
                                            noFilter = true;
                                            break;
                                        }
                                        else
                                        {
                                            filters.Add(esFilter.ElasticSearchFilter);
                                        }
                                    }
                                    if (noFilter)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (typeof(IPermissionPolicyRole).IsAssignableFrom(security.RoleType))
                    {
                        foreach (var role in u.Roles.OfType<IPermissionPolicyRole>())
                        {
                            var typePermission = role.FindFirstTypePermission(type);
                            if (role.IsAdministrative ||
                                ((typePermission == null || !typePermission.ReadState.HasValue) && role.PermissionPolicy.In(SecurityPermissionPolicy.AllowAllByDefault, SecurityPermissionPolicy.ReadOnlyAllByDefault)) ||
                                (typePermission != null && typePermission.ReadState.HasValue && typePermission.ReadState.Value == SecurityPermissionState.Allow))
                            {
                                noFilter = true;
                                break;
                            }
                            else
                            {
                                if (typePermission != null)
                                {
                                    // TODO: Deny Rule with MustNot; and respect ServerPermissionPolicyRequestProcessor.AllowPermissionPriorityInSameRole and AllowPermissionPriorityDifferentRole
                                    foreach (var objectPermission in typePermission.ObjectPermissions.Where(t => t.ReadState.HasValue && t.ReadState.Value == SecurityPermissionState.Allow))
                                    {
                                        var esFilter = objectPermission as IObjectPermissionElasticSearchFilter;
                                        if (esFilter == null || string.IsNullOrWhiteSpace(esFilter.ElasticSearchFilter))
                                        {
                                            noFilter = true;
                                            break;
                                        }
                                        else
                                        {
                                            filters.Add(esFilter.ElasticSearchFilter);
                                        }
                                    }
                                    if (noFilter)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                if (!noFilter && filters.Any())
                {
                    if (filters.Count > 1)
                    {
                        StringWriter sw = null;
                        try
                        {
                            var strw = sw = new StringWriter(CultureInfo.InvariantCulture);
                            using (var writer = new JsonTextWriter(sw))
                            {
                                sw = null;
                                writer.WritePropertyName("bool");
                                writer.WriteStartObject();
                                writer.WritePropertyName("should");
                                writer.WriteStartArray();
                                foreach (var filter in filters)
                                {
                                    writer.WriteStartObject();
                                    writer.WriteRaw(filter);
                                    writer.WriteEnd();
                                }
                                writer.WriteEndArray();
                                writer.WriteEnd();
                                writer.Flush();
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
                    else
                    {
                        return filters.First();
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Returns all possible ElasticSearch Fields to search on for the type
        /// </summary>
        /// <param name="type">The Type to search for all possible fields</param>
        /// <param name="wildcards">True if wildcard possibilities should also be returned</param>
        /// <returns>List of all names of possible ElasticSearch Fields</returns>
        [CLSCompliant(false)]
        public static IEnumerable<string> ElasticSearchFields(Type type, bool wildcards)
        {
            if (type != null)
            {
                var bti = BYteWareTypeInfo.GetBYteWareTypeInfo(type);
                return bti.ESFields(wildcards);
            }
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// Prepare the search text to send it to ElasticSearch
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        /// <param name="fuzzy">Was a fuzzy search requested</param>
        /// <param name="wildcard">Was a wildcard search requested</param>
        /// <returns>Special characters and words escaped search text</returns>
        public static string PrepareSearchText(string searchText, out bool fuzzy, out bool wildcard)
        {
            if (searchText == null)
            {
                throw new ArgumentNullException(nameof(searchText));
            }
            fuzzy = searchText.Contains('~');
            wildcard = searchText.Contains('*') || searchText.Contains('?') || searchText.Contains('!');
            if (fuzzy && !wildcard)
            {
                searchText = searchText.Replace("~", string.Empty);
            }
            else if (wildcard)
            {
                searchText = searchText.Replace(" AND ", " and ");
                searchText = searchText.Replace(" OR ", " or ");
                searchText = searchText.Replace(" NOT ", " not ");
                var sb = new StringBuilder();
                foreach (var c in searchText)
                {
                    char[] specialChars =
                    {
                                     '+',
                                     '-',
                                     '&',
                                     '|',
                                     '(',
                                     ')',
                                     '{',
                                     '}',
                                     '[',
                                     ']',
                                     '^',
                                     ':',
                                     '\\',
                                     '/'
                                };
                    if (specialChars.Contains(c))
                    {
                        sb.Append('\\');
                    }
                    sb.Append(c);
                }
                searchText = sb.ToString();
            }
            return searchText;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticSearchClient"/> class.
        /// </summary>
        /// <param name="indexPersistentType">BusinessClass Type to store the state of ElasticSearch indexes</param>
        /// <param name="indexRefreshPersistentType">BusinessClass Type to store if an ElasticSearch index should be refreshed</param>
        public ElasticSearchClient(Type indexPersistentType, Type indexRefreshPersistentType)
        {
            elasticSearchIndexPersistentType = indexPersistentType;
            elasticSearchIndexRefreshPersistentType = indexRefreshPersistentType;
            elasticSearchNodes.CollectionChanged += ElasticSearchNodes_CollectionChanged;
        }

        /// <summary>
        /// Prefix to use for all ElasticSearch index names
        /// </summary>
        public string ElasticSearchIndexPrefix
        {
            get;
            set;
        }
        = string.Empty;

        /// <summary>
        /// Use Async Methods if possible
        /// </summary>
        public bool UseAsync
        {
            get;
            set;
        }
        = true;

        /// <summary>
        /// The list of Uri's of the ElasticSearch nodes to use
        /// </summary>
        public ObservableCollection<Uri> ElasticSearchNodes
        {
            get
            {
                return elasticSearchNodes;
            }
        }

        /// <summary>
        /// The ElasticSearch.Net low level client instance
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Objekte verwerfen, bevor Bereich verloren geht", Justification = nameof(ElasticSearch))]
        [CLSCompliant(false)]
        public ElasticLowLevelClient ElasticLowLevelClient
        {
            get
            {
                if (elasticClient == null && ElasticSearchNodes.Count > 0)
                {
                    try
                    {
                        elasticClient = new ElasticLowLevelClient(new ConnectionConfiguration(new SniffingConnectionPool(ElasticSearchNodes)));
                    }
                    catch (ElasticsearchClientException e)
                    {
                        Tracing.Tracer.LogError(e);
                        elasticClient = null;
                    }
                }
                return elasticClient;
            }
        }

        /// <summary>
        /// Adds a new Parameter with value for usage in Queries
        /// </summary>
        /// <param name="parameter">Name of the Parameter</param>
        /// <param name="value">Parameter Value</param>
        public void AddParameter(string parameter, string value)
        {
            parameters[parameter] = value;
        }

        /// <summary>
        /// Removes a Parameter
        /// </summary>
        /// <param name="parameter">Name of the Parameter</param>
        /// <returns>True if the Parameter could be deleted; otherwise False</returns>
        public bool RemoveParameter(string parameter)
        {
            return parameters.Remove(parameter);
        }

        /// <summary>
        /// Returns the value of the Parameter named parameter
        /// </summary>
        /// <param name="parameter">Name of the Parameter</param>
        /// <returns>Assigned Value of the parameter or null if parameter could not be found</returns>
        public string GetParameterValue(string parameter)
        {
            parameters.TryGetValue(parameter, out string s);
            return ParameterContent(s);
        }

        /// <summary>
        /// List of all Parameter Names
        /// </summary>
        public ICollection<string> ParameterNames
        {
            get
            {
                return parameters.Keys;
            }
        }

        /// <summary>
        /// Json.Net Settings for Serializing instances
        /// </summary>
        public JsonSerializerSettings JsonSerializerSettings
        {
            get
            {
                if (jsonSerializerSettings == null)
                {
                    jsonSerializerSettings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        ContractResolver = new ElasticSearchContractResolver(),
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    jsonSerializerSettings.Converters.Add(new StringEnumConverter());
                }
                return jsonSerializerSettings;
            }
        }

        /// <summary>
        /// Json.Net Settings for Deserializing Results
        /// </summary>
        public JsonSerializerSettings JsonDeserializerSettings
        {
            get
            {
                if (jsonDeserializerSettings == null)
                {
                    jsonDeserializerSettings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        MissingMemberHandling = MissingMemberHandling.Ignore,
                        NullValueHandling = NullValueHandling.Ignore
                    };
                }
                return jsonDeserializerSettings;
            }
        }

        /// <summary>
        /// Registers the ObjectSpace to send the changes on commit to ElasticSearch
        /// </summary>
        /// <param name="objectSpace">A XPObjectSpace instance to register</param>
        [CLSCompliant(false)]
        public void RegisterObjectSpace(XPObjectSpace objectSpace)
        {
            if (objectSpace != null && !(objectSpace is INestedObjectSpace))
            {
                _RegisteredObjectSpaces[objectSpace] = null;
                objectSpace.Committing += ObjectSpace_Committing;
                objectSpace.Committed += ObjectSpace_Committed;
                objectSpace.Disposed += ObjectSpace_Disposed;
            }
        }

        /// <summary>
        /// Unregisters an ObjectSpace to end sending changes on commit to ElasticSearch
        /// </summary>
        /// <param name="objectSpace">A XPObjectSpace instance to unregister</param>
        [CLSCompliant(false)]
        public void UnregisterObjectSpace(XPObjectSpace objectSpace)
        {
            if (objectSpace != null)
            {
                _RegisteredObjectSpaces.Remove(objectSpace);
                objectSpace.Disposed -= ObjectSpace_Disposed;
                objectSpace.Committed -= ObjectSpace_Committed;
                objectSpace.Committing -= ObjectSpace_Committing;
            }
        }

        /// <summary>
        /// Registers the UnitOfWork to send the changes on commit to ElasticSearch
        /// </summary>
        /// <param name="uow">A XPO UnitOfWork instance to register</param>
        [CLSCompliant(false)]
        public void RegisterUnitOfWork(UnitOfWork uow)
        {
            if (uow != null && !(uow is NestedUnitOfWork))
            {
                _RegisteredUnitOfWork[uow] = null;
                uow.BeforeFlushChanges += Uow_BeforeFlushChanges;
                uow.AfterFlushChanges += Uow_AfterFlushChanges;
                uow.Disposed += Uow_Disposed;
            }
        }

        /// <summary>
        /// Unregisters an UnitOfWork to end sending changes on commit to ElasticSearch
        /// </summary>
        /// <param name="uow">A XPO UnitOfWork instance to unregister</param>
        [CLSCompliant(false)]
        public void UnregisterUnitOfWork(UnitOfWork uow)
        {
            if (uow != null)
            {
                _RegisteredUnitOfWork.Remove(uow);
                uow.Disposed -= Uow_Disposed;
                uow.AfterFlushChanges -= Uow_AfterFlushChanges;
                uow.BeforeFlushChanges -= Uow_BeforeFlushChanges;
            }
        }

        /// <summary>
        /// Index Name for ElasticSearch Server
        /// </summary>
        /// <param name="name">Name of the Index</param>
        /// <returns>Converted Index Name</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = nameof(Elasticsearch))]
        public string IndexName(string name)
        {
            return "{0}{1}".FormatWith(ElasticSearchIndexPrefix ?? string.Empty, name).ToLowerInvariant();
        }

        /// <summary>
        /// Type Name for ElasticSearch Server
        /// </summary>
        /// <param name="name">Name of the Type</param>
        /// <returns>Converted Type Name</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = nameof(Elasticsearch))]
        public string TypeName(string name)
        {
            return name?.ToLowerInvariant();
        }

        /// <summary>
        /// Sends an Index Request for BusinessObject bo
        /// </summary>
        /// <param name="bo">XPO BusinessClass Instance</param>
        public void DoIndex(XPBaseObject bo)
        {
            if (bo == null)
            {
                throw new ArgumentNullException(nameof(bo));
            }
            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(bo.GetType());
            if (ci != null && ci.IsESIndexed && ElasticLowLevelClient != null)
            {
                var version = ci.GetVersion(bo);
                if (UseAsync)
                {
                    DoIndexAsync(bo, ci, SerializeObject(bo), version).Forget();
                }
                else
                {
                    DoIndexSync(bo, ci, SerializeObject(bo), version);
                }
            }
        }

        /// <summary>
        /// Returns a string with the serialized values of bo for usage in a bulk Request
        /// </summary>
        /// <param name="session">XPO Session for the object</param>
        /// <param name="bo">XPO BusinessClass Instance</param>
        /// <returns>Json string for bulk Request</returns>
        public string SerializeObjectForBulk(Session session, XPBaseObject bo)
        {
            if (bo == null)
            {
                throw new ArgumentNullException(nameof(bo));
            }
            return SerializeObjectForBulk(session, bo, BYteWareTypeInfo.GetBYteWareTypeInfo(bo.GetType()));
        }

        /// <summary>
        /// Returns a string with the serialized values of bo for usage in a bulk Request
        /// </summary>
        /// <param name="session">XPO Session for the object</param>
        /// <param name="bo">XPO BusinessClass Instance</param>
        /// <param name="ci">A BYteWareTypeInfo instance of the type of bo</param>
        /// <returns>Json string for bulk Request</returns>
        public string SerializeObjectForBulk(Session session, XPBaseObject bo, BYteWareTypeInfo ci)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            if (bo != null && ci != null && ci.IsESIndexed)
            {
                StringWriter sw = null;
                try
                {
                    var strw = sw = new StringWriter(CultureInfo.InvariantCulture);
                    using (var writer = new JsonTextWriter(sw))
                    {
                        sw = null;
                        writer.WriteStartObject();
                        if (bo.IsDeleted)
                        {
                            writer.WritePropertyName("delete");
                        }
                        else
                        {
                            writer.WritePropertyName("index");
                        }

                        writer.WriteStartObject();
                        writer.WritePropertyName("_index");
                        writer.WriteValue(IndexName(ci.ESIndexName));
                        writer.WritePropertyName("_type");
                        writer.WriteValue(TypeName(ci.ESTypeName));
                        writer.WritePropertyName("_version_type");
                        writer.WriteValue("external_gte");
                        writer.WritePropertyName("_version");
                        writer.WriteValue(ci.GetVersion(bo));
                        writer.WritePropertyName("_id");
                        writer.WriteValue(session.GetKeyValue(bo).ToString());
                        writer.WriteEnd();

                        writer.WriteEndObject();
                        writer.Flush();
                        strw.WriteLine();
                        if (!bo.IsDeleted)
                        {
                            strw.WriteLine(SerializeObject(bo));
                        }
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
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sends a Delete Request for BusinessObject bo
        /// </summary>
        /// <param name="bo">XPO BusinessClass Instance</param>
        public void DoIndexDelete(XPBaseObject bo)
        {
            if (bo == null)
            {
                throw new ArgumentNullException(nameof(bo));
            }
            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(bo.GetType());
            if (ci != null && ci.IsESIndexed && ElasticLowLevelClient != null)
            {
                var version = ci.GetVersion(bo);
                if (UseAsync)
                {
                    DoIndexDeleteAsync(bo, ci, version).Forget();
                }
                else
                {
                    DoIndexDeleteSync(bo, ci, version);
                }
            }
        }

        /// <summary>
        /// Saves the ElasticSearch Type mapping for bo
        /// </summary>
        /// <param name="bo">XPO BusinessClass Instance</param>
        /// <returns>True when the mapping could be saved or was already existing; otherwise False</returns>
        public bool DoMapping(PersistentBase bo)
        {
            if (bo == null)
            {
                throw new ArgumentNullException(nameof(bo));
            }
            return DoMapping(BYteWareTypeInfo.GetBYteWareTypeInfo(bo.GetType()), bo.Session);
        }

        /// <summary>
        /// Saves the ElasticSearch Type mapping for bo
        /// </summary>
        /// <param name="ci">A BYteWareTypeInfo instance of the type where the mapping should be created for</param>
        /// <param name="session">A XPO session</param>
        /// <returns>True when the mapping could be saved or was already existing; otherwise False</returns>
        public bool DoMapping(BYteWareTypeInfo ci, Session session)
        {
            if (ci == null)
            {
                throw new ArgumentNullException(nameof(ci));
            }
            var mapping = false;
            if (!ci.ElasticIndexError)
            {
                try
                {
                    mapping = ElasticSearchMapping(ci);
                }
                finally
                {
                    if (!mapping && session != null)
                    {
                        ci.ElasticIndexError = true;
                        if (ci.IsESIndexed && !string.IsNullOrWhiteSpace(ci.ESIndexName))
                        {
                            IndexChange(ci.ESIndexName, session, false);
                        }
                    }
                }
            }
            return mapping;
        }

        /// <summary>
        /// Indexes bo and all related instances which contain or reference values from bo in their index
        /// </summary>
        /// <param name="bo">XPO BusinessClass Instance</param>
        /// <param name="propertyChanged">True if a indexed property was changed; False otherwise</param>
        public void IndexOnSaved(XPBaseObject bo, bool propertyChanged)
        {
            if (bo != null)
            {
                var bulk = new StringBuilder();
                var typeInfos = new HashSet<BYteWareTypeInfo>();
                DoIndexOnSaved(bo, new HashSet<object>(), bulk, typeInfos, propertyChanged);
                BulkIndex(bulk, bo.Session, typeInfos);
            }
        }

        /// <summary>
        /// Indexes instances (and all related instances which contain or reference values of) with key values in List keyList of Type described by xci
        /// </summary>
        /// <param name="session">A XPO Session</param>
        /// <param name="xci">A XPClassInfo instance describing the type to index</param>
        /// <param name="keyList">List of key values of instances to index</param>
        /// <param name="indexed">List of already indexed instances</param>
        /// <param name="progress">Progress Callback</param>
        public void IndexList(Session session, XPClassInfo xci, IReadOnlyCollection<object> keyList, Dictionary<XPClassInfo, HashSet<object>> indexed, Action<IWorkerProgress> progress)
        {
            IndexList(session, xci, keyList, indexed, null, progress);
        }

        /// <summary>
        /// Indexes instances (and all related instances which contain or reference values of) with key values in List keyList of Type described by xci
        /// </summary>
        /// <param name="session">A XPO Session</param>
        /// <param name="xci">A XPClassInfo instance describing the type to index</param>
        /// <param name="keyList">List of key values of instances to index</param>
        /// <param name="indexed">List of already indexed instances</param>
        /// <param name="memberInfo">A Member Info to filter for containing instances instead of key values</param>
        /// <param name="progress">Progress Callback</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = nameof(ElasticSearch))]
        [CLSCompliant(false)]
        public void IndexList(Session session, XPClassInfo xci, IReadOnlyCollection<object> keyList, Dictionary<XPClassInfo, HashSet<object>> indexed, IMemberInfo memberInfo, Action<IWorkerProgress> progress)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            if (keyList == null)
            {
                throw new ArgumentNullException(nameof(keyList));
            }
            if (indexed == null)
            {
                indexed = new Dictionary<XPClassInfo, HashSet<object>>();
            }
            if (!indexed.ContainsKey(xci))
            {
                indexed.Add(xci, new HashSet<object>());
            }
            var bulk = new StringBuilder();
            var typeProgress = new WorkerProgress
            {
                Name = string.Format(CultureInfo.CurrentCulture, CaptionHelper.GetLocalizedText(MessageGroup, "IndexListProgress"), xci.TableName, memberInfo == null ? string.Empty : memberInfo.Name),
                Maximum = keyList.Count
            };
            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(xci.ClassType);
            var typeLists = new Dictionary<ContainingType, HashSet<object>>();
            var typeInfos = new HashSet<BYteWareTypeInfo>();
            PrepareTypes(session, typeInfos, ci, typeLists);
            try
            {
                foreach (var keys in keyList.Where(t => memberInfo != null || !indexed[xci].Contains(t)).Chunk(2000))
                {
                    progress?.Invoke(typeProgress);
                    CriteriaOperator crit = new InOperator(xci.KeyProperty.Name, keys);
                    if (memberInfo != null)
                    {
                        if (memberInfo.ListElementTypeInfo != null)
                        {
                            crit = new ContainsOperator(memberInfo.Name, new InOperator(memberInfo.ListElementTypeInfo.KeyMember.Name, keys));
                        }
                        else
                        {
                            if (memberInfo.MemberTypeInfo?.KeyMember?.Name == null)
                            {
                                throw new ElasticIndexException(string.Format(CultureInfo.InvariantCulture, CaptionHelper.GetLocalizedText(IndexExceptionGroup, "MissingKeyMemberName"), memberInfo.Name, memberInfo.Owner.Name));
                            }
                            crit = new InOperator(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", memberInfo.Name, memberInfo.MemberTypeInfo.KeyMember.Name), keys);
                        }
                    }
                    var cursor = new XPCursor(session, xci, crit)
                    {
                        PageSize = 100
                    };
                    foreach (XPBaseObject bo in cursor)
                    {
                        BulkIndex(session, indexed, bulk, typeInfos, ci, typeLists, bo);
                    }
                    typeProgress.Position += 2000;
                }
                typeProgress.Position = typeProgress.Maximum;
                progress?.Invoke(typeProgress);
                BulkIndex(bulk, session, typeInfos);
            }
            finally
            {
                if (progress != null)
                {
                    typeProgress.Position = -1;
                    progress(typeProgress);
                }
            }
            foreach (var tl in typeLists.Where(t => t.Value.Any()))
            {
                IndexList(session, tl.Key.BYteWareType.ClassInfo, tl.Value.AsReadOnly(), indexed, tl.Key.MemberInfo, progress);
            }
        }

        /// <summary>
        /// Calls the ElasticSearch bulk api with the content of sb
        /// </summary>
        /// <param name="sb">A StringBuilder instance where the content will be sent to the BulkIndex API</param>
        /// <param name="session">A XPO Session</param>
        /// <param name="typeInfos">A List of BYteWareTypeInfo's where instances of these types are to be indexed</param>
        public void BulkIndex(StringBuilder sb, Session session, HashSet<BYteWareTypeInfo> typeInfos)
        {
            if (sb == null)
            {
                throw new ArgumentNullException(nameof(sb));
            }
            var s = sb.ToString();
            if (!string.IsNullOrWhiteSpace(s))
            {
                if (UseAsync)
                {
                    DoBulkAsync(s, session, typeInfos).Forget();
                }
                else
                {
                    DoBulkSync(s, session, typeInfos);
                }
            }
        }

        /// <summary>
        /// Test if searching with ElasticSearch is available for Type identified by ti
        /// </summary>
        /// <param name="ti">The Type Info to test if searching with ElasticSearch is available</param>
        /// <returns>True if searching with ElasticSearch is available; False otherwise</returns>
        [CLSCompliant(false)]
        public bool IsElasticSearchAvailable(ITypeInfo ti)
        {
            if (ti != null && ti.Type != null && ElasticLowLevelClient != null)
            {
                var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(ti.Type);
                if (ci.IsESIndexed && ci.ESIndexes.HasValues() && ci.ESTypes.HasValues())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Search in indexes and types with the query string body
        /// </summary>
        /// <param name="indexes">Enumeration of index names</param>
        /// <param name="types">Enumeration of type names</param>
        /// <param name="body">The query string in ElasticSearch Query DSL</param>
        /// <returns>A HitsMetaData instance with the results of the query</returns>
        public HitsMetaData Search(IEnumerable<string> indexes, IEnumerable<string> types, string body)
        {
            HitsMetaData hits = null;
            var ec = ElasticLowLevelClient;
            if (ec != null)
            {
                var res = ec.Search<DynamicResponse>(string.Join(",", indexes.Select(IndexName)), string.Join(",", types.Select(TypeName)), body);
                if (res.Success)
                {
#pragma warning disable CC0021 // Use nameof
                    string json = res.Body["hits"];
#pragma warning restore CC0021 // Use nameof
                    hits = JsonConvert.DeserializeObject<HitsMetaData>(json, JsonDeserializerSettings);
                }
            }
            return hits;
        }

        /// <summary>
        /// Search for instances of type described by ci
        /// </summary>
        /// <param name="ci">A BYteWareTypeInfo instance of the type where the query should be issued for</param>
        /// <param name="searchText">The text to search for</param>
        /// <param name="fields">A semicolon separated List of Elasticsearch Field Names to search in</param>
        /// <param name="results">Amount of results to return at maximum</param>
        /// <param name="filter">Restrict the results to this filter string</param>
        /// <returns>A HitsMetaData instance with the results of the query</returns>
        public HitsMetaData Search(BYteWareTypeInfo ci, string searchText, string fields, int results, string filter)
        {
            if (ci == null)
            {
                throw new ArgumentNullException(nameof(ci));
            }
            searchText = PrepareSearchText(searchText, out bool fuzzy, out bool wildcard);
            return Search(ci.ESIndexes, ci.ESTypes, SearchBody(searchText, results, fuzzy, wildcard, filter, ci.ESSecurityFilter, null, fields, false));
        }

        /// <summary>
        /// Replaces all occurrences of '@{ParameterName}' with the value of the Parameter
        /// </summary>
        /// <param name="filter">filter string with optional parameters</param>
        /// <returns>The string filter with all parameters replaced with their values</returns>
        public string ReplaceParameters(string filter)
        {
            foreach (var param in parameters)
            {
                filter = filter.Replace(string.Format(CultureInfo.InvariantCulture, @"'@{0}'", param.Key), ParameterContent(param.Value));
            }
            return filter;
        }

        /// <summary>
        /// Returns a query string in ElasticSearch Query DSL for the given parameters to use with the Search method
        /// </summary>
        /// <param name="searchText">The text to search for</param>
        /// <param name="results">Amount of results to return at maximum</param>
        /// <param name="fuzzy">Should the search be fuzzy</param>
        /// <param name="wildcard">Should the search support wildcards in the searchText</param>
        /// <param name="filter">Restrict the results to this filter string</param>
        /// <param name="securityFilter">Filter string defined through the Security System</param>
        /// <param name="modelItem">XAF Model Settings for the search</param>
        /// <returns>A query string in ElasticSearch Query DSL</returns>
        [CLSCompliant(false)]
        public string SearchBody(string searchText, int results, bool fuzzy, bool wildcard, string filter, string securityFilter, IModelElasticSearchFieldsItem modelItem)
        {
            return SearchBody(searchText, results, fuzzy, wildcard, filter, securityFilter, modelItem, null, false);
        }

        /// <summary>
        /// Returns a query string in ElasticSearch Query DSL for the given parameters to use with the Search method
        /// </summary>
        /// <param name="searchText">The text to search for</param>
        /// <param name="fuzzy">Should the search be fuzzy</param>
        /// <param name="wildcard">Should the search support wildcards in the searchText</param>
        /// <param name="filter">Restrict the results to this filter string</param>
        /// <param name="validate">True if the there should be only a check for correct syntax of the query</param>
        /// <returns>A query string in ElasticSearch Query DSL</returns>
        [CLSCompliant(false)]
        public string SearchBody(string searchText, bool fuzzy, bool wildcard, string filter, bool validate)
        {
            return SearchBody(searchText, 200, fuzzy, wildcard, filter, null, null, null, validate);
        }

        /// <summary>
        /// Checks if the provided filter string is valid for the Type with ITypeInfo ti
        /// </summary>
        /// <param name="ti">The Type Info to test if searching with ElasticSearch is available</param>
        /// <param name="filter">The filter string to test</param>
        /// <returns>The error messages; empty array if valid</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = nameof(ElasticSearch))]
        [CLSCompliant(false)]
        public string[] ValidateFilter(ITypeInfo ti, string filter)
        {
            if (ti == null)
            {
                throw new ArgumentNullException(nameof(ti));
            }
            var res = new List<string>();
            if (IsElasticSearchAvailable(ti))
            {
                try
                {
                    JObject.Parse(string.Format(CultureInfo.InvariantCulture, "{{{0}}}", filter));
                    var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(ti.Type);
                    var explanations = ElasticLowLevelClient.IndicesValidateQuery<DynamicResponse>(string.Join(",", ci.ESIndexes.Select(IndexName)), string.Join(",", ci.ESTypes.Select(TypeName)), SearchBody("*", false, true, filter, true), t => t.Explain(true));
                    if (explanations.Success)
                    {
                        dynamic response = explanations.Body;
                        if (!response["valid"])
                        {
#pragma warning disable CC0021 // Use nameof
                            foreach (var explanation in response["explanations"])
#pragma warning restore CC0021 // Use nameof
                            {
                                if (!explanation["valid"])
                                {
                                    res.Add(string.Format("{0}: {1}", explanation["index"], explanation["error"]));
                                }
                            }
                        }
                    }
                    else
                    {
                        res.Add(explanations.ServerError?.ToString());
                    }
                }
                catch (JsonReaderException e)
                {
                    res.Add(e.Message);
                }
                catch (SocketException e)
                {
                    res.Add(e.Message);
                }
                catch (WebException e)
                {
                    res.Add(e.Message);
                }
                catch (ElasticsearchClientException e)
                {
                    res.Add(e.Message);
                }
            }
            else
            {
                res.Add(CaptionHelper.GetLocalizedText(MessageGroup, "ValidationWrongType"));
            }
            return res.ToArray();
        }

        /// <summary>
        /// Refreshes all indexes which don't exist, are marked as inactive or to be refreshed
        /// </summary>
        /// <param name="application">A XafApplication instance</param>
        /// <param name="progress">Optional Progress Callback method</param>
        [CLSCompliant(false)]
        public void RefreshNecessaryIndexes(XafApplication application, Action<IWorkerProgress> progress)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            using (var os = (XPObjectSpace)application.ObjectSpaceProvider.CreateUpdatingObjectSpace(false))
            {
                string[] indexes = null;
                var indexMap = new Dictionary<string, string>();
                using (var lck = new ResourceLock(os.Session, ElasticSearchCheckNecessary, LockMode.Shared))
                {
                    if (lck.Acquire(0))
                    {
                        var existingIndexes = os.GetObjects(elasticSearchIndexPersistentType).Cast<IElasticSearchIndex>().Select(t => t.Name).ToList();
                        foreach (var ti in os.TypesInfo.PersistentTypes.Where(t => t.IsPersistent))
                        {
                            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(ti.Type);
                            if (ci.IsESIndexed && !string.IsNullOrWhiteSpace(ci.ESIndexName))
                            {
                                indexMap[IndexName(ci.ESIndexName)] = ci.ESIndexName;
                                if (!existingIndexes.Contains(IndexName(ci.ESIndexName)))
                                {
                                    existingIndexes.Add(IndexName(ci.ESIndexName));
                                    IndexChange(ci.ESIndexName, os.Session, false);
                                }
                            }
                        }
                        indexes = os.GetObjects(elasticSearchIndexPersistentType).Cast<IElasticSearchIndex>().Where(t => !t.Active).
                            Concat(os.GetObjects(elasticSearchIndexRefreshPersistentType).Cast<IElasticSearchIndexRefresh>().Select(t => t.Index)).
                            Where(t => indexMap.ContainsKey(t.Name)).Select(t => indexMap[t.Name]).Distinct().ToArray();
                    }
                }
                if (indexes != null && indexes.Any())
                {
                    RefreshIndexes(application, progress, indexes);
                }
            }
        }

        /// <summary>
        /// Refreshes all indexes which are named in indexNames or all of them if indexNames is null or an empty string
        /// </summary>
        /// <param name="application">A XafApplication instance</param>
        /// <param name="progress">Optional Progress Callback method</param>
        /// <param name="indexNames">Array of the Names of the indexes to refresh</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = nameof(ElasticSearch))]
        [CLSCompliant(false)]
        public void RefreshIndexes(XafApplication application, Action<IWorkerProgress> progress, params string[] indexNames)
        {
            var workerProgress = new WorkerProgress
            {
                Name = CaptionHelper.GetLocalizedText(MessageGroup, "RefreshIndexesProgressName"),
                Phase = CaptionHelper.GetLocalizedText(MessageGroup, "RefreshIndexesProgressPhase"),
                Position = 0
            };
            try
            {
                if (ElasticSearchNodes.Any())
                {
                    if (ElasticLowLevelClient == null)
                    {
                        throw new ElasticIndexException(CaptionHelper.GetLocalizedText(IndexExceptionGroup, "ElasticSearchServerNoConnection"));
                    }
                    else
                    {
                        var indexes = new HashSet<string>();
                        using (var osp = new XPObjectSpaceProvider(application.GetConnectionString(), null, true))
                        using (var objectspace = (XPObjectSpace)osp.CreateObjectSpace())
                        {
                            using (var lck = new ResourceLock(objectspace.Session, ElasticSearchRefresh, LockMode.Exclusive))
                            {
                                if (lck.Acquire(0))
                                {
                                    var typeCount = 0;
                                    foreach (var ti in objectspace.TypesInfo.PersistentTypes.Where(t => t.IsPersistent))
                                    {
                                        var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(ti.Type);
                                        if (ci.IsESIndexed)
                                        {
                                            if (string.IsNullOrWhiteSpace(ci.ESIndexName))
                                            {
                                                throw new ElasticIndexException(string.Format(CultureInfo.CurrentCulture, CaptionHelper.GetLocalizedText(IndexExceptionGroup, "ElasticSearchAttributeWrong"), ti.Name));
                                            }
                                            else
                                            {
                                                if (indexNames == null || indexNames.Length == 0 || indexNames.Contains(ci.ESIndexName))
                                                {
                                                    if (indexes.Add(ci.ESIndexName))
                                                    {
                                                        IndexChange(ci.ESIndexName, objectspace.Session, false);
                                                    }
                                                    typeCount++;
                                                }
                                            }
                                        }
                                    }

                                     var indexList = objectspace.GetObjects(elasticSearchIndexPersistentType).OfType<IElasticSearchIndex>().ToList();

                                    // Delete ElasticSearchIndex'es, who are in indexNames but not in indexes
                                    objectspace.Delete(indexList.Where(t => !indexes.Select(IndexName).Contains(t.Name) && (indexNames == null || indexNames.Length == 0 || indexNames.Contains(t.Name))).ToList());

                                    workerProgress.Maximum = typeCount;
                                    indexList = indexList.Where(t => indexes.Select(IndexName).Contains(t.Name)).ToList();
                                    Parallel.ForEach(
                                        new OrderableListPartitioner<IElasticSearchIndex>(indexList),
                                    new ParallelOptions
                                    {
                                        MaxDegreeOfParallelism = Environment.ProcessorCount
                                    },
                                    ei =>
                                    {
                                        Reindex(ei, (ci, i) =>
                                        {
                                            if (i == -1)
                                            {
                                                workerProgress.Position++;
                                                progress?.Invoke(workerProgress);
                                            }
                                        });
                                    });
                                    var si = string.Join(",", indexes.ToArray());
                                    if (!string.IsNullOrWhiteSpace(si))
                                    {
                                        var res = ElasticLowLevelClient.IndicesForcemerge<VoidResponse>(string.Join(",", indexes.Select(IndexName).ToArray()));
                                        if (!res.Success)
                                        {
                                            throw new ElasticIndexException(string.Format(CultureInfo.CurrentCulture, CaptionHelper.GetLocalizedText(IndexExceptionGroup, "ElasticSearchIndexMergeError"), si, res.HttpStatusCode));
                                        }
                                    }
                                    objectspace.CommitChanges();
                                }
                                else
                                {
                                    throw new ElasticIndexLockingException(CaptionHelper.GetLocalizedText(IndexExceptionGroup, "ReindexAtWork"));
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (progress != null)
                {
                    workerProgress.Position = -1;
                    progress(workerProgress);
                }
            }
        }

        private class Suggester
        {
            public string Field
            {
                get;
                set;
            }

            public int Results
            {
                get;
                set;
            }

            public bool Fuzzy
            {
                get;
                set;
            }

            public Dictionary<string, string> Contexts
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Returns a list of suggestions for the provided text
        /// </summary>
        /// <param name="text">The text to search suggestions for</param>
        /// <param name="typeInfo">The Type Info instance to restrict the suggestions to</param>
        /// <param name="modelItem">XAF Modell Settings</param>
        /// <returns>An awaitable Enumeration of strings with suggestions for text</returns>
        [CLSCompliant(false)]
        public async Task<IEnumerable<string>> SuggestAsync(string text, ITypeInfo typeInfo, IModelElasticSearchFieldsItem modelItem)
        {
            if (!string.IsNullOrEmpty(text) && typeInfo != null)
            {
                var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(typeInfo.Type);
                if (!ci.ElasticIndexError && ci.ESSuggestFields != null)
                {
                    var suggesters = new List<Suggester>();
                    if (modelItem == null)
                    {
                        foreach (var field in ci.ESSuggestFields.Where(t => t.Default))
                        {
                            var suggester = new Suggester
                            {
                                Field = field.FieldName,
                                Results = 7,
                                Fuzzy = true,
                                Contexts = field.ContextSettings.ToDictionary(t => t.ContextName, t => GetParameterValue(t.QueryParameter)),
                            };
                            suggesters.Add(suggester);
                        }
                    }
                    else
                    {
                        foreach (var modelSuggestField in modelItem.SuggestFields)
                        {
                            var field = ci.ESSuggestFields.FirstOrDefault(t => t.FieldName == modelSuggestField.ElasticSearchSuggestField);
                            if (field != null)
                            {
                                var suggester = new Suggester
                                {
                                    Field = field.FieldName,
                                    Results = modelSuggestField.Results,
                                    Fuzzy = modelSuggestField.Fuzzy,
                                    Contexts = field.ContextSettings.ToDictionary(t => t.ContextName, t => GetParameterValue(t.QueryParameter)),
                                };
                                foreach (var modelContext in modelSuggestField.Contexts)
                                {
                                    var sci = field.ContextSettings.First(cs => cs.ContextName == modelContext.Name);
                                    if (!string.IsNullOrEmpty(modelContext.Value))
                                    {
                                        suggester.Contexts[modelContext.Name] = ParameterContent(modelContext.Value);
                                    }
                                    if (!string.IsNullOrEmpty(modelContext.Parameter))
                                    {
                                        suggester.Contexts[modelContext.Name] = GetParameterValue(modelContext.Parameter);
                                    }
                                }
                                suggesters.Add(suggester);
                            }
                        }
                    }
                    if (suggesters.Any())
                    {
                        var sw = new StringWriter();
                        var writer = new JsonTextWriter(sw);
                        writer.WriteStartObject();
                        writer.WritePropertyName("suggest");
                        writer.WriteStartObject();
                        foreach (var suggester in suggesters)
                        {
                            writer.WritePropertyName(suggester.Field);
                            writer.WriteRawValue(SuggestCompletion(text, suggester.Field, suggester.Results, suggester.Fuzzy, suggester.Contexts));
                        }
                        writer.WriteEndObject();
                        writer.WriteEndObject();
                        writer.Flush();
                        ElasticsearchResponse<SuggestResponse> response;
                        if (UseAsync)
                        {
                            response = await DoSuggestAsync(ci, sw.ToString()).ConfigureAwait(false);
                        }
                        else
                        {
                            response = DoSuggestSync(ci, sw.ToString());
                        }
                        if (response != null)
                        {
                            if (response.Success && response.Body != null)
                            {
                                var suggestions = new HashSet<string>();
                                foreach (var suggestion in response.Body.Suggestions)
                                {
                                    foreach (var suggest in suggestion.Value)
                                    {
                                        suggestions.UnionWith(suggest.Options.Select(t => t.Text));
                                    }
                                }
                                return suggestions;
                            }
                            else
                            {
                                Tracing.Tracer.LogError("Suggest Error StatusCode: {0}", response.HttpStatusCode);
                                if (response.ServerError != null)
                                {
                                    Tracing.Tracer.LogValue(nameof(response.ServerError), response.ServerError);
                                    Tracing.Tracer.LogValue("Suggest Query", sw.ToString());
                                }
                                else if (!string.IsNullOrWhiteSpace(response.DebugInformation))
                                {
                                    Tracing.Tracer.LogValue(nameof(response.DebugInformation), response.DebugInformation);
                                }
                                else
                                {
                                    Tracing.Tracer.LogValue("Suggest Query", sw.ToString());
                                }
                            }
                        }
                        else
                        {
                            Tracing.Tracer.LogError("Suggest Response is null");
                            Tracing.Tracer.LogValue("Suggest Query", sw.ToString());
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Changes the active state on the Index
        /// </summary>
        /// <param name="indexName">Name of the Index</param>
        /// <param name="session">XPO Session</param>
        /// <param name="aktiv">State to set Active to</param>
        public void IndexChange(string indexName, Session session, bool aktiv)
        {
            using (var xpColl = new XPCollection(session, elasticSearchIndexPersistentType))
            {
                indexName = IndexName(indexName);
                var ei = xpColl.OfType<IElasticSearchIndex>().FirstOrDefault(t => indexName.Equals(t.Name, StringComparison.OrdinalIgnoreCase));
                if (ei == null || ei.Active != aktiv)
                {
                    IndexChangeCore(indexName, session, aktiv);
                }
            }
        }

        /// <summary>
        /// Rebuilds an Index
        /// </summary>
        /// <param name="index">The index to rebuild</param>
        /// <param name="progress">Progress Callback</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = nameof(ElasticSearch))]
        public void Reindex(IElasticSearchIndex index, Action<BYteWareTypeInfo, int> progress)
        {
            if (index == null)
            {
                throw new ArgumentNullException(nameof(index));
            }
            using (var lck = new ResourceLock(index.Session, ElasticSearchRefresh, LockMode.Exclusive))
            {
                if (lck.Acquire(0))
                {
                    var maxAktualisieren = DateTime.Now;
                    IndexChangeCore(index.Name, index.Session, false);
                    ElasticLowLevelClient.IndicesDelete<VoidResponse>(index.Name);
                    var typeList = XafTypesInfo.Instance.PersistentTypes
                        .Where(t =>
                        {
                            if (t.IsPersistent)
                            {
                                var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(t.Type);
                                return ci.ESIndexName != null && IndexName(ci.ESIndexName).Equals(index.Name, StringComparison.OrdinalIgnoreCase);
                            }
                            return false;
                        }).ToList();
                    foreach (var ti in typeList)
                    {
                        var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(ti.Type);
                        ci.IsESMapped = false;
                        ci.ElasticIndexError = false;
                        try
                        {
                            if (!DoMapping(ci, index.Session))
                            {
                                throw new ElasticIndexException(string.Format(CultureInfo.CurrentCulture, CaptionHelper.GetLocalizedText(IndexExceptionGroup, "PutMappingError"), ti.Name));
                            }
                        }
                        catch (Exception ex)
                        {
                            Tracing.Tracer.LogError(ex);
                            throw;
                        }
                    }
                    IndexChangeCore(index.Name, index.Session, true);
                    Parallel.ForEach(
                        new OrderableListPartitioner<ITypeInfo>(typeList),
                        new ParallelOptions
                        {
                            MaxDegreeOfParallelism = Environment.ProcessorCount
                        },
                        ti =>
                        {
                            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(ti.Type);
                            progress?.Invoke(ci, 0);
                            if (!ti.IsAbstract && !ti.IsInterface)
                            {
                                using (var uSession = index.Session.DataLayer != null ? new Session(index.Session.DataLayer) : new Session(index.Session.ObjectLayer))
                                {
                                    CriteriaOperator criteria = null;
                                    if (ti.HasDescendants)
                                    {
                                        criteria = new BinaryOperator(
                                                XPObjectType.ObjectTypePropertyName,
                                                uSession.GetObjectType(uSession.GetClassInfo(ti.Type)));
                                    }
                                    var cursor = new XPCursor(
                                            uSession,
                                            ti.Type,
                                            criteria)
                                    {
                                        PageSize = 100
                                    };

                                    var sb = new StringBuilder();
                                    var j = 0;

                                    // Loop through all the objects.
                                    foreach (var theObject in cursor)
                                    {
                                        if (theObject is XPBaseObject o)
                                        {
                                            sb.Append(SerializeObjectForBulk(uSession, o, ci));
                                            j++;
                                            if (sb.Length > BulkSize)
                                            {
                                                BulkIndex(ti, sb.ToString());
                                                sb.Clear();
                                            }
                                            if (j % 500 == 0 && progress != null)
                                            {
                                                progress?.Invoke(ci, 500);
                                            }
                                        }
                                    }
                                    if (sb.Length > 0)
                                    {
                                        BulkIndex(ti, sb.ToString());
                                    }
                                    progress?.Invoke(ci, j % 500);
                                }
                            }
                            progress?.Invoke(ci, -1);
                        });
                    if (maxAktualisieren != null)
                    {
                        DeleteIndexAktualisieren(index.Name, index.Session, maxAktualisieren);
                    }
                }
                else
                {
                    throw new ElasticIndexLockingException(CaptionHelper.GetLocalizedText(IndexExceptionGroup, "ReindexAtWork"));
                }
            }
        }

        /// <summary>
        /// Is the Index active?
        /// </summary>
        /// <param name="indexName">Name of the Index</param>
        /// <param name="session">XPO Session</param>
        /// <returns>True if an index of the name could be found and was active; False otherwise</returns>
        public bool IsIndexActive(string indexName, Session session)
        {
            using (var xpColl = new XPCollection(session, elasticSearchIndexPersistentType))
            {
                var ei = xpColl.OfType<IElasticSearchIndex>().FirstOrDefault(t => IndexName(indexName).Equals(t.Name, StringComparison.OrdinalIgnoreCase));
                return (ei != null) && ei.Active;
            }
        }

        /// <summary>
        /// Add Index Refresh Entry
        /// </summary>
        /// <param name="indexName">Name of the Index</param>
        /// <param name="session">XPO Session</param>
        public void AddIndexRefresh(string indexName, Session session)
        {
            if (indexName == null)
            {
                throw new ArgumentNullException(nameof(indexName));
            }
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            for (int attempt = 1; attempt <= MaxAttemptsCounter; ++attempt)
            {
                try
                {
                    using (var uSession = session.DataLayer != null ? new Session(session.DataLayer) : new Session(session.ObjectLayer))
                    {
                        var ci = uSession.GetClassInfo(elasticSearchIndexPersistentType);
                        using (var xpColl = new XPCollection(PersistentCriteriaEvaluationBehavior.BeforeTransaction, uSession, ci, null, true))
                        {
                            var ei = xpColl.OfType<IElasticSearchIndex>().FirstOrDefault(t => indexName.Equals(t.Name, StringComparison.OrdinalIgnoreCase));
                            if (ei == null)
                            {
                                ei = ci.CreateNewObject(uSession) as IElasticSearchIndex;
                                ei.Name = indexName;
                                ei.Active = false;
                            }
                            if (ei is XPBaseObject pb && pb.IsDeleted)
                            {
                                pb.SetMemberValue(GCRecordField.StaticName, null);
                                pb.Save();
                            }
                            ci = uSession.GetClassInfo(elasticSearchIndexRefreshPersistentType);
                            var ia = ci.CreateNewObject(uSession) as IElasticSearchIndexRefresh;
                            ia.Index = ei;
                            ia.Timestamp = DateTime.Now;
                            pb = ia as XPBaseObject;
                            if (pb != null)
                            {
                                pb.Save();
                            }
                            break;
                        }
                    }
                }
                catch (LockingException)
                {
                    if (attempt >= MaxAttemptsCounter)
                    {
                        throw;
                    }
                }
            }
        }

        private static void WriteFields(JsonTextWriter writer, IModelElasticSearchFieldsItem modelItem, string esFields)
        {
            writer.WritePropertyName("fields");
            writer.WriteStartArray();
            var fieldWritten = false;
            if (modelItem != null)
            {
                foreach (var field in modelItem.Fields)
                {
                    var f = string.Format(CultureInfo.InvariantCulture, "{0}{1}", field.ElasticSearchField, field.Boost == 1.0 ? string.Empty : '^' + field.Boost.ToString(CultureInfo.InvariantCulture));
                    writer.WriteValue(f);
                    fieldWritten = true;
                }
            }
            if (!string.IsNullOrWhiteSpace(esFields))
            {
                foreach (var field in esFields.Split(';'))
                {
                    writer.WriteValue(field);
                    fieldWritten = true;
                }
            }
            if (!fieldWritten)
            {
                writer.WriteValue("_all");
            }
            writer.WriteEndArray();
        }

        private static IEnumerable<string> SplitValues(string input)
        {
            var csvSplit = new Regex("((?<=\")[^\"]*(?=\"(,|$)+)|(?<=,|^)[^,\"]*(?=,|$))", RegexOptions.Compiled);
            foreach (Match match in csvSplit.Matches(input))
            {
                yield return match.Value;
            }
        }

        private static string ParameterContent(string value)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            foreach (var item in SplitValues(value).Where(s => !string.IsNullOrEmpty(s)))
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, "\"{0}\",", item);
            }
            sb.Length--;
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// Returns a query string in ElasticSearch Query DSL for the given parameters to use with the Search method
        /// </summary>
        /// <param name="searchText">The text to search for</param>
        /// <param name="results">Amount of results to return at maximum</param>
        /// <param name="fuzzy">Should the search be fuzzy</param>
        /// <param name="wildcard">Should the search support wildcards in the searchText</param>
        /// <param name="filter">Restrict the results to this filter string</param>
        /// <param name="securityFilter">Filter string defined through the Security System</param>
        /// <param name="modelItem">XAF Model Settings for the search</param>
        /// <param name="fields">A semicolon separated List of Elasticsearch Field Names to search in</param>
        /// <param name="validate">True if the there should be only a check for correct syntax of the query</param>
        /// <returns>A query string in ElasticSearch Query DSL</returns>
        private string SearchBody(string searchText, int results, bool fuzzy, bool wildcard, string filter, string securityFilter, IModelElasticSearchFieldsItem modelItem, string fields, bool validate)
        {
            if (searchText == null)
            {
                throw new ArgumentNullException(nameof(searchText));
            }
            var queryType = ElasticQueryType.best_fields;
            var minimumShouldMatch = DefaultMinimumShouldMatch;
            var tieBreaker = 0.3;
            if (modelItem != null)
            {
                queryType = modelItem.QueryType;
                minimumShouldMatch = modelItem.MinimumShouldMatch;
                tieBreaker = modelItem.TieBreaker;
            }
            StringWriter sw = null;
            try
            {
                var strw = sw = new StringWriter(CultureInfo.InvariantCulture);
                using (var writer = new JsonTextWriter(sw))
                {
                    sw = null;
                    writer.WriteStartObject();
                    if (!validate)
                    {
                        writer.WritePropertyName("size");
                        writer.WriteValue(results);
                        writer.WritePropertyName("stored_fields");
                        writer.WriteStartArray();
                        writer.WriteEndArray();
                    }
                    writer.WritePropertyName("query");
                    writer.WriteStartObject();
                    var filtered = !string.IsNullOrWhiteSpace(filter) || !string.IsNullOrWhiteSpace(securityFilter);
                    if (filtered)
                    {
                        writer.WritePropertyName("bool");
                        writer.WriteStartObject();
                        writer.WritePropertyName("must");
                        writer.WriteStartObject();
                    }
                    if (wildcard)
                    {
                        if (searchText.Count(t => t == '"') % 2 != 0)
                        {
                            searchText += '"';
                        }
                        writer.WritePropertyName("query_string");
                        writer.WriteStartObject();
                        WriteFields(writer, modelItem, fields);
                        writer.WritePropertyName("query");
                        writer.WriteValue(searchText);
                        writer.WritePropertyName("analyze_wildcard");
                        writer.WriteValue("true");
                        writer.WritePropertyName("minimum_should_match");
                        writer.WriteValue(minimumShouldMatch);
                        writer.WritePropertyName("lenient");
                        writer.WriteValue("true");
                        if (queryType == ElasticQueryType.phrase || queryType == ElasticQueryType.phrase_prefix)
                        {
                            writer.WritePropertyName("auto_generate_phrase_queries");
                            writer.WriteValue("true");
                        }
                        writer.WriteEnd();
                    }
                    else
                    {
                        if (searchText.Contains('"'))
                        {
                            searchText = searchText.Replace("\"", string.Empty);
                            if (queryType != ElasticQueryType.phrase || queryType != ElasticQueryType.phrase_prefix)
                            {
                                queryType = fuzzy ? ElasticQueryType.phrase_prefix : ElasticQueryType.phrase;
                            }
                        }
                        writer.WritePropertyName("multi_match");
                        writer.WriteStartObject();
                        WriteFields(writer, modelItem, fields);
                        writer.WritePropertyName("type");
                        writer.WriteValue(Enum.GetName(typeof(ElasticQueryType), queryType));
                        writer.WritePropertyName("query");
                        writer.WriteValue(searchText);
                        writer.WritePropertyName("minimum_should_match");
                        writer.WriteValue(minimumShouldMatch);
                        writer.WritePropertyName("tie_breaker");
                        writer.WriteValue(tieBreaker);
                        writer.WritePropertyName("lenient");
                        writer.WriteValue("true");
                        if (fuzzy && (queryType == ElasticQueryType.best_fields || queryType == ElasticQueryType.most_fields))
                        {
                            writer.WritePropertyName("fuzziness");
                            writer.WriteValue("AUTO");
                            writer.WritePropertyName("max_expansions");
                            writer.WriteValue(50);
                        }
                        writer.WriteEnd();
                    }
                    if (filtered)
                    {
                        writer.WriteEnd();
                        writer.WritePropertyName(nameof(filter));
                        writer.WriteStartArray();
                        if (!string.IsNullOrWhiteSpace(filter))
                        {
                            writer.WriteStartObject();
                            writer.WriteRaw(ReplaceParameters(filter));
                            writer.WriteEnd();
                        }
                        if (!string.IsNullOrWhiteSpace(securityFilter))
                        {
                            writer.WriteStartObject();
                            writer.WriteRaw(ReplaceParameters(securityFilter));
                            writer.WriteEnd();
                        }
                        writer.WriteEndArray();
                        writer.WriteEnd();
                    }
                    writer.WriteEnd();
                    writer.WriteEndObject();
                    writer.Flush();
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

        private void PrepareTypes(Session session, HashSet<BYteWareTypeInfo> typeInfos, BYteWareTypeInfo ci, Dictionary<ContainingType, HashSet<object>> typeLists)
        {
            if (typeInfos.Add(ci))
            {
                try
                {
                    DoMapping(ci, session);
                }
                catch (ElasticIndexException ex)
                {
                    Tracing.Tracer.LogError(ex);
                }
                if (ci.ContainingTypes != null)
                {
                    foreach (var containingType in ci.ContainingTypes)
                    {
                        typeLists.Add(containingType, new HashSet<object>());
                    }
                }
                if (ci.ESReferences != null)
                {
                    foreach (var reference in ci.ESReferences)
                    {
                        PrepareTypes(session, typeInfos, BYteWareTypeInfo.GetBYteWareTypeInfo(reference.MemberType), typeLists);
                    }
                }
            }
        }

        private void BulkIndex(Session session, Dictionary<XPClassInfo, HashSet<object>> indexed, StringBuilder bulk, HashSet<BYteWareTypeInfo> typeInfos, BYteWareTypeInfo ci, Dictionary<ContainingType, HashSet<object>> typeLists, XPBaseObject bo)
        {
            if (!indexed.TryGetValue(bo.ClassInfo, out HashSet<object> indexedKeys))
            {
                indexedKeys = new HashSet<object>();
                indexed.Add(bo.ClassInfo, indexedKeys);
            }
            if (indexedKeys.Add(session.GetKeyValue(bo)))
            {
                bulk.Append(SerializeObjectForBulk(session, bo, ci));
                if (bulk.Length >= BulkSize)
                {
                    BulkIndex(bulk, session, typeInfos);
                    bulk.Clear();
                }
                if (ci.ESReferences != null)
                {
                    foreach (var esReference in ci.ESReferences)
                    {
                        if (esReference.GetValue(bo) is XPBaseObject reference)
                        {
                            BulkIndex(session, indexed, bulk, typeInfos, BYteWareTypeInfo.GetBYteWareTypeInfo(reference.GetType()), typeLists, reference);
                        }
                    }
                }
                if (ci.ContainingTypes != null)
                {
                    foreach (var containingType in ci.ContainingTypes)
                    {
                        if (!typeLists.TryGetValue(containingType, out HashSet<object> keys))
                        {
                            keys = new HashSet<object>();
                            typeLists.Add(containingType, keys);
                        }
                        keys.Add(session.GetKeyValue(bo));
                    }
                }
            }
        }

        private void DeleteIndexAktualisieren(string indexName, Session session, DateTime timeStamp)
        {
            for (int attempt = 1; attempt <= MaxAttemptsCounter; ++attempt)
            {
                try
                {
                    using (var uSession = session.DataLayer != null ? new Session(session.DataLayer) : new Session(session.ObjectLayer))
                    {
                        using (var xpColl = new XPCollection(uSession, elasticSearchIndexRefreshPersistentType))
                        {
                            uSession.Delete(xpColl.OfType<IElasticSearchIndexRefresh>().Where(t => indexName.Equals(t.Index?.Name, StringComparison.OrdinalIgnoreCase) && t.Timestamp <= timeStamp).ToList());
                            break;
                        }
                    }
                }
                catch (LockingException)
                {
                    if (attempt >= MaxAttemptsCounter)
                    {
                        throw;
                    }
                }
            }
        }

        private void IndexChangeCore(string indexName, Session session, bool active)
        {
            for (int attempt = 1; attempt <= MaxAttemptsCounter; ++attempt)
            {
                try
                {
                    using (var uSession = session.DataLayer != null ? new Session(session.DataLayer) : new Session(session.ObjectLayer))
                    {
                        var ci = uSession.GetClassInfo(elasticSearchIndexPersistentType);
                        using (var xpColl = new XPCollection(PersistentCriteriaEvaluationBehavior.BeforeTransaction, uSession, ci, null, true))
                        {
                            var ei = xpColl.OfType<IElasticSearchIndex>().FirstOrDefault(t => indexName.Equals(t.Name, StringComparison.OrdinalIgnoreCase));
                            if (ei == null)
                            {
                                ei = ci.CreateNewObject(uSession) as IElasticSearchIndex;
                                ei.Name = indexName;
                            }
                            ei.Active = active;
                            if (ei is XPBaseObject pb)
                            {
                                pb.SetMemberValue(GCRecordField.StaticName, null);
                                pb.Save();
                            }
                            break;
                        }
                    }
                }
                catch (LockingException)
                {
                    if (attempt >= MaxAttemptsCounter)
                    {
                        throw;
                    }
                }
            }
        }

        private void BulkIndex(ITypeInfo ti, string s)
        {
            var errors = true;
            var retries = 0;
            Exception e = null;
            ElasticsearchResponse<string> res = null;
            while (errors && retries++ < 3 && e == null)
            {
                res = ElasticLowLevelClient.Bulk<string>(s);
                e = res.OriginalException;
                errors = !res.Success || res.Body == null || !res.Body.Contains("\"errors\":false");
            }
            if (e == null && errors && res != null)
            {
                e = new ElasticIndexException(res.Body);
            }
            if (e != null)
            {
                throw new ElasticIndexException(string.Format(CultureInfo.CurrentCulture, CaptionHelper.GetLocalizedText(IndexExceptionGroup, "IndexError"), ti.Name, res.HttpStatusCode), e);
            }
        }

        private bool ElasticSearchMapping(BYteWareTypeInfo ci)
        {
            if (ci != null && !ci.IsESMapped)
            {
                ci.IsESMapped = true;
                if (ci.IsESIndexed && !string.IsNullOrWhiteSpace(ci.ESIndexName))
                {
                    try
                    {
                        var ec = ElasticLowLevelClient;
                        if (ec != null)
                        {
                            var indexName = IndexName(ci.ESIndexName);
                            var res = ec.IndicesExists<VoidResponse>(indexName);
                            if (!res.HttpStatusCode.HasValue || res.HttpStatusCode.Value != 200)
                            {
                                res = ec.IndicesCreate<VoidResponse>(indexName, ci.ESIndexSettings);
                                if (!res.Success)
                                {
                                    throw new ElasticIndexException(res.DebugInformation + Environment.NewLine + res.ServerError?.ToString());
                                }
                            }
                            var typeName = TypeName(ci.ESTypeName);
                            res = ec.IndicesExistsType<VoidResponse>(indexName, typeName);
                            if (!res.HttpStatusCode.HasValue || res.HttpStatusCode.Value != 200)
                            {
                                var tmw = new TypeMappingWriter(this, ci, 0, true);
                                res = ec.IndicesPutMapping<VoidResponse>(indexName, typeName, tmw.Map());
                                if (!res.Success)
                                {
                                    throw new ElasticIndexException(res.DebugInformation + Environment.NewLine + res.ServerError?.ToString());
                                }
                            }
                            return res.Success;
                        }
                    }
                    catch (SocketException)
                    {
                        return false;
                    }
                    catch (WebException)
                    {
                        return false;
                    }
                    catch (ElasticsearchClientException)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void ObjectSpace_Disposed(object sender, EventArgs e)
        {
            UnregisterObjectSpace(sender as XPObjectSpace);
        }

        private void ObjectSpace_Committing(object sender, CancelEventArgs e)
        {
            if (sender is XPObjectSpace os && _RegisteredObjectSpaces.ContainsKey(os))
            {
                _RegisteredObjectSpaces[os] = new Changes(os.GetObjectsToDelete(false).OfType<XPBaseObject>().Concat(os.GetObjectsToSave(false).OfType<XPBaseObject>()));
            }
        }

        private void ObjectSpace_Committed(object sender, EventArgs e)
        {
            if (sender is XPObjectSpace os && _RegisteredObjectSpaces.TryGetValue(os, out Changes changes))
            {
                IndexChanges(os.Session, changes);
            }
        }

        private void Uow_Disposed(object sender, EventArgs e)
        {
            UnregisterUnitOfWork(sender as UnitOfWork);
        }

        private void Uow_BeforeFlushChanges(object sender, SessionManipulationEventArgs e)
        {
            if (e.Session is UnitOfWork uow && _RegisteredUnitOfWork.ContainsKey(uow))
            {
                _RegisteredUnitOfWork[uow] = new Changes(uow.GetObjectsToDelete(false).OfType<XPBaseObject>().Concat(uow.GetObjectsToSave(false).OfType<XPBaseObject>()));
            }
        }

        private void Uow_AfterFlushChanges(object sender, SessionManipulationEventArgs e)
        {
            if (e.Session is UnitOfWork uow && _RegisteredUnitOfWork.TryGetValue(uow, out Changes changes))
            {
                IndexChanges(uow, changes);
            }
        }

        private void IndexChanges(Session session, Changes changes)
        {
            var indexed = new Dictionary<XPClassInfo, HashSet<object>>();
            foreach (var tkl in changes.ModifiedObjects.GroupBy(t => t.ClassInfo))
            {
                var bulk = new StringBuilder();
                var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(tkl.Key.ClassType);
                var typeLists = new Dictionary<ContainingType, HashSet<object>>();
                var typeInfos = new HashSet<BYteWareTypeInfo>();
                PrepareTypes(session, typeInfos, ci, typeLists);
                foreach (XPBaseObject bo in tkl)
                {
                    BulkIndex(session, indexed, bulk, typeInfos, ci, typeLists, bo);
                }
                BulkIndex(bulk, session, typeInfos);
                foreach (var tl in typeLists.Where(t => t.Value.Any()))
                {
                    IndexList(session, tl.Key.BYteWareType.ClassInfo, tl.Value.AsReadOnly(), indexed, tl.Key.MemberInfo, null);
                }
            }
        }

        private void DoIndexOnSaved(XPBaseObject bo, HashSet<object> indexed, StringBuilder bulk, HashSet<BYteWareTypeInfo> typeInfos, bool propertyChanged)
        {
            if (bo != null)
            {
                var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(bo.GetType());
                try
                {
                    DoMapping(ci, bo.Session);
                }
                catch (ElasticIndexException ex)
                {
                    Tracing.Tracer.LogError(ex);
                }
                if (propertyChanged)
                {
                    bulk.Append(SerializeObjectForBulk(bo.Session, bo, ci));
                    typeInfos.Add(ci);
                }
                indexed.Add(bo.Session.GetKeyValue(bo));
                if (bulk.Length >= BulkSize)
                {
                    BulkIndex(bulk, bo.Session, typeInfos);
                    typeInfos.Clear();
                    bulk.Clear();
                }
                IndexReferences(bo, ci, indexed, bulk, typeInfos, propertyChanged);
                IndexContainingInstances(bo, ci, indexed, bulk, typeInfos, propertyChanged);
            }
        }

        private void IndexReferences(XPBaseObject bo, BYteWareTypeInfo ci, HashSet<object> indexed, StringBuilder bulk, HashSet<BYteWareTypeInfo> typeInfos, bool propertyChanged)
        {
            if (ci.ESReferences != null && propertyChanged)
            {
                foreach (var esReference in ci.ESReferences)
                {
                    if (esReference.GetValue(bo) is XPBaseObject reference)
                    {
                        var oid = reference.Session.GetKeyValue(reference);
                        if (!indexed.Contains(oid))
                        {
                            DoIndexOnSaved(reference, indexed, bulk, typeInfos, true);
                            indexed.Add(oid);
                        }
                    }
                }
            }
        }

        private void IndexContainingInstances(XPBaseObject bo, BYteWareTypeInfo ci, HashSet<object> indexed, StringBuilder bulk, HashSet<BYteWareTypeInfo> typeInfos, bool propertyChanged)
        {
            if (ci.ContainingTypes != null)
            {
                foreach (var containingType in ci.ContainingTypes)
                {
                    if (containingType.HasContainingSetting || propertyChanged)
                    {
                        CriteriaOperator crit = new BinaryOperator(containingType.MemberInfo.Name, bo);
                        if (containingType.MemberInfo.ListElementType != null)
                        {
                            crit = new ContainsOperator(containingType.MemberInfo.Name, new BinaryOperator(nameof(XPBaseObject.This), bo));
                        }
                        var cursor = new XPCursor(
                            bo.Session,
                            containingType.BYteWareType.Type,
                            crit)
                        {
                            PageSize = 100
                        };

                        // Loop through all the objects.
                        foreach (var theObject in cursor)
                        {
                            if (theObject is XPBaseObject reference)
                            {
                                var oid = reference.Session.GetKeyValue(reference);
                                if (!indexed.Contains(oid))
                                {
                                    DoIndexOnSaved(reference, indexed, bulk, typeInfos, true);
                                    indexed.Add(oid);
                                }
                            }
                        }
                    }
                }
            }
        }

        private async Task DoBulkAsync(string json, Session session, IEnumerable<BYteWareTypeInfo> typeInfos)
        {
            var success = false;
            try
            {
                if (ElasticLowLevelClient != null)
                {
                    var res = await ElasticLowLevelClient.BulkAsync<DynamicResponse>(json).ConfigureAwait(false);
                    success = res.Success && !res.Body["errors"];
                    if (!res.Success)
                    {
                        Tracing.Tracer.LogError("Bulk Async Error StatusCode: {0}", res.HttpStatusCode);
                        if (res.ServerError != null)
                        {
                            Tracing.Tracer.LogValue(nameof(res.ServerError), res.ServerError);
                        }
                        else
                        {
                            Tracing.Tracer.LogValue(nameof(res.DebugInformation), res.DebugInformation);
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (WebException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (ElasticsearchClientException e)
            {
                Tracing.Tracer.LogError(e);
            }
            if (!success)
            {
                using (var uSession = session.DataLayer != null ? new Session(session.DataLayer) : new Session(session.ObjectLayer))
                {
                    SetIndexError(uSession, typeInfos);
                }
            }
        }

        private void DoBulkSync(string json, Session session, IEnumerable<BYteWareTypeInfo> typeInfos)
        {
            var success = false;
            try
            {
                if (ElasticLowLevelClient != null)
                {
                    var res = ElasticLowLevelClient.Bulk<DynamicResponse>(json);
                    success = res.Success && !res.Body["errors"];
                    if (!res.Success)
                    {
                        Tracing.Tracer.LogError("Bulk Error StatusCode: {0}", res.HttpStatusCode);
                        if (res.ServerError != null)
                        {
                            Tracing.Tracer.LogValue(nameof(res.ServerError), res.ServerError);
                        }
                        else
                        {
                            Tracing.Tracer.LogValue(nameof(res.DebugInformation), res.DebugInformation);
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (WebException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (ElasticsearchClientException e)
            {
                Tracing.Tracer.LogError(e);
            }
            if (!success)
            {
                SetIndexError(session, typeInfos);
            }
        }

        private void SetIndexError(Session session, IEnumerable<BYteWareTypeInfo> typeInfos)
        {
            foreach (var ci in typeInfos.Where(t => !t.ElasticIndexError && t.IsESIndexed))
            {
                ci.ElasticIndexError = true;
                IndexChange(ci.ESIndexName, session, false);
            }
        }

        private async Task DoIndexAsync(XPBaseObject bo, BYteWareTypeInfo ci, string json, int version)
        {
            var success = false;
            try
            {
                if (ElasticLowLevelClient != null)
                {
                    var res = await ElasticLowLevelClient.IndexAsync<VoidResponse>(IndexName(ci.ESIndexName), TypeName(ci.ESTypeName), bo.Session.GetKeyValue(bo).ToString(), json, i => i.VersionType(VersionType.ExternalGte).Version(version)).ConfigureAwait(false);
                    success = res.Success;
                    if (!res.Success)
                    {
                        Tracing.Tracer.LogError("Index Async Error StatusCode: {0}", res.HttpStatusCode);
                        if (res.ServerError != null)
                        {
                            Tracing.Tracer.LogValue(nameof(res.ServerError), res.ServerError);
                            Tracing.Tracer.LogValue("Json", json);
                        }
                        else
                        {
                            Tracing.Tracer.LogValue(nameof(res.DebugInformation), res.DebugInformation);
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (WebException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (ElasticsearchClientException e)
            {
                Tracing.Tracer.LogError(e);
            }
            if (!success && !ci.ElasticIndexError)
            {
                ci.ElasticIndexError = true;
                using (var uSession = bo.Session.DataLayer != null ? new Session(bo.Session.DataLayer) : new Session(bo.Session.ObjectLayer))
                {
                    IndexChange(ci.ESIndexName, uSession, false);
                }
            }
        }

        private void DoIndexSync(XPBaseObject bo, BYteWareTypeInfo ci, string json, int version)
        {
            var success = false;
            try
            {
                if (ElasticLowLevelClient != null)
                {
                    var res = ElasticLowLevelClient.Index<VoidResponse>(IndexName(ci.ESIndexName), TypeName(ci.ESTypeName), bo.Session.GetKeyValue(bo).ToString(), json, i => i.VersionType(VersionType.ExternalGte).Version(version));
                    success = res.Success;
                    if (!res.Success)
                    {
                        Tracing.Tracer.LogError("Index Error StatusCode: {0}", res.HttpStatusCode);
                        if (res.ServerError != null)
                        {
                            Tracing.Tracer.LogValue(nameof(res.ServerError), res.ServerError);
                            Tracing.Tracer.LogValue("Json", json);
                        }
                        else
                        {
                            Tracing.Tracer.LogValue(nameof(res.DebugInformation), res.DebugInformation);
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (WebException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (ElasticsearchClientException e)
            {
                Tracing.Tracer.LogError(e);
            }
            if (!success && !ci.ElasticIndexError)
            {
                ci.ElasticIndexError = true;
                IndexChange(ci.ESIndexName, bo.Session, false);
            }
        }

        private async Task DoIndexDeleteAsync(XPBaseObject bo, BYteWareTypeInfo ci, int version)
        {
            var success = false;
            try
            {
                if (ElasticLowLevelClient != null)
                {
                    var res = await ElasticLowLevelClient.DeleteAsync<VoidResponse>(IndexName(ci.ESIndexName), TypeName(ci.ESTypeName), bo.Session.GetKeyValue(bo).ToString(), i => i.VersionType(VersionType.ExternalGte).Version(version)).ConfigureAwait(false);
                    success = res.Success;
                    if (!res.Success)
                    {
                        Tracing.Tracer.LogError("Index Delete Async Error StatusCode: {0}", res.HttpStatusCode);
                        if (res.ServerError != null)
                        {
                            Tracing.Tracer.LogValue(nameof(res.ServerError), res.ServerError);
                        }
                        else
                        {
                            Tracing.Tracer.LogValue(nameof(res.DebugInformation), res.DebugInformation);
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (WebException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (ElasticsearchClientException e)
            {
                Tracing.Tracer.LogError(e);
            }
            if (!success && !ci.ElasticIndexError)
            {
                ci.ElasticIndexError = true;
                using (var uSession = bo.Session.DataLayer != null ? new Session(bo.Session.DataLayer) : new Session(bo.Session.ObjectLayer))
                {
                    IndexChange(ci.ESIndexName, uSession, false);
                }
            }
        }

        private void DoIndexDeleteSync(XPBaseObject bo, BYteWareTypeInfo ci, int version)
        {
            var success = false;
            try
            {
                if (ElasticLowLevelClient != null)
                {
                    var res = ElasticLowLevelClient.Delete<VoidResponse>(IndexName(ci.ESIndexName), TypeName(ci.ESTypeName), bo.Session.GetKeyValue(bo).ToString(), i => i.VersionType(VersionType.ExternalGte).Version(version));
                    success = res.Success;
                    if (!res.Success)
                    {
                        Tracing.Tracer.LogError("Index Delete Error StatusCode: {0}", res.HttpStatusCode);
                        if (res.ServerError != null)
                        {
                            Tracing.Tracer.LogValue(nameof(res.ServerError), res.ServerError);
                        }
                        else
                        {
                            Tracing.Tracer.LogValue(nameof(res.DebugInformation), res.DebugInformation);
                        }
                    }
                }
            }
            catch (SocketException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (WebException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (ElasticsearchClientException e)
            {
                Tracing.Tracer.LogError(e);
            }
            if (!success && !ci.ElasticIndexError)
            {
                ci.ElasticIndexError = true;
                IndexChange(ci.ESIndexName, bo.Session, false);
            }
        }

        private ElasticsearchResponse<SuggestResponse> DoSuggestSync(BYteWareTypeInfo ci, string request)
        {
            ElasticsearchResponse<SuggestResponse> response = null;
            try
            {
                if (ElasticLowLevelClient != null)
                {
                    response = ElasticLowLevelClient.Search<SuggestResponse>(string.Join(",", ci.ESIndexes.Select(IndexName)), string.Join(",", ci.ESTypes.Select(TypeName)), request, t => t.DeserializationOverride(DeserializeSuggestResponse));
                }
            }
            catch (JsonReaderException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (SocketException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (WebException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (ElasticsearchClientException e)
            {
                Tracing.Tracer.LogError(e);
            }
            return response;
        }

        private async Task<ElasticsearchResponse<SuggestResponse>> DoSuggestAsync(BYteWareTypeInfo ci, string request)
        {
            try
            {
                if (ElasticLowLevelClient != null)
                {
                    return await ElasticLowLevelClient.SearchAsync<SuggestResponse>(string.Join(",", ci.ESIndexes.Select(IndexName)), string.Join(",", ci.ESTypes.Select(TypeName)), request, t => t.DeserializationOverride(DeserializeSuggestResponse)).ConfigureAwait(false);
                }
            }
            catch (JsonReaderException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (SocketException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (WebException e)
            {
                Tracing.Tracer.LogError(e);
            }
            catch (ElasticsearchClientException e)
            {
                Tracing.Tracer.LogError(e);
            }
            return null;
        }

        private SuggestResponse DeserializeSuggestResponse(IApiCallDetails connectionStatus, Stream stream)
        {
            if (!connectionStatus.Success || stream == null)
            {
                return new SuggestResponse();
            }
            var s = new StreamReader(stream).ReadToEnd();
            return JsonConvert.DeserializeObject<SuggestResponse>(s, JsonDeserializerSettings);
        }

        private string SerializeObject(XPBaseObject bo)
        {
            return JsonConvert.SerializeObject(bo, JsonSerializerSettings);
        }

        private void ElasticSearchNodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            elasticClient = null;
        }
    }
}
