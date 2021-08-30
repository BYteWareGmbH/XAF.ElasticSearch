namespace BYteWare.XAF.ElasticSearch
{
    using DevExpress.Xpo;
    using Fasterflect;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Json.Net Serializer Settings
    /// </summary>
    public class ElasticSearchContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticSearchContractResolver"/> class.
        /// </summary>
        public ElasticSearchContractResolver() : base()
        {
        }

        /// <inheritdoc/>
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            var props = base.CreateProperties(type, memberSerialization);
            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(type);
            foreach (var pathField in ci.ESSuggestContextPathFields.Where(cpi => ci.ClassInfo.Members.Contains(cpi.MemberInfo) && !cpi.IsIndexed))
            {
                var jp = new JsonProperty
                {
                    PropertyName = pathField.ESFieldName,
                    PropertyType = typeof(IEnumerable<string>),
                    DeclaringType = type,
                    ValueProvider = new ContextPathValueProvider(pathField),
                    AttributeProvider = new EmptyAttributeProvider(),
                    Readable = true,
                    Writable = false,
                };
                if (pathField.MemberInfo != null)
                {
                    if (typeof(IEnumerable).IsAssignableFrom(pathField.MemberInfo.MemberType))
                    {
                        jp.ShouldSerialize = t => (pathField.MemberInfo.GetValue(t) as IEnumerable)?.Cast<object>().Select(o => o?.ToString()).Any(s => !string.IsNullOrEmpty(s)) ?? false;
                    }
                    else
                    {
                        jp.ShouldSerialize = t => !string.IsNullOrEmpty(pathField.MemberInfo.GetValue(t)?.ToString());
                    }
                }
                props.Add(jp);
            }
            return props;
        }

        /// <inheritdoc/>
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

            // Omit empty values, because Suggest Fields will raise errors otherwise
            if (defType == FieldType.text || defType == FieldType.keyword)
            {
                if (typeof(IEnumerable).IsAssignableFrom(member.Type()))
                {
                    jp.ShouldSerialize = t => (member.Get(t) as IEnumerable)?.Cast<object>().Any() ?? false;
                }
                else
                {
                    jp.ShouldSerialize = t => !string.IsNullOrEmpty(member.Get(t)?.ToString());
                }
            }
            if (!string.IsNullOrEmpty(props.WeightField))
            {
                jp.PropertyType = typeof(object);
                jp.ValueProvider = new SuggestWeightFieldValueProvider(member, jp.PropertyName);
            }
            return jp;
        }

        /// <inheritdoc/>
        protected override string ResolvePropertyName(string propertyName)
        {
            return ElasticSearchClient.FieldName(propertyName);
        }
    }
}