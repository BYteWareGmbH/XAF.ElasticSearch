using System;
using DevExpress.Xpo;
using System.Collections.Generic;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using BYteWare.XAF.ElasticSearch;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System.ComponentModel;
using System.Linq;
using DevExpress.ExpressApp;
using DevExpress.Persistent.Validation;

namespace MainDemo.Module.BusinessObjects
{
    [DefaultClassOptions]
    [ElasticSearch(nameof(Contact))]
    public class Contact : Person
    {
        private string webPageAddress;
        private Contact manager;
        private string nickName;
        private string spouseName;
        private TitleOfCourtesy titleOfCourtesy;
        private string notes;
        private DateTime? anniversary;

        public Contact(Session session) :
            base(session)
        {

        }
        public string WebPageAddress
        {
            get
            {
                return webPageAddress;
            }
            set
            {
                SetPropertyValue("WebPageAddress", ref webPageAddress, value);
            }
        }

        [DataSourceProperty("Department.Contacts", DataSourcePropertyIsNullMode.SelectAll)]
        [DataSourceCriteria("Position.Title = 'Manager' AND Oid != '@This.Oid'")]
        public Contact Manager
        {
            get
            {
                return manager;
            }
            set
            {
                SetPropertyValue("Manager", ref manager, value);
            }
        }
        [ElasticProperty]
        [RuleRequiredField]
        //[ElasticMultiField("suggest", Type = FieldType.completion, DefaultSuggestField = true), ElasticSuggestContext("suggestcontext", nameof(ContactContextList), "ContactContext")]
        public string NickName
        {
            get
            {
                return nickName;
            }
            set
            {
                SetPropertyValue("NickName", ref nickName, value);
            }
        }

        [ElasticProperty]
        [RuleRequiredField]
        //[ElasticMultiField("suggest", Type = FieldType.completion, DefaultSuggestField = true), ElasticSuggestContextMultiField("suggest", "suggestcontext", nameof(ContactContextList), "ContactContext")]
        public string SpouseName
        {
            get
            {
                return spouseName;
            }
            set
            {
                SetPropertyValue("SpouseName", ref spouseName, value);
            }
        }

        public TitleOfCourtesy TitleOfCourtesy
        {
            get
            {
                return titleOfCourtesy;
            }
            set
            {
                SetPropertyValue("TitleOfCourtesy", ref titleOfCourtesy, value);
            }
        }

        public DateTime? Anniversary
        {
            get
            {
                return anniversary;
            }
            set
            {
                SetPropertyValue("Anniversary", ref anniversary, value);
            }
        }
        [ElasticProperty(Type = FieldType.text, IncludeInAll = false)]
        [Size(4096)]
        public string Notes
        {
            get
            {
                return notes;
            }
            set
            {
                SetPropertyValue("Notes", ref notes, value);
            }
        }
        private Department department;
        [Association("Department-Contacts"), ImmediatePostData]
        public Department Department
        {
            get
            {
                return department;
            }
            set
            {
                SetPropertyValue("Department", ref department, value);
                if (!IsLoading)
                {
                    Position = null;
                    if (Manager != null && Manager.Department != value)
                    {
                        Manager = null;
                    }
                }
            }
        }
        private Position position;
        public Position Position
        {
            get
            {
                return position;
            }
            set
            {
                SetPropertyValue("Position", ref position, value);
            }
        }
        [Association("Contact-DemoTask")]
        public XPCollection<DemoTask> Tasks
        {
            get
            {
                return GetCollection<DemoTask>("Tasks");
            }
        }
        private XPCollection<AuditDataItemPersistent> changeHistory;
        public XPCollection<AuditDataItemPersistent> ChangeHistory
        {
            get
            {
                if (changeHistory == null)
                {
                    changeHistory = AuditedObjectWeakReference.GetAuditTrail(Session, this);
                }
                return changeHistory;
            }
        }

        [Association]
        [RuleRequiredField]
        public XPCollection<UserRole> UserRoles
        {
            get
            {
                return GetCollection<UserRole>("UserRoles");
            }
        }

        [VisibleInListView(false), VisibleInDetailView(false)]
        [ElasticProperty(Type = FieldType.completion, DefaultSuggestField = true), ElasticSuggestContext("suggestcontext", nameof(ContactContextList), "ContactContext")]
        public IEnumerable<string> NickNameSuggest
        {
            get
            {
                if (ContactSuggest == null)
                    return null;
                return ContactSuggest.Where(x => x.NickName == NickName).Select(t => t.NickName);
            }
        }

        [VisibleInListView(false), VisibleInDetailView(false)]
        [ElasticProperty]
        [ElasticMultiField("suggest", Type = FieldType.completion, DefaultSuggestField = true), ElasticSuggestContextMultiField("suggest", "suggestcontext", nameof(ContactContextList), "ContactContext")]
        public IEnumerable<string> SpouseNameSuggest
        {
            get
            {
                if (ContactSuggest == null)
                    return null;
                return ContactSuggest.Where(x => x.SpouseName == SpouseName).Select(t => t.SpouseName);
            }
        }
        
        /// <summary>
        /// Suggest Contact
        /// </summary>
        [Browsable(false)]
        public virtual IEnumerable<Contact> ContactSuggest
        {
            get
            {
                List<Contact> c = new List<Contact>();
                var currentUser = SecuritySystem.CurrentUser as PermissionPolicyUser;

                if (currentUser.Roles.Any(x=>x.IsAdministrative))
                {
                    foreach (var item in UserRoles.Select(contact => contact.Contacts))
                    {
                        c.AddRange(item);
                    }
                    return c;
                }
                else
                {
                    foreach (var item in currentUser.Roles)
                    {
                        var rList = UserRoles.Where(r => r.Name == item.Name);
                        foreach (var r in rList)
                        {
                            c.AddRange(r.Contacts);
                        }
                    }
                    return c;
                }
            }
        }

        [ElasticProperty(IncludeInAll = false)]
        [Browsable(false)]
        public IEnumerable<string> ContactContextList
        {
            get
            {
                var currentUser = SecuritySystem.CurrentUser as PermissionPolicyUser;
                return UserRoles.Select(u => u.Oid.ToString("N"));
            }
        }
    }
    public enum TitleOfCourtesy
    {
        Dr,
        Miss,
        Mr,
        Mrs,
        Ms
    };
}
