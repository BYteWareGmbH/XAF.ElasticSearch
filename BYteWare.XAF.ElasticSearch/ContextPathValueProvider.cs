namespace BYteWare.XAF.ElasticSearch
{
    using BYteWare.Utils.Extension;
    using DevExpress.Xpo.Metadata;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Json.Net Value Provider to supply the type name for the Suggest Type context
    /// </summary>
    internal class ContextPathValueProvider : IValueProvider
    {
        private readonly XPMemberInfo _Member;

        /// <summary>
        /// Initalizes a new instance of the <see cref="ContextPathValueProvider"/> class.
        /// </summary>
        /// <param name="pathFieldInfo">Path Field Informations</param>
        public ContextPathValueProvider(SuggestContextPathFieldInfo pathFieldInfo)
        {
            _Member = pathFieldInfo.MemberInfo;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="target">The target to get the value from.</param>
        /// <returns>The value.</returns>
        public object GetValue(object target)
        {
            if (_Member != null)
            {
                var v = _Member.GetValue(target);
                var e = v as IEnumerable;
                if (e != null)
                {
                    return e.Cast<object>().Select(o => o?.ToString()).Where(s => !string.IsNullOrEmpty(s));
                }
                return new string[] { v.ToString() };
            }
            return null;
        }

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