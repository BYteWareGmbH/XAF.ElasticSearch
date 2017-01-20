namespace MainDemo.Module.BusinessObjects
{
    using System;
    using System.ComponentModel;
    using DevExpress.Persistent.Validation;
    using DevExpress.Xpo;
    using BYteWare.XAF.ElasticSearch.BusinessObjects;

    /// <summary>
    /// Index Refresh Business Class
    /// </summary>
    [DeferredDeletion(false)]
    [DefaultProperty("Index")]
    public sealed class ElasticSearchIndexRefresh : XPObject, IElasticSearchIndexRefresh
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ElasticSearchIndexRefresh" /> class.
        /// </summary>
        /// <param name="session">XPO session</param>
        public ElasticSearchIndexRefresh(Session session)
            : base(session)
        {
        }

        // Fields...
        private ElasticSearchIndex _Index;
        private DateTime _Timestamp;

        /// <summary>
        /// Timestamp of the request to refresh the index
        /// </summary>
        public DateTime Timestamp
        {
            get
            {
                return _Timestamp;
            }
            set
            {
                SetPropertyValue(nameof(Timestamp), ref _Timestamp, value);
            }
        }

        /// <summary>
        /// Index who needs to be refreshed
        /// </summary>
        [Association]
        [RuleRequiredField]
        public ElasticSearchIndex Index
        {
            get
            {
                return _Index;
            }
            set
            {
                SetPropertyValue(nameof(Index), ref _Index, value);
            }
        }

        /// <summary>
        /// IElasticSearchIndexRefresh implementation
        /// </summary>
        IElasticSearchIndex IElasticSearchIndexRefresh.Index
        {
            get
            {
                return Index;
            }
            set
            {
                Index = (ElasticSearchIndex)value;
            }
        }
    }
}