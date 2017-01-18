namespace BYteWare.XAF.ElasticSearch.BusinessObjects
{
    using DevExpress.Xpo;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface which should be implemented to store if an ElasticSearch index should be refreshed
    /// </summary>
    public interface IElasticSearchIndexRefresh : IXPObject
    {
        /// <summary>
        /// IElasticSearchIndex implementation
        /// </summary>
        IElasticSearchIndex Index
        {
            get;
            set;
        }

        /// <summary>
        /// When was the entry added
        /// </summary>
        DateTime Timestamp
        {
            get;
            set;
        }
    }
}
