namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using DevExpress.Persistent.Base;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// List of Filter Field Action items
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelElasticSearchFieldsList : IModelNode, IModelList<IModelElasticSearchFieldsItem>
    {
        /// <summary>
        /// Default Filter Field action settings to use
        /// </summary>
        [DataSourceProperty("this")]
        [ModelPersistentName("CurrentElasticSearchFieldsId")]
        [Category("Behavior")]
        [Description("Default Filter Field action settings to use")]
        IModelElasticSearchFieldsItem DefaultElasticSearchFields
        {
            get;
            set;
        }
    }
}
