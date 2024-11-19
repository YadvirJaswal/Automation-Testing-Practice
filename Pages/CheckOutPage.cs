using AutomationPracticeSiteProject.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace AutomationPracticeSiteProject.Pages
{
    public class CheckOutPage
    {
        private readonly IWebDriver driver;
        private BasketPage basketPage;
        private OrderRecievedPage orderRecievedPage;
        private ITestOutputHelper testOutputHelper;
        private WebDriverWait wait;

        public CheckOutPage(IWebDriver driver)
        {
            this.driver = driver;
            basketPage = new BasketPage(driver, testOutputHelper);
            orderRecievedPage = new OrderRecievedPage(driver);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void VerifyUrlOfCheckOutPage()
        {
            // Get the Url of CheckoutPage
            var expectedUrl = "https://practice.automationtesting.in/checkout/";
            // Get the current page Url
            var actualUrl = driver.Url;
            // Assert the url
            Assert.Equal(expectedUrl, actualUrl);
        }
        public void VerifyThatCheckoutPageContainsPaymentGateway()
        {
            // Locate the payment gateway
            var paymentGateway = driver.FindElement(By.Id("payment"));
            // Locate all the Payment Methods
            var paymentMethods = paymentGateway.FindElements(By.TagName("li"));
            // Checkout page contains the payment methods
            Assert.True(paymentMethods.Count > 0, $"Url- {driver.Url}, 'Checkout page should be contain payment methods'");
            // Checkout page should be contain the four Methods  
            Assert.True(paymentMethods.Count == 4, $"Url- {driver.Url}, 'Checkout page should be contain payment methods'");
            // Validate the cash on delivery payment method should be there
            var hasCashOnDeliveryOption = paymentMethods.Any(li => li.Text.Contains("Cash on Delivery"));
            Assert.True(hasCashOnDeliveryOption, $"Url- {driver.Url} Cash on Delivery Method should be available");
        }
       
        public void EnterAndApplyCouponInCheckoutPage()
        {
            //Get the total price before apply coupon code
            var initiaPrice = basketPage.GetTotalPriceOfBook();

            var couponCode = "krishnasakinala";
            // Click on a link to enter a coupon
            var lnkEnterYourCode = driver.FindElement(By.ClassName("showcoupon"));
            lnkEnterYourCode.Click();
            // Locate input field for coupon code
            var enterCodeField = driver.FindElement(By.Id("coupon_code"));
            enterCodeField.SendKeys(couponCode);
            // Click on Apply Coupon button
            var applyCouponButton = driver.FindElement(By.Name("apply_coupon"));
            applyCouponButton.Submit();
            // Assert success Message
            basketPage.ValidateSuccessMessage();
            // Validate coupon code in order total
            basketPage.VerifyTheCouponCodeInOrderTotal(couponCode);
            // //Get the total price after apply coupon code
            var actualPrice = basketPage.GetTotalPriceOfBook();
            var expectedPrice = initiaPrice - 51;
            //Assert the price after apply coupon
            Assert.Equal(expectedPrice, actualPrice);
        }
        private double EnterDetailsInBilling(BillingDetails billingDetails)
        {
            // Locate and Enter first Name
            var lnkFirstName = driver.FindElement(By.Id("billing_first_name"));
            lnkFirstName.SendKeys(billingDetails.FirstName);
            // Locate and Enter Last Name
            var lnkLastName = driver.FindElement(By.Id("billing_last_name"));
            lnkLastName.SendKeys(billingDetails.LastName);
            // Locate and Enter email Address
            var lnkEmail = driver.FindElement(By.Id("billing_email"));
            lnkEmail.SendKeys(billingDetails.Email);
            //Locate and Enter phone number
            var lnkPhone = driver.FindElement(By.Id("billing_phone"));
            lnkPhone.SendKeys(billingDetails.PhoneNumber);
            //Locate and fill additional information
            var additionalInformation = driver.FindElement(By.Id("order_comments"));
            additionalInformation.SendKeys(billingDetails.AdditionalInformation);
            //Locate and Select phone number
            var lnkCountryDropDown = driver.FindElement(By.Id("s2id_billing_country"));
            // Use JavascriptExecutor to avoid element click intercepted exception
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", lnkPhone);
            lnkCountryDropDown.Click();
            var lnkCountrySearch = driver.FindElement(By.Id("s2id_autogen1_search"));
            lnkCountrySearch.SendKeys(billingDetails.Country + Keys.Enter);
            // Locate and enter address
            var lnkAddress = driver.FindElement(By.Id("billing_address_1"));
            lnkAddress.SendKeys(billingDetails.Address);
            // Locate and enter address
            var lnkCity = driver.FindElement(By.Id("billing_city"));
            lnkCity.SendKeys(billingDetails.City);
            // Locate and enter postcode
            var lnkPostCode = driver.FindElement(By.Id("billing_postcode"));
            lnkPostCode.SendKeys(billingDetails.PostCode.ToString());


            var isPaymentDivDisplayed = wait.Until(d => driver.FindElement(By.Id("payment")).Displayed);
            if (isPaymentDivDisplayed)
            {
                // Select payment gateway
                var lnkPaymentMethod = driver.FindElement(By.Id("payment_method_cod"));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", driver.FindElement(By.Id("payment")));
                lnkPaymentMethod.Click();
            }
            var initiaPrice = basketPage.GetTotalPriceOfBook();
            var isPlaceOrderDisplayed = wait.Until(d => driver.FindElement(By.Id("place_order")).Displayed);
            if (isPlaceOrderDisplayed)
            {
                IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
                // Click on place order method
                var lnkPlaceOrderButton = driver.FindElement(By.Id("place_order"));

                jsExecutor.ExecuteScript("arguments[0].click();", lnkPlaceOrderButton);
            }
            return initiaPrice;
        }
        public void PlaceAnOrderByFillingOnlyMandatoryFields(BillingDetails billingDetails)
        {
            var initialPrice = EnterDetailsInBilling(billingDetails);
            
            // Assert order details after placing order
            orderRecievedPage.ValidateOrderDetails(initialPrice,5);
        }
        public void PlaceAnOrderByNotFillingOnlyMandatoryFields(BillingDetails billingDetails)
        {
            // enter data into fields
            EnterDetailsInBilling(billingDetails);

            // Assert that the order should not be placed without filling mandatory fields, and error message for mandatory
            // fields should be displayed
            var isErrorMessageDisplayed = driver.FindElement(By.ClassName("woocommerce-error")).Displayed;
            Assert.True(isErrorMessageDisplayed,$"Url- {driver.Url}, 'Error message should be displayed for the mandatory fields'");
            var errorMessageUl = driver.FindElement(By.ClassName("woocommerce-error"));
            var errorMessageLi = errorMessageUl.FindElements(By.TagName("li")).ToList();

            // Assert Message for First Name
            var expectedFirstNameError = "Billing First Name is a required field.";
            var actualFirstNameError = errorMessageLi[0].Text;
            Assert.Equal(expectedFirstNameError, actualFirstNameError);

            // Assert Message for Last Name
            var expectedLastNameError = "Billing Last Name is a required field.";
            var actualLastNameError = errorMessageLi[1].Text;
            Assert.Equal(expectedLastNameError, actualLastNameError);

            // Assert Message for Email
            var expectedEmailError = "Billing Email Address is a required field.";
            var actualEmailError = errorMessageLi[2].Text;
            Assert.Equal(expectedEmailError, actualEmailError);

            // Assert Message for Phone Number
            var expectedPhoneError = "Billing Phone is a required field.";
            var actualPhoneError = errorMessageLi[3].Text;
            Assert.Equal(expectedPhoneError, actualPhoneError);

           
        }
        public void PlaceAnOrderByFillingInvalidEmail(BillingDetails billingDetails)
        {
            // enter data into fields
            EnterDetailsInBilling(billingDetails);

            // Assert error message for invalid email address
            var emailElement = driver.FindElement(By.Id("billing_email_field"));
            var classElement = emailElement.GetAttribute("class");
            Assert.Contains("woocommerce-invalid-email",classElement);
        }
        public void PlaceAnOrderByFillingInvalidPhoneNumber(BillingDetails billingDetails)
        {
            // enter data into fields
            EnterDetailsInBilling(billingDetails);

            // Assert error message for invalid email address
            var isErrorMessageDisplayed = driver.FindElement(By.ClassName("woocommerce-error")).Displayed;
            Assert.True(isErrorMessageDisplayed, $"Url- {driver.Url}, 'Error message should be displayed for the mandatory fields'");
            var errorMessageUl = driver.FindElement(By.ClassName("woocommerce-error"));
            var errorMessageLi = errorMessageUl.FindElements(By.TagName("li")).ToList();
            var expectedPhoneError = "Phone is not a valid phone number.";
            var actualPhoneError = errorMessageLi[0].Text;
            Assert.Equal(expectedPhoneError,actualPhoneError);
        }

       


    }
}
