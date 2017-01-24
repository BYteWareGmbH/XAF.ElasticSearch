namespace BYteWare.XAF.ElasticSearch
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using System;
    using System.Collections.Generic;
    using System.Linq;

#pragma warning disable SA1300
    /// <summary>
    /// Term vectors contain information about the terms produced by the analysis process.
    /// </summary>
    public enum TermVectorOption
    {
        /// <summary>
        /// No term vectors are stored. (default)
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = nameof(no), Justification = nameof(ElasticSearch))]
        no,

        /// <summary>
        /// Just the terms in the field are stored.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = nameof(yes), Justification = nameof(ElasticSearch))]
        yes,

        /// <summary>
        /// Terms and positions are stored.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "with", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "positions", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(ElasticSearch))]
        with_positions,

        /// <summary>
        /// Terms and character offsets are stored.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "with", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "offsets", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(ElasticSearch))]
        with_offsets,

        /// <summary>
        /// Terms, positions, and character offsets are stored.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "with", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "positions", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "offsets", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(ElasticSearch))]
        with_positions_offsets
    }

    /// <summary>
    /// The index_options parameter controls what information is added to the inverted index, for search and highlighting purposes.
    /// </summary>
    public enum IndexOptions
    {
        /// <summary>
        /// Only the doc number is indexed. Can answer the question Does this term exist in this field?
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = nameof(docs), Justification = nameof(ElasticSearch))]
        docs,

        /// <summary>
        /// Doc number and term frequencies are indexed. Term frequencies are used to score repeated terms higher than single terms.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = nameof(freqs), Justification = nameof(ElasticSearch))]
        freqs,

        /// <summary>
        /// Doc number, term frequencies, and term positions (or order) are indexed. Positions can be used for proximity or phrase queries.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = nameof(positions), Justification = nameof(ElasticSearch))]
        positions,

        /// <summary>
        /// Doc number, term frequencies, positions, and start and end character offsets (which map the term back to the original string) are indexed. Offsets are used by the postings highlighter.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = nameof(offsets), Justification = nameof(ElasticSearch))]
        offsets
    }
