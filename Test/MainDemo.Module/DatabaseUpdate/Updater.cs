using System;
using System.Data.SqlClient;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.PivotChart;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;
using MainDemo.Module.BusinessObjects;
using DevExpress.Persistent.BaseImpl.PermissionPolicy;
using System.Drawing;
using DevExpress.ExpressApp.Utils;

namespace MainDemo.Module.DatabaseUpdate
{
    public class Updater : ModuleUpdater
    {
        public Updater(IObjectSpace objectSpace, Version currentDBVersion) : base(objectSpace, currentDBVersion) { }
        public override void UpdateDatabaseAfterUpdateSchema()
        {
            base.UpdateDatabaseAfterUpdateSchema();
            UpdateAnalysisCriteriaColumn();

            #region Create a User for the Simple Security Strategy
            //// If a simple user named 'Sam' doesn't exist in the database, create this simple user
            //SecuritySimpleUser adminUser = ObjectSpace.FindObject<SecuritySimpleUser>(new BinaryOperator("UserName", "Sam"));
            //if(adminUser == null) {
            //    adminUser = ObjectSpace.CreateObject<SecuritySimpleUser>();
            //    adminUser.UserName = "Sam";
            //}
            //// Make the user an administrator
            //adminUser.IsAdministrator = true;
            //// Set a password if the standard authentication type is used
            //adminUser.SetPassword("");
            #endregion

            #region Create Users for the Complex Security Strategy
            // If a user named 'Sam' doesn't exist in the database, create this user
            PermissionPolicyUser user1 = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "Sam"));
            if (user1 == null)
            {
                user1 = ObjectSpace.CreateObject<PermissionPolicyUser>();
                user1.UserName = "Sam";
                // Set a password if the standard authentication type is used
                user1.SetPassword("");
            }
            // If a user named 'John' doesn't exist in the database, create this user
            PermissionPolicyUser user2 = ObjectSpace.FindObject<PermissionPolicyUser>(new BinaryOperator("UserName", "John"));
            if (user2 == null)
            {
                user2 = ObjectSpace.CreateObject<PermissionPolicyUser>();
                user2.UserName = "John";
                // Set a password if the standard authentication type is used
                user2.SetPassword("");
            }

            // If a role with the Administrators name doesn't exist in the database, create this role
            UserRole adminRole = ObjectSpace.FindObject<UserRole>(new BinaryOperator("Name", "Administrators"));
            if (adminRole == null)
            {
                adminRole = ObjectSpace.CreateObject<UserRole>();
                adminRole.Name = "Administrators";
            }
            adminRole.IsAdministrative = true;

            // If a role with the Users name doesn't exist in the database, create this role
            UserRole userRole = ObjectSpace.FindObject<UserRole>(new BinaryOperator("Name", "Users"));


            if (userRole == null)
            {
                userRole = ObjectSpace.CreateObject<UserRole>();
                userRole.Name = "Users";
                userRole.PermissionPolicy = SecurityPermissionPolicy.AllowAllByDefault;
                userRole.AddTypePermission<PermissionPolicyRole>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                userRole.AddTypePermission<PermissionPolicyUser>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                userRole.AddObjectPermission<PermissionPolicyUser>(SecurityOperations.ReadOnlyAccess, "[Oid] = CurrentUserId()", SecurityPermissionState.Allow);
                userRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "ChangePasswordOnFirstLogon", null, SecurityPermissionState.Allow);
                userRole.AddMemberPermission<PermissionPolicyUser>(SecurityOperations.Write, "StoredPassword", null, SecurityPermissionState.Allow);
                userRole.AddTypePermission<PermissionPolicyRole>(SecurityOperations.ReadWriteAccess, SecurityPermissionState.Allow);
                userRole.AddTypePermission<PermissionPolicyTypePermissionObject>("Write;Delete;Navigate;Create", SecurityPermissionState.Deny);
                userRole.AddTypePermission<PermissionPolicyMemberPermissionsObject>("Write;Delete;Navigate;Create", SecurityPermissionState.Deny);
                userRole.AddTypePermission<PermissionPolicyObjectPermissionsObject>("Write;Delete;Navigate;Create", SecurityPermissionState.Deny);

