using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPracticeSiteProject.Models
{
    public class BillingDetails
    {
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        
        public int PostCode { get; set; }
        public string State {  get; set; }
        public string AdditionalInformation {  get; set; }


        public BillingDetails(string firstName, string lastName, string email, string phoneNumber, string address,
            string city, string country, int postCode, string state, string additionalInformation)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            City = city;
            Country = country;
            PostCode = postCode;
            State = state;
            AdditionalInformation = additionalInformation;

        }
    }
    public class CheckoutTestData
    {
        public static IEnumerable<object[]> BillingDetailsDataMandatoryFields => new List<object[]>
        {
            new object[]
            {
                new BillingDetails(
    "John",                // FirstName
    "Doe",                 // LastName
    "john.doe@example.com",// Email
    "+1234567890",         // PhoneNumber
    "123 Main St",         // Address
    "Cityville",           // City
    "Afghanistan",                 // Country
    12345,                 // PostCode
    "" ,                   // State
    ""                    // Additional Information
                      // IsMandatory
)
            }
           
        };
        public static IEnumerable<object[]> BillingDetailsDataNonMandatoryFields => new List<object[]>
        {
            new object[]
            {
                new BillingDetails("","","","","123 Main St","Cityville","Afghanistan",12345,"","Delivery Notes")

            }
        };
        public static IEnumerable<object[]> BillingDetailsDataWithInvalidEmail => new List<object[]>
        {
            new object[]
            {
                new BillingDetails("John","Doe","john.doe","+1234567890","123 Main St","Cityville","Afghanistan",12345,"","Delivery Notes")
            }
        };
        public static IEnumerable<object[]> BillingDetailsDataWithInvalidPhone => new List<object[]>
        {
            new object[]
            {
                new BillingDetails("John","Doe","john.doe@example.com","abc","123 Main St","Cityville","Afghanistan",12345,"","Delivery Notes")
            }
        };
    }
}
