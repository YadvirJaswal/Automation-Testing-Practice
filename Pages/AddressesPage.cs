using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationPracticeSiteProject.Models;
using OpenQA.Selenium;

namespace AutomationPracticeSiteProject.Pages
{
    public class AddressesPage
    {
        private readonly IWebDriver driver;

        public AddressesPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void Validate_AddressPageDetails()
        {
            // Assert Navigate to address page
            Assert.Contains("/edit-address/",driver.Url);

            // Assert that the Billing Address section is visible
            var billingAddress = driver.FindElement(By.ClassName("u-column1"));
            Assert.True(billingAddress.Displayed, $"Url- {driver.Url},'Billing Address section is not visible'");

            // Assert that the Shipping Address section is visible
            var shipAddress = driver.FindElement(By.ClassName("u-column2"));
            Assert.True(shipAddress.Displayed, $"Url- {driver.Url},'Ship Address section is not visible'");
        }
        public void ClickOnEditButton()
        {
            // Locate Shipping section
            var shipAddress = driver.FindElement(By.ClassName("u-column2"));
            // Locate and click on edit button under shipping section
            var editShippingAddress = shipAddress.FindElement(By.ClassName("edit"));
            editShippingAddress.Click();
        }
        public void EnterShippingDetails(BillingDetails billingDetails)
        {
            // Locate and Enter first Name
            var lnkFirstName = driver.FindElement(By.Id("shipping_first_name"));
            lnkFirstName.Clear();
            lnkFirstName.SendKeys(billingDetails.FirstName);
            // Locate and Enter Last Name
            var lnkLastName = driver.FindElement(By.Id("shipping_last_name"));
            lnkLastName.Clear();
            lnkLastName.SendKeys(billingDetails.LastName);
            //Locate and Select country
            var lnkCountryDropDown = driver.FindElement(By.Id("s2id_shipping_country"));
            lnkCountryDropDown.Click();
            var lnkCountrySearch = driver.FindElement(By.Id("s2id_autogen1_search"));
            lnkCountrySearch.Clear();
            lnkCountrySearch.SendKeys(billingDetails.Country + Keys.Enter);
            // Locate and enter address
            var lnkAddress = driver.FindElement(By.Id("shipping_address_1"));
            lnkAddress.Clear();
            lnkAddress.SendKeys(billingDetails.Address);
            // Locate and enter city
            var lnkCity = driver.FindElement(By.Id("shipping_city"));
            lnkCity.Clear();
            lnkCity.SendKeys(billingDetails.City);
            // Locate and enter postcode
            var lnkPostCode = driver.FindElement(By.Id("shipping_postcode"));
            lnkPostCode.Clear();
            lnkPostCode.SendKeys(billingDetails.PostCode.ToString());
            // Locate and click on save address button
            var saveAddressButton = driver.FindElement(By.CssSelector("input[value='Save Address']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", saveAddressButton);
        }
        public void AssertNavigationAndMessage()
        {
            Assert.Contains("/my-account/", driver.Url);

            // Get success Message
            var successMessage = driver.FindElement(By.ClassName("woocommerce-message"));
            var expectedMessage = "Address changed successfully.";
            var actualMessage = successMessage.Text;
            Assert.Equal(expectedMessage, actualMessage);
        }
        
    }
}
