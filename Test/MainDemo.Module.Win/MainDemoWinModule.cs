using System;
using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Updating;

namespace MainDemo.Module.Win {
	[ToolboxItemFilter("Xaf.Platform.Win")]
	public sealed partial class MainDemoWinModule : ModuleBase {
		public MainDemoWinModule() {
			InitializeComponent();
			DevExpress.ExpressApp.Scheduler.Win.SchedulerListEditor.DailyPrintStyleCalendarHeaderVisible = false;
		}
        public override IEnumerable<ModuleUpdater> GetModuleUpdaters(IObjectSpace objectSpace, Version versionFromDB) {
            ModuleUpdater updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
            return new ModuleUpdater[] { updater };
        }
    }
}
