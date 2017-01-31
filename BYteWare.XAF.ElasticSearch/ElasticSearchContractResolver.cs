namespace BYteWare.XAF.ElasticSearch
{
    using DevExpress.Xpo;
    using Fasterflect;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Json.Net Serializer Settings
    /// </summary>
    public class ElasticSearchContractResolver : DefaultContractResolver
    {
        private readonly ElasticSearchClient elasticSearchClient;

        /// <summary>
        /// Initalizes a new instance of the <see cref="ElasticSearchContractResolver"/> class.
        /// </summary>
        /// <param name="ec">ElasticSearchClient instance</param>
        public ElasticSearchContractResolver(ElasticSearchClient ec) : base()
        {
            elasticSearchClient = ec;
        }

        /// <inheritdoc/>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);
            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(type);
            if (ci.ESSuggestFields.Any())
            {
                props.Add(new JsonProperty
                {
                    PropertyName = ElasticSearchClient.TypeContext,
                    PropertyType = typeof(string[]),
                    DeclaringType = type,
                    ValueProvider = new TypeNameValueProvider(elasticSearchClient.TypeName(ci.ESTypeName)),
                    AttributeProvider = new EmptyAttributeProvider(),
                    Readable = true,
                    Writable = false
                });
            }
            return props;
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = nameof(Newtonsoft.Json))]
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var source = new List<MemberInfo>();
            var bti = BYteWareTypeInfo.GetBYteWareTypeInfo(objectType);
            foreach (var p in bti.GetTopPropertyInfos)
            {
                var props = bti.ESProperties(p.Name);
                var type = ElasticSearchClient.GetElasticSearchType(props, p.PropertyType);
                if (props != null && !props.OptOut && type != null)
                {
                    source.Add(p);
                }
            }
            return source;
        }

        /// <inheritdoc/>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }
            var jp = base.CreateProperty(member, memberSerialization);
            var bti = BYteWareTypeInfo.GetBYteWareTypeInfo(member.DeclaringType);
            var props = bti.ESProperties(member.Name);
            jp.PropertyName = ElasticSearchClient.FieldName(string.IsNullOrEmpty(props?.FieldName) ? member.Name : props.FieldName);
            var defType = ElasticSearchClient.GetFieldTypeFromType(member.Type());
            if (defType == FieldType.text || defType == FieldType.keyword)
            {
                jp.ShouldSerialize = t => !string.IsNullOrEmpty(member.Get(t)?.ToString());
            }
            if (!string.IsNullOrEmpty(props.WeightField))
            {
                jp.PropertyType = typeof(object);
                jp.ValueProvider = new SuggestWeightFieldValueProvider(member, jp.PropertyName);
            }
            return jp;
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = nameof(Elasticsearch))]
        protected override string ResolvePropertyName(string propertyName)
        {
            return ElasticSearchClient.FieldName(propertyName);
        }
    }
}