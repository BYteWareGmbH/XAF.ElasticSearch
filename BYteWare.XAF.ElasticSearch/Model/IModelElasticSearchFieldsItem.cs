namespace BYteWare.XAF.ElasticSearch.Model
{
    using DevExpress.ExpressApp.DC;
    using DevExpress.ExpressApp.Model;
    using ElasticSearch;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

#pragma warning disable SA1300
    /// <summary>
    /// List of ElasticSearch multi_match query types
    /// </summary>
    public enum ElasticQueryType
    {
        /// <summary>
        /// (default) Finds documents which match any field, but uses the _score from the best field.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "fields", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "best", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(ElasticSearch))]
        best_fields,

        /// <summary>
        /// Finds documents which match any field and combines the _score from each field.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "most", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "fields", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(ElasticSearch))]
        most_fields,

        /// <summary>
        /// Treats fields with the same analyzer as though they were one big field. Looks for each word in any field.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "fields", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "cross", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(ElasticSearch))]
        cross_fields,

        /// <summary>
        /// Runs a match_phrase query on each field and combines the _score from each field.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = nameof(phrase), Justification = nameof(ElasticSearch))]
        phrase,

        /// <summary>
        /// Runs a match_phrase_prefix query on each field and combines the _score from each field.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "prefix", Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = nameof(phrase), Justification = nameof(ElasticSearch))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", Justification = nameof(ElasticSearch))]
        phrase_prefix
    }
#pragma warning restore SA1300

    /// <summary>
    /// Filter Field Action item settings
    /// </summary>
    [CLSCompliant(false)]
    [ModelPersistentName("ElasticSearchFields")]
    public interface IModelElasticSearchFieldsItem : IModelBaseChoiceActionItem
    {
        /// <summary>
        /// ElasticSearch multi_match query type
        /// </summary>
        [Category("Behavior")]
        [Description("ElasticSearch multi_match query type")]
        [DefaultValue(0)]
        ElasticQueryType QueryType
        {
            get;
            set;
        }

        /// <summary>
        /// Minimum number of search elements which should match for a hit
        /// </summary>
        [Category("Behavior")]
        [Description("Minimum number of search elements which should match for a hit")]
        [DefaultValue(ElasticSearchClient.DefaultMinimumShouldMatch)]
        string MinimumShouldMatch
        {
            get;
            set;
        }

        /// <summary>
        /// Take the single best score plus tie_breaker multiplied by each of the scores from other matching fields.
        /// </summary>
        [Category("Behavior")]
        [Description("Take the single best score plus tie_breaker multiplied by each of the scores from other matching fields.")]
        [DefaultValue(0.3)]
        double TieBreaker
        {
            get;
            set;
        }

        /// <summary>
        /// List of ElasticSearch Field Settings
        /// </summary>
        [Description("List of ElasticSearch Field Settings")]
        IModelElasticSearchFieldList Fields
        {
            get;
        }

        /// <summary>
        /// List of ElasticSearch Suggest settings
        /// </summary>
        [Description("List of ElasticSearch Suggest settings")]
        IModelElasticSearchSuggestFieldList SuggestFields
        {
            get;
        }

        /// <summary>
        /// The Type Info of the Business Class
        /// </summary>
        [Browsable(false)]
        ITypeInfo TypeInfo
        {
            get;
        }
    }
}