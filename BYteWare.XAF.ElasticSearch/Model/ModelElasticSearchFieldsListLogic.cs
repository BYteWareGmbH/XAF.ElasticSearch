namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.DC;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Methods for the IModelElasticSearchFieldsList Model Interface
    /// </summary>
    [CLSCompliant(false)]
    [DomainLogic(typeof(IModelElasticSearchFieldsList))]
    public static class ModelElasticSearchFieldsListLogic
    {
        /// <summary>
        /// Returns a List of Filter Field Action item names
        /// </summary>
        /// <param name="model">IModelElasticSearchFieldsList instance</param>
        /// <returns>List of Filter Field Action item names</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(XAF))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = nameof(XAF))]
        public static List<string> Get_ElasticSearchFieldsList(IModelElasticSearchFieldsList model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var elasticSearchFields = new List<string>();
            foreach (IModelElasticSearchFieldsItem filterItem in model)
            {
                elasticSearchFields.Add(filterItem.Id);
            }
            return elasticSearchFields;
        }
    }
}
