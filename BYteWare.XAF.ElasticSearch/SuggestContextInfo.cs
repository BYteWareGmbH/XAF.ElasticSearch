namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Suggest Context Informations
    /// </summary>
    public class SuggestContextInfo : IElasticSearchSuggestContext
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SuggestContextInfo"/> class.
        /// </summary>
        /// <param name="source">Source Suggest settings</param>
        public SuggestContextInfo(IElasticSearchSuggestContext source)
        {
            ContextName = source.ContextName;
            ContextType = source.ContextType;
            PathField = source.PathField;
            Precision = source.Precision;
            QueryParameter = source.QueryParameter;
        }

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
        /// Path Field Info
        /// </summary>
        public SuggestContextPathFieldInfo ContextPathFieldInfo
        {
            get;
            set;
        }
    }
}
