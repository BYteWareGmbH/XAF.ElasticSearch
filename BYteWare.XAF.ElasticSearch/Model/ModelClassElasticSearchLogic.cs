namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.DC;
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Methods for the IModelClassElasticSearch Model Interface
    /// </summary>
    [CLSCompliant(false)]
    [DomainLogic(typeof(IModelClassElasticSearch))]
    public static class ModelClassElasticSearchLogic
    {
        /// <summary>
        /// Returns an Enumeration of all defined ElasticSearch Indexes
        /// </summary>
        /// <param name="modelClassES">IModelClassElasticSearch instance</param>
        /// <returns>Enumeration of all defined ElasticSearch Indexes</returns>
        public static IEnumerable<IModelElasticSearchIndex> Get_ElasticSearchIndexes(IModelClassElasticSearch modelClassES)
        {
            if (modelClassES is IModelClass model && model.Application is IModelApplicationElasticSearch esApplication)
            {
                return esApplication.ElasticSearch.Indexes;
            }

            return Enumerable.Empty<IModelElasticSearchIndex>();
        }
    }
}
