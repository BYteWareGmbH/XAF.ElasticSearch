namespace BYteWare.XAF.ElasticSearch
{
    using DevExpress.ExpressApp.Model;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Common ElasticSearch Field Settings
    /// </summary>
    public interface IElasticSearchFieldProperties
    {
        /// <summary>
        /// Name of the field
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("ElasticSearch Name for the field")]
        string FieldName
        {
            get;
            set;
        }

        /// <summary>
        /// The ElasticSearch Field Type
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The ElasticSearch Field Type")]
        [RefreshProperties(RefreshProperties.All)]
        FieldType? FieldType
        {
            get;
            set;
        }

        /// <summary>
        /// Text, Completion Type: The analyzer which should be used for analyzed string fields, both at index-time and at search-time (unless overridden by the search_analyzer). Defaults to the default index analyzer, or the standard analyzer.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The analyzer which should be used for analyzed string fields, both at index-time and at search-time (unless overridden by the search_analyzer). Defaults to the default index analyzer, or the standard analyzer.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        string Analyzer
        {
            get;
            set;
        }

        /// <summary>
        /// Text, Completion Type: The analyzer which should be used for analyzed string fields, both at index-time and at search-time (unless overridden by the search_analyzer). Defaults to the default index analyzer, or the standard analyzer.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The normalizer property of keyword fields is similar to analyzer except that it guarantees that the analysis chain produces a single token.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        string Normalizer
        {
            get;
            set;
        }

        /// <summary>
        /// Mapping field-level query time boosting. Accepts a floating point number, defaults to 1.0.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Mapping field-level query time boosting. Accepts a floating point number, defaults to 1.0.")]
        double? Boost
        {
            get;
            set;
        }

        /// <summary>
        /// Not Text Type: Should the field be stored on disk in a column-stride fashion, so that it can later be used for sorting, aggregations, or scripting? Accepts true (default) or false.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Should the field be stored on disk in a column-stride fashion, so that it can later be used for sorting, aggregations, or scripting? Accepts true (default) or false.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        bool? DocValues
        {
            get;
            set;
        }

        /// <summary>
        /// Text, Keyword Type: Should global ordinals be loaded eagerly on refresh? Accepts true or false (default). Enabling this is a good idea on fields that are frequently used for (significant) terms aggregations.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Should global ordinals be loaded eagerly on refresh? Accepts true or false (default). Enabling this is a good idea on fields that are frequently used for (significant) terms aggregations.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        bool? EagerGlobalOrdinals
        {
            get;
            set;
        }

        /// <summary>
        /// Text Type: Can the field use in-memory fielddata for sorting, aggregations, or scripting? Accepts true or false (default).
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Can the field use in-memory fielddata for sorting, aggregations, or scripting? Accepts true or false (default).")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        bool? FieldData
        {
            get;
            set;
        }

        /*/// <summary>
        /// Text Type: Expert settings which allow to decide which values to load in memory when fielddata is enabled. By default all values are loaded.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The ElasticSearch Index")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        public string FieldDataFrequencyFilter
        {
            get;
            set;
        }*/

        /// <summary>
        /// Should the field be searchable? Accepts true (default) or false.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Should the field be searchable? Accepts true (default) or false.")]
        bool? ESIndex
        {
            get;
            set;
        }

        /// <summary>
        /// What information should be stored in the index, for search and highlighting purposes. Defaults to positions.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("What information should be stored in the index, for search and highlighting purposes. Defaults to positions.")]
        IndexOptions? IndexOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Text, Keyword Type: Whether field-length should be taken into account when scoring queries. Accepts true (default) or false.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Whether field-length should be taken into account when scoring queries. Accepts true (default) or false.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        bool? Norms
        {
            get;
            set;
        }

        /// <summary>
        /// Text Type: The number of fake term position which should be inserted between each element of an array of strings. Defaults to the position_increment_gap configured on the analyzer which defaults to 100. 100 was chosen because it prevents phrase queries with reasonably large slops (less than 100) from matching terms across field values.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The number of fake term position which should be inserted between each element of an array of strings. Defaults to the position_increment_gap configured on the analyzer which defaults to 100. 100 was chosen because it prevents phrase queries with reasonably large slops (less than 100) from matching terms across field values.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        int? PositionOffsetGap
        {
            get;
            set;
        }

        /// <summary>
        /// Whether the field value should be stored and retrievable separately from the _source field. Accepts true or false (default).
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Whether the field value should be stored and retrievable separately from the _source field. Accepts true or false (default).")]
        bool? Store
        {
            get;
            set;
        }

        /// <summary>
        /// Text, Keyword, Completion Type: The analyzer that should be used at search time on analyzed fields. Defaults to the analyzer setting.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The analyzer that should be used at search time on analyzed fields. Defaults to the analyzer setting.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        string SearchAnalyzer
        {
            get;
            set;
        }

        /// <summary>
        /// Text Type: The analyzer that should be used at search time when a phrase is encountered. Defaults to the search_analyzer setting.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The analyzer that should be used at search time when a phrase is encountered. Defaults to the search_analyzer setting.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        string SearchQuoteAnalyzer
        {
            get;
            set;
        }

        /// <summary>
        /// Text, Keyword Type: Which scoring algorithm or similarity should be used. Defaults to classic, which uses TF/IDF.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Which scoring algorithm or similarity should be used. Defaults to classic, which uses TF/IDF.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        string Similarity
        {
            get;
            set;
        }

        /// <summary>
        /// Text Type: Whether term vectors should be stored for an analyzed field. Defaults to no.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Whether term vectors should be stored for an analyzed field. Defaults to no.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        TermVectorOption? TermVector
        {
            get;
            set;
        }

        /// <summary>
        /// Keyword Type: Do not index any string longer than this value. Defaults to 2147483647 so that all values would be accepted.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Do not index any string longer than this value. Defaults to 2147483647 so that all values would be accepted.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        int? IgnoreAbove
        {
            get;
            set;
        }

        /// <summary>
        /// Accepts a string value which is substituted for any explicit null values. Defaults to null, which means the field is treated as missing.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Accepts a string value which is substituted for any explicit null values. Defaults to null, which means the field is treated as missing.")]
        string NullValue
        {
            get;
            set;
        }

        /// <summary>
        /// Numeric Types: Try to convert strings to numbers and truncate fractions for integers. Accepts true (default) and false.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Try to convert strings to numbers and truncate fractions for integers. Accepts true (default) and false.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        bool? Coerce
        {
            get;
            set;
        }

        /// <summary>
        /// Numeric, Date Types: If true, malformed numbers are ignored. If false (default), malformed numbers throw an exception and reject the whole document.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("If true, malformed numbers are ignored. If false (default), malformed numbers throw an exception and reject the whole document.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        bool? IgnoreMalformed
        {
            get;
            set;
        }

        /// <summary>
        /// scaled_float Type: The scaling factor to use when encoding values. Values will be multiplied by this factor at index time and rounded to the closest long value. For instance, a scaled_float with a scaling_factor of 10 would internally store 2.34 as 23 and all search-time operations (queries, aggregations, sorting) will behave as if the document had a value of 2.3. High values of scaling_factor improve accuracy but also increase space requirements. This parameter is required.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The scaling factor to use when encoding values. Values will be multiplied by this factor at index time and rounded to the closest long value. For instance, a scaled_float with a scaling_factor of 10 would internally store 2.34 as 23 and all search-time operations (queries, aggregations, sorting) will behave as if the document had a value of 2.3. High values of scaling_factor improve accuracy but also increase space requirements. This parameter is required.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        int? ScalingFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Date Type: The date format(s) that can be parsed. Defaults to strict_date_optional_time||epoch_millis.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The date format(s) that can be parsed. Defaults to strict_date_optional_time||epoch_millis.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        string DateFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Date Type: The locale to use when parsing dates since months do not have the same names and/or abbreviations in all languages. The default is the ROOT locale.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The locale to use when parsing dates since months do not have the same names and/or abbreviations in all languages. The default is the ROOT locale.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        string Locale
        {
            get;
            set;
        }

        /// <summary>
        /// Completion Type: Preserves the separators, defaults to true. If disabled, you could find a field starting with Foo Fighters, if you suggest for foof.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Preserves the separators, defaults to true. If disabled, you could find a field starting with Foo Fighters, if you suggest for foof.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        bool? PreserveSeparators
        {
            get;
            set;
        }

        /// <summary>
        /// Completion Type: Enables position increments, defaults to true. If disabled and using stopwords analyzer, you could get a field starting with The Beatles, if you suggest for b. Note: You could also achieve this by indexing two inputs, Beatles and The Beatles, no need to change a simple analyzer, if you are able to enrich your data.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Enables position increments, defaults to true. If disabled and using stopwords analyzer, you could get a field starting with The Beatles, if you suggest for b. Note: You could also achieve this by indexing two inputs, Beatles and The Beatles, no need to change a simple analyzer, if you are able to enrich your data.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        bool? PreservePositionIncrements
        {
            get;
            set;
        }

        /// <summary>
        /// Completion Type: Limits the length of a single input, defaults to 50 UTF-16 code points.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Limits the length of a single input, defaults to 50 UTF-16 code points.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        int? MaxInputLength
        {
            get;
            set;
        }

        /// <summary>
        /// Completion Type: Should this field be used for suggestions if no specific filter field was selected and therefore the search will be in the _all field.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Should this field be used for suggestions if no specific filter field was selected and therefore the search will be in the _all field.")]
        [ModelBrowsable(typeof(ModelElasticSearchMappingVisibilityCalculator))]
        bool DefaultSuggestField
        {
            get;
            set;
        }
    }
}
