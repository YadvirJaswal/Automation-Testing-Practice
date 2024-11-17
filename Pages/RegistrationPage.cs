using AutomationPracticeSiteProject.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;


namespace AutomationPracticeSiteProject.Pages
{
    public class RegistrationPage
    {
        private readonly IWebDriver driver;
        private CommonFeatures commonFeatures;
        private WebDriverWait wait;

        public RegistrationPage(IWebDriver driver)
        {
            this.driver = driver;
            commonFeatures = new CommonFeatures(driver);
            wait = new WebDriverWait(driver,TimeSpan.FromSeconds(10));
        }

       public void EnterEmailIdAndPassword_ClickOnRegisterButton(RegistrationModel registrationModel)
        {
            // Locate and enter into email Id field
            var emailIdField = driver.FindElement(By.Id("reg_email"));
            emailIdField.SendKeys(registrationModel.EmailAddress);

            // Locate and enter into password Id field
            var passwordField = driver.FindElement(By.Id("reg_password"));
            passwordField.SendKeys(registrationModel.Password);

            //driver.FindElement(By.ClassName("woocomerce-FormRow")).Click();
           
            // wait for the paasword strenght message to be displayed
            //var isPasswordStrengthDisplayed = wait.Until(d => driver.FindElement(By.ClassName("woocommerce-password-strength")).Displayed);

            var isRegisterButtonEnabled = wait.Until(d => driver.FindElement(By.CssSelector("input[value='Register']")).Enabled);
            if (isRegisterButtonEnabled)
            {
                // Locate and click on Register button
                var registerButton = driver.FindElement(By.CssSelector("input[value='Register']"));
                registerButton.Click();
            }
          
        }
        public void ValidateNavigationToMyAccountPage()
        {
            // Assert that user should navigate to my account page after click on Register and login button
            var expectedUrl = "https://practice.automationtesting.in/my-account/";
            var actualUrl = driver.Url;
            Assert.Equal(expectedUrl, actualUrl);
        }
        public void ValidateEmailAddress()
        {

        }
        public string ErrorMessageText()
        {
            var isErrorMessageDisplay = wait.Until(d => driver.FindElement(By.ClassName("woocommerce-error")).Displayed);
            Assert.True(isErrorMessageDisplay, $" Url- {driver.Url}, 'Error Message is not displayed.'");
            // Locate the error Message for password and get the text
            var errorMessage = driver.FindElement(By.ClassName("woocommerce-error")).Text;
            return errorMessage;
        }
        public void ValidatePasswordErrorMessage()
        {
            var errorMessage = ErrorMessageText();
           
            // Assert the error message
            var expectedMessage = "Error: Please enter an account password.";
            var actualMessage = errorMessage;
            Assert.Equal(expectedMessage, actualMessage);
        }
        public void ValidateEmailErrorMessage()
        {
            var errorMessage = ErrorMessageText();

            // Assert the error message
            var expectedMessage = "Error: Please provide a valid  email address.";
            var actualMessage = errorMessage;
            Assert.Equal(expectedMessage, actualMessage);

        }
       
    }
}
