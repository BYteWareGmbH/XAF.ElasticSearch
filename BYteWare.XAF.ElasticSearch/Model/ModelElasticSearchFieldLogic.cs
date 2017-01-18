namespace BYteWare.XAF.ElasticSearch.Model
{
    using BYteWare.Utils.Extension;
    using DevExpress.ExpressApp.DC;
    using ElasticSearch;
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(XAF))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = nameof(XAF))]
        public static IEnumerable<string> Get_ElasticSearchFields(IModelElasticSearchField elasticSearchField)
        {
            if (elasticSearchField != null)
            {
                var modelElasticSearchFieldList = elasticSearchField.Parent as IModelElasticSearchFieldList;
                if (modelElasticSearchFieldList != null)
                {
                    var modelElasticSearchFieldsItem = modelElasticSearchFieldList.Parent as IModelElasticSearchFieldsItem;
                    if (modelElasticSearchFieldsItem != null)
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
                }
            }
            return Enumerable.Empty<string>();
        }
    }
}