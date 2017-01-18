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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(XAF))]
        public static IEnumerable<IModelElasticSearchIndex> Get_ElasticSearchIndexes(IModelClassElasticSearch modelClassES)
        {
            var model = modelClassES as IModelClass;
            if (model != null)
            {
                var esApplication = model.Application as IModelApplicationElasticSearch;
                if (esApplication != null)
                {
                    return esApplication.ElasticSearch.Indexes;
                }
            }
            return Enumerable.Empty<IModelElasticSearchIndex>();
        }
    }
}
