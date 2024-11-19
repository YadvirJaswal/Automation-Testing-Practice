using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Bson;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;

namespace AutomationPracticeSiteProject.Pages
{
    public class ShopPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private readonly ITestOutputHelper testOutputHelper;

        public ShopPage(IWebDriver driver, ITestOutputHelper testOutputHelper)
        {
            this.driver = driver;
            this.testOutputHelper = testOutputHelper;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void MoveSliderLeft_VerifyPostion()
        {
            // Get the price slider
            var sliderHandles = driver.FindElements(By.ClassName("ui-slider-handle"));
            // Get the right handle of price slider
            var rightHandle = sliderHandles[1];
            // Locate the slider track
            var sliderTrack = driver.FindElement(By.ClassName("ui-slider-range"));
            // Get the slider width
            var sliderWidth = sliderTrack.Size.Width;
            testOutputHelper.WriteLine(sliderWidth.ToString());
            // Calculate offset to move to 85.7143 %
            var moveByOffset = -(int)(sliderWidth * 0.148571);


            // Move the right handle
            var actions = new Actions(driver);
            actions.ClickAndHold(rightHandle).MoveByOffset(moveByOffset, 0).Release().Perform();

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            string rightPosition = rightHandle.GetCssValue("left");
            double leftValue = double.Parse(rightPosition.Replace("px", "").Trim());
            int integerPart = (int)leftValue;

            // Assert that the left position is now 450
            Assert.Equal(142, integerPart);
        }
        public void ClickOnFilterButton_AssertPrices()
        {
            var filterButtonElement = driver.FindElement(By.ClassName("price_slider_amount"));
            var filterButton = filterButtonElement.FindElement(By.TagName("button"));
            filterButton.Submit();

            // Assert the Prices after apply filter
            var minPrice = driver.FindElement(By.Id("min_price")).GetAttribute("value");
            Assert.Equal("150", minPrice);
            var maxPrices = driver.FindElement(By.Id("max_price")).GetAttribute("value");
            Assert.Equal("450", maxPrices);
        }
        public void ValidatePricesOfBook_AfterFilter()
        {
            // Locate the ul which contains lists of books
            var ulElement = driver.FindElement(By.ClassName("masonry-done"));
            // Locate all the lists in ul
            var lis = ulElement.FindElements(By.TagName("li"));

            foreach (var li in lis)
            {
                var priceOfBookElement = li.FindElement(By.ClassName("price"));
                // Get the sale price of the book (ins tag)
                var salePriceText = priceOfBookElement.FindElement(By.XPath("//*[@id=\"content\"]/ul/li[1]/a[1]/span[2]/ins/span")).Text;
                if (salePriceText != null)
                {
                    //var priceOfBookElement = ins.FindElement(By.ClassName("woocommerce-Price-amount")).Text;
                    var salePriceOfBook = double.Parse(salePriceText.Replace("₹", "").Trim());
                    Assert.InRange(salePriceOfBook, 150, 450);
                }
                else
                {
                    var priceOfBookText = li.FindElement(By.ClassName("woocommerce-Price-amount")).Text;
                    var priceOfBook = double.Parse(priceOfBookText.Replace("₹", "").Trim());
                    Assert.InRange(priceOfBook, 150, 450);
                }
            }
        }
        private List<IWebElement> GetProductCategoriesList()
        {
            // Get the url of product castegories
            var categoriesUlElement = driver.FindElement(By.ClassName("product-categories"));
            // Get the categories Lists
            var categoriesLi = categoriesUlElement.FindElements(By.TagName("li")).ToList();
            return categoriesLi;
        }
        public void ClickOnCategory_VerifyBooksDisplayedOnPage(int index)
        {
            // Get the category List
            var categoriesLi = GetProductCategoriesList();
            // Get the specific category which you want to click
            var category = categoriesLi[index].FindElement(By.TagName("a"));
            // Get the anchor tag link to assert the navigation
            var expectedUrl = category.GetAttribute("href");
            // Get the Count of the books from Product category section
            var countOfBooksText = categoriesLi[index].FindElement(By.ClassName("count")).Text;
            var expectedCount = int.Parse(countOfBooksText.Replace("(", "").Replace(")", "").Trim());
            testOutputHelper.WriteLine(expectedCount.ToString());
            // Click on the category
            category.Click();
            // Get the url of the current page
            var actualUrl = driver.Url;
            // Assert the navigation after click on the category
            Assert.Equal(expectedUrl, actualUrl);

            // Assert the count of books
            // Get the actual count of the books displayed on the page
            var actualCount = driver.FindElement(By.ClassName("masonry-done")).
                FindElements(By.TagName("li")).Count();
            testOutputHelper.WriteLine(actualCount.ToString());
            Assert.Equal(expectedCount, actualCount);
        }
        public void SelectOption_Sorting_DropDown(string value)
        {
            // Locate sorting drop down
            var dropDownElement = driver.FindElement(By.ClassName("orderby"));
            // Using select element to select option from dropdown
            var dropdown = new SelectElement(dropDownElement);
            dropdown.SelectByValue(value);

            // Re-locate the dropdown to avoid stale element
            var refreshedDropDownElement = driver.FindElement(By.ClassName("orderby"));
            var refreshedDropDown = new SelectElement(refreshedDropDownElement);

            // Assert the selected option
            var actualOption = refreshedDropDown.SelectedOption.GetDomAttribute("value");
            Assert.Equal(value, actualOption);
        }
        private List<IWebElement> GetListOfProducts()
        {
            // Locate the ul containing the lists of products
            var productLists = driver.FindElement(By.ClassName("masonry-done"));
            // Get all the li elements within the ul
            var productItems = productLists.FindElements(By.TagName("li")).ToList();
            return productItems;
        }
        private IWebElement GetProductPrices(IWebElement item)
        {

            // Locate the price element within each li
            var priceElement = item.FindElement(By.ClassName("woocommerce-Price-amount"));
            return priceElement;
        }
        private List<double> GetListOfPrices(List<IWebElement> productItems)
        {
            // Extract prices from each li
            List<double> prices = new List<double>();
            foreach (var item in productItems)
            {
                var priceElement = GetProductPrices(item);
                // Get Sale prices of books
                var salePriceText = priceElement.FindElement(By.XPath("//*[@id=\"content\"]/ul/li/a[1]/span[2]/ins/span")).Text;

                if (salePriceText != null)
                {
                    // Remove currency symbol and parse the price to a double
                    var salePriceOfBook = double.Parse(salePriceText.Replace("₹", "").Trim());
                    prices.Add(salePriceOfBook);
                }
                else
                {
                    // Remove currency symbol and parse the price to a double
                    var price = double.Parse(priceElement.Text.Replace("₹", "").Trim());
                    prices.Add(price);
                }
            }
            return prices;
        }
        public void Validate_LowToHigh_Sorting_Functionality()
        {
            // Get all the li elements within the ul
            var productItems = GetListOfProducts();

            // Extract prices from each li
            var prices = GetListOfPrices(productItems);

            // Verify that the prices list is sorted in ascending order
            List<double> sortedPrices = new List<double>(prices);
            sortedPrices.Sort(); // Sort the copied list in ascending order
            // Assert that the original list matches the sorted list
            Assert.Equal(sortedPrices, prices);
        }
        public void Validate_HighToLow_Sorting_Functionality()
        {
            // Get all the li elements within the ul
            var productItems = GetListOfProducts();

            // Extract prices from each li
            var prices = GetListOfPrices(productItems);

            // Verify that the prices list is sorted in ascending order
            List<double> sortedPrices = new List<double>(prices);
            // Sort the copied list in ascending order
            sortedPrices.Sort(); 
            // Reverse it to get descending order
            sortedPrices.Reverse();
            // Assert that the original list matches the sorted list
            Assert.Equal(sortedPrices, prices);
        }
        public void ValidateSalesFunctionality()
        {
            // Get the list of products
            var productItems = GetListOfProducts();

            foreach(var item in productItems)
            {
                wait.Until(d => item.FindElement(By.ClassName("onsale")).Displayed);
                var isOnSale = item.FindElement(By.ClassName("onsale"));
                if (isOnSale != null)
                {
                    var oldPriceElement = isOnSale.FindElement(By.XPath("//*[@id=\"content\"]/ul/li/a[1]/span[2]/del/span"));
                    Assert.True(oldPriceElement.Displayed, "Old price is not displayed for the sale product.");

                    var oldPriceStyle = oldPriceElement.GetCssValue("text-decoration");
                    testOutputHelper.WriteLine($"Old price text-decoration value: {oldPriceStyle}");
                    //Assert.Contains("line-through", oldPriceStyle, StringComparison.OrdinalIgnoreCase);

                    var actualPriceElement = isOnSale.FindElement(By.XPath("//*[@id=\"content\"]/ul/li/a[1]/span[2]/ins/span"));
                    Assert.True(actualPriceElement.Displayed, "Actual (discounted) price is not displayed for the sale product.");
                }
            }

        }
    }
}
