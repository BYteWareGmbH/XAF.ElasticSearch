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

        public bool IsReadOnly(IModelNode node, string propertyName)
        {
            return false;
        }
    }
}