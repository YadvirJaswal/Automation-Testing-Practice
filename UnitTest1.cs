using AutomationPracticeSiteProject.Models;
using AutomationPracticeSiteProject.Pages;
using OpenQA.Selenium;
using OpenQA.Selenium.BiDi.Communication;
using OpenQA.Selenium.BiDi.Modules.BrowsingContext;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics.Metrics;
using Xunit.Abstractions;


namespace AutomationPracticeSiteProject
{
    public class UnitTest1
    {
        private IWebDriver driver;
        private HomePage homePage;
        private WebDriverWait wait;
        private ProductPage productPage;
        private BasketPage basketPage;
        private CheckOutPage checkOutPage;
        private CommonFeatures commonFeatures;
        private RegistrationPage registrationPage;
        private ShopPage shopPage;
        private LoginPage loginPage;
        private readonly ITestOutputHelper _testOutputHelper;
        public UnitTest1(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://practice.automationtesting.in/");
            homePage = new HomePage(driver);
            productPage = new ProductPage(driver, testOutputHelper);
            basketPage = new BasketPage(driver, _testOutputHelper);
            commonFeatures = new CommonFeatures(driver);
            checkOutPage = new CheckOutPage(driver);
            registrationPage = new RegistrationPage(driver);
            loginPage = new LoginPage(driver);
            shopPage = new ShopPage(driver,_testOutputHelper);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Fact]
        public void HomePage_Slider_CountValidation()
        {
            //Arrange
            homePage.NavigateToHomePage();

            int expectedNumberOfSliders = 3;
            //Act
            int actualNumberOfSliders = homePage.GetNumberOfSliders();
            //Assert
            Assert.Equal(expectedNumberOfSliders, actualNumberOfSliders);
        }
        [Fact]
        public void HomePage_NumberOfNewArrivalImages_ShouldBeEqualToThree()
        {
            //arrange
            homePage.NavigateToHomePage();
            int expectedNumberOfImages = 3;

            //Act and Assert
            homePage.VerifyNumberOfArrivalImages(expectedNumberOfImages);
        }

        [Fact]
        public void HomePage_ClickOnNewArrivalImages_ShouldNavigateToProductPage()
        {
            //Navigate to HomePage
            homePage.NavigateToHomePage();
            //Test whether the Home page has Three Arrivals only
            int expectedNumberOfImages = 3;
            homePage.VerifyNumberOfArrivalImages(expectedNumberOfImages);

            for (int i = 0; i < expectedNumberOfImages; i++)
            {
                //test Click on each new arrival and validate if the navigation is correct
                homePage.ClickNewArrivalImageAndValidateNavigation(i);
                //go back to the home page
                driver.Navigate().Back();
            }
        }

        [Fact]
        public void HomePage_ClickOnNewArrivalImages_ShouldNavigateToProductPageAndShouldContainProductDescription()
        {
            homePage.NavigateToHomePage();
            //Test whether the Home page has Three Arrivals only
            int expectedNumberOfImages = 3;
            homePage.VerifyNumberOfArrivalImages(expectedNumberOfImages);

            for (int i = 0; i < expectedNumberOfImages; i++)
            {
                //test Click on each new arrival and validate if the navigation is correct
                homePage.ClickNewArrivalImageAndValidateNavigation(i);

                productPage.VerifyProductDescription();

                //go back to the home page 
                driver.Navigate().Back();
            }
        }

        [Fact]
        public void HomePage_ClickOnNewArrivalImages_ProductPage_ClickOnReviewTab_AddReviewsAndVerifyAddedReview()
        {
            homePage.NavigateToHomePage();
            //Test whether the Home page has Three Arrivals only
            int expectedNumberOfImages = 3;
            homePage.VerifyNumberOfArrivalImages(expectedNumberOfImages);

            for (int i = 0; i < expectedNumberOfImages; i++)
            {
                homePage.ClickNewArrivalImageAndValidateNavigation(i);

                var guid = Guid.NewGuid();
                //Enter the reviews under the books
                var comment = $"I am happy with this Book:{guid}";
                var name = $"Joe:{guid}";
                var email = $"Joe{guid}@gmail.com";
                var rating = "4";
                var currentUrl = productPage.SubmitReviewsUnderBookReviewTab(comment, name, email, rating);
                productPage.VerifyAddedReview(currentUrl, comment, rating);
            }
        }

        [Fact]
        public void HomePage_ClickOnArrivalImage_ClickOnAddToBasketButton_ShouldAddProductToTheBasket()
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            //Test whether the Home page has Three Arrivals only
            int expectedNumber = 3;
            homePage.VerifyNumberOfArrivalImages(expectedNumber);

