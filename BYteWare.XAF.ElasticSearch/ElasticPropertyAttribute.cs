namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// ElasticSearch index parameters for a property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class ElasticPropertyAttribute : ElasticAttribute, IElasticProperties
    {
        /// <summary>
        /// Whether or not the field value should be included in the _all field? Accepts true or false. Defaults to false if index is set to no, or if a parent object field sets include_in_all to false. Otherwise defaults to true.
        /// </summary>
        public bool IncludeInAll
        {
            get
            {
                return ((IElasticProperties)this).IncludeInAll.GetValueOrDefault();
            }
            set
            {
                ((IElasticProperties)this).IncludeInAll = value;
            }
        }

        /// <summary>
        /// Whether or not the field value should be included in the _all field? Accepts true or false. Defaults to false if index is set to no, or if a parent object field sets include_in_all to false. Otherwise defaults to true.
        /// </summary>
        bool? IElasticProperties.IncludeInAll
        {
            get;
            set;
        }

        /// <summary>
        /// Should this field be serialized: Makes only sense to set it to true in a class, where the field was set to be indexed in a base class but should be omitted for the current type.
        /// </summary>
        public bool OptOut
        {
            get;
            set;
        }

        /// <summary>
        /// The copy_to parameter allows you to create group fields, which can then be queried as a single field.
        /// </summary>
        public string CopyTo
        {
            get;
            set;
        }

        /// <summary>
        /// Completion Type: A member name which returns a positive integer or a string containing a positive integer, which defines a weight and allows you to rank your suggestions.
        /// </summary>
        public string WeightField
        {
            get;
            set;
        }
    }
}