namespace BYteWare.XAF.ElasticSearch
{
    using BYteWare.XAF.ElasticSearch.Model;
    using DevExpress.Persistent.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

#pragma warning disable SA1300
    /// <summary>
    /// List of context types
    /// </summary>
    public enum SuggestContextType
    {
        /// <summary>
        /// The category context allows you to associate one or more categories with suggestions at index time. At query time, suggestions can be filtered and boosted by their associated categories.
        /// </summary>
        category,

        /// <summary>
        /// A geo context allows you to associate one or more geo points or geohashes with suggestions at index time. At query time, suggestions can be filtered and boosted if they are within a certain distance of a specified geo location.
        /// </summary>
        geo,
    }
#pragma warning restore SA1300

    /// <summary>
    /// Base class to defines a context for an Elasticsearch suggest field
    /// </summary>
    public abstract class SuggestContextAttribute : Attribute, IElasticSearchSuggestContext
    {
        /// <summary>
        /// Name for the context
        /// </summary>
        public string ContextName
        {
            get;
            set;
        }

        /// <summary>
        /// The context type, category or geo
        /// </summary>
        public SuggestContextType ContextType
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the field to use as content for the context
        /// </summary>
        public string PathField
        {
            get;
            set;
        }

        /// <summary>
        /// This defines the precision of the geohash to be indexed and can be specified as a distance value (5m, 10km etc.), or as a raw geohash precision (1..12). Defaults to a raw geohash precision value of 6.
        /// </summary>
        public string Precision
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the parameter whose value will be used on querying for suggestions
        /// </summary>
        public string QueryParameter
        {
            get;
            set;
        }

        /// <summary>
        /// Initalizes a new instance of the <see cref="SuggestContextAttribute"/> class.
        /// </summary>
        /// <param name="contextName">Name for the context</param>
        /// <param name="pathField">Name of the field to use as content for the context</param>
        /// <param name="queryParameter">Name of the parameter whose value will be used on querying for suggestions</param>
        protected SuggestContextAttribute(string contextName, string pathField, string queryParameter)
        {
            ContextName = contextName;
            PathField = pathField;
            QueryParameter = queryParameter;
        }
    }
}