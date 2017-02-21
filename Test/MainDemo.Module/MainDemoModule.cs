using System;
using System.Collections.Generic;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.ViewVariantsModule;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl;
using MainDemo.Module.BusinessObjects;
using MainDemo.Module.Reports;
using BYteWare.XAF.ElasticSearch;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System.Linq;
using System.Globalization;

namespace MainDemo.Module {
    public sealed partial class MainDemoModule : ModuleBase {
        public MainDemoModule() {
            InitializeComponent();
        }

        public override void Setup(XafApplication application)
        {
            base.Setup(application);
            application.LoggedOn += Application_LoggedOn;
        }

        private void Application_LoggedOn(object sender, LogonEventArgs e)
        {
            var app = sender as XafApplication;
            var user = app.Security.User as PermissionPolicyUser;
            ElasticSearchClient.Instance?.AddParameter("ContactContext", string.Join(",", user.Roles.Select(t => string.Format(CultureInfo.InvariantCulture, "\"{0}\"", t.Oid.ToString("N")))));
        }

        public override void CustomizeTypesInfo(ITypesInfo typesInfo) {
            base.CustomizeTypesInfo(typesInfo);
            CalculatedPersistentAliasHelper.CustomizeTypesInfo(typesInfo);
        }

        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            PredefinedReportsUpdater predefinedReportsUpdater = new PredefinedReportsUpdater(Application, objectSpace, versionFromDB);
            predefinedReportsUpdater.AddPredefinedReport<ContactsReport>("Contacts Report", typeof(Contact), true);
            return new ModuleUpdater[] { updater, predefinedReportsUpdater };
        }
        static MainDemoModule() {
            /*Note that you can specify the required format in a configuration file:
            <appSettings>
               <add key="FullAddressFormat" value="{Country.Name} {City} {Street}">
               <add key="FullAddressPersistentAlias" value="Country.Name+City+Street">
               ...
            </appSettings>

            ... and set the specified format here in code:
            Address.SetFullAddressFormat(ConfigurationManager.AppSettings["FullAddressFormat"], ConfigurationManager.AppSettings["FullAddressPersistentAlias"]);
            */

            Person.SetFullNameFormat("{LastName} {FirstName} {MiddleName}", "concat(FirstName, MiddleName, LastName)");
            Address.SetFullAddressFormat("City: {City}, Street: {Street}", "concat(City, Street)");
        }
    }
}
