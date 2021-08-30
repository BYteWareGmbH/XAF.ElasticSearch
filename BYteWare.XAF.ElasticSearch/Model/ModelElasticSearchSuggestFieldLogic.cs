namespace BYteWare.XAF.ElasticSearch.Model
{
    using BYteWare.XAF.ElasticSearch;
    using DevExpress.ExpressApp.DC;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Methods for the IModelElasticSearchSuggestField Model Interface
    /// </summary>
    [CLSCompliant(false)]
    [DomainLogic(typeof(IModelElasticSearchSuggestField))]
    public static class ModelElasticSearchSuggestFieldLogic
    {
        /// <summary>
        /// Returns a List of potential ElasticSearch Suggest field names
        /// </summary>
        /// <param name="suggestField">IModelElasticSearchSuggestField instance</param>
        /// <returns>List of potential ElasticSearch Suggest field names</returns>
        public static IList<string> Get_ElasticSearchSuggestFields(IModelElasticSearchSuggestField suggestField)
        {
            if (suggestField != null && suggestField.Parent is IModelElasticSearchSuggestFieldList modelElasticSearchSuggestFieldList && modelElasticSearchSuggestFieldList.Parent is IModelElasticSearchFieldsItem modelElasticSearchFieldsItem)
            {
                var typeInfo = modelElasticSearchFieldsItem.TypeInfo;
                if (typeInfo != null && typeInfo.Type != null)
                {
                    return ElasticSearchClient.ElasticSearchSuggestFields(typeInfo).ToList();
                }
            }
            return new List<string>();
        }
    }
}