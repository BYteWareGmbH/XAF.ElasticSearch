namespace BYteWare.XAF.ElasticSearch.Model
{
    using BYteWare.Utils.Extension;
    using BYteWare.XAF.ElasticSearch;
    using DevExpress.ExpressApp.DC;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Methods for the IModelElasticSearchField Model Interface
    /// </summary>
    [CLSCompliant(false)]
    [DomainLogic(typeof(IModelElasticSearchField))]
    public static class ModelElasticSearchFieldLogic
    {
        /// <summary>
        /// Returns an Enumeration of potential ElasticSearch Field Names
        /// </summary>
        /// <param name="elasticSearchField">IModelElasticSearchField instance</param>
        /// <returns>Enumeration of potential ElasticSearch Field Names</returns>
        public static IEnumerable<string> Get_ElasticSearchFields(IModelElasticSearchField elasticSearchField)
        {
            if (elasticSearchField != null && elasticSearchField.Parent is IModelElasticSearchFieldList modelElasticSearchFieldList && modelElasticSearchFieldList.Parent is IModelElasticSearchFieldsItem modelElasticSearchFieldsItem)
            {
                var typeInfo = modelElasticSearchFieldsItem.TypeInfo;
                if (typeInfo != null && typeInfo.Type != null)
                {
                    var esFields = new HashSet<string>();
                    foreach (var ti in typeInfo.DescendantsAndSelf(t => t.Descendants))
                    {
                        esFields.UnionWith(ElasticSearchClient.ElasticSearchFields(ti.Type, true));
                    }
                    return esFields;
                }
            }
            return Enumerable.Empty<string>();
        }
    }
}