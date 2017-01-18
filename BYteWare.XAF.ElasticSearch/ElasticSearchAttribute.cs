namespace BYteWare.XAF.ElasticSearch
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Decorates a XPO BusinessClass to be indexed and searchable through ElasticSearch
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = true)]
    public sealed class ElasticSearchAttribute : Attribute
    {
        /// <summary>
        /// Initalizes a new instance of the <see cref="ElasticSearchAttribute"/> class.
        /// </summary>
        /// <param name="indexName">Name of the ElasticSearch Index</param>
        public ElasticSearchAttribute(string indexName)
        {
            _IndexName = indexName;
        }

        private string _IndexName;

        /// <summary>
        /// Name of the ElasticSearch Index
        /// </summary>
        public string IndexName
        {
            get
            {
                return ElasticSearchClient.Instance.IndexName(_IndexName);
            }
        }

        /// <summary>
        /// Name for the ElasticSearch Type, defaults to the Classname
        /// </summary>
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// Disables the storage of the source Json string
        /// </summary>
        public bool SourceFieldDisabled
        {
            get;
            set;
        }
    }
}