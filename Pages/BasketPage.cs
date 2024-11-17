using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System.Data.Common;
using System;
using Xunit.Abstractions;


namespace AutomationPracticeSiteProject.Pages
{
    public class BasketPage
    {
        private readonly ITestOutputHelper testOutputHelper;
            
        private readonly IWebDriver driver;
        private WebDriverWait wait;

        public BasketPage(IWebDriver driver, ITestOutputHelper testOutputHelper)
        {
            this.driver = driver;
            this.testOutputHelper = testOutputHelper;
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        }
        public void VerifyProductDetailsInTheBasket(string expectedProductName, string expectedProductURL, string expectedProductPrice)
        {
            // get table row for the added product in the basket
            var basketItemNameTrElement = driver.FindElement(By.ClassName("cart_item"));

            var tds = basketItemNameTrElement.FindElements(By.TagName("td")).ToList();

            // get the product name and url from basket page
            var basketItemNameTdElement = basketItemNameTrElement.FindElement(By.ClassName("product-name"));
            var basketItemNameElement = basketItemNameTdElement.FindElement(By.TagName("a"));

            var actualBasketItemNameText = basketItemNameElement.Text;
            var actualBasketItemUrl = basketItemNameElement.GetAttribute("href");

            // Assert the product name and url
            Assert.Equal(expectedProductName, actualBasketItemNameText);
            Assert.Equal(expectedProductURL, actualBasketItemUrl);

            //Get the price of the product from basket page
            var basketItemPriceTdElement = tds[3];
            var actualItemPrice = basketItemPriceTdElement.Text;
            Assert.Equal(expectedProductPrice, actualItemPrice);
        }
        public double GetTotalPriceOfBook()
        {
            //testOutputHelper.WriteLine("Get intial prices .....");
            // Get initial price from order table
            var initialPriceTrElement = driver.FindElement(By.ClassName("order-total"));
            var td = initialPriceTrElement.FindElements(By.TagName("td")).ToList();
            var initialPriceElement = td[0];
            var initialPriceText = initialPriceElement.Text;
            double totalPrice = Convert.ToDouble(initialPriceText.Replace("₹", "").Trim());
            //testOutputHelper.WriteLine($"Initial total price before applying coupon - {totalPrice}");
            return totalPrice;
        }
        private void EnterValidCouponCode(string couponCode)
        {
            // Get couponcode field and enter the valid coupon code
            var CouponCodeField = driver.FindElement(By.Id("coupon_code"));
            CouponCodeField.SendKeys(couponCode);

            // Click on Apply Coupon button
            var applyCouponButton = driver.FindElement(By.CssSelector("input[value='Apply Coupon']"));
            applyCouponButton.Click();
        }
        public string GetSuccessMessage()
        {
            // wait for the element to be revealed
            var successMessage = wait.Until(d => driver.FindElement(By.ClassName("woocommerce-message")).Displayed);

            // Success message should display on the page
            Assert.True(successMessage, $"Current Url- {driver.Url}, 'Success message did'nt display on the page'");

            // get the text of the success
            var successMessageText = driver.FindElement(By.ClassName("woocommerce-message")).Text;
            return successMessageText;
        }
        public void ValidateSuccessMessage()
        {
            var successMessageText = GetSuccessMessage();

            // Assert the text of success message
            var expectedMessage = "Coupon code applied successfully.";
            var actualMessage = successMessageText;
            Assert.Equal(expectedMessage, actualMessage);
        }
        
        public void VerifyTheCouponCodeInOrderTotal(string couponCode)
        {
            // Get the updated price after applying coupon
            wait.Until(d => driver.FindElement(By.ClassName("coupon-krishnasakinala")).Displayed);
            var cartDiscountTrElement = driver.FindElement(By.ClassName("coupon-krishnasakinala"));
            var actualCouponCode = cartDiscountTrElement.FindElement(By.TagName("th")).Text;
            var expectedCouponCode = $"Coupon: {couponCode}";
            Assert.Equal(expectedCouponCode, actualCouponCode);
        }
        public void VerifyPriceAfterApplyValidCouponForPricesGreaterThan450()
        {
            // get the initial price of product
            var initialTotalPrice = GetTotalPriceOfBook();

            // Enter the valid coupon code 
            var couponCode = "krishnasakinala";
            EnterValidCouponCode(couponCode);
            testOutputHelper.WriteLine("Coupon applied");

            // Validate the success message after applying valid coupon code
            ValidateSuccessMessage();

            // Verify the coupon code in the order total table
            VerifyTheCouponCodeInOrderTotal(couponCode);

            // get the updated price of product after applying valid coupon code 
            var actualPrice = GetTotalPriceOfBook();

            // Verify that the user gets 51 rps discount after applying valid coupon code 
            var expectedPrice = initialTotalPrice - 51;
            testOutputHelper.WriteLine($"Expected price After applying coupon - {expectedPrice}");
            Assert.Equal(expectedPrice, actualPrice);
        }
        private void ValidateErrorMessageForLowerPrices()
        {
            // wait for the element to be revealed
            var errorMessage = wait.Until(d => driver.FindElement(By.ClassName("woocommerce-error")).Displayed);

            //Error message should display on the page
            Assert.True(errorMessage, $"Current Url- {driver.Url}, 'Error message did'nt display after applying valid coupon code for lower prices.'");

            // get the text of the success
            var errorMessageText = driver.FindElement(By.ClassName("woocommerce-error")).Text;

            // Assert the text of error message
            var expectedMessage = "The minimum spend for this coupon is ₹450.00.";
            var actualMessage = errorMessageText;
            Assert.Equal(expectedMessage, actualMessage);
        }
        
        public void VerifyPriceAfterApplyValidCouponForPricesLesserThan450()
        {
            // get the initial price of product
            var expectedPrice = GetTotalPriceOfBook();

            // Enter the valid coupon code 
            var couponCode = "krishnasakinala";
            EnterValidCouponCode(couponCode);
            testOutputHelper.WriteLine("Coupon applied");

            // Assert the error message after applying valid coupon code for lower prices.
            ValidateErrorMessageForLowerPrices();

            var actualPrice = GetTotalPriceOfBook();

            // Verify that the user get 0 rps discount after applying valid coupon code for lower price books
            testOutputHelper.WriteLine($"Expected price After applying coupon - {expectedPrice}");
            Assert.Equal(expectedPrice, actualPrice);
        }   
        private void EnterInvalidCouponCode(string invalidCouponCode)
        {
            // Get couponcode field and enter the invalid coupon code
            var CouponCodeField = driver.FindElement(By.Id("coupon_code"));
            CouponCodeField.SendKeys(invalidCouponCode);

            // Click on Apply Coupon button
            var applyCouponButton = driver.FindElement(By.CssSelector("input[value='Apply Coupon']"));
            applyCouponButton.Click();
        }
        private void ValidateErrorMessageForInvalidCoupon(string invalidCouponCode)
        {
            // wait for the element to be revealed
            var errorMessage = wait.Until(d => driver.FindElement(By.ClassName("woocommerce-error")).Displayed);

            //Error message should display on the page
            Assert.True(errorMessage, $"Current Url- {driver.Url}, 'Error message did'nt display after applying invalid.'");

            // get the text of the success
            var errorMessageText = driver.FindElement(By.ClassName("woocommerce-error")).Text;

            // Assert the text of error message
            var expectedMessage = $"Coupon \"{ invalidCouponCode}\" does not exist!";
            var actualMessage = errorMessageText;
            Assert.Equal(expectedMessage, actualMessage);
        }
       
        public void VerifyPricesAfterApplyingInValidCoupon()
        {
            // get the initial price of product
            var expectedPrice = GetTotalPriceOfBook();

            // Enter and apply invalid coupon code
            var invalidCouponCode = "abcd";
            EnterInvalidCouponCode(invalidCouponCode);

            // Assert the error message after applying invalid coupon code.
            ValidateErrorMessageForInvalidCoupon(invalidCouponCode) ;

            var actualPrice = GetTotalPriceOfBook();

            // Verify that the user get 0 rps discount after applying valid coupon code for lower price books
            testOutputHelper.WriteLine($"Expected price After applying coupon - {expectedPrice}");
            Assert.Equal(expectedPrice, actualPrice);
        }
       
        private List<IWebElement> GetTableColumnsFromCart(int index)
        {
            // Get the table row element from table
            var trs = driver.FindElements(By.ClassName("cart_item")).ToList();
            // Get the row of the book which we want to remove
            var tr = trs[index];
            // Get the table data element from the row
            var tds = tr.FindElements(By.TagName("td")).ToList();
            // Get the td that contains remove icon 
            
            return tds;
        }
        private void ClickonRemoveIcon()
        {
            var tds = GetTableColumnsFromCart(1);
            
            // Get the td that contains remove icon 
            var td = tds[0];
            // Get the remove icon link
            var removeIcon = td.FindElement(By.TagName("a"));
            // Click on remove icon
            removeIcon.Click();
        }
        private string GetNameOfBookToBeRemoved()
        {
            // get the name of the book which we want to remove
            var tds = GetTableColumnsFromCart(1);
            var td = tds[2].Text;
            return td;
        }
        private void ValidateSuccessMessageAfterRemovingBook()
        {
            // get the name of the book which we want to remove
            var td = GetNameOfBookToBeRemoved();
            // Success Message after removing the book should be displayed on the page
            var successMessageText = GetSuccessMessage();

            // Assert text of the success Message
            var expectedMessage = $"{td} removed. Undo?";
            var actualMessage = successMessageText;
            Assert.Equal(expectedMessage, actualMessage);
        }
        private int GetTheRowCountBeforeRemovingBook()
        {
            // Get the table row element from table
            var trElements = driver.FindElements(By.ClassName("cart_item")).ToList();
            var trCount = trElements.Count();
            return trCount;
        }
        private double GetThePriceOfBook(int rowIndex, int colIndex)
        {
            var tds = GetTableColumnsFromCart(rowIndex);
            var td = tds[colIndex].Text;
            double priceOfBook = Convert.ToDouble(td.Replace("₹", "").Trim());
            return priceOfBook;
        }
       
        private double GetTheSubTotalOFCart()
        {
            // Get the tr element of Cart total Table
            var trElements = driver.FindElement(By.ClassName("cart-subtotal"));
            var tds = trElements.FindElements(By.TagName("td")).ToList();
            var td = tds[0].Text;
            double priceInOrderSubTotal = Convert.ToDouble(td.Replace("₹", "").Trim());
            return priceInOrderSubTotal;

        }
        public void RemoveBookAndVerifyRemovedBookDetails()
        {
            // Get the price of the book which we want to remove
            var priceOfBookToBeRemoved = GetThePriceOfBook(1,3);

            // Get the subtotal Before removing the book
            var orderSubTotalBeforeRemoving = GetTheSubTotalOFCart();

            // Get the count of rows Before removing book
            var trCountBeforeRemovingBook = GetTheRowCountBeforeRemovingBook();

            //Click on Remove icon
            ClickonRemoveIcon();

            // Assert the success message after removing the book
            ValidateSuccessMessageAfterRemovingBook();

            // Get the count of rows after removing book
            var trCountAfterRemovingBook = GetTheRowCountBeforeRemovingBook();

            // Verify that the book is removed after click on removing icon
            var expectedCount = trCountBeforeRemovingBook - 1;
            var actualCount = trCountAfterRemovingBook;
            Assert.Equal(expectedCount, actualCount);

            // Get the subtotal after removing the book
            var orderSubTotalAfterRemovingBook = GetTheSubTotalOFCart();

            // Verify the price after removing the book
            var expectedPrice = orderSubTotalBeforeRemoving - priceOfBookToBeRemoved;
            var actualPrice = orderSubTotalAfterRemovingBook; 
            Assert.Equal(expectedPrice, actualPrice);
        }
        private void GetAndClickOnUndoOptionInSuccessMessage()
        {
            // wait for the element to be revealed
            var successMessage = wait.Until(d => driver.FindElement(By.ClassName("woocommerce-message")).Displayed);

            // Success message should display on the page
            Assert.True(successMessage, $"Current Url- {driver.Url}, 'Success message did'nt display on the page'");

            // Get the Undo option from successMessage
            var successMessageElement = driver.FindElement(By.ClassName("woocommerce-message"));
            var undoOption = successMessageElement.FindElement(By.TagName("a"));
            undoOption.Click();
        }
        public void VerifyUndoOptionInSuccessMessage()
        {
            //Get the name of the book before Undo
            var nameOfBookBeforeUndo = GetNameOfBookToBeRemoved();

            // Get the price of the book before Undo
            var priceOfBookBeforeUndo = GetThePriceOfBook(1,3);

            //Click on Remove icon
            ClickonRemoveIcon();

            // Assert the success message after removing the book
            ValidateSuccessMessageAfterRemovingBook();

            // Get the count of rows before undo
            var trCountBeforeUndo = GetTheRowCountBeforeRemovingBook();

            // Click on Undo option
            GetAndClickOnUndoOptionInSuccessMessage();

            // Get the count of rows before undo
            var trCountAfterUndo = GetTheRowCountBeforeRemovingBook();

            // Assert the row count after undo
            var expectedCount = trCountBeforeUndo + 1;
            var actualCount = trCountAfterUndo;
            Assert.Equal(expectedCount, actualCount);

            // Get the Name of the book After undo
            var nameBookAfterUndo = GetNameOfBookToBeRemoved();

            // Assert the name of the book after undo
            var expectedName = nameOfBookBeforeUndo;
            var actualName = nameBookAfterUndo;
            Assert.Equal(expectedName, actualName);

            // Get the price of the book after Undo
            var priceOfBookAfterUndo = GetThePriceOfBook(1,3);

            // Assert Price of the book after undo
            var expectedPrice = priceOfBookBeforeUndo;
            var actualPrice = priceOfBookAfterUndo;
            Assert.Equal(expectedPrice,actualPrice);
        }
        private IWebElement GetQuantityField(int rowIndex, int colIndex)
        {
           // Get the quantity input field column
            var tds = GetTableColumnsFromCart(rowIndex);
            var td = tds[colIndex];

            // Get the quantity input field from the coilumn
            var quantityField = td.FindElement(By.ClassName("qty"));
            return quantityField;
        }
        private void EnterQuantity(int quantity)
        {
            // Get the quantity input field
            var quantityField = GetQuantityField(0, 4);

            //Clear the quantity field
            quantityField.Clear();

            // Enter quantity into quantity field
            quantityField.SendKeys(quantity.ToString());
        }
        public void VerifyUpdateBasketButton()
        {
            //Get the subtotal before update quantity
            var orderSubTotalBeforeUpdate = GetTheSubTotalOFCart();

            // GetQuantity Field
            var quantityfield = GetQuantityField(0, 4);

            // Get the quantity value before update quantity
            var quantityValueBeforeUpdate = Convert.ToDouble(quantityfield.GetAttribute("value"));

            // Enter quantity into quantity field
            var quantity = 3;
            EnterQuantity(quantity);

            

            // Get the update basket button
            var updateBasketButton = driver.FindElement(By.Name("update_cart"));
            // Wait for the update basket button to be enabled
            var isUpdateBasketEnable = wait.Until(d => updateBasketButton.Enabled);

            if(isUpdateBasketEnable)
            {
                // Click on update basket button
                updateBasketButton.Submit();
            }
            // Assert the message after update the quantity
            // Get the text of the success message
            var successMessageText = GetSuccessMessage();
            var expectedMessage = "Basket updated.";
            var actualMessage = successMessageText;
            Assert.Equal(expectedMessage, actualMessage);

            // Assert the Total Price of book after update the basket

            // Get the Price of the book to be updated
            var priceOfBook = GetThePriceOfBook(0, 3);

            //GetQuantity Field after update
            var quantityfieldAfterUpdate = GetQuantityField(0, 4);
            // Get the value of quantity field
            var quantityTextAfterUpdation = Convert.ToDouble(quantityfieldAfterUpdate.GetAttribute("value")); 
            
            // Get total Price of book
            var totalPriceOfBook = GetThePriceOfBook(0, 5);

            // Assert the total price of book after update the quantity
            var expectedPrice = priceOfBook * quantityTextAfterUpdation;
            var actualPrice = totalPriceOfBook;
            Assert.Equal(expectedPrice, actualPrice);

            // Assert the subtotal of the cart after updation
            var orderSubTotalAfterUpdate = GetTheSubTotalOFCart();
            var quantityValueForSubtotal = quantityTextAfterUpdation - quantityValueBeforeUpdate;
            var expectedSubtotal = (quantityValueForSubtotal* priceOfBook) + orderSubTotalBeforeUpdate;
            var actualSubtotal = orderSubTotalAfterUpdate;
            Assert.Equal(expectedSubtotal, actualSubtotal);
        }
        public void VerifyTotalAlwaysGreaterThanSubTotal()
        {
            var priceInOrderSubTotal = GetTheSubTotalOFCart();
            var initialTotalPrice = GetTotalPriceOfBook();

            Assert.True(initialTotalPrice > priceInOrderSubTotal, $"Current URL = {driver.Url}, Total Price is not greater than subtotal.");
        }
        public void GetAndClickOnProceedToCheckoutButton()
        {
            // Get the proceed to checkout button
            var proceedToCheckoutButton = driver.FindElement(By.ClassName("checkout-button"));
            // Click on checkout button
            proceedToCheckoutButton.Click();
            
        }
    }
}