#pragma warning restore SA1300

    /// <summary>
    /// ElasticSearch index parameters
    /// </summary>
    public abstract class ElasticAttribute : Attribute, IElasticSearchFieldProperties
    {
        /// <summary>
        /// Name of the field
        /// </summary>
        public string FieldName
        {
            get;
            set;
        }

        /// <summary>
        /// The ElasticSearch Field Type
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods", Justification = nameof(ElasticSearch))]
        public FieldType Type
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).FieldType.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).FieldType = value;
            }
        }

        /// <summary>
        /// Model Field Type
        /// </summary>
        FieldType? IElasticSearchFieldProperties.FieldType
        {
            get;
            set;
        }

        /// <summary>
        /// Text, Completion Type: The analyzer which should be used for analyzed string fields, both at index-time and at search-time (unless overridden by the search_analyzer). Defaults to the default index analyzer, or the standard analyzer.
        /// </summary>
        public string Analyzer
        {
            get;
            set;
        }

        /// <summary>
        /// Mapping field-level query time boosting. Accepts a floating point number, defaults to 1.0.
        /// </summary>
        public double Boost
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).Boost.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).Boost = value;
            }
        }

        /// <summary>
        /// Mapping field-level query time boosting. Accepts a floating point number, defaults to 1.0.
        /// </summary>
        double? IElasticSearchFieldProperties.Boost
        {
            get;
            set;
        }

        /// <summary>
        /// Should the field be stored on disk in a column-stride fashion, so that it can later be used for sorting, aggregations, or scripting? Accepts true (default) or false.
        /// </summary>
        public bool DocValues
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).DocValues.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).DocValues = value;
            }
        }

        /// <summary>
        /// Should the field be stored on disk in a column-stride fashion, so that it can later be used for sorting, aggregations, or scripting? Accepts true (default) or false.
        /// </summary>
        bool? IElasticSearchFieldProperties.DocValues
        {
            get;
            set;
        }

        /// <summary>
        /// Should global ordinals be loaded eagerly on refresh? Accepts true or false (default). Enabling this is a good idea on fields that are frequently used for (significant) terms aggregations.
        /// </summary>
        public bool EagerGlobalOrdinals
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).EagerGlobalOrdinals.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).EagerGlobalOrdinals = value;
            }
        }

        /// <summary>
        /// Should global ordinals be loaded eagerly on refresh? Accepts true or false (default). Enabling this is a good idea on fields that are frequently used for (significant) terms aggregations.
        /// </summary>
        bool? IElasticSearchFieldProperties.EagerGlobalOrdinals
        {
            get;
            set;
        }

        /// <summary>
        /// Text Type: Can the field use in-memory fielddata for sorting, aggregations, or scripting? Accepts true or false (default).
        /// </summary>
        public bool FieldData
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).FieldData.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).FieldData = value;
            }
        }

        /// <summary>
        /// Text Type: Can the field use in-memory fielddata for sorting, aggregations, or scripting? Accepts true or false (default).
        /// </summary>
        bool? IElasticSearchFieldProperties.FieldData
        {
            get;
            set;
        }

        /*/// <summary>
        /// Text Type: Expert settings which allow to decide which values to load in memory when fielddata is enabled. By default all values are loaded.
        /// </summary>
        public string FieldDataFrequencyFilter
        {
            get;
            set;
        }*/

        /// <summary>
        /// Should the field be searchable? Accepts true (default) or false.
        /// </summary>
        public bool? Index
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).ESIndex.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).ESIndex = value;
            }
        }

        /// <summary>
        /// Model Index setting
        /// </summary>
        bool? IElasticSearchFieldProperties.ESIndex
        {
            get;
            set;
        }

        /// <summary>
        /// What information should be stored in the index, for search and highlighting purposes. Defaults to positions.
        /// </summary>
        public IndexOptions IndexOptions
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).IndexOptions.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).IndexOptions = value;
            }
        }

        /// <summary>
        /// What information should be stored in the index, for search and highlighting purposes. Defaults to positions.
        /// </summary>
        IndexOptions? IElasticSearchFieldProperties.IndexOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Whether field-length should be taken into account when scoring queries. Accepts true (default) or false.
        /// </summary>
        public bool Norms
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).Norms.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).Norms = value;
            }
        }

        /// <summary>
        /// Whether field-length should be taken into account when scoring queries. Accepts true (default) or false.
        /// </summary>
        bool? IElasticSearchFieldProperties.Norms
        {
            get;
            set;
        }

        /// <summary>
        /// Text Type: The number of fake term position which should be inserted between each element of an array of strings. Defaults to the position_increment_gap configured on the analyzer which defaults to 100. 100 was chosen because it prevents phrase queries with reasonably large slops (less than 100) from matching terms across field values.
        /// </summary>
        public int PositionOffsetGap
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).PositionOffsetGap.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).PositionOffsetGap = value;
            }
        }

        /// <summary>
        /// Text Type: The number of fake term position which should be inserted between each element of an array of strings. Defaults to the position_increment_gap configured on the analyzer which defaults to 100. 100 was chosen because it prevents phrase queries with reasonably large slops (less than 100) from matching terms across field values.
        /// </summary>
        int? IElasticSearchFieldProperties.PositionOffsetGap
        {
            get;
            set;
        }

        /// <summary>
        /// Whether the field value should be stored and retrievable separately from the _source field. Accepts true or false (default).
        /// </summary>
        public bool Store
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).Store.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).Store = value;
            }
        }

        /// <summary>
        /// Whether the field value should be stored and retrievable separately from the _source field. Accepts true or false (default).
        /// </summary>
        bool? IElasticSearchFieldProperties.Store
        {
            get;
            set;
        }

        /// <summary>
        /// Text, Keyfield, Completion Type: The analyzer that should be used at search time on analyzed fields. Defaults to the analyzer setting.
        /// </summary>
        public string SearchAnalyzer
        {
            get;
            set;
        }

        /// <summary>
        /// Text Type: The analyzer that should be used at search time when a phrase is encountered. Defaults to the search_analyzer setting.
        /// </summary>
        public string SearchQuoteAnalyzer
        {
            get;
            set;
        }

        /// <summary>
        /// Text, Keyfield Type: Which scoring algorithm or similarity should be used. Defaults to classic, which uses TF/IDF.
        /// </summary>
        public string Similarity
        {
            get;
            set;
        }

        /// <summary>
        /// Text Type: Whether term vectors should be stored for an analyzed field. Defaults to no.
        /// </summary>
        public TermVectorOption TermVector
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).TermVector.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).TermVector = value;
            }
        }

        /// <summary>
        /// Text Type: Whether term vectors should be stored for an analyzed field. Defaults to no.
        /// </summary>
        TermVectorOption? IElasticSearchFieldProperties.TermVector
        {
            get;
            set;
        }

        /// <summary>
        /// Keyfield Type: Do not index any string longer than this value. Defaults to 2147483647 so that all values would be accepted.
        /// </summary>
        public int IgnoreAbove
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).IgnoreAbove.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).IgnoreAbove = value;
            }
        }

        /// <summary>
        /// Keyfield Type: Do not index any string longer than this value. Defaults to 2147483647 so that all values would be accepted.
        /// </summary>
        int? IElasticSearchFieldProperties.IgnoreAbove
        {
            get;
            set;
        }

        /// <summary>
        /// Accepts a string value which is substituted for any explicit null values. Defaults to null, which means the field is treated as missing.
        /// </summary>
        public string NullValue
        {
            get;
            set;
        }

        /// <summary>
        /// Numeric Types: Try to convert strings to numbers and truncate fractions for integers. Accepts true (default) and false.
        /// </summary>
        public bool Coerce
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).Coerce.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).Coerce = value;
            }
        }

        /// <summary>
        /// Numeric Types: Try to convert strings to numbers and truncate fractions for integers. Accepts true (default) and false.
        /// </summary>
        bool? IElasticSearchFieldProperties.Coerce
        {
            get;
            set;
        }

        /// <summary>
        /// Numeric, Date Types: If true, malformed numbers are ignored. If false (default), malformed numbers throw an exception and reject the whole document.
        /// </summary>
        public bool IgnoreMalformed
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).IgnoreMalformed.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).IgnoreMalformed = value;
            }
        }

        /// <summary>
        /// Numeric, Date Types: If true, malformed numbers are ignored. If false (default), malformed numbers throw an exception and reject the whole document.
        /// </summary>
        bool? IElasticSearchFieldProperties.IgnoreMalformed
        {
            get;
            set;
        }

        /// <summary>
        /// scaled_float Type: The scaling factor to use when encoding values. Values will be multiplied by this factor at index time and rounded to the closest long value. For instance, a scaled_float with a scaling_factor of 10 would internally store 2.34 as 23 and all search-time operations (queries, aggregations, sorting) will behave as if the document had a value of 2.3. High values of scaling_factor improve accuracy but also increase space requirements. This parameter is required.
        /// </summary>
        public int ScalingFactor
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).ScalingFactor.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).ScalingFactor = value;
            }
        }

        /// <summary>
        /// scaled_float Type: The scaling factor to use when encoding values. Values will be multiplied by this factor at index time and rounded to the closest long value. For instance, a scaled_float with a scaling_factor of 10 would internally store 2.34 as 23 and all search-time operations (queries, aggregations, sorting) will behave as if the document had a value of 2.3. High values of scaling_factor improve accuracy but also increase space requirements. This parameter is required.
        /// </summary>
        int? IElasticSearchFieldProperties.ScalingFactor
        {
            get;
            set;
        }

        /// <summary>
        /// Date Type: The date format(s) that can be parsed. Defaults to strict_date_optional_time||epoch_millis.
        /// </summary>
        public string DateFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Date Type: The locale to use when parsing dates since months do not have the same names and/or abbreviations in all languages. The default is the ROOT locale.
        /// </summary>
        public string Locale
        {
            get;
            set;
        }

        /// <summary>
        /// Completion Type: Preserves the separators, defaults to true. If disabled, you could find a field starting with Foo Fighters, if you suggest for foof.
        /// </summary>
        public bool PreserveSeparators
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).PreserveSeparators.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).PreserveSeparators = value;
            }
        }

        /// <summary>
        /// Completion Type: Preserves the separators, defaults to true. If disabled, you could find a field starting with Foo Fighters, if you suggest for foof.
        /// </summary>
        bool? IElasticSearchFieldProperties.PreserveSeparators
        {
            get;
            set;
        }

        /// <summary>
        /// Completion Type: Enables position increments, defaults to true. If disabled and using stopwords analyzer, you could get a field starting with The Beatles, if you suggest for b. Note: You could also achieve this by indexing two inputs, Beatles and The Beatles, no need to change a simple analyzer, if you are able to enrich your data.
        /// </summary>
        public bool PreservePositionIncrements
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).PreservePositionIncrements.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).PreservePositionIncrements = value;
            }
        }

        /// <summary>
        /// Completion Type: Enables position increments, defaults to true. If disabled and using stopwords analyzer, you could get a field starting with The Beatles, if you suggest for b. Note: You could also achieve this by indexing two inputs, Beatles and The Beatles, no need to change a simple analyzer, if you are able to enrich your data.
        /// </summary>
        bool? IElasticSearchFieldProperties.PreservePositionIncrements
        {
            get;
            set;
        }

        /// <summary>
        /// Completion Type: Limits the length of a single input, defaults to 50 UTF-16 code points.
        /// </summary>
        public int MaxInputLength
        {
            get
            {
                return ((IElasticSearchFieldProperties)this).MaxInputLength.GetValueOrDefault();
            }
            set
            {
                ((IElasticSearchFieldProperties)this).MaxInputLength = value;
            }
        }

        /// <summary>
        /// Completion Type: Limits the length of a single input, defaults to 50 UTF-16 code points.
        /// </summary>
        int? IElasticSearchFieldProperties.MaxInputLength
        {
            get;
            set;
        }

        /// <summary>
        /// Completion Type: Should this field be used for suggestions if no specific filter field was selected and therefore the search will be in the _all field
        /// </summary>
        public bool DefaultSuggestField
        {
            get;
            set;
        }
    }
}