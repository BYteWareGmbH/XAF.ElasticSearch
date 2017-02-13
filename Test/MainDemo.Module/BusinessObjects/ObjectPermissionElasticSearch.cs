using System;
using DevExpress.Xpo;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using BYteWare.XAF.ElasticSearch.BusinessObjects;

namespace MainDemo.Module.BusinessObjects
{
    public class ObjectPermissionElasticSearch : PermissionPolicyObjectPermissionsObject, IObjectPermissionElasticSearchFilter
    {
        public ObjectPermissionElasticSearch(Session session) : base(session)
        {
        }

        private string _ElasticSearchFilter;

        public string ElasticSearchFilter
        {
            get
            {
                return _ElasticSearchFilter;
            }
            set
            {
                SetPropertyValue(nameof(ElasticSearchFilter), ref _ElasticSearchFilter, value);
            }
        }
    }
}