namespace BYteWare.XAF.ElasticSearch.BusinessObjects
{
    using DevExpress.Xpo.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface to be implemented by ElasticSearch indexed Business Classes to store the search position
    /// </summary>
    public interface ISearchPosition : ISessionProvider
    {
        /// <summary>
        /// The Position inside the last search
        /// </summary>
        int SearchPosition
        {
            get;
            set;
        }
    }
}
