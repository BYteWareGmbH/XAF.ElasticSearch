namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.SystemModule;
    using DevExpress.ExpressApp.Utils;
    using System;
    using System.ComponentModel;
    using System.Drawing.Design;

    /// <summary>
    /// Extends the List View Filter settings with ElasticSearch filter strings
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelListViewFilterItemElasticSearch
    {
        /// <summary>
        /// Filter string in ElasticSearch syntax, to filter the ElasticSearch results to the filter used in the list view
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Filter string in ElasticSearch syntax, to filter the ElasticSearch results to the filter used in the list view")]
        [RefreshProperties(RefreshProperties.All)]
        [Editor(Constants.MultilineStringEditorType, typeof(UITypeEditor))]
        string ElasticSearchFilter
        {
            get;
            set;
        }

        /// <summary>
        /// Returns validation errors for the ElasticSearchFilter, if there are any
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Returns validation errors for the ElasticSearchFilter, if there are any")]
        [Editor(Constants.MultilineStringEditorType, typeof(UITypeEditor))]
        string ElasticSearchFilterSyntax
        {
            get;
            set;
        }
    }
}