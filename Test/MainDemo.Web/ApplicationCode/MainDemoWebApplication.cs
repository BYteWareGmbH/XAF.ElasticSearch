using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Strategy;
using DevExpress.ExpressApp.Web;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace MainDemo.Web {
    public class MainDemoWebApplication : WebApplication {
        private DevExpress.ExpressApp.SystemModule.SystemModule systemModule1;
        private DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule webSystemModule1;
        private SecurityModule securityModule1;
        private SecurityStrategyComplex securityStrategyComplex1;
        private DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule objectsModule1;
        private DevExpress.ExpressApp.AuditTrail.AuditTrailModule auditTrailModule1;
        private DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule fileAttachmentsWebModule1;
        private DevExpress.ExpressApp.ReportsV2.ReportsModuleV2 reportsModuleV21;
        private DevExpress.ExpressApp.ReportsV2.Web.ReportsAspNetModuleV2 reportsAspNetModuleV21;
        private DevExpress.ExpressApp.Validation.ValidationModule validationModule1;
        private DevExpress.ExpressApp.Validation.Web.ValidationAspNetModule validationAspNetModule1;
        private DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule viewVariantsModule1;
        private DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule conditionalAppearanceModule1;
        private MainDemo.Module.Web.MainDemoWebModule mainDemoWebModule1;
        private MainDemo.Module.MainDemoModule mainDemoModule1;
        private AuthenticationStandard authenticationStandard1;
        private DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule schedulerAspNetModule1;
        private DevExpress.ExpressApp.Scheduler.SchedulerModuleBase schedulerModuleBase1;
        private DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule pivotChartAspNetModule1;
        private DevExpress.ExpressApp.PivotChart.PivotChartModuleBase pivotChartModuleBase1;
        private DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule htmlPropertyEditorAspNetModule1;
        private DevExpress.ExpressApp.ScriptRecorder.Web.ScriptRecorderAspNetModule scriptRecorderAspNetModule1;
        private DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase scriptRecorderModuleBase1;
        private DevExpress.ExpressApp.Notifications.NotificationsModule notificationsModule;
        private BYteWare.XAF.ElasticSearch.ElasticSearchModule elasticSearchModule1;
        private DevExpress.ExpressApp.Notifications.Web.NotificationsAspNetModule notificationsModuleWeb;

        public MainDemoWebApplication() {
            InitializeComponent();
            DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditor.AllowFilterControlHierarchy = true;
            DevExpress.ExpressApp.Web.Editors.ASPx.ASPxGridListEditor.MaxFilterControlHierarchyDepth = 3;
            DevExpress.ExpressApp.Web.Editors.ASPx.ASPxCriteriaPropertyEditor.AllowFilterControlHierarchyDefault = true;
            DevExpress.ExpressApp.Web.Editors.ASPx.ASPxCriteriaPropertyEditor.MaxHierarchyDepthDefault = 3;

        }
        protected override void DisposeCore() {
            base.DisposeCore();
        }
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainDemoWebApplication));
            this.systemModule1 = new DevExpress.ExpressApp.SystemModule.SystemModule();
            this.webSystemModule1 = new DevExpress.ExpressApp.Web.SystemModule.SystemAspNetModule();
            this.securityModule1 = new DevExpress.ExpressApp.Security.SecurityModule();
            this.securityStrategyComplex1 = new DevExpress.ExpressApp.Security.SecurityStrategyComplex();
            this.authenticationStandard1 = new DevExpress.ExpressApp.Security.AuthenticationStandard();
            this.objectsModule1 = new DevExpress.ExpressApp.Objects.BusinessClassLibraryCustomizationModule();
            this.auditTrailModule1 = new DevExpress.ExpressApp.AuditTrail.AuditTrailModule();
            this.fileAttachmentsWebModule1 = new DevExpress.ExpressApp.FileAttachments.Web.FileAttachmentsAspNetModule();
            this.reportsModuleV21 = new DevExpress.ExpressApp.ReportsV2.ReportsModuleV2();
            this.reportsAspNetModuleV21 = new DevExpress.ExpressApp.ReportsV2.Web.ReportsAspNetModuleV2();
            this.validationModule1 = new DevExpress.ExpressApp.Validation.ValidationModule();
            this.validationAspNetModule1 = new DevExpress.ExpressApp.Validation.Web.ValidationAspNetModule();
            this.viewVariantsModule1 = new DevExpress.ExpressApp.ViewVariantsModule.ViewVariantsModule();
            this.conditionalAppearanceModule1 = new DevExpress.ExpressApp.ConditionalAppearance.ConditionalAppearanceModule();
            this.mainDemoModule1 = new MainDemo.Module.MainDemoModule();
            this.mainDemoWebModule1 = new MainDemo.Module.Web.MainDemoWebModule();
            this.schedulerAspNetModule1 = new DevExpress.ExpressApp.Scheduler.Web.SchedulerAspNetModule();
            this.schedulerModuleBase1 = new DevExpress.ExpressApp.Scheduler.SchedulerModuleBase();
            this.pivotChartAspNetModule1 = new DevExpress.ExpressApp.PivotChart.Web.PivotChartAspNetModule();
            this.pivotChartModuleBase1 = new DevExpress.ExpressApp.PivotChart.PivotChartModuleBase();
            this.htmlPropertyEditorAspNetModule1 = new DevExpress.ExpressApp.HtmlPropertyEditor.Web.HtmlPropertyEditorAspNetModule();
            this.scriptRecorderModuleBase1 = new DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderModuleBase();
            this.scriptRecorderAspNetModule1 = new DevExpress.ExpressApp.ScriptRecorder.Web.ScriptRecorderAspNetModule();
            this.notificationsModule = new DevExpress.ExpressApp.Notifications.NotificationsModule();
            this.notificationsModuleWeb = new DevExpress.ExpressApp.Notifications.Web.NotificationsAspNetModule();
            this.elasticSearchModule1 = new BYteWare.XAF.ElasticSearch.ElasticSearchModule();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // securityStrategyComplex1
            // 
            this.securityStrategyComplex1.Authentication = this.authenticationStandard1;
            this.securityStrategyComplex1.RoleType = typeof(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyRole);
            this.securityStrategyComplex1.UserType = typeof(DevExpress.Persistent.BaseImpl.PermissionPolicy.PermissionPolicyUser);
            // 
            // authenticationStandard1
            // 
            this.authenticationStandard1.LogonParametersType = typeof(DevExpress.ExpressApp.Security.AuthenticationStandardLogonParameters);
            // 
            // auditTrailModule1
            // 
            this.auditTrailModule1.AuditDataItemPersistentType = typeof(DevExpress.Persistent.BaseImpl.AuditDataItemPersistent);
            // 
            // reportsModuleV21
            // 
            this.reportsModuleV21.EnableInplaceReports = true;
            this.reportsModuleV21.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.ReportDataV2);
            this.reportsModuleV21.ReportStoreMode = DevExpress.ExpressApp.ReportsV2.ReportStoreModes.XML;
            // 
            // validationModule1
            // 
            this.validationModule1.AllowValidationDetailsAccess = true;
            this.validationModule1.IgnoreWarningAndInformationRules = false;
            // 
            // pivotChartModuleBase1
            // 
            this.pivotChartModuleBase1.DataAccessMode = DevExpress.ExpressApp.CollectionSourceDataAccessMode.Client;
            this.pivotChartModuleBase1.ShowAdditionalNavigation = false;
            // 
            // notificationsModule
            // 
            this.notificationsModule.CanAccessPostponedItems = false;
            this.notificationsModule.NotificationsRefreshInterval = System.TimeSpan.Parse("00:05:00");
            this.notificationsModule.NotificationsStartDelay = System.TimeSpan.Parse("00:00:05");
            this.notificationsModule.ShowDismissAllAction = false;
            this.notificationsModule.ShowNotificationsWindow = true;
            this.notificationsModule.ShowRefreshAction = false;
            // 
            // elasticSearchModule1
            // 
            this.elasticSearchModule1.ElasticSearchIndexPersistentType = null;
            this.elasticSearchModule1.ElasticSearchIndexRefreshPersistentType = null;
            // 
            // MainDemoWebApplication
            // 
            this.ApplicationName = "MainDemo";
            this.CheckCompatibilityType = DevExpress.ExpressApp.CheckCompatibilityType.DatabaseSchema;
            this.Modules.Add(this.systemModule1);
            this.Modules.Add(this.webSystemModule1);
            this.Modules.Add(this.securityModule1);
            this.Modules.Add(this.objectsModule1);
            this.Modules.Add(this.auditTrailModule1);
            this.Modules.Add(this.fileAttachmentsWebModule1);
            this.Modules.Add(this.reportsModuleV21);
            this.Modules.Add(this.reportsAspNetModuleV21);
            this.Modules.Add(this.validationModule1);
            this.Modules.Add(this.validationAspNetModule1);
            this.Modules.Add(this.viewVariantsModule1);
            this.Modules.Add(this.conditionalAppearanceModule1);
            this.Modules.Add(this.pivotChartModuleBase1);
            this.Modules.Add(this.elasticSearchModule1);
            this.Modules.Add(this.mainDemoModule1);
            this.Modules.Add(this.htmlPropertyEditorAspNetModule1);
            this.Modules.Add(this.schedulerModuleBase1);
            this.Modules.Add(this.schedulerAspNetModule1);
            this.Modules.Add(this.mainDemoWebModule1);
            this.Modules.Add(this.pivotChartAspNetModule1);
            this.Modules.Add(this.scriptRecorderModuleBase1);
            this.Modules.Add(this.scriptRecorderAspNetModule1);
            this.Modules.Add(this.notificationsModule);
            this.Modules.Add(this.notificationsModuleWeb);
            this.Security = this.securityStrategyComplex1;
            this.DatabaseVersionMismatch += new System.EventHandler<DevExpress.ExpressApp.DatabaseVersionMismatchEventArgs>(this.MainDemoWebApplication_DatabaseVersionMismatch);
            this.LastLogonParametersRead += new System.EventHandler<DevExpress.ExpressApp.LastLogonParametersReadEventArgs>(this.MainDemoWebApplication_LastLogonParametersRead);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
        }
        private void MainDemoWebApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
            e.Updater.Update();
            e.Handled = true;
        }
        private void MainDemoWebApplication_LastLogonParametersRead(object sender, LastLogonParametersReadEventArgs e) {
            // Just to read demo user name for logon.
            AuthenticationStandardLogonParameters logonParameters = e.LogonObject as AuthenticationStandardLogonParameters;
            if(logonParameters != null) {
                if(String.IsNullOrEmpty(logonParameters.UserName)) {
                    logonParameters.UserName = "Sam";
                }
            }
        }
		protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(GetDataStoreProvider(args.ConnectionString, args.Connection), true));
			args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
		}
        private IXpoDataStoreProvider GetDataStoreProvider(string connectionString, System.Data.IDbConnection connection) {
            System.Web.HttpApplicationState application = (System.Web.HttpContext.Current != null) ? System.Web.HttpContext.Current.Application : null;
            IXpoDataStoreProvider dataStoreProvider = null;
            if(application != null && application["DataStoreProvider"] != null) {
                dataStoreProvider = application["DataStoreProvider"] as IXpoDataStoreProvider;
            }
            else {
                if(!String.IsNullOrEmpty(connectionString)) {
                    connectionString = DevExpress.Xpo.XpoDefault.GetConnectionPoolString(connectionString);
                    dataStoreProvider = new ConnectionStringDataStoreProvider(connectionString, true);
                }
                else if(connection != null) {
                    dataStoreProvider = new ConnectionDataStoreProvider(connection);
                }
                if(application != null) {
                    application["DataStoreProvider"] = dataStoreProvider;
                }
            }
            return dataStoreProvider;
        }
	}
}
