namespace BYteWare.XAF.ElasticSearch.BusinessObjects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface Definition to specify a Filter for Object Instances also for the Search
    /// </summary>
    public interface IObjectPermissionElasticSearchFilter
    {
        /// <summary>
        /// Filter string to use for ElasticSearch Query
        /// </summary>
        string ElasticSearchFilter
        {
            get;
            set;
        }
    }
}
