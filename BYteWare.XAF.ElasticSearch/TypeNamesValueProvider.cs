namespace BYteWare.XAF.ElasticSearch
{
    using BYteWare.Utils.Extension;
    using Newtonsoft.Json.Serialization;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Json.Net Value Provider to supply the type name for the Suggest Type context
    /// </summary>
    internal class TypeNameValueProvider : IValueProvider
    {
        private readonly string[] _Types;

        /// <summary>
        /// Initalizes a new instance of the <see cref="TypeNameValueProvider"/> class.
        /// </summary>
        /// <param name="typeName">Type Name to return</param>
        public TypeNameValueProvider(string typeName)
        {
            _Types = new string[] { typeName };
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="target">The target to get the value from.</param>
        /// <returns>The value.</returns>
        public object GetValue(object target)
        {
            return _Types;
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