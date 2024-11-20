using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutomationPracticeSiteProject.Pages
{
    public class ViewOrderPage
    {
        private readonly IWebDriver driver;
        
        public ViewOrderPage(IWebDriver driver)
        {
            this.driver = driver;
            
        }

        public void ValidateNavigationToViewOrderPage(string orderNumber)
        {
            // Assert the url 
            Assert.Contains($"/view-order/{orderNumber}/",driver.Url);
        }

        public void Validate_PageContainsOrder_Customer_BillingDetails()
        {
            // Assert that the Order Details section is visible
            var orderDetails = driver.FindElement(By.ClassName("order_details"));
            Assert.True(orderDetails.Displayed, $"Url- {driver.Url},'Order Details section is not visible'");

            // Assert that the Customer Details section is visible
            var customerDetails = driver.FindElement(By.ClassName("customer_details"));
            Assert.True(customerDetails.Displayed, $"Url- {driver.Url},'Customer Details section is not visible'");

            // Assert that the Billing Details section is visible
            var billingAddressElement = driver.FindElement(By.ClassName("woocommerce-MyAccount-content"));
            var billingDetails = billingAddressElement.FindElement(By.TagName("address"));
            Assert.True(billingDetails.Displayed, $"Url- {driver.Url},'Billing Details section is not visible'");
        }
        public void ValidateOrderDetails(string expectedDetails)
        {
            // Assert the order details
            var actualDetails = driver.FindElement(By.ClassName("woocommerce-MyAccount-content"))
                .FindElement(By.TagName("p")).Text;
            Assert.Equal(expectedDetails, actualDetails);
        }
    }
}
