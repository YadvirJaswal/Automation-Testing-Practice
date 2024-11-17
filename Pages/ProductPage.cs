using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Xunit.Abstractions;
using Xunit.Sdk;


namespace AutomationPracticeSiteProject.Pages
{
    public class ProductPage
    {
        private readonly IWebDriver driver;
        private BasketPage basketPage;
        private CommonFeatures commonFeature;
        private readonly ITestOutputHelper testOutputHelper;
        public ProductPage(IWebDriver driver, ITestOutputHelper testOutputHelper)
        {
            this.driver = driver;
            this.testOutputHelper = testOutputHelper;
            basketPage = new BasketPage(driver, testOutputHelper);
            commonFeature = new CommonFeatures(driver);
        }

     
        public void VerifyProductPageContainsAddToBasketButton()
        {
            var addToBasketButton = driver.FindElement(By.ClassName("single_add_to_cart_button"));
            var hasAddToBasketButton = addToBasketButton is not null && addToBasketButton.Text.ToLower() == "add to basket";
            Assert.True(hasAddToBasketButton, "Did not navigate to book page");
        }
        private IEnumerable<IWebElement> GetParagraphsFromDescriptionDiv()
        {
            var descriptionTab = driver.FindElement(By.Id("tab-description"));
            var paragraphInDescriptionTabs = descriptionTab.FindElements(By.TagName("p"));
            foreach (var paragraphInDescriptionTab in paragraphInDescriptionTabs)
            {
                yield return paragraphInDescriptionTab;
            }
        }
        public void VerifyProductDescription()
        {
            //Click on Description tab for the book you clicked on.
            var paragraphs = GetParagraphsFromDescriptionDiv();
            foreach (var paragraphInDescriptionTab in paragraphs)
            {
                string paragraphText = paragraphInDescriptionTab.Text;
                // There should be a description regarding that book the user clicked on
                Assert.False(string.IsNullOrEmpty(paragraphText), $"Current URL - {driver.Url}, The <p> tag should contain text but it is empty.");
            }
        }
        public string SubmitReviewsUnderBookReviewTab(string yourReview, string nameOfReviewer, string emailOfReviewer, string starRating)
        {
            var currentURL = driver.Url;
            //Click on the review tab under the book
            var reviewTab = driver.FindElement(By.ClassName("reviews_tab"));
            reviewTab.Click();


            // Give rating to the book under review section
            var rating = driver.FindElement(By.ClassName($"star-{starRating}"));
            rating.Click();

            //Enter review into "Your Review" textfield
            var reviewContent = driver.FindElement(By.Id("comment"));
            reviewContent.SendKeys(yourReview);

            // Enter the name of the reviewer
            var reviewerName = driver.FindElement(By.Id("author"));
            reviewerName.SendKeys(nameOfReviewer);

            // Enter the name of the reviewer
            var reviewerEmail = driver.FindElement(By.Id("email"));
            reviewerEmail.SendKeys(emailOfReviewer);

            // Submit the review
            var submitButton = driver.FindElement(By.Id("submit"));
            submitButton.Click();
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            return currentURL;
        }
        public void VerifyAddedReview(string currentUrl, string addedComment, string addedRating)
        {
            var orderedListOfComments = driver.FindElement(By.CssSelector(".commentlist"));
            var listOfComments = orderedListOfComments.FindElements(By.TagName("li"));
            var hasCommentMatched = false;
            var hasRatingMatched = false;
            var commentText = "";
            var ratingValue = "";
            foreach (var comments in listOfComments)
            {
                var description = comments.FindElement(By.ClassName("description"));
                commentText = description.FindElement(By.TagName("p")).Text;
                if (commentText == addedComment)
                {
                    hasCommentMatched = true;
                    var rating = comments.FindElement(By.ClassName("star-rating"));
                    //var ratingSpan = rating.FindElement(By.TagName("span"));
                    ratingValue = rating.FindElement(By.TagName("strong")).GetAttribute("innerHTML");
                    if (addedRating == ratingValue)
                    {
                        hasRatingMatched = true;                       
                        break;
                    }
                }
            }

            Assert.True(hasCommentMatched, $"Current Url - {currentUrl}, Expected Comment - {addedComment} but found {commentText}");
            Assert.True(hasRatingMatched, $"Current Url - {currentUrl}, Comment- {addedComment}, Added rating -{addedRating} but found {ratingValue}");
            driver.Navigate().GoToUrlAsync(currentUrl);

            commonFeature.ClickHomeMenu();
        }
        public void ClickAddToBasketButton()
        {
            var lnkAddToBasketButtonClass = driver.FindElement(By.ClassName("cart"));
            var lnkAddToBasketButton = lnkAddToBasketButtonClass.FindElement(By.TagName("button"));
            lnkAddToBasketButton.Submit();
        }
        public void SuccessMessageContainViewBasketButton()
        {
            var successMessage = driver.FindElement(By.ClassName("woocommerce-message"));
            var viewBasketButton = successMessage.FindElement(By.TagName("a"));

            Assert.True(viewBasketButton.Displayed, $"Current url - {driver.Url}, View basket is not displayed");
            Assert.Equal("https://practice.automationtesting.in/basket/", viewBasketButton.GetAttribute("href"));
        }
        public void ClickOnViewBasketButtonInSuccessMessage()
        {
            // Verify that the correct item is added to the basket
            Dictionary<string, string> expectedProduct = new Dictionary<string, string>();
            var expectedProductName = driver.FindElement(By.ClassName("product_title")).Text;
            var expectedProductUrl = driver.Url;
            var expectedProductPrice = driver.FindElement(By.ClassName("woocommerce-Price-amount")).Text;
            if (expectedProduct.ContainsKey(expectedProductName))
            {
                expectedProduct[expectedProductName] = expectedProductUrl;
            }
            else
            {
                expectedProduct.Add(expectedProductName, expectedProductUrl);
            }

            // Click on the view basket link
            var viewBasketLink = driver.FindElement(By.ClassName("button"));
            viewBasketLink.Click();

            // verify product details
            basketPage.VerifyProductDetailsInTheBasket(expectedProductName, expectedProduct[expectedProductName], expectedProductPrice);
        }
        public void ClickOnViewBasketButton()
        {
            // Click on the view basket link
            var viewBasketLink = driver.FindElement(By.ClassName("button"));
            viewBasketLink.Click();
        }

    }
}
