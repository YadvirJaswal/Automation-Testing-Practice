﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            actions.ClickAndHold(rightHandle).MoveByOffset(moveByOffset,0).Release().Perform();

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
            Assert.Equal("450",maxPrices);
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
    }
}