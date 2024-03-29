using System;
using System.Collections.Generic;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.XtraGrid.Columns;

namespace MainDemo.Module.Win.Controllers {
    public partial class WinTooltipController : ViewController {
        public WinTooltipController() {
            InitializeComponent();
            RegisterActions(components);
        }
        private void WinTooltipController_ViewControlsCreated(object sender, EventArgs e) {
            if (((ListView)View).Editor is GridListEditor listEditor)
            {
                foreach (GridColumn column in listEditor.GridView.Columns)
                {
                    column.ToolTip = "Click to sort by " + column.Caption;
                }
            }
        }
    }
}
