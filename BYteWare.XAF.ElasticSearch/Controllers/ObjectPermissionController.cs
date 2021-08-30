namespace BYteWare.XAF.ElasticSearch.Controllers
{
    using BYteWare.XAF.ElasticSearch.BusinessObjects;
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.SystemModule;
    using DevExpress.Persistent.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// XAF View Controller
    /// </summary>
    /// <remarks>Class usage tutorial:
    /// <a href="http://documentation.devexpress.com/#Xaf/clsDevExpressExpressAppViewControllertopic">DevExpress ViewController Documentation</a>
    /// </remarks>
    [CLSCompliant(false)]
    public class ObjectPermissionController : ViewController
    {
        private NewObjectViewController newController;

        /// <summary>
        /// Initalizes a new instance of the <see cref="ObjectPermissionController"/> class.
        /// </summary>
        public ObjectPermissionController()
        {
        }

        /// <inheritdoc/>
        protected override void OnFrameAssigned()
        {
            base.OnFrameAssigned();
            if (Frame != null)
            {
                newController = Frame.GetController<NewObjectViewController>();
                if (newController != null)
                {
                    newController.CollectCreatableItemTypes += NewController_CollectDescendantTypes;
                    newController.CollectDescendantTypes += NewController_CollectDescendantTypes;
                }
            }
        }

        private void NewController_CollectDescendantTypes(object sender, CollectTypesEventArgs e)
        {
            if (newController != null && newController.View is ListView listView && listView.ObjectTypeInfo != null)
            {
                List<Type> esFilterTypes = null;
                if (typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemObjectPermissionsObject).IsAssignableFrom(listView.ObjectTypeInfo.Type))
                {
                    esFilterTypes = XafTypesInfo.Instance.PersistentTypes.Where(p => p.Implements<IObjectPermissionElasticSearchFilter>() && typeof(DevExpress.ExpressApp.Security.Strategy.SecuritySystemObjectPermissionsObject).IsAssignableFrom(p.Type)).Select(p => p.Type).ToList();
                }
                if (typeof(IPermissionPolicyObjectPermissionsObject).IsAssignableFrom(listView.ObjectTypeInfo.Type))
                {
                    esFilterTypes = XafTypesInfo.Instance.PersistentTypes.Where(p => p.Implements<IObjectPermissionElasticSearchFilter>() && p.Implements<IPermissionPolicyObjectPermissionsObject>()).Select(p => p.Type).ToList();
                }
                if (esFilterTypes != null && esFilterTypes.Any())
                {
                    e.Types.Clear();
                    esFilterTypes.ForEach(e.Types.Add);
                }
            }
        }
    }
}
