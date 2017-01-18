namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// List of ElasticSearch Index settings
    /// </summary>
    [CLSCompliant(false)]
    [ModelNodesGenerator(typeof(ElasticSearchIndexGenerator))]
    public interface IModelElasticSearchIndexes : IModelNode, IModelList<IModelElasticSearchIndex>
    {
    }
}
