namespace MainDemo.Module.BusinessObjects
{
    using System;
    using DevExpress.Persistent.Validation;
    using DevExpress.Xpo;
    using DevExpress.Xpo.Metadata;
    using BYteWare.XAF.ElasticSearch.BusinessObjects;
    using DevExpress.Persistent.Base;

    /// <summary>
    /// ElasticSearch Index Class
    /// </summary>
    [DefaultClassOptions]
    public class ElasticSearchIndex : XPObject, IElasticSearchIndex
    {
        private bool _Active;
        private string _Name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticSearchIndex" /> class.
        /// </summary>
        /// <param name="session">XPO session</param>
        public ElasticSearchIndex(Session session)
            : base(session)
        {
        }

        /// <summary>
        /// Is the Index active 
        /// </summary>
        public bool Active
        {
            get
            {
                return _Active;
            }
            set
            {
                SetPropertyValue(nameof(Active), ref _Active, value);
            }
        }

        /// <summary>
        /// Name of the Index
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
                SetPropertyValue(nameof(Name), ref _Name, value);
            }
        }

        /// <summary>
        /// Does the index need to be refreshed
        /// </summary>
        [Association, Aggregated]
        public XPCollection<ElasticSearchIndexRefresh> IndexRefresh
        {
            get
            {
                return GetCollection<ElasticSearchIndexRefresh>(nameof(IndexRefresh));
            }
        }
    }

}