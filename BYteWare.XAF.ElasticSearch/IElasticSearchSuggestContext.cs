namespace BYteWare.XAF.ElasticSearch
{
    using DevExpress.Persistent.Base;
    using Model;
    using System;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// ElasticSearch Suggest Context settings
    /// </summary>
    public interface IElasticSearchSuggestContext
    {
        /// <summary>
        /// Name for the context
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Name for the context")]
        string ContextName
        {
            get;
            set;
        }

        /// <summary>
        /// The context type, category or geo
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The context type, category or geo")]
        SuggestContextType ContextType
        {
            get;
            set;
        }

        /// <summary>
        /// ElasticSearch Field to use as content for the context
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("ElasticSearch Field to use as content for the context")]
        [DataSourceProperty(nameof(IModelMemberElasticSearchSuggestContext.ElasticSearchFields))]
        string PathField
        {
            get;
            set;
        }

        /// <summary>
        /// This defines the precision of the geohash to be indexed and can be specified as a distance value (5m, 10km etc.), or as a raw geohash precision (1..12). Defaults to a raw geohash precision value of 6.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("This defines the precision of the geohash to be indexed and can be specified as a distance value (5m, 10km etc.), or as a raw geohash precision (1..12). Defaults to a raw geohash precision value of 6.")]
        string Precision
        {
            get;
            set;
        }

        /// <summary>
        /// Name of the parameter whose value will be used on querying for suggestions
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Name of the parameter whose value will be used on querying for suggestions.")]
        [TypeConverter(typeof(DefaultElasticSearchParameterConverter))]
        string QueryParameter
        {
            get;
            set;
        }
    }
}