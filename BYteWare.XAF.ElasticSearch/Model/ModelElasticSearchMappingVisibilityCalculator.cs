namespace BYteWare.XAF.ElasticSearch.Model
{
    using BYteWare.Utils.Extension;
    using DevExpress.ExpressApp.Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Determines visibility of ElasticSearch mapping properties
    /// </summary>
    [CLSCompliant(false)]
    public class ModelElasticSearchMappingVisibilityCalculator : IModelIsVisible
    {
        /// <summary>
        /// Determines visibility of ElasticSearch mapping properties dependend on field_type
        /// </summary>
        /// <param name="node">The Model node to check</param>
        /// <param name="propertyName">name of the property to be checked</param>
        /// <returns>True if the Property should be visible; False otherwise</returns>
        public bool IsVisible(IModelNode node, string propertyName)
        {
            if (node is IModelElasticSearchFieldProperties esProperties)
            {
                var fieldType = esProperties.FieldType;
                switch (propertyName)
                {
                    case nameof(IModelMemberElasticSearchField.WeightFieldMember):
                        return fieldType == FieldType.completion || fieldType == FieldType.text;
                    case nameof(IModelElasticSearchFieldProperties.Analyzer):
                        return fieldType == FieldType.completion || fieldType == FieldType.text;
                    case nameof(IModelElasticSearchFieldProperties.Normalizer):
                        return fieldType == FieldType.keyword;
                    case nameof(IModelElasticSearchFieldProperties.DocValues):
                        return !(fieldType == FieldType.completion || fieldType == FieldType.text);
                    case nameof(IModelElasticSearchFieldProperties.EagerGlobalOrdinals):
                        return fieldType == FieldType.keyword || fieldType == FieldType.text;
                    case nameof(IModelElasticSearchFieldProperties.FieldData):
                        return fieldType == FieldType.text;
                    case nameof(IModelElasticSearchFieldProperties.Norms):
                        return fieldType == FieldType.keyword || fieldType == FieldType.text;
                    case nameof(IModelElasticSearchFieldProperties.PositionOffsetGap):
                        return fieldType == FieldType.text;
                    case nameof(IModelElasticSearchFieldProperties.SearchAnalyzer):
                        return fieldType == FieldType.keyword || fieldType == FieldType.text || fieldType == FieldType.completion;
                    case nameof(IModelElasticSearchFieldProperties.SearchQuoteAnalyzer):
                        return fieldType == FieldType.text;
                    case nameof(IModelElasticSearchFieldProperties.Similarity):
                        return fieldType == FieldType.keyword || fieldType == FieldType.text;
                    case nameof(IModelElasticSearchFieldProperties.TermVector):
                        return fieldType == FieldType.text;
                    case nameof(IModelElasticSearchFieldProperties.IgnoreAbove):
                        return fieldType == FieldType.keyword;
                    case nameof(IModelElasticSearchFieldProperties.Coerce):
                        return fieldType.In(FieldType.byte_type, FieldType.double_type, FieldType.float_type, FieldType.half_float, FieldType.integer_type, FieldType.long_type, FieldType.scaled_float, FieldType.short_type);
                    case nameof(IModelElasticSearchFieldProperties.IgnoreMalformed):
                        return fieldType.In(FieldType.byte_type, FieldType.double_type, FieldType.float_type, FieldType.half_float, FieldType.integer_type, FieldType.long_type, FieldType.scaled_float, FieldType.short_type, FieldType.date_type);
                    case nameof(IModelElasticSearchFieldProperties.ScalingFactor):
                        return fieldType == FieldType.scaled_float;
                    case nameof(IModelElasticSearchFieldProperties.DateFormat):
                        return fieldType == FieldType.date_type;
                    case nameof(IModelElasticSearchFieldProperties.Locale):
                        return fieldType == FieldType.date_type;
                    case nameof(IModelElasticSearchFieldProperties.PreserveSeparators):
                        return fieldType == FieldType.completion;
                    case nameof(IModelElasticSearchFieldProperties.PreservePositionIncrements):
                        return fieldType == FieldType.completion;
                    case nameof(IModelElasticSearchFieldProperties.MaxInputLength):
                        return fieldType == FieldType.completion;
                    case nameof(IModelElasticSearchFieldProperties.DefaultSuggestField):
                        return fieldType == FieldType.completion;
                    default:
                        break;
                }
            }
            return false;
        }
    }
}
