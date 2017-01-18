namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.DC;
    using ElasticSearch;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(XAF))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = nameof(XAF))]
        public static IList<string> Get_ElasticSearchSuggestFields(IModelElasticSearchSuggestField suggestField)
        {
            if (suggestField != null)
            {
                var modelElasticSearchSuggestFieldList = suggestField.Parent as IModelElasticSearchSuggestFieldList;
                if (modelElasticSearchSuggestFieldList != null)
                {
                    var modelElasticSearchFieldsItem = modelElasticSearchSuggestFieldList.Parent as IModelElasticSearchFieldsItem;
                    if (modelElasticSearchFieldsItem != null)
                    {
                        var typeInfo = modelElasticSearchFieldsItem.TypeInfo;
                        if (typeInfo != null && typeInfo.Type != null)
                        {
                            return ElasticSearchClient.ElasticSearchSuggestFields(typeInfo).ToList();
                        }
                    }
                }
            }
            return new List<string>();
        }
    }
}