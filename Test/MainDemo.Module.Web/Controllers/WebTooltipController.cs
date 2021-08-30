using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Web.Editors.ASPx;
using DevExpress.Web;
using System;

namespace MainDemo.Module.Web.Controllers {
    public partial class WebTooltipController : ViewController {
        public WebTooltipController() {
            InitializeComponent();
            RegisterActions(components);
        }
        private void WebTooltipController_ViewControlsCreated(object sender, EventArgs e) {
            if (((ListView)View).Editor is ASPxGridListEditor listEditor)
            {
                ASPxGridView gridControl = listEditor.Grid;
                foreach (GridViewColumn column in gridControl.Columns)
                {
                    if ((column as GridViewDataColumn) != null)
                        column.ToolTip = "Click to sort by " + column.Caption;
                }
            }
        }
    }
}
