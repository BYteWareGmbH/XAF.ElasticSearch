using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Web.Editors;
using DevExpress.Web;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Utils;

namespace MainDemo.Module.Web.Controllers {
    public partial class WebNullTextEditorController : ViewController {
        public WebNullTextEditorController() {
            InitializeComponent();
            RegisterActions(components);
        }

        private void InitNullText(WebPropertyEditor propertyEditor) {
            if(propertyEditor.ViewEditMode == DevExpress.ExpressApp.Editors.ViewEditMode.Edit) {
                ((ASPxDateEdit)propertyEditor.Editor).NullText = CaptionHelper.NullValueText;
            }
        }
        private void WebNullTextEditorController_Activated(object sender, EventArgs e) {
            WebPropertyEditor propertyEditor = ((DetailView)View).FindItem("Anniversary") as WebPropertyEditor;
            if(propertyEditor != null) {
                if(propertyEditor.Control != null) {
                    InitNullText(propertyEditor);
                }
                else {
                    propertyEditor.ControlCreated += new EventHandler<EventArgs>(propertyEditor_ControlCreated);
                }
            }
        }
        private void propertyEditor_ControlCreated(object sender, EventArgs e) {
            InitNullText((WebPropertyEditor)sender);
        }
    }
}
