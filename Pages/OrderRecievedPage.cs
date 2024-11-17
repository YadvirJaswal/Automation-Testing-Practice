using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationPracticeSiteProject.Pages
{
    public class OrderRecievedPage
    {
        private readonly IWebDriver driver;

        public OrderRecievedPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        public void ValidateOrderDetails(double initialPrice,int index)
        {
            // Assert the order confirmation
            var confirmOrderTableText = driver.FindElement(By.ClassName("woocommerce-thankyou-order-received")).Text;
            var isOrderConfirm = driver.PageSource.Contains(confirmOrderTableText);
            Assert.True(isOrderConfirm);

            // Assert the details of confirm order table
            var confirmOrderTable = driver.FindElement(By.ClassName("woocommerce-thankyou-order-details"));
            var confirmOrderTableDetails = confirmOrderTable.FindElements(By.TagName("li")).ToList();
            // Assert that Confirm Order table contain Payment Method
            var hasOrderNumber = confirmOrderTableDetails[3].FindElement(By.TagName("strong")).Text == "Cash on Delivery";
            Assert.True(hasOrderNumber, $"Url- {driver.Url}, 'Confirm Order Table should contain order number.'");

            // Assert Name of book and Total Price in Order details table
            var orderDetailsTable = driver.FindElement(By.ClassName("shop_table"));
            var trs = orderDetailsTable.FindElements(By.TagName("tr")).ToList();
            var totalPriceText = trs[index].FindElement(By.TagName("td")).Text;
            double totalPrice = Convert.ToDouble(totalPriceText.Replace("₹", "").Trim());

            Assert.Equal(initialPrice, totalPrice);
        }
    }
}
