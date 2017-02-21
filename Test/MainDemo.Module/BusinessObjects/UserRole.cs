using BYteWare.XAF.ElasticSearch;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainDemo.Module.BusinessObjects
{
    [ImageName("BO_Role")]
    [DefaultClassOptions]
    public class UserRole : PermissionPolicyRole,IComparable /*IPermissionPolicyRole, IPermissionPolicyRoleWithUsers*/
    {
        public UserRole(Session session)
            : base(session)
        {
        }

        [ToolTip("View, assign or remove contacts for the current task")]
        [Association][VisibleInListView(false), VisibleInDetailView(false)]
        public XPCollection<Contact> Contacts
        {
            get
            {
                return GetCollection<Contact>("Contacts");
            }
        }

        public virtual string RoleName
        {
            get
            {
                return this.Name;
            }
            
        }
    }
}