            // Click the image in the arrivals, Image should be clickable, test whether it is naviagate to next page
            homePage.ClickNewArrivalImageAndValidateNavigation(0);

            // Click on Add to basket button 
            productPage.ClickAddToBasketButton();

            // User can view that book in the success message
            productPage.SuccessMessageContainViewBasketButton();

            // Click on View basket button and assert the Name
            productPage.ClickOnViewBasketButtonInSuccessMessage();
        }
        [Fact]
        public void HomePage_ClickOnArrivalImage_ClickOnAddToBasket_EnterCouponCode_ShouldBeAbleToGetDiscountOnTotalPriceForPricesGreaterThan450()
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            //Test whether the Home page has Three Arrivals only
            int expectedNumber = 3;
            homePage.VerifyNumberOfArrivalImages(expectedNumber);

            // Click the image in the arrivals, Image should be clickable, test whether it is naviagate to next page
            homePage.ClickNewArrivalImageAndValidateNavigation(0);

            // Click on Add to basket button 
            productPage.ClickAddToBasketButton();

            // User can view that book in the success message
            productPage.SuccessMessageContainViewBasketButton();

            // Click on View basket button and assert the Name
            productPage.ClickOnViewBasketButtonInSuccessMessage();

            // Enter the Coupon code as ‘krishnasakinala’ to get 50rps off on the total.
            basketPage.VerifyPriceAfterApplyValidCouponForPricesGreaterThan450();
        }
        [Fact]
        public void HomePage_ClickOnArrivalImage_ClickOnAddToBasket_EnterCouponCode_ShouldNotBeAbleToGetDiscountOnTotalPriceForPricesLesserThan450()
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            //Test whether the Home page has Three Arrivals only
            int expectedNumber = 3;
            homePage.VerifyNumberOfArrivalImages(expectedNumber);

            // Click the image in the arrivals, Image should be clickable, test whether it is naviagate to next page
            homePage.ClickNewArrivalImageAndValidateNavigation(1);

            // Click on Add to basket button 
            productPage.ClickAddToBasketButton();

            // User can view that book in the success message
            productPage.SuccessMessageContainViewBasketButton();

            // Click on View basket button and assert the Name
            var viewBasketLink = driver.FindElement(By.ClassName("button"));
            viewBasketLink.Click();

            // should not applicable for books whose price less than 450 rps
            basketPage.VerifyPriceAfterApplyValidCouponForPricesLesserThan450();
        }

        [Fact]
        public void HomePage_ClickOnArrivalImage_ClickOnAddToBasket_EnterCouponCode_ShouldNotBeAbleToGetDiscountOnApplyingInvalidCouponCode()
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            //Test whether the Home page has Three Arrivals only
            int expectedNumber = 3;
            homePage.VerifyNumberOfArrivalImages(expectedNumber);

            // Click the image in the arrivals, Image should be clickable, test whether it is naviagate to next page
            homePage.ClickNewArrivalImageAndValidateNavigation(0);

            // Click on Add to basket button 
            productPage.ClickAddToBasketButton();

            // User can view that book in the success message
            productPage.SuccessMessageContainViewBasketButton();

            // Click on View basket button and assert the Name
            productPage.ClickOnViewBasketButtonInSuccessMessage();

            // Enter the invalid coupon code and verify the prices.
            basketPage.VerifyPricesAfterApplyingInValidCoupon();
        }
        [Fact]
        public void HomePage_ClickOnArrivalImage_ClickOnAddToBasket_ClickOnRemoveThisIcon_ShouldBeAbleToRemoveBookFromGrid()
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            //Test whether the Home page has Three Arrivals only
            int expectedNumber = 3;
            homePage.VerifyNumberOfArrivalImages(expectedNumber);

            // Click the first image in the arrivals and click on add to basket button
            homePage.ClickNewArrivalImageAndValidateNavigation(0);
            productPage.ClickAddToBasketButton();
            commonFeatures.ClickHomeMenu();

            // Click the third image in the arrivals and click on add to basket button
            homePage.ClickNewArrivalImageAndValidateNavigation(2);
            productPage.ClickAddToBasketButton();

            // User can view that book in the success message
            productPage.SuccessMessageContainViewBasketButton();

            // Click on View basket button 
            productPage.ClickOnViewBasketButton();

            //Click on remove icon of the book which we want to delete or remove from the grid
            basketPage.RemoveBookAndVerifyRemovedBookDetails();
        }
        [Fact]
        public void HomePage_ClickOnArrivalImage_ClickOnAddToBasket_ClickOnRemoveThisIcon_VerifyUndoOption()
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            //Test whether the Home page has Three Arrivals only
            int expectedNumber = 3;
            homePage.VerifyNumberOfArrivalImages(expectedNumber);

            // Click the first image in the arrivals and click on add to basket button
            homePage.ClickNewArrivalImageAndValidateNavigation(0);
            productPage.ClickAddToBasketButton();
            commonFeatures.ClickHomeMenu();

            // Click the third image in the arrivals and click on add to basket button
            homePage.ClickNewArrivalImageAndValidateNavigation(2);
            productPage.ClickAddToBasketButton();

            // User can view that book in the success message
            productPage.SuccessMessageContainViewBasketButton();

            // Click on View basket button 
            productPage.ClickOnViewBasketButton();

            //Click on remove icon of the book which we want to delete or remove from the grid
            basketPage.VerifyUndoOptionInSuccessMessage();


        }

        [Fact]
        public void HomePage_ClickOnFirstImageInArrivals_ClickOnAddToBasket_ClickOnViewBasket_VerifyTheUpdateBasketFunctionality()
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            //Test whether the Home page has Three Arrivals only
            int expectedNumber = 3;
            homePage.VerifyNumberOfArrivalImages(expectedNumber);

            // Click the first image in the arrivals and click on add to basket button
            //homePage.ClickOnAddToBasketAndViewBasketOfFirstImageInArrivals();

            // Click the first image in the arrivals and click on add to basket button
            homePage.ClickNewArrivalImageAndValidateNavigation(0);
            productPage.ClickAddToBasketButton();
            commonFeatures.ClickHomeMenu();

            // Click the third image in the arrivals and click on add to basket button
            homePage.ClickNewArrivalImageAndValidateNavigation(2);
            productPage.ClickAddToBasketButton();

            // User can view that book in the success message
            productPage.SuccessMessageContainViewBasketButton();

            // Click on View basket button 
            productPage.ClickOnViewBasketButton();

            //Click on remove icon of the book which we want to delete or remove from the grid
            basketPage.VerifyUndoOptionInSuccessMessage();
            basketPage.VerifyUpdateBasketButton();
        }

        [Fact]
        public void HomePage_ClickOnFirstImageInArrivals_ClickOnAddToBasket_ClickOnViewBasket_TotalShouldBeGreaterThanSubtotal()
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            //Test whether the Home page has Three Arrivals only
            int expectedNumber = 3;
            homePage.VerifyNumberOfArrivalImages(expectedNumber);

            // Click the first image in the arrivals and click on add to basket button
            homePage.ClickOnAddToBasketAndViewBasketOfFirstImageInArrivals();

            // Assert that total is greater than subtotal
            basketPage.VerifyTotalAlwaysGreaterThanSubTotal();
        }

        [Fact]
        public void HomePage_ClickOnFirstImageInArrivals_ClickOnAddToBasket_ClickOnViewBasket_ClickOnProceedToCheckOutButton_MustLeadToPaymentGateWay()
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            // Click the first image in the arrivals and click on add to basket button
            homePage.ClickOnAddToBasketAndViewBasketOfFirstImageInArrivals();

            // Click on Checkout button
            basketPage.GetAndClickOnProceedToCheckoutButton();

            // verify the Url of Checkout page
            checkOutPage.VerifyUrlOfCheckOutPage();

            //Verify That CheckoutPage Contains PaymentGateway
            checkOutPage.VerifyThatCheckoutPageContainsPaymentGateway();
        }
        [Fact]
        public void HomePage_ClickOnFirstImageInArrivals_ClickOnAddToBasket_ClickOnViewBasket_ClickOnProceedToCheckOutButton_UserCanApplyCoupon()
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            // Click the first image in the arrivals and click on add to basket button
            homePage.ClickOnAddToBasketAndViewBasketOfFirstImageInArrivals();

            // Click on Checkout button
            basketPage.GetAndClickOnProceedToCheckoutButton();

            // verify the Url of Checkout page
            checkOutPage.VerifyUrlOfCheckOutPage();

            // Enter and apply the coupon code in Checkout page
            checkOutPage.EnterAndApplyCouponInCheckoutPage();
        }

        [Theory]
        [MemberData(nameof(CheckoutTestData.BillingDetailsDataMandatoryFields), MemberType = typeof(CheckoutTestData))]
        public void CheckoutPage_PlaceAnOrderByFillingMandatoryFields(BillingDetails billingDetails)
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            // Click the first image in the arrivals and click on add to basket button
            homePage.ClickOnAddToBasketAndViewBasketOfFirstImageInArrivals();

            // Click on Checkout button
            basketPage.GetAndClickOnProceedToCheckoutButton();

            // Place an order by filling all mandatory fields
            checkOutPage.PlaceAnOrderByFillingOnlyMandatoryFields(billingDetails);

        }
        [Theory]
        [MemberData(nameof(CheckoutTestData.BillingDetailsDataNonMandatoryFields), MemberType = typeof(CheckoutTestData))]
        public void CheckoutPage_PlaceAnOrderByNotFillingMandatoryFields(BillingDetails billingDetails)
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            // Click the first image in the arrivals and click on add to basket button
            homePage.ClickOnAddToBasketAndViewBasketOfFirstImageInArrivals();

            // Click on Checkout button
            basketPage.GetAndClickOnProceedToCheckoutButton();

            // Place an order by not filling mandatory fields
            checkOutPage.PlaceAnOrderByNotFillingOnlyMandatoryFields(billingDetails);
        }
        [Theory]
        [MemberData(nameof(CheckoutTestData.BillingDetailsDataWithInvalidEmail), MemberType = typeof(CheckoutTestData))]
        public void CheckoutPage_PlaceAnOrder_ByFilling_InvalidEmail(BillingDetails billingDetails)
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            // Click the first image in the arrivals and click on add to basket button
            homePage.ClickOnAddToBasketAndViewBasketOfFirstImageInArrivals();

            // Click on Checkout button
            basketPage.GetAndClickOnProceedToCheckoutButton();

            //Place an order by not filling Invalid Email
            checkOutPage.PlaceAnOrderByFillingInvalidEmail(billingDetails);
        }
        [Theory]
        [MemberData(nameof(CheckoutTestData.BillingDetailsDataWithInvalidPhone), MemberType = typeof(CheckoutTestData))]
        public void CheckoutPage_PlaceAnOrder_ByFilling_InvalidPhone(BillingDetails billingDetails)
        {
            // Navigate to homepage
            homePage.NavigateToHomePage();

            // Click the first image in the arrivals and click on add to basket button
            homePage.ClickOnAddToBasketAndViewBasketOfFirstImageInArrivals();

            // Click on Checkout button
            basketPage.GetAndClickOnProceedToCheckoutButton();

            //Place an order by not filling Invalid Email
            checkOutPage.PlaceAnOrderByFillingInvalidPhoneNumber(billingDetails);
        }

        [Theory]
        [MemberData(nameof(RegistrationModel.validRegisterDetails1), MemberType = typeof(RegistrationModel))]
        public void ClickOnMyAccount_RegisterAnAccountWithValidCredentials_ShouldBeAbleToRegistered(RegistrationModel registrationModel)
        {
            // Click on My Account option in the menu
            commonFeatures.ClickMyAccountMenu();

            // Enter And Submit registeration form
            registrationPage.EnterEmailIdAndPassword_ClickOnRegisterButton(registrationModel);

            // Assert Navigation
            registrationPage.ValidateNavigationToMyAccountPage();
        }
        [Theory]
        [MemberData(nameof(RegistrationModel.InValidEmailId_ValidPassword), MemberType = typeof(RegistrationModel))]
        public void ClickOnMyAccount_RegisterAnAccountWith_InvalidEmail_ValidPassword_ShouldNotBeAbleToRegistered(RegistrationModel registrationModel)
        {
            // Click on My Account option in the menu
            commonFeatures.ClickMyAccountMenu();

            // Enter And Submit registeration form
            registrationPage.EnterEmailIdAndPassword_ClickOnRegisterButton(registrationModel);

            // Assert the Error Message for email

        }
        [Theory]
        [MemberData(nameof(RegistrationModel.EmptyPassword), MemberType = typeof(RegistrationModel))]
        public void ClickOnMyAccount_RegisterAnAccountWith_ValidEmail_EmptyPassword_ShouldNotBeAbleToRegistered(RegistrationModel registrationModel)
        {
            // Click on My Account option in the menu
            commonFeatures.ClickMyAccountMenu();

            // Enter And Submit registeration form
            registrationPage.EnterEmailIdAndPassword_ClickOnRegisterButton(registrationModel);

            // Assert the Error Message for email
            registrationPage.ValidatePasswordErrorMessage();

        }
        [Theory]
        [MemberData(nameof(RegistrationModel.EmptyEmailId), MemberType = typeof(RegistrationModel))]
        public void ClickOnMyAccount_RegisterAnAccountWith_EmptyEmail_ValidPassword_ShouldNotBeAbleToRegistered(RegistrationModel registrationModel)
        {
            // Click on My Account option in the menu
            commonFeatures.ClickMyAccountMenu();

            // Enter And Submit registeration form
            registrationPage.EnterEmailIdAndPassword_ClickOnRegisterButton(registrationModel);

            // Assert the Error Message for email
            registrationPage.ValidateEmailErrorMessage();
        }
        [Theory]
        [InlineData("sumit@gmail.com", "Sumit@001")]
        public void ClickOnMyAccount_LoginWithValidCredentials_UserMustSuccessfullyLogin(string userName, string password)
        {
            // Click on My Account option in the menu
            commonFeatures.ClickMyAccountMenu();

            // Enter And Submit Login form
            loginPage.Enter_UserName_Password_ClickOnLogin(userName, password);

            // Assert Navigation
            registrationPage.ValidateNavigationToMyAccountPage();

        }
        [Theory]
        [InlineData("sumitjass@gmail.com", "Sumit@00111")]
        public void ClickOnMyAccount_LoginWithInvalidCredentials_ProperErrorMustBeDisplayed(string userName, string password)
        {
            // Click on My Account option in the menu
            commonFeatures.ClickMyAccountMenu();

            // Enter And Submit Login form
            loginPage.Enter_UserName_Password_ClickOnLogin(userName, password);

            // Assert Error Message
            loginPage.ValiadateErrorMessage_InvalidCredentials("Error: A user could not be found with this  email address.");
        }
        [Theory]
        [InlineData("sumit@gmail.com", "")]
        public void ClickOnMyAccount_LoginWithValidUsername_EmptyPassword_ProperErrorMustBeDisplayed(string userName, string password)
        {
            // Click on My Account option in the menu
            commonFeatures.ClickMyAccountMenu();

            // Enter And Submit Login form
            loginPage.Enter_UserName_Password_ClickOnLogin(userName, password);

            // Assert Error Message
            loginPage.ValiadateErrorMessage_InvalidCredentials("Error: Password is required.");
        }
        [Theory]
        [InlineData("", "Sumit@00111")]
        public void ClickOnMyAccount_LoginWithEmptyUsername_ValidPassword_ProperErrorMustBeDisplayed(string userName, string password)
        {
            // Click on My Account option in the menu
            commonFeatures.ClickMyAccountMenu();

            // Enter And Submit Login form
            loginPage.Enter_UserName_Password_ClickOnLogin(userName, password);

            //Assert Error Message
            loginPage.ValiadateErrorMessage_InvalidCredentials("Error: Username is required.");
        }
        [Theory]
        [InlineData("", "")]
        public void ClickOnMyAccount_LoginWithEmptyUsername_EmptyPassword_ProperErrorMustBeDisplayed(string userName, string password)
        {
            // Click on My Account option in the menu
            commonFeatures.ClickMyAccountMenu();

            // Enter And Submit Login form
            loginPage.Enter_UserName_Password_ClickOnLogin(userName, password);

            //Assert Error Message
            loginPage.ValiadateErrorMessage_InvalidCredentials("Error: Username is required.");
        }
        [Fact]
        public void ClickOnMyAccount_LoginPage_PasswordField_ShouldHideInput()
        {
            // Click on My Account option in the menu
            commonFeatures.ClickMyAccountMenu();
            // Check the type of password field
            loginPage.Validate_PasswordField_HideInput();
        }
        [Theory]
        [InlineData("SuMit@gmail.com", "suMit@001")]
        public void ClickOnMyAccount_Login_CaseChangedUserName_Password_LoginMustFail(string userName,string password)
        {
            // Click on My Account option in the menu
            commonFeatures.ClickMyAccountMenu();

            // Enter And Submit Login form
            loginPage.Enter_UserName_Password_ClickOnLogin(userName, password);

            //Assert Error Message
            loginPage.ValiadateErrorMessage_InvalidCredentials("Error: The password you entered for the username SuMit@gmail.com is incorrect. Lost your password?");
        }
        [Fact]
        public void Shop_Validate_FliterByPrice_Functionality()
        {
            // Click on shop
            commonFeatures.ClickShopMenu();

            // Adjust the price slider between 150-450
            shopPage.MoveSliderLeft_VerifyPostion();

            // Click on Filter Button and assert prices after applying filter
            shopPage.ClickOnFilterButton_AssertPrices();
            
            // Assert the price of each book on the page
            shopPage.ValidatePricesOfBook_AfterFilter();
        }

    }
}
