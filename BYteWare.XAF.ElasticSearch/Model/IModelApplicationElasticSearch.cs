namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Application ElasticSearch Model Entry
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelApplicationElasticSearch
    {
        /// <summary>
        /// ElasticSearch Model Settings
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("ElasticSearch Model Settings")]
        IModelElasticSearch ElasticSearch
        {
            get;
        }
    }
}
