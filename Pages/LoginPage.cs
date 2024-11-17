

using AutomationPracticeSiteProject.Models;
using OpenQA.Selenium;

namespace AutomationPracticeSiteProject.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver driver;
        private RegistrationPage registrationPage;

        public LoginPage(IWebDriver driver)
        {
            this.driver = driver;
            registrationPage = new RegistrationPage(driver);
        }

        public void Enter_UserName_Password_ClickOnLogin(string userName, string password)
        {
            // Locate and enter into username field
            var usernameField = driver.FindElement(By.Id("username"));
            usernameField.SendKeys(userName);

            // Locate and enter into password field
            var passwordField = driver.FindElement(By.Id("password"));
            passwordField.SendKeys(password);

            // Locate and click Login button
            var loginButton = driver.FindElement(By.CssSelector("input[value = 'Login']"));
            loginButton.Click();
        }
        public void ValiadateErrorMessage_InvalidCredentials(string expectedErrorMessage)
        {
            // Get error Message
            var errorMessageText = registrationPage.ErrorMessageText();
            // Assert Message for invalid credentials
            var expectedMessage = expectedErrorMessage;
            var actualMessage = errorMessageText;
            Assert.Equal(expectedMessage, actualMessage);
        }
        public void Validate_PasswordField_HideInput()
        {
            // Locate password field
            var passwordField = driver.FindElement(By.Id("password"));
            // Get type of password field
            var fieldType = passwordField.GetAttribute("type");
            // type of password should be 'password'
            Assert.Equal("password",fieldType);
        }
    }
}
