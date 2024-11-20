using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutomationPracticeSiteProject.Pages
{
    public class OrdersPage
    {
        private readonly IWebDriver driver;
        private ViewOrderPage viewOrderPage;
        public OrdersPage(IWebDriver driver)
        {
            this.driver = driver;
            viewOrderPage = new ViewOrderPage(driver);
        }

        public void Verify_OrdersShouldDisplay()
        {
            // Assert the url of the orders page after click on orders option
            Assert.Contains("/orders/", driver.Url);

            //Assert that user must view their orders on clicking orders link
            var isOrdersDisplayed = driver.FindElement(By.ClassName("account-orders-table")).Displayed;
            Assert.True(isOrdersDisplayed, $"Current url- {driver.Url}, 'Orders are not displayed on orders page'");
        }
        private List<IWebElement> GetOrderTable(int trIndex)
        {
            // Get the Order Table Element
            var OrderTableElement = driver.FindElement(By.ClassName("woocommerce-MyAccount-orders"));

            // Get trs from table
            var trs = OrderTableElement.FindElements(By.TagName("tr"));

            // Get the tr which contains OrderNumber, OrderDate and order status
            var tr = trs[trIndex];

            // Get all the columns of above row
            var tds = tr.FindElements(By.TagName("td")).ToList();
            return tds;

        }
      
        public string GetOrderNumber()
        {
            var tds = GetOrderTable(1);
            // Get the column which contains Order Number
            var orderNumberText = tds[0].Text;
            var orderNumber = orderNumberText.Replace("#","").Trim();
            return orderNumber;
        }
        public string GetOrderDate()
        {
            var tds = GetOrderTable(1);
            // Get the column which contains Order Date
            var orderDate = tds[1].Text;
            return orderDate;
        }
        public string GetOrderStatus()
        {
            var tds = GetOrderTable(1);
            // Get the column which contains Order Date
            var orderStatus = tds[2].Text;
            return orderStatus;
        }
        public void ClickOnViewOrder_AssertNavigation()
        {
            //Get Order number before click on view button
            var orderNumber = GetOrderNumber();
            // Locate and click on view order button
            var lnkViewButton = driver.FindElement(By.ClassName("view"));
            lnkViewButton.Click();

            // Assert Navigation
            viewOrderPage.ValidateNavigationToViewOrderPage(orderNumber);
        }
        public void ClickOnViewOrder_AssertDetails()
        {
            // Get all the details before click the view order button
            var orderNumber = GetOrderNumber();
            var orderDate = GetOrderDate();
            var orderStatus = GetOrderStatus();

            // Locate and click on view order button
            var lnkViewButton = driver.FindElement(By.ClassName("view"));
            lnkViewButton.Click();

            // Assert the deetails after click the view basket button
            var expectedDetails = $"Order #{orderNumber} was placed on {orderDate} and is currently {orderStatus}.";
            viewOrderPage.ValidateOrderDetails(expectedDetails);
        }
    }
    
}
