namespace BYteWare.XAF.ElasticSearch.BusinessObjects
{
    using System;
    using System.ComponentModel;
    using DevExpress.Persistent.Validation;
    using DevExpress.Xpo;

    /// <summary>
    /// IndexAktualisieren Business Klasse.
    /// </summary>
    [DeferredDeletion(false)]
    [DefaultProperty("Index")]
    public sealed class ElasticSearchIndexUpdate : XPObject
    {
        /// <summary>
        /// Initialisiert eine neue Instanz der <see cref="ElasticSearchIndexUpdate" /> Klasse.
        /// </summary>
        /// <param name="session">XPO session</param>
        public ElasticSearchIndexUpdate(Session session)
            : base(session)
        {
        }

        /// <summary>
        /// Initialisiert die Eigenschaften einer neu erzeugten Instanz der <see cref="ElasticSearchIndexUpdate" /> Klasse.
        /// </summary>
        public override void AfterConstruction()
        {
            base.AfterConstruction();
        }

        // Fields...
        private ElasticSearchIndex _Index;
        private DateTime _datetime;

        /// <summary>
        /// Zeitpunkt der Synchronisierung
        /// </summary>
        public DateTime Datetime
        {
            get
            {
                return _datetime;
            }
            set
            {
                SetPropertyValue("Datetime", ref _datetime, value);
            }
        }

        /// <summary>
        /// Index der aktualisiert werden muss
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
                SetPropertyValue("Index", ref _Index, value);
            }
        }
    }

}