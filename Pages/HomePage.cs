using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Modules.BrowsingContext;
using OpenQA.Selenium.Support.UI;
using System.Xml.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;


namespace AutomationPracticeSiteProject.Pages
{
    public class HomePage
    {
        private IWebDriver driver;
        private ProductPage productPage;
        private CommonFeatures commonFeature;
        private readonly ITestOutputHelper testOutputHelper;
        private WebDriverWait wait;

        public HomePage(IWebDriver driver)
        {
            this.driver = driver;
            productPage = new ProductPage(driver, testOutputHelper);
            commonFeature = new CommonFeatures(driver);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }       
       
        public void NavigateToHomePage()
        {
            // Click on Shop Menu
            commonFeature.ClickShopMenu();
            // Now click on Home menu button
            commonFeature.ClickHomeMenu();
        }
        public int GetNumberOfSliders()
        {
            var homePageSliders = driver.FindElements(By.ClassName("n2-ss-slide"));

            return homePageSliders.Count();
        }
        public void VerifyNumberOfArrivalImages(int expectedNumberOfImages)
        {
            // get all the anchor tags for new arrivals
            var arrivalBookLinks = driver.FindElements(By.ClassName("woocommerce-LoopProduct-link")).ToList();
            int actualNumberOfImages = 0;
            foreach (var anchorTag in arrivalBookLinks)
            {
                var image = anchorTag.FindElement(By.TagName("img"));
                if (image != null)
                {
                    actualNumberOfImages++;
                }
            }
            //The Home page must contains only three Arrivals
            Assert.Equal(expectedNumberOfImages, actualNumberOfImages);
        }

        public void ClickNewArrivalImageAndValidateNavigation(int index)
        {
            // get all the anchor tags for new arrivals
            var arrivalBookLinks = driver.FindElements(By.ClassName("woocommerce-LoopProduct-link")).ToList();

            var anchorTagToClick = arrivalBookLinks[index];
            var anchorTagNavigateURL = anchorTagToClick.GetDomAttribute("href");

            var imageToClick = anchorTagToClick.FindElement(By.TagName("img"));

            // get the title 
            var imageTitle = imageToClick.GetDomAttribute("title");
            //Now click the image in the Arrivals      
            imageToClick.Click();

            var currentURL = driver.Url;

            //Test whether the image navigated to the correct link
            Assert.Equal(currentURL, anchorTagNavigateURL);

            var productTitle = driver.FindElement(By.ClassName("product_title")).Text;

            // Test whether it is navigating to next page where the user can add that book into his basket.
            //Image should be clickable and shoul navigate to next page where user can add that book to his basket
            Assert.Equal(imageTitle, productTitle);
            productPage.VerifyProductPageContainsAddToBasketButton();
        }
        private List<IWebElement> GetTheAnchorTagsForImageInArrivals()
        {
            // Locate the div Element
            var divElement = driver.FindElement(By.Id("text-22-sub_row_1-0-2-0-0"));
    
            // Get the all anchor tags from div
            var anchorTags = divElement.FindElements(By.TagName("a")).ToList();
            return anchorTags;
        }      
        public void ClickOnAddToBasketOfBook()
        {
            // Locate the div Element and Get the all anchor tags from div
            var anchorTags = GetTheAnchorTagsForImageInArrivals();

            // Get the achor tag which we want to click
            var lnkAddToBasket = anchorTags[1];

            //Click on Add to basket button
            lnkAddToBasket.Click();
        }
  
        public void ClickOnAddToBasketAndViewBasketOfBook()
        {           
            // Click on Add To basket button of image in arrivals
            ClickOnAddToBasketOfBook();

           var firstImageDivId = "text-22-sub_row_1-0-2-0-0";
           var lnkViewBasketClassName = "added_to_cart";
            var isAnchorTagDisplayed = wait.Until(d => d.FindElement(By.Id(firstImageDivId))
            .FindElement(By.ClassName(lnkViewBasketClassName))).Displayed;

            if (isAnchorTagDisplayed)
            {
                var lnkViewBasket = driver.FindElement(By.Id(firstImageDivId))
            .FindElement(By.ClassName(lnkViewBasketClassName));
                lnkViewBasket.Click();
            }
        }

    }

}
