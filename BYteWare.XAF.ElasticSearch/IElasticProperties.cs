namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.ComponentModel;
    using System.Linq;

    /// <summary>
    /// ElasticSearch Property settings
    /// </summary>
    public interface IElasticProperties : IElasticSearchFieldProperties
    {
        /// <summary>
        /// Whether or not the field value should be included in the _all field? Accepts true or false. Defaults to false if index is set to no, or if a parent object field sets include_in_all to false. Otherwise defaults to true.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Whether or not the field value should be included in the _all field? Accepts true or false. Defaults to false if index is set to no, or if a parent object field sets include_in_all to false. Otherwise defaults to true.")]
        bool? IncludeInAll
        {
            get;
            set;
        }

        /// <summary>
        /// Should this field be serialized: Makes only sense to set it to true in a class, where the field was set to be indexed in a base class but should be omitted for the current type.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("Should this field be serialized: Makes only sense to set it to true in a class, where the field was set to be indexed in a base class but should be omitted for the current type.")]
        bool OptOut
        {
            get;
            set;
        }

        /// <summary>
        /// The copy_to parameter allows you to create group fields, which can then be queried as a single field.
        /// </summary>
        [Category(nameof(ElasticSearch))]
        [Description("The copy_to parameter allows you to create group fields, which can then be queried as a single field.")]
        string CopyTo
        {
            get;
            set;
        }

        /// <summary>
        /// Completion Type: A member name which returns a positive integer or a string containing a positive integer, which defines a weight and allows you to rank your suggestions.
        /// </summary>
        [Browsable(false)]
        string WeightField
        {
            get;
        }
    }
}