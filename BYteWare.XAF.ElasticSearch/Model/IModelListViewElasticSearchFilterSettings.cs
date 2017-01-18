namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// Elasticsearch Model Settings for List Views
    /// </summary>
    [CLSCompliant(false)]
    [ModelInterfaceImplementor(typeof(IModelElasticSearchFilterSettings), "ModelClass")]
    public interface IModelListViewElasticSearchFilterSettings : IModelElasticSearchFilterSettings
    {
        /// <summary>
        /// Refresh the data before issuing a Search
        /// </summary>
        [Category("Filter")]
        [Description("Refresh the data before issuing a Search")]
        bool RefreshBeforeSearch
        {
            get;
            set;
        }
    }
}
