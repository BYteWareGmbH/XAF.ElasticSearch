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
        public static ITypeInfo Get_TypeInfo(IModelElasticSearchFieldsItem elasticSearchFieldsItem)
        {
            if (elasticSearchFieldsItem != null && elasticSearchFieldsItem.Parent is IModelElasticSearchFieldsList modelElasticSearchFieldsList && modelElasticSearchFieldsList.Parent is IModelElasticSearchFilterSettings modelClassElasticSearchFilterSettings)
            {
                if (modelClassElasticSearchFilterSettings is IModelClass modelClass)
                {
                    return modelClass.TypeInfo;
                }
                if (modelClassElasticSearchFilterSettings is IModelListView modelListView && modelListView.ModelClass != null)
                {
                    return modelListView.ModelClass.TypeInfo;
                }
            }
            return null;
        }
    }
}
