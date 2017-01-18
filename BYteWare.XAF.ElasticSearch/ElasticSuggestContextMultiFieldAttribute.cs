namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines a context for an Elasticsearch suggest field of an additional(=multi) field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class ElasticSuggestContextMultiFieldAttribute : SuggestContextAttribute
    {
        /// <summary>
        /// Initalizes a new instance of the <see cref="ElasticSuggestContextMultiFieldAttribute"/> class.
        /// </summary>
        /// <param name="fieldName">Name of the multi field</param>
        /// <param name="contextName">Name for the context</param>
        /// <param name="pathField">Name of the field to use as content for the context</param>
        /// <param name="queryParameter">Name of the parameter whose value will be used on querying for suggestions</param>
        public ElasticSuggestContextMultiFieldAttribute(string fieldName, string contextName, string pathField, string queryParameter)
            : base(contextName, pathField, queryParameter)
        {
            FieldName = fieldName;
        }

        /// <summary>
        /// Name of the multi field
        /// </summary>
        public string FieldName
        {
            get;
        }
    }
}