namespace MainDemo.Module.Controllers
{
    partial class BogusController
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AddContacts = new DevExpress.ExpressApp.Actions.SimpleAction(this.components);
            // 
            // AddContacts
            // 
            this.AddContacts.Caption = "Testdaten hinzufügen";
            this.AddContacts.Category = "Edit";
            this.AddContacts.ConfirmationMessage = "Wollen Sie wirklich Tausende von Testdaten erzeugen?";
            this.AddContacts.Id = "Testdaten hinzufügen";
            this.AddContacts.TargetObjectType = typeof(MainDemo.Module.BusinessObjects.Contact);
            this.AddContacts.TargetViewType = DevExpress.ExpressApp.ViewType.ListView;
            this.AddContacts.ToolTip = null;
            this.AddContacts.TypeOfView = typeof(DevExpress.ExpressApp.ListView);
            this.AddContacts.Execute += new DevExpress.ExpressApp.Actions.SimpleActionExecuteEventHandler(this.AddContacts_Execute);
            // 
            // BogusController
            // 
            this.Actions.Add(this.AddContacts);

        }

        #endregion

        private DevExpress.ExpressApp.Actions.SimpleAction AddContacts;
    }
}
