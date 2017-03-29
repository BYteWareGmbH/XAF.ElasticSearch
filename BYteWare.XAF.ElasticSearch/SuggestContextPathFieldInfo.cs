namespace BYteWare.XAF.ElasticSearch
{
    using DevExpress.Xpo.Metadata;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Supplies information about Suggest Context Path Fields
    /// </summary>
    public class SuggestContextPathFieldInfo
    {
        /// <summary>
        /// Field Name
        /// </summary>
        public string PathMemberName
        {
            get;
            set;
        }

        /// <summary>
        /// ElasticSearch Field Name
        /// </summary>
        public string ESFieldName
        {
            get;
            set;
        }

        /// <summary>
        /// XPO Member Info
        /// </summary>
        public XPMemberInfo MemberInfo
        {
            get;
            set;
        }

        /// <summary>
        /// Is the Path field already marked to be indexed
        /// </summary>
        public bool IsIndexed
        {
            get;
            set;
        }
    }
}
