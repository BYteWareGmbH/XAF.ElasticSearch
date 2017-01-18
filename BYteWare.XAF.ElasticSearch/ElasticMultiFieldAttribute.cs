namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Multi-fields allow the same string value to be indexed in multiple ways for different purposes, such as one field for search and a multi-field for sorting and aggregations, or the same string value analyzed by different analyzers.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class ElasticMultiFieldAttribute : ElasticAttribute
    {
        /// <summary>
        /// Initalizes a new instance of the <see cref="ElasticMultiFieldAttribute"/> class.
        /// </summary>
        /// <param name="name">Name of the additional field</param>
        public ElasticMultiFieldAttribute(string name)
            : base()
        {
            Name = name;
        }
    }
}