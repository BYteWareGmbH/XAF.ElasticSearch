namespace BYteWare.XAF.ElasticSearch
{
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.DC;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Json.Net Value Provider for a Suggest with specified Weight field
    /// </summary>
    internal class SuggestWeightFieldValueProvider : IValueProvider
    {
        private readonly IMemberInfo _Member;
        private readonly IMemberInfo _WeightField;

        /// <summary>
        /// Initalizes a new instance of the <see cref="SuggestWeightFieldValueProvider"/> class.
        /// </summary>
        /// <param name="member">Member Info for the suggest field</param>
        /// <param name="fieldName">ElasticSearch field name</param>
        public SuggestWeightFieldValueProvider(MemberInfo member, string fieldName)
        {
            var ci = BYteWareTypeInfo.GetBYteWareTypeInfo(member.DeclaringType);
            _Member = ci.TypeInfo?.FindMember(member.Name);
            _WeightField = ci.ESSuggestFields.First(t => t.FieldName == fieldName).WeightField;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="target">The target to get the value from.</param>
        /// <returns>The value.</returns>
        public object GetValue(object target) => new
        {
            input = _Member?.GetValue(target),
            weight = _WeightField?.GetValue(target),
        };

        /// <summary>
        /// Sets the value.
        /// </summary>
        /// <param name="target">The target to set the value on.</param>
        /// <param name="value">The value to set on the target.</param>
        public void SetValue(object target, object value)
        {
            throw new NotImplementedException();
        }
    }
}