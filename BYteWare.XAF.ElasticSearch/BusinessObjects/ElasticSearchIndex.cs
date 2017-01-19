namespace BYteWare.XAF.ElasticSearch.BusinessObjects
{
    using System;
    using DevExpress.Persistent.Validation;
    using DevExpress.Xpo;
    using DevExpress.Xpo.Metadata;

    public class ElasticSearchIndex : XPObject, IElasticSearchIndex
    {
        public ElasticSearchIndex() : base()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        private bool _Active;
        private string _Name;

        /// <summary>
        /// Ist der Index aktiv 
        /// </summary>
        public bool Active
        {
            get
            {
                return _Active;
            }
            set
            {
                SetPropertyValue("Active", ref _Active, value);
            }
        }

        /// <summary>
        /// Name des Index
        /// </summary>
        [Size(255)]
        [RuleRequiredField, RuleUniqueValue]
        [Indexed(Unique = true)]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        /// <summary>
        /// Indexes Refersh after synchronization
        /// </summary>
        [Association, DevExpress.Xpo.Aggregated]
        public XPCollection<ElasticSearchIndexUpdate> ElasticSearchIndexesUpdate
        {
            get
            {
                return GetCollection<ElasticSearchIndexUpdate>("ElasticSearchIndexesUpdate");
            }
        }
    }

}