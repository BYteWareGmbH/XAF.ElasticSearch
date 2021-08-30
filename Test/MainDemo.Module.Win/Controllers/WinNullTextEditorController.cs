using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.Persistent.Base;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.ExpressApp.Utils;

namespace MainDemo.Module.Win.Controllers {
    public partial class WinNullTextEditorController : ViewController {
        public WinNullTextEditorController() {
            InitializeComponent();
            RegisterActions(components);
        }
        private void InitNullText(PropertyEditor propertyEditor) {
            ((BaseEdit)propertyEditor.Control).Properties.NullText = CaptionHelper.NullValueText;
        }
        public void TryInitializeAnniversaryItem() {
            if (((DetailView)View).FindItem("Anniversary") is PropertyEditor propertyEditor)
            {
                if (propertyEditor.Control != null)
                {
                    InitNullText(propertyEditor);
                }
                else
                {
                    propertyEditor.ControlCreated += new EventHandler<EventArgs>(propertyEditor_ControlCreated);
                }
            }
        }
        private void WinNullTextEditorController_Activated(object sender, EventArgs e) {
            ((CompositeView)View).ItemsChanged += WinNullTextEditorController_ItemsChanged;
            TryInitializeAnniversaryItem();
        }
        private void WinNullTextEditorController_ItemsChanged(object sender, ViewItemsChangedEventArgs e) {
            if(e.ChangedType == ViewItemsChangedType.Added && e.Item.Id == "Anniversary") {
                TryInitializeAnniversaryItem();
            }
        }
        private void propertyEditor_ControlCreated(object sender, EventArgs e) {
            InitNullText((PropertyEditor)sender);
        }
    }
}
