using Bogus;
using Bogus.DataSets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainDemo.Module
{
    public class BogusContact
    {
        public BogusContact()
        {
            string locale = "de";
            Initialize(locale);
        }
        protected virtual void Initialize(string locale)
        {
            var PhoneType = new[] { "Home", "Office", "Private", "Personal" };
             var TitleOfCourtesy = new[] { "Dr", "Miss", "Mr", "Mrs", "Ms" };
            var PositionTitle = new[] { "Head", "Developer", "Manager", "Admin", "Marketing" };

            var lorem = new Bogus.DataSets.Lorem();
            var random = new Bogus.Randomizer();

            var bDeptName = new Bogus.DataSets.Name(locale);
            Title = bDeptName.JobTitle();
            Office = bDeptName.JobType();

            var gaddress = new Address(locale);

            this.Address = new CAddress
            {
                Street = gaddress.StreetAddress(),
                Suite = gaddress.SecondaryAddress(),
                City = gaddress.City(),
                ZipCode = gaddress.ZipCode(),
            };
            var cSpouseName = new Name(locale);
            var rValue = new Random();
            var cPerson = new Person(locale);
            this.Person = new Contact
            {
                CPerson = new Contact.Person
                {
                    FirstName = cPerson.FirstName,
                    LastName = cPerson.LastName,
                    NickName = cPerson.UserName,
                    DateOfBirth = cPerson.DateOfBirth,
                    Email = cPerson.Email,
                    Phone = cPerson.Phone,
                    Website = cPerson.Website,
                    SpouseName = cSpouseName.FindName()
                },
            };

            var cManager = new Person(locale);
          
            this.Manager = new Contact
            {
                CPerson = new Contact.Person
                {
                    FirstName = cManager.FirstName,
                    LastName = cManager.LastName,
                    NickName = cManager.UserName,
                    DateOfBirth = cManager.DateOfBirth,
                    Email = cManager.Email,
                    Phone = cManager.Phone,
                    SpouseName = cSpouseName.FindName(),
                    Website = cManager.Website
                },
                //TitleOfCourtesy = TitleOfCourtesy[rValue.Next(TitleOfCourtesy.Length)]
            };

            this.Positions = new Position
            {
                Title = PositionTitle[rValue.Next(PositionTitle.Length)]
            };
        }

        public class CAddress
        {
            public string Street;
            public string Suite;
            public string City;
            public string ZipCode;
        }

        public class Position
        {
            public string Title;
        }

        public class Contact
        {
            public class Person
            {
                public string FirstName;
                public string SpouseName;
                public string LastName;
                public string Email;
                public DateTime DateOfBirth;
                public CAddress Address;
                public string NickName;
                public string Phone;
                public string PhoneType;
                public string Website;
            }
            public Person CPerson;
            public string TitleOfCourtesy;
        }

        public string Title;
        public string Office;
        public string Description;
        public CAddress Address;
        public Contact Person;
        public Position Positions;
        public Contact Manager;
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
