namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Elasticsearch Model Settings for Business Classes and List Views
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelElasticSearchFilterSettings
    {
        /// <summary>
        /// Don't display a record until a full text filter is set
        /// </summary>
        [Category("Filter")]
        [Description("Don't display a record until a full text filter is set")]
        bool OnlyLoadWhenFullTextFilter
        {
            get;
            set;
        }

        /// <summary>
        /// Don't display a record until a user filter is set
        /// </summary>
        [Category("Filter")]
        [Description("Don't display a record until a user filter is set")]
        bool OnlyLoadWhenUserFilter
        {
            get;
            set;
        }

        /// <summary>
        /// List of Filter Field Model settings to search in
        /// </summary>
        [Description("List of Filter Field Model settings to search in")]
        IModelElasticSearchFieldsList ElasticSearchFieldsList
        {
            get;
        }

        /// <summary>
        /// Maximum Number of ElasticSearch Results
        /// </summary>
        [DefaultValue(200)]
        [Category(nameof(ElasticSearch))]
        [Description("Maximum Number of ElasticSearch Results")]
        int ElasticSearchResults
        {
            get;
            set;
        }
    }
}
