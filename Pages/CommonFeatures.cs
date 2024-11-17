

using OpenQA.Selenium;

namespace AutomationPracticeSiteProject.Pages
{
    public class CommonFeatures
    {
        private IWebDriver driver;
        public CommonFeatures(IWebDriver driver)
        {
            this.driver = driver;
        }

        IWebElement lnkShop => driver.FindElement(By.LinkText("Shop"));
        IWebElement lnkHome => driver.FindElement(By.LinkText("Home"));
        IWebElement lnkMyAccount => driver.FindElement(By.LinkText("My Account"));

        public void ClickShopMenu()
        {
            lnkShop.Click();
        }
        public void ClickHomeMenu()
        {
            lnkHome.Click();
        }
        public void ClickMyAccountMenu()
        {
            lnkMyAccount.Click();
        }
    }
}
