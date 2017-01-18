namespace BYteWare.XAF.ElasticSearch
{
    using DevExpress.ExpressApp.DC;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Information about a Type where properties of this type's instances are used to construct their ElasticSearch document
    /// </summary>
    public class ContainingType
    {
        /// <summary>
        /// The BYteWareTypeInfo instance
        /// </summary>
        public BYteWareTypeInfo BYteWareType
        {
            get;
            internal set;
        }

        /// <summary>
        /// The MemberInfo to filter the containing instances
        /// </summary>
        [CLSCompliant(false)]
        public IMemberInfo MemberInfo
        {
            get;
            internal set;
        }

        /// <summary>
        /// Was the connection defined through a ElasticContainingAttribute
        /// </summary>
        public bool HasContainingAttribute
        {
            get;
            internal set;
        }
    }
}