                userRole.AddTypePermission<Contact>(SecurityOperations.Read, SecurityPermissionState.Deny);
                userRole.AddTypePermission<Contact>("Write;Navigate;Create", SecurityPermissionState.Allow);
                userRole.AddObjectPermission<Contact>(SecurityOperations.ReadOnlyAccess, "StartsWith(FirstName, 'E')", SecurityPermissionState.Allow);
                userRole.AddObjectPermission<Contact>(SecurityOperations.FullObjectAccess, "UserRoles[Users[Oid = CurrentUserId()]]", SecurityPermissionState.Allow);
                //userRole.AddObjectPermission<Contact>(SecurityOperations.ReadWriteAccess, "\"bool\" : {\"must\" : [{\"terms\" : { \"userroles.name\" : ['users']}}]}", SecurityPermissionState.Allow);
            }

            // Add the Administrators role to the user1
            user1.Roles.Add(adminRole);
            // Add the Users role to the user2
            user2.Roles.Add(userRole);
            #endregion

            // PermissionPolicyRole defaultRole = CreateDefaultRole();

            Position developerPosition = ObjectSpace.FindObject<Position>(CriteriaOperator.Parse("Title == 'Developer'"));
            if (developerPosition == null)
            {
                developerPosition = ObjectSpace.CreateObject<Position>();
                developerPosition.Title = "Developer";
            }
            Position managerPosition = ObjectSpace.FindObject<Position>(CriteriaOperator.Parse("Title == 'Manager'"));
            if (managerPosition == null)
            {
                managerPosition = ObjectSpace.CreateObject<Position>();
                managerPosition.Title = "Manager";
            }

            Department devDepartment = ObjectSpace.FindObject<Department>(CriteriaOperator.Parse("Title == 'Development Department'"));
            if (devDepartment == null)
            {
                devDepartment = ObjectSpace.CreateObject<Department>();
                devDepartment.Title = "Development Department";
                devDepartment.Office = "205";
                devDepartment.Positions.Add(developerPosition);
                devDepartment.Positions.Add(managerPosition);
            }
            Department seoDepartment = ObjectSpace.FindObject<Department>(CriteriaOperator.Parse("Title == 'SEO'"));
            if (seoDepartment == null)
            {
                seoDepartment = ObjectSpace.CreateObject<Department>();
                seoDepartment.Title = "SEO";
                seoDepartment.Office = "703";
                seoDepartment.Positions.Add(developerPosition);
                seoDepartment.Positions.Add(managerPosition);
            }
            ImageConverter imageConverter = new ImageConverter();
            Contact contactMary = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Mary' && LastName == 'Tellitson'"));
            if (contactMary == null)
            {
                contactMary = ObjectSpace.CreateObject<Contact>();
                contactMary.FirstName = "Mary";
                contactMary.LastName = "Tellitson";
                contactMary.NickName = "Emma";
                contactMary.SpouseName = "Harry";
                contactMary.Email = "mary_tellitson@md.com";
                contactMary.Birthday = new DateTime(1980, 11, 27);
                contactMary.Department = devDepartment;
                contactMary.Notes = "In duties included control software components";
                contactMary.Position = managerPosition;
                contactMary.Photo = (byte[])imageConverter.ConvertTo(ImageLoader.Instance.GetImageInfo("Tellitson_Mary_Photo").Image, typeof(byte[]));
                contactMary.UserRoles.Add(userRole);
            }
            Contact contactJohn = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'John' && LastName == 'Nilsen'"));
            if (contactJohn == null)
            {
                contactJohn = ObjectSpace.CreateObject<Contact>();
                contactJohn.FirstName = "John";
                contactJohn.LastName = "Nilsen";
                contactJohn.NickName = "Eric";
                contactJohn.SpouseName = "Emma Watson";
                contactJohn.Email = "john_nilsen@md.com";
                contactJohn.Birthday = new DateTime(1981, 10, 3);
                contactJohn.Department = devDepartment;
                contactJohn.Position = developerPosition;
                contactJohn.Notes = "In duties included development of software modules";
                contactJohn.Photo = (byte[])imageConverter.ConvertTo(ImageLoader.Instance.GetImageInfo("Nilsen_John_Photo").Image, typeof(byte[]));
                contactJohn.UserRoles.Add(userRole);
                contactJohn.UserRoles.Add(adminRole);
            }
            Contact contactJanete = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Janete' && LastName == 'Limeira'"));
            if (contactJanete == null)
            {
                contactJanete = ObjectSpace.CreateObject<Contact>();
                contactJanete.TitleOfCourtesy = TitleOfCourtesy.Miss;
                contactJanete.FirstName = "Janete";
                contactJanete.LastName = "Limeira";
                contactJanete.NickName = "Dieter";
                contactJanete.SpouseName = "Monika";
                contactJanete.Email = "janete_limeira@md.com";
                contactJanete.Birthday = new DateTime(1981, 12, 21);
                contactJanete.Department = devDepartment;
                contactJanete.Position = managerPosition;
                contactJanete.Notes = "In duties included control software components";
                contactJanete.Photo = (byte[])imageConverter.ConvertTo(ImageLoader.Instance.GetImageInfo("Limeira_Janete_Photo").Image, typeof(byte[]));
                contactJanete.UserRoles.Add(userRole);
                contactJanete.UserRoles.Add(adminRole);
            }
            Contact contactKarl = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Karl' && LastName == 'Jablonski'"));
            if (contactKarl == null)
            {
                contactKarl = ObjectSpace.CreateObject<Contact>();
                contactKarl.FirstName = "Karl";
                contactKarl.LastName = "Jablonski";
                contactKarl.NickName = "Herbert";
                contactKarl.SpouseName = "Erna";
                contactKarl.Email = " karl_jablonski@md.com";
                contactKarl.Birthday = new DateTime(1975, 12, 19);
                contactKarl.Department = devDepartment;
                contactKarl.Position = developerPosition;
                contactKarl.Manager = contactJanete;
                contactKarl.Notes = "In duties included  development of software modules";
                contactKarl.Photo = (byte[])imageConverter.ConvertTo(ImageLoader.Instance.GetImageInfo("Jablonski_Karl_Photo").Image, typeof(byte[]));
                contactKarl.UserRoles.Add(userRole);
                contactKarl.UserRoles.Add(adminRole);
            }
            Contact contactCatherine = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Catherine' && LastName == 'Dewey'"));
            if (contactCatherine == null)
            {
                contactCatherine = ObjectSpace.CreateObject<Contact>();
                contactCatherine.TitleOfCourtesy = TitleOfCourtesy.Miss;
                contactCatherine.FirstName = "Catherine";
                contactCatherine.LastName = "Dewey";
                contactCatherine.NickName = "Frank";
                contactCatherine.SpouseName = "Heike";
                contactCatherine.Email = "catherine_dewey@md.com";
                contactCatherine.Birthday = new DateTime(1993, 7, 9);
                contactCatherine.Department = seoDepartment;
                contactCatherine.Position = managerPosition;
                contactCatherine.Notes = "In duties included control software components";
                contactCatherine.Photo = (byte[])imageConverter.ConvertTo(ImageLoader.Instance.GetImageInfo("Dewey_Catherine_Photo").Image, typeof(byte[]));
                contactCatherine.UserRoles.Add(adminRole);
            }
            Contact contactPaul = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Paul' && LastName == 'Henriot'"));
            if (contactPaul == null)
            {
                contactPaul = ObjectSpace.CreateObject<Contact>();
                contactPaul.TitleOfCourtesy = TitleOfCourtesy.Mr;
                contactPaul.FirstName = "Paul";
                contactPaul.LastName = "Henriot";
                contactPaul.NickName = "Lukas";
                contactPaul.SpouseName = "Christiana";
                contactPaul.Email = "paul_henriot@md.com";
                contactPaul.Birthday = new DateTime(1958, 1, 30);
                contactPaul.Department = seoDepartment;
                contactPaul.Position = developerPosition;
                contactPaul.Notes = "In duties included  development of software modules";
                contactPaul.Photo = (byte[])imageConverter.ConvertTo(ImageLoader.Instance.GetImageInfo("Henriot_Paul_Photo").Image, typeof(byte[]));
                contactPaul.UserRoles.Add(userRole);
                contactPaul.UserRoles.Add(adminRole);
            }
            Contact contactElizabeth = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Elizabeth' && LastName == 'Lincoln'"));
            if (contactElizabeth == null)
            {
                contactElizabeth = ObjectSpace.CreateObject<Contact>();
                contactElizabeth.TitleOfCourtesy = TitleOfCourtesy.Ms;
                contactElizabeth.FirstName = "Elizabeth";
                contactElizabeth.LastName = "Lincoln";
                contactElizabeth.NickName = "Que";
                contactElizabeth.SpouseName = "Alen NIx";
                contactElizabeth.Email = "elizabeth_lincoln@md.com";
                contactElizabeth.Birthday = new DateTime(1988, 3, 14);
                contactElizabeth.Department = seoDepartment;
                contactElizabeth.Position = managerPosition;
                contactElizabeth.Manager = contactCatherine;
                contactElizabeth.Notes = "In duties included control software components";
                contactElizabeth.Photo = (byte[])imageConverter.ConvertTo(ImageLoader.Instance.GetImageInfo("Lincoln_Elizabeth_Photo").Image, typeof(byte[]));
                contactElizabeth.UserRoles.Add(userRole);
                contactElizabeth.UserRoles.Add(adminRole);
            }

            Contact contactDaniel = ObjectSpace.FindObject<Contact>(CriteriaOperator.Parse("FirstName == 'Daniel' && LastName == 'Tonini'"));
            if (contactDaniel == null)
            {
                contactDaniel = ObjectSpace.CreateObject<Contact>();
                contactDaniel.FirstName = "Daniel";
                contactDaniel.LastName = "Tonini";
                contactDaniel.NickName = "Alexander";
                contactDaniel.SpouseName = "keth";
                contactDaniel.Email = "daniel_tonini@md.com";
                contactDaniel.Birthday = new DateTime(1980, 12, 30);
                contactDaniel.Department = seoDepartment;
                contactDaniel.Notes = "In duties included development of software modules";
                contactDaniel.Position = developerPosition;
                contactDaniel.Manager = contactElizabeth;
                contactDaniel.Photo = (byte[])imageConverter.ConvertTo(ImageLoader.Instance.GetImageInfo("Tonini_Daniel_Photo").Image, typeof(byte[]));
                contactDaniel.UserRoles.Add(adminRole);
            }

            if (ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Review reports'")) == null)
            {
                DemoTask task = ObjectSpace.CreateObject<DemoTask>();
                task.Subject = "Review reports";
                task.AssignedTo = contactJohn;
                task.StartDate = DateTime.Parse("May 03, 2008");
                task.DueDate = DateTime.Parse("September 06, 2008");
                task.Status = DevExpress.Persistent.Base.General.TaskStatus.InProgress;
                task.Priority = Priority.High;
                task.EstimatedWork = 60;
                task.Description = "Analyse the reports and assign new tasks to employees.";
            }

            if (ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Fix breakfast'")) == null)
            {
                DemoTask task = ObjectSpace.CreateObject<DemoTask>();
                task.Subject = "Fix breakfast";
                task.AssignedTo = contactMary;
                task.StartDate = DateTime.Parse("May 03, 2008");
                task.DueDate = DateTime.Parse("May 04, 2008");
                task.Status = DevExpress.Persistent.Base.General.TaskStatus.Completed;
                task.Priority = Priority.Low;
                task.EstimatedWork = 1;
                task.ActualWork = 3;
                task.Description = "The Development Department - by 9 a.m.\r\nThe R&QA Department - by 10 a.m.";
            }
            if (ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Task1'")) == null)
            {
                DemoTask task = ObjectSpace.CreateObject<DemoTask>();
                task.Subject = "Task1";
                task.AssignedTo = contactJohn;
                task.StartDate = DateTime.Parse("June 03, 2008");
                task.DueDate = DateTime.Parse("June 06, 2008");
                task.Status = DevExpress.Persistent.Base.General.TaskStatus.Completed;
                task.Priority = Priority.High;
                task.EstimatedWork = 10;
                task.ActualWork = 15;
                task.Description = "A task designed specially to demonstrate the PivotChart module. Switch to the Reports navigation group to view the generated analysis.";
            }
            if (ObjectSpace.FindObject<DemoTask>(CriteriaOperator.Parse("Subject == 'Task2'")) == null)
            {
                DemoTask task = ObjectSpace.CreateObject<DemoTask>();
                task.Subject = "Task2";
                task.AssignedTo = contactJohn;
                task.StartDate = DateTime.Parse("July 03, 2008");
                task.DueDate = DateTime.Parse("July 06, 2008");
                task.Status = DevExpress.Persistent.Base.General.TaskStatus.Completed;
                task.Priority = Priority.Low;
                task.EstimatedWork = 8;
                task.ActualWork = 16;
                task.Description = "A task designed specially to demonstrate the PivotChart module. Switch to the Reports navigation group to view the generated analysis.";
            }
            UpdateStatus("CreateAnalysis", "", "Creating analysis reports in the database...");
            CreateDataToBeAnalysed();
            UpdateStatus("CreateSecurityData", "", "Creating users and roles in the database...");


            ObjectSpace.CommitChanges();
        }
        private void CreateDataToBeAnalysed()
        {
            Analysis taskAnalysis1 = ObjectSpace.FindObject<Analysis>(CriteriaOperator.Parse("Name='Completed tasks'"));
            if (taskAnalysis1 == null)
            {
                taskAnalysis1 = ObjectSpace.CreateObject<Analysis>();
                taskAnalysis1.Name = "Completed tasks";
                taskAnalysis1.ObjectTypeName = typeof(DemoTask).FullName;
                taskAnalysis1.Criteria = "[Status] = ##Enum#DevExpress.Persistent.Base.General.TaskStatus,Completed#";
            }
            Analysis taskAnalysis2 = ObjectSpace.FindObject<Analysis>(CriteriaOperator.Parse("Name='Estimated and actual work comparison'"));
            if (taskAnalysis2 == null)
            {
                taskAnalysis2 = ObjectSpace.CreateObject<Analysis>();
                taskAnalysis2.Name = "Estimated and actual work comparison";
                taskAnalysis2.ObjectTypeName = typeof(DemoTask).FullName;
            }
        }
        private void UpdateAnalysisCriteriaColumn()
        {
            if (((XPObjectSpace)ObjectSpace).Session.Connection is SqlConnection)
            {
                int length = (int)ExecuteScalarCommand(@"select CHARACTER_MAXIMUM_LENGTH from INFORMATION_SCHEMA.Columns WHERE TABLE_NAME = 'Analysis' AND COLUMN_NAME = 'Criteria'", true);
                if (length != -1)
                {
                    ExecuteNonQueryCommand("alter table Analysis alter column Criteria nvarchar(max)", true);
                }
            }
        }
        //private PermissionPolicyRole CreateDefaultRole() {
        //    PermissionPolicyRole defaultRole = ObjectSpace.FindObject<PermissionPolicyRole>(new BinaryOperator("Name", "Default"));
        //    if(defaultRole == null) {
        //        defaultRole = ObjectSpace.CreateObject<PermissionPolicyRole>();
        //        defaultRole.Name = "Default";


        //    }
        //    return defaultRole;
        //}
    }
    public abstract class TaskAnalysis1LayoutUpdaterBase
    {
        protected IObjectSpace objectSpace;
        protected abstract IAnalysisControl CreateAnalysisControl();
        protected abstract IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control);
        public TaskAnalysis1LayoutUpdaterBase(IObjectSpace objectSpace)
        {
            this.objectSpace = objectSpace;
        }
        public void Update(Analysis analysis)
        {
            if (analysis != null && !PivotGridSettingsHelper.HasPivotGridSettings(analysis))
            {
                IAnalysisControl control = CreateAnalysisControl();
                control.DataSource = new AnalysisDataSource(analysis, objectSpace.GetObjects(typeof(DemoTask)));
                control.Fields["Priority"].Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
                control.Fields["Subject"].Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                control.Fields["AssignedTo.DisplayName"].Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
                PivotGridSettingsHelper.SavePivotGridSettings(CreatePivotGridSettingsStore(control), analysis);
            }
        }
    }
    public abstract class TaskAnalysis2LayoutUpdaterBase
    {
        protected IObjectSpace objectSpace;
        protected abstract IAnalysisControl CreateAnalysisControl();
        protected abstract IPivotGridSettingsStore CreatePivotGridSettingsStore(IAnalysisControl control);
        public TaskAnalysis2LayoutUpdaterBase(IObjectSpace objectSpace)
        {
            this.objectSpace = objectSpace;
        }
        public void Update(Analysis analysis)
        {
            if (analysis != null && !PivotGridSettingsHelper.HasPivotGridSettings(analysis))
            {
                IAnalysisControl control = CreateAnalysisControl();
                control.DataSource = new AnalysisDataSource(analysis, objectSpace.GetObjects(typeof(DemoTask)));
                control.Fields["Status"].Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
                control.Fields["Priority"].Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
                control.Fields["EstimatedWork"].Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                control.Fields["ActualWork"].Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
                control.Fields["AssignedTo.DisplayName"].Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
                PivotGridSettingsHelper.SavePivotGridSettings(CreatePivotGridSettingsStore(control), analysis);
            }
        }
    }
}
