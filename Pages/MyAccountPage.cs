using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace AutomationPracticeSiteProject.Pages
{
    public class MyAccountPage
    {
        private readonly IWebDriver driver;
        public MyAccountPage(IWebDriver driver)
        {
            this.driver = driver;
        }

        private List<IWebElement> GetMyAccountNavigations(int liIndex)
        {
            // Get My Account naviagtion class element
            var myAccountNavigationsElement = driver.FindElement(By.ClassName("woocommerce-MyAccount-navigation"));
            // Get all the lists of options of navigation
            var optionsLists = myAccountNavigationsElement.FindElements(By.TagName("li")).ToList();
            var optionList = optionsLists[liIndex];
            // Get anchor tags which contains the link of option
            var optionAnchorTags = optionList.FindElements(By.TagName("a")).ToList();
            return optionAnchorTags;
        }
        public void ValidateDashboardFunctionality()
        {
            var optionAnchorTags = GetMyAccountNavigations(0);

            // Get and click on dashboard option
            var lnkDashboard = optionAnchorTags[0];
            lnkDashboard.Click();

            // Get the dashboard content
            var dashboardContentElement = driver.FindElement(By.ClassName("woocommerce-MyAccount-content"));
            // Get the paragraphs in content
            var dashboardContentParagraphs = dashboardContentElement.FindElements(By.TagName("p"));
            // Get the text of first paragraph
            var actualText = dashboardContentParagraphs[0].Text;
            var expectedText = "Hello sumit2 (not sumit2? Sign out)";
            //Assert the text displayed on the dashboard
            Assert.Equal(expectedText, actualText);
        }
    }
}
