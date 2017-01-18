namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using DevExpress.Persistent.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// ElasticSearch index settings for the business class
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelClassElasticSearch
    {
        /// <summary>
        /// The ElasticSearch Index
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The ElasticSearch Index")]
        [DataSourceProperty(nameof(ElasticSearchIndexes))]
        IModelElasticSearchIndex ElasticSearchIndex
        {
            get;
            set;
        }

        /// <summary>
        /// Name for the ElasticSearch Type, defaults to the Classname
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Name for the ElasticSearch Type, defaults to the Classname")]
        string TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// Disables the storage of the source Json string
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Disables the storage of the source Json string")]
        bool SourceFieldDisabled
        {
            get;
            set;
        }

        /// <summary>
        /// Returns an enumeration of all defined ElasticSearch Indexes
        /// </summary>
        [Browsable(false)]
        IEnumerable<IModelElasticSearchIndex> ElasticSearchIndexes
        {
            get;
        }
    }
}
