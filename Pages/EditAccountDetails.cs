using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutomationPracticeSiteProject.Pages
{
    public class EditAccountDetails
    {
        private readonly IWebDriver driver;
        public EditAccountDetails(IWebDriver driver)
        {
            this.driver = driver;
        }
        public void AssertNavigation()
        {
            Assert.Contains("/edit-account/", driver.Url);
        }
        public void AssertAccountDetails(string emailUsername)
        {
            // Locate the first Name from account details
            var accountFirstName = driver.FindElement(By.Id("account_first_name"));
            // Get the value from first name 
             var actualFirstName = accountFirstName.GetAttribute("value");
            // Assert First name
            Assert.Equal("yadvir",actualFirstName);

            // Locate the Last Name from account details
            var accountLastName = driver.FindElement(By.Id("account_last_name"));
            // Get the value from Last name 
            var actualLastName = accountLastName.GetAttribute("value");
            // Assert Last name
            Assert.Equal("jaswal", actualLastName);

            // Locate the  email address field from account details
            var accountEmail = driver.FindElement(By.Id("account_email"));
            // Get the value from email address 
            var actualEmail = accountEmail.GetAttribute("value");
            // Assert email address
            Assert.Equal(emailUsername, actualEmail);
        }
        public void ChangeExistingPassword(string currentPassword,string newPassword)
        {
            // Locate and enter into current password field
            var currentPasswordField = driver.FindElement(By.Id("password_current"));
            currentPasswordField.SendKeys(currentPassword);

            // Locate and enter into new password field
            var newPasswordField = driver.FindElement(By.Id("password_1"));
            newPasswordField.SendKeys(newPassword);

            // Locate and enter into confirm password field
            var confirmPasswordField = driver.FindElement(By.Id("password_2"));
            confirmPasswordField.SendKeys(newPassword);

            // Click on save changes button
            var saveChangesButton = driver.FindElement(By.CssSelector("input[value = 'Save changes']"));
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", saveChangesButton);
            
        }
        public void AssertSuccessMessage()
        {
            // Locate the success message element
            var successMessage = driver.FindElement(By.ClassName("woocommerce-message"));
            // Get the text from success message element
            var actualMessage = successMessage.Text;
            var expectedMessage = "Account details changed successfully.";
            Assert.Equal(expectedMessage, actualMessage);

            // Assert Navigation
            Assert.Contains("/my-account/", driver.Url);
        }
    }
}
