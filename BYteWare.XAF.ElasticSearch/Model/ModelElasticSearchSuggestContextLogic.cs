namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.DC;
    using ElasticSearch;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Methods for the IModelElasticSearchSuggestContext Model Interface
    /// </summary>
    [CLSCompliant(false)]
    [DomainLogic(typeof(IModelElasticSearchSuggestContext))]
    public static class ModelElasticSearchSuggestContextLogic
    {
        /// <summary>
        /// Returns a List of all potential Context names
        /// </summary>
        /// <param name="suggestContext">IModelElasticSearchSuggestContext instance</param>
        /// <returns>List of all potential Context names</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(XAF))]
        public static IList<string> Get_ElasticSearchSuggestFieldContexts(IModelElasticSearchSuggestContext suggestContext)
        {
            if (suggestContext != null)
            {
                var typeInfo = suggestContext.TypeInfo;
                if (typeInfo != null && typeInfo.Type != null)
                {
                    return ElasticSearchClient.ElasticSearchSuggestFieldContexts(typeInfo, suggestContext.ModelElasticSearchSuggestField.ElasticSearchSuggestField).ToList();
                }
            }
            return new List<string>();
        }

        /// <summary>
        /// Returns the Name of the Parameter whose value should be used for the context that was defined through the Suggest attribute
        /// </summary>
        /// <param name="suggestContext">IModelElasticSearchSuggestContext instance</param>
        /// <returns>Name of the Parameter whose value should be used for the context that was defined through the Suggest attribute</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(XAF))]
        public static string Get_DefaultParameter(IModelElasticSearchSuggestContext suggestContext)
        {
            if (suggestContext != null)
            {
                var typeInfo = suggestContext.TypeInfo;
                if (typeInfo != null && typeInfo.Type != null)
                {
                    return ElasticSearchClient.ElasticSearchSuggestFieldContextParameter(typeInfo, suggestContext.ModelElasticSearchSuggestField.ElasticSearchSuggestField, suggestContext.Name);
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the ElasticSearch Suggest Field Model Settings
        /// </summary>
        /// <param name="suggestContext">IModelElasticSearchSuggestContext instance</param>
        /// <returns>ElasticSearch Suggest Field Model Settings</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(XAF))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = nameof(XAF))]
        public static IModelElasticSearchSuggestField Get_ModelElasticSearchSuggestField(IModelElasticSearchSuggestContext suggestContext)
        {
            if (suggestContext != null)
            {
                var modelElasticSearchSuggestContextList = suggestContext.Parent as IModelElasticSearchSuggestContextList;
                if (modelElasticSearchSuggestContextList != null)
                {
                    return modelElasticSearchSuggestContextList.Parent as IModelElasticSearchSuggestField;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns The Type Info of the Business Class
        /// </summary>
        /// <param name="suggestContext">IModelElasticSearchSuggestContext instance</param>
        /// <returns>The Type Info of the Business Class</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(XAF))]
        public static ITypeInfo Get_TypeInfo(IModelElasticSearchSuggestContext suggestContext)
        {
            if (suggestContext != null)
            {
                var modelElasticSearchSuggestField = suggestContext.ModelElasticSearchSuggestField;
                if (modelElasticSearchSuggestField != null)
                {
                    var modelElasticSearchSuggestFieldList = modelElasticSearchSuggestField.Parent as IModelElasticSearchSuggestFieldList;
                    if (modelElasticSearchSuggestFieldList != null)
                    {
                        var modelElasticSearchFieldsItem = modelElasticSearchSuggestFieldList.Parent as IModelElasticSearchFieldsItem;
                        if (modelElasticSearchFieldsItem != null)
                        {
                            return modelElasticSearchFieldsItem.TypeInfo;
                        }
                    }
                }
            }
            return null;
        }
    }
}