namespace BYteWare.XAF.ElasticSearch.BusinessObjects
{
    using DevExpress.Xpo;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface which should be implemented to store the state of ElasticSearch indexes
    /// </summary>
    public interface IElasticSearchIndex : IXPObject
    {
        /// <summary>
        /// Name of the Index
        /// </summary>
        string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Is the Index active
        /// </summary>
        bool Active
        {
            get;
            set;
        }
    }
}
