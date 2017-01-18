namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using DevExpress.ExpressApp.Utils;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Drawing.Design;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// ElasticSearch Index settings
    /// </summary>
    [CLSCompliant(false)]
    [KeyProperty(nameof(Name))]
    public interface IModelElasticSearchIndex : IModelNode
    {
        /// <summary>
        /// Index Name
        /// </summary>
        [Required]
        [Category(nameof(ElasticSearch))]
        [Description("The ElasticSearch Index Name")]
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// ElasticSearch index settings without analysis part and 'settings' string. E. g. "index.codec":"best_compression","index.number_of_shards":10
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("ElasticSearch index settings without analysis part and 'settings' string. E. g. \"index.codec\":\"best_compression\",\"index.number_of_shards\":10")]
        [Editor(Constants.MultilineStringEditorType, typeof(UITypeEditor))]
        string Settings
        {
            get;
            set;
        }

        /// <summary>
        /// ElasticSearch analysis char_filter definitions delimited by comma
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("ElasticSearch analysis char_filter definitions delimited by comma")]
        [Editor(Constants.MultilineStringEditorType, typeof(UITypeEditor))]
        string CharFilters
        {
            get;
            set;
        }

        /// <summary>
        /// ElasticSearch analysis tokenizer definitions delimited by comma
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("ElasticSearch analysis tokenizer definitions delimited by comma")]
        [Editor(Constants.MultilineStringEditorType, typeof(UITypeEditor))]
        string Tokenizers
        {
            get;
            set;
        }

        /// <summary>
        /// ElasticSearch analysis token filter definitions delimited by comma
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("ElasticSearch analysis token filter definitions delimited by comma")]
        [Editor(Constants.MultilineStringEditorType, typeof(UITypeEditor))]
        string TokenFilters
        {
            get;
            set;
        }

        /// <summary>
        /// ElasticSearch analysis analyzer definitions delimited by comma
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("ElasticSearch analysis analyzer definitions delimited by comma")]
        [Editor(Constants.MultilineStringEditorType, typeof(UITypeEditor))]
        string Analyzers
        {
            get;
            set;
        }
    }
}
