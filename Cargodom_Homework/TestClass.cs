using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Expect = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Cargodom_Homework
{
    [TestFixture]
    internal class TestClass : BaseClass
    {
        public const string clientUrl = "http://18.156.17.83:9095/client/home";
        public const string providerUrl = "http://18.156.17.83:9095/provider/home";

        private const string myRequestsButtonCss = "a[href='/client/my-requests']";
        private const string activeMyRequestsButtonCss = "a[ui-sref='client-my-active-requests']";
        private const string acceptedOffersSideMenuButtonCss = "a[ui-sref='client-accepted-offers']";
        private const string myAcceptedOffersButtonCss = "a[ui-sref='client-accepted-offers-list']";
        private const string moreButtonCss = "a[class='flex-table__expander-btn']";

        [TestCase("petko.gqa@gmail.com", "Gond0r!15", "sanja_transporterNov@gmail.com", "Sanja1986")]
        [Test]
        public void CreateRequestCreateOfferConfirmOffer(string usernameAsUser, string passwordAsUser, string usernameAsTransporter, string passwordAsTransporter)
        {
            string titleRequest = RandomGenerateCharacters(7);

            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;

            LogMeIn(usernameAsUser, passwordAsUser);

            wait.Until(Expect.UrlToBe(clientUrl));

            CreateRequest(titleRequest);

            //After creating the request, you must confirm that the request is present in ‘My Requests’. (Sekogas e vo prvata redica)

            wait.Until(Expect.InvisibilityOfElementLocated(By.ClassName("alert")));
            
            IWebElement myRequest = FindInMyRequestsOrAcceptedOffers(myRequestsButtonCss, activeMyRequestsButtonCss)[0].FindElement(By.TagName("a"));                     

            string actualText = myRequest.Text;

            Console.WriteLine(actualText);
            Console.WriteLine(titleRequest);

            Driver.Navigate().Refresh();

            Assert.That(actualText.Equals(titleRequest), Is.True.After(1000).MilliSeconds);
            
            LogMeOut();
                        
            wait.Until(Expect.UrlToBe(homePageUrl));
            LogMeIn(usernameAsTransporter, passwordAsTransporter);

            //find the request you created - home page

            wait.Until(Expect.VisibilityOfAllElementsLocatedBy(By.CssSelector(".table-body__row > .column1 > a")));
            List<IWebElement> listOfTitlesActiveRequestsHome = Driver.FindElements(By.CssSelector(".table-body__row > .column1 > a")).ToList();

            foreach (IWebElement element in listOfTitlesActiveRequestsHome)
            {
                if (element.Text == titleRequest) 
                {
                    wait.Until(Expect.ElementToBeClickable(element));
                    element.Click();
                    break;
                }
            }

            //Make an offer for it

            wait.Until(Expect.ElementIsVisible(By.CssSelector("button[class='details-panel__make-offer-btn']")));
            IWebElement createOffer = Driver.FindElement(By.CssSelector("button[class='details-panel__make-offer-btn']"));
            createOffer.Click();
                        
            wait.Until(Expect.ElementIsVisible(By.CssSelector("body > div.main > div.main-wrapper > div.main__content > form")));
            IWebElement cashForm = Driver.FindElement(By.CssSelector("body > div.main > div.main-wrapper > div.main__content > form"));

            wait.Until(Expect.ElementToBeClickable(By.CssSelector("div > div.make-offer__table.table > div.table-body > table > tbody > tr > td.table-body__cell.column3 > input")));
            IWebElement cacheOnPickupInput = cashForm.FindElement(By.CssSelector("div > div.make-offer__table.table > div.table-body > table > tbody > tr > td.table-body__cell.column3 > input"));

            cacheOnPickupInput.Click();
            cacheOnPickupInput.SendKeys("1");

            wait.Until(Expect.ElementIsVisible(By.Name("expirationDateInput")));
            IWebElement expirationDateInput = Driver.FindElement(By.Name("expirationDateInput"));
            expirationDateInput.Click();          
            
            DatePicker(3,1); // 4ta sedmica, 2 den

            wait.Until(Expect.ElementIsVisible(By.CssSelector("textarea[ng-model='vm.offerComment']")));
            IWebElement textArea = Driver.FindElement(By.CssSelector("textarea[ng-model='vm.offerComment']"));
            textArea.Click();

            wait.Until(Expect.ElementIsVisible(By.ClassName("make-offer__btn-create")));
            IWebElement createOfferButton = Driver.FindElement(By.ClassName("make-offer__btn-create"));
            createOfferButton.Click();

            wait.Until(Expect.ElementIsVisible(By.ClassName("modal-content")));
            IWebElement allert = Driver.FindElement(By.ClassName("modal-content"));
            Assert.True(allert.Displayed);

            wait.Until(Expect.ElementIsVisible(By.ClassName("modal-footer__btn-save")));
            IWebElement saveOfferButton = Driver.FindElement(By.ClassName("modal-footer__btn-save"));
            saveOfferButton.Click();

            //After making the offer, you must confirm that the offer was sent, which you can check via the ‘My offers’ page

            Driver.Navigate().Refresh();

            IWebElement myOffersButton = Driver.FindElement(By.CssSelector("a[ui-sref='provider-my-active-offers']"));
            myOffersButton.Click();

            wait.Until(Expect.ElementIsVisible(By.CssSelector("table > tbody")));
            IWebElement myOffersTableBody = Driver.FindElement(By.CssSelector("table > tbody"));

            List<IWebElement> myOffersfirstRowColumns = myOffersTableBody.FindElements(By.TagName("td")).ToList();

            IWebElement myOffersTitle = myOffersfirstRowColumns[0];

            Assert.AreEqual(myOffersTitle.Text, titleRequest);

            LogMeOut();

            wait.Until(Expect.UrlToBe(homePageUrl));
            LogMeIn(usernameAsUser, passwordAsUser);

            //accept the offer

            wait.Until(Expect.UrlToBe(clientUrl));
                                                            
            IWebElement titleClick = FindInMyRequestsOrAcceptedOffers(myRequestsButtonCss, activeMyRequestsButtonCss).First().FindElement(By.TagName("a"));
            titleClick.Click();
                                  
            IWebElement moreButton = Driver.FindElement(By.CssSelector(moreButtonCss));
            moreButton.Click();

            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
                        
            IWebElement ratioButton = Driver.FindElement(By.Id("offer0"));
            ratioButton.Click();
                        
            IWebElement acceptButton = Driver.FindElement(By.CssSelector("input[class='btn btn-default']"));
            acceptButton.Click();

            //Confirm that the offer was accepted and the request is closed
                        
            wait.Until(Expect.ElementIsVisible(By.CssSelector(acceptedOffersSideMenuButtonCss))); 
            IWebElement acceptedOffersSideMenuButton = Driver.FindElement(By.CssSelector(acceptedOffersSideMenuButtonCss));
            acceptedOffersSideMenuButton.Click();

            wait.Until(Expect.ElementIsVisible(By.CssSelector(myAcceptedOffersButtonCss)));
            IWebElement myAcceptedOffersButton = Driver.FindElement(By.CssSelector(myAcceptedOffersButtonCss));
            myAcceptedOffersButton.Click();

            Assert.That(actualText.Equals(titleRequest), Is.EqualTo(true));

            string expectedStatusPaternRegex = "^.*\\sПРИФАТЕН$";
            string actualStatus = FindInMyRequestsOrAcceptedOffers(acceptedOffersSideMenuButtonCss, myAcceptedOffersButtonCss)[6].Text;
                                           
            Assert.That(actualStatus, Does.Match(expectedStatusPaternRegex));
                       
            IWebElement myRequestsButton = Driver.FindElement(By.CssSelector(myRequestsButtonCss));
            myRequestsButton.Click();

            wait.Until(Expect.ElementIsVisible(By.CssSelector("div[ng-show='!vm.requests.length && vm.requests']")));
            IWebElement alertForNoActiveRequests = Driver.FindElement(By.CssSelector("div[ng-show='!vm.requests.length && vm.requests']"));

            string expectedTextInNoActiveRequests = "Извинете! Во оваа листа нема барања.";

            Assert.AreEqual(expectedTextInNoActiveRequests, alertForNoActiveRequests.Text);

            LogMeOut();

            wait.Until(Expect.UrlToBe(homePageUrl));

            Assert.AreEqual(homePageUrl, Driver.Url);
        }
    }
}