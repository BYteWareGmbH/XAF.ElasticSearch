using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Bogus;
using MainDemo.Module.BusinessObjects;
using BYteWare.XAF;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;

namespace MainDemo.Module.Controllers
{
    // For more typical usage scenarios, be sure to check out https://documentation.devexpress.com/eXpressAppFramework/clsDevExpressExpressAppWindowControllertopic.aspx.
    public partial class BogusController : ViewController
    {
        public BogusController()
        {
            InitializeComponent();
            // Target required Windows (via the TargetXXX properties) and create their Actions.
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target Window.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }

        private void AddContacts_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            WaitScreen.Instance.Show("Wait", " Please wait while the test data is adding.");
            if (ObjectSpace.IsModified)
            {
                ObjectSpace.CommitChanges();
                ObjectSpace.Refresh();
            }
       
            var verCList = new List<Contact>();
            CreateBogusContactData();
            WaitScreen.Instance.Hide();
        }

        private bool CreateBogusContactData()
        {
            try
            {
                var testContact = new Faker<BogusContact>("de");
                var contactList = testContact.Generate(1);

                foreach (var cObject in contactList)
                {
                    var random = new Random();
                    var c = ObjectSpace.CreateObject<Contact>();
                    c.FirstName = cObject.Person.CPerson.FirstName;
                    c.LastName = cObject.Person.CPerson.LastName;
                    c.NickName = cObject.Person.CPerson.NickName;
                    c.Email = cObject.Person.CPerson.Email;
                    c.WebPageAddress = cObject.Person.CPerson.Website;
                    c.Birthday = cObject.Person.CPerson.DateOfBirth;
                    c.SpouseName = cObject.Person.CPerson.SpouseName;
                    c.TitleOfCourtesy = (TitleOfCourtesy)Enum.GetValues(typeof(TitleOfCourtesy)).GetValue(random.Next(Enum.GetValues(typeof(TitleOfCourtesy)).Length));
                  
                    var cDepartment = ObjectSpace.FindObject<Department>(new BinaryOperator("Office", cObject.Office));
                    if (cDepartment == null)
                    {
                        c.Department = ObjectSpace.CreateObject<Department>();
                        c.Department.Title = cObject.Title;
                        c.Department.Office = cObject.Office;
                    }
                    else
                    {
                        c.Department = cDepartment;
                    }

                    var cPosition = ObjectSpace.FindObject<Position>(new BinaryOperator("Title", cObject.Positions.Title));
                    if (cPosition == null)
                    {
                        c.Position = ObjectSpace.CreateObject<Position>();
                        c.Position.Title = cObject.Positions.Title;
                    }
                    else
                    {
                        c.Position = cPosition;
                    }
                    var uRole = ObjectSpace.FindObject<UserRole>(new BinaryOperator("Name", cObject.UserRole));
                    if(uRole != null)
                    {
                        c.UserRoles.Add(uRole);
                    }
                }
                if (ObjectSpace.IsModified)
                {
                    ObjectSpace.CommitChanges();
                    ObjectSpace.Refresh();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
