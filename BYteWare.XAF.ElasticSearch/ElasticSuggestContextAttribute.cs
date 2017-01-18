namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines a context for an Elasticsearch suggest field
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class ElasticSuggestContextAttribute : SuggestContextAttribute
    {
        /// <summary>
        /// Initalizes a new instance of the <see cref="ElasticSuggestContextAttribute"/> class.
        /// </summary>
        /// <param name="contextName">Name for the context</param>
        /// <param name="pathField">Name of the field to use as content for the context</param>
        /// <param name="queryParameter">Name of the parameter whose value will be used on querying for suggestions</param>
        public ElasticSuggestContextAttribute(string contextName, string pathField, string queryParameter)
            : base(contextName, pathField, queryParameter)
        {
        }
    }
}