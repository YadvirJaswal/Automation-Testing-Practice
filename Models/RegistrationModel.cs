using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPracticeSiteProject.Models
{
    public class RegistrationModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }

        public RegistrationModel(string emailAddress, string password)
        {
            EmailAddress = emailAddress;
            Password = password;
        }

        public static IEnumerable<object[]> validRegisterDetails1 = new List<object[]>
        {
            new object[]
            {
                new RegistrationModel(GenerateUniqueEmailWithGuid("John","gmail.com"),"Johndeo@001")
            }          
        };
        public static IEnumerable<object[]> validRegisterDetails2 = new List<object[]>
        {
            new object[]
            {
                new RegistrationModel("johndoe@example.com","Johndeo@003")
            }
        };
        public static IEnumerable<object[]> InValidEmailId_ValidPassword = new List<object[]>
        {
            new object[]
            {
                new RegistrationModel("john.doe","Johndeo@001")
            }
        };
        public static IEnumerable<object[]> EmptyEmailId = new List<object[]>
        {
            new object[]
            {
                new RegistrationModel("","John#deo@001")
            }
        };
        public static IEnumerable<object[]> EmptyPassword = new List<object[]>
        {
            new object[]
            {
                new RegistrationModel("john@example.com","")
            }
        };
        public static IEnumerable<object[]> EmptyPasswordAndEmailId = new List<object[]>
        {
            new object[]
            {
                new RegistrationModel("","")
            }
        };
        private static string GenerateUniqueEmailWithGuid(string baseName, string domain)
        {
            string guid = Guid.NewGuid().ToString();
            return $"{baseName}{guid}@{domain}";
        }

    }
}
