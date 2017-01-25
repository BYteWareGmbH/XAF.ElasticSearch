namespace BYteWare.XAF.ElasticSearch.Model
{
    using System;
    using DevExpress.ExpressApp.Model;

    /// <summary>
    /// Determines if the Suggest Contexts Node should be Read Only, becuase Visibility doens't work dynamically
    /// </summary>
    [CLSCompliant(false)]
    public class ModelSuggestContextsReadOnlyCalculator : IModelIsReadOnly
    {
        /// <summary>
        /// Should the Child Node be Read Only
        /// </summary>
        /// <param name="node">IModelMemberElasticSearchSuggestContexts instance</param>
        /// <param name="childNode">Child Node to check</param>
        /// <returns>True if the Elastic search Field Type is completion; False otherwise</returns>
        public bool IsReadOnly(IModelNode node, IModelNode childNode)
        {
            var result = false;
            var props = node?.Parent as IModelElasticSearchFieldProperties;
            if (props != null)
            {
                result = props.FieldType != FieldType.completion;
            }
            return result;
        }

        /// <summary>
        /// Should the Property be Read Only, not used in this context, because this class is only used for a list
        /// </summary>
        /// <param name="node">IModelMemberElasticSearchSuggestContexts instance</param>
        /// <param name="propertyName">Name of the Propterty to be checked</param>
        /// <returns>Always returns false</returns>
        public bool IsReadOnly(IModelNode node, string propertyName)
        {
            return false;
        }
    }
}