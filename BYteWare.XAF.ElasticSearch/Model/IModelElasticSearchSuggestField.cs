namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using DevExpress.Persistent.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// ElasticSearch Suggest Field Model Settings
    /// </summary>
    [KeyProperty(nameof(ElasticSearchSuggestField))]
    [CLSCompliant(false)]
    public interface IModelElasticSearchSuggestField : IModelNode
    {
        /// <summary>
        /// Name of the ElasticSearch Suggest Field
        /// </summary>
        [Required]
        [DataSourceProperty(nameof(ElasticSearchSuggestFields))]
        [Category(nameof(ElasticSearch))]
        [Description("Name of the ElasticSearch Suggest Field")]
        string ElasticSearchSuggestField
        {
            get;
            set;
        }

        /// <summary>
        /// Maximum Number of Results to return for this field
        /// </summary>
        [DefaultValue(7)]
        [Category(nameof(ElasticSearch))]
        [Description("Maximum Number of Results to return for this field")]
        int Results
        {
            get;
            set;
        }

        /// <summary>
        /// Should a fuzzy search be performed
        /// </summary>
        [DefaultValue(true)]
        [Category(nameof(ElasticSearch))]
        [Description("Should a fuzzy search be performed")]
        bool Fuzzy
        {
            get;
            set;
        }

        /// <summary>
        /// List of contexts settings for the suggest field
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("List of contexts settings for the suggest field")]
        IModelElasticSearchSuggestContextList Contexts
        {
            get;
        }

        /// <summary>
        /// List of potential ElasticSearch Suggest field names
        /// </summary>
        [Browsable(false)]
        IList<string> ElasticSearchSuggestFields
        {
            get;
        }
    }
}
