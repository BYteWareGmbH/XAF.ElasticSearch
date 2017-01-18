namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.DC;
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Methods for the IModelElasticSearchFieldsItem Model Interface
    /// </summary>
    [CLSCompliant(false)]
    [DomainLogic(typeof(IModelElasticSearchFieldsItem))]
    public static class ModelElasticSearchFieldsItemLogic
    {
        /// <summary>
        /// Returns the Type Info for a IModelElasticSearchFieldsItem instance
        /// </summary>
        /// <param name="elasticSearchFieldsItem">IModelElasticSearchFieldsItem instance</param>
        /// <returns>Type Info for a IModelElasticSearchFieldsItem instance</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(XAF))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = nameof(XAF))]
        public static ITypeInfo Get_TypeInfo(IModelElasticSearchFieldsItem elasticSearchFieldsItem)
        {
            if (elasticSearchFieldsItem != null)
            {
                var modelElasticSearchFieldsList = elasticSearchFieldsItem.Parent as IModelElasticSearchFieldsList;
                if (modelElasticSearchFieldsList != null)
                {
                    var modelClassElasticSearchFilterSettings = modelElasticSearchFieldsList.Parent as IModelElasticSearchFilterSettings;
                    if (modelClassElasticSearchFilterSettings != null)
                    {
                        var modelClass = modelClassElasticSearchFilterSettings as IModelClass;
                        if (modelClass != null)
                        {
                            return modelClass.TypeInfo;
                        }
                        var modelListView = modelClassElasticSearchFilterSettings as IModelListView;
                        if (modelListView != null && modelListView.ModelClass != null)
                        {
                            return modelListView.ModelClass.TypeInfo;
                        }
                    }
                }
            }
            return null;
        }
    }
}
