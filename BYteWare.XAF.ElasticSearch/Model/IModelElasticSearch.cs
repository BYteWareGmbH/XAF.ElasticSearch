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
    /// ElasticSearch Model Settings
    /// </summary>
    [CLSCompliant(false)]
    public interface IModelElasticSearch : IModelNode
    {
        /// <summary>
        /// List of ElasticSearch Index settings
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("List of ElasticSearch Index settings")]
        IModelElasticSearchIndexes Indexes
        {
            get;
        }
    }
}
