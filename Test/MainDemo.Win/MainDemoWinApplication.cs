using System;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Xpo;

namespace MainDemo.Win {
	public partial class MainDemoWinApplication : WinApplication {
		public MainDemoWinApplication() {
			InitializeComponent();
            
            DevExpress.ExpressApp.ScriptRecorder.ScriptRecorderControllerBase.ScriptRecorderEnabled = true;
		}
		private void MainDemoWinApplication_DatabaseVersionMismatch(object sender, DatabaseVersionMismatchEventArgs e) {
			e.Updater.Update();
			e.Handled = true;
		}
        private void MainDemoWebApplication_LastLogonParametersRead(object sender, LastLogonParametersReadEventArgs e) {
            // Just to read demo user name for logon.
            if (e.LogonObject is AuthenticationStandardLogonParameters logonParameters)
            {
                if (String.IsNullOrEmpty(logonParameters.UserName))
                {
                    logonParameters.UserName = "Sam";
                }
            }
        }
        protected override void CreateDefaultObjectSpaceProvider(CreateCustomObjectSpaceProviderEventArgs args) {
            args.ObjectSpaceProviders.Add(new XPObjectSpaceProvider(args.ConnectionString, args.Connection));
			args.ObjectSpaceProviders.Add(new NonPersistentObjectSpaceProvider(TypesInfo, null));
		}
	}
}
