using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Expect = SeleniumExtras.WaitHelpers.ExpectedConditions;

namespace Cargodom_Homework
{
    public class BaseClass
    {        
        public IWebDriver Driver;

        public WebDriverWait wait;

        public static Random random = new Random();

        public string homePageUrl = "http://18.156.17.83:9095/";
               
        public const string loginSubmitButtonLocatorCss = "button[translate = 'login.form.button']";

        

        [SetUp]
        public void Setup() 
        {
            Driver = new ChromeDriver();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(30); //da se pojavat elementite vo DOM
            Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(25);  // da se gledaat elementite na stranata
            Driver.Manage().Window.Maximize();

            Driver.Navigate().GoToUrl(homePageUrl);

            wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(65));

        }

        [TearDown]
        public void TearDown() 
        { 
            Driver.Close();
            Driver.Dispose();

        }
                
        /// <summary>
        /// Ovaa metoda ne logira na stranata 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void LogMeIn(string username, string password)
        {
            IWebElement signInButton = Driver.FindElement(By.Id("login"));
            signInButton.Click();

            IWebElement usernameInput = Driver.FindElement(By.Id("username"));
            usernameInput.Clear();
            usernameInput.SendKeys(username);

            IWebElement passwordInput = Driver.FindElement(By.Id("password"));
            passwordInput.Clear();
            passwordInput.SendKeys(password);

            IWebElement rememberMeCheckBox = Driver.FindElement(By.Id("rememberMe"));
            rememberMeCheckBox.Click();

            IWebElement loginSubmitButton = Driver.FindElement(By.CssSelector(loginSubmitButtonLocatorCss));
            loginSubmitButton.Click();
        }

        /// <summary>
        /// Ovaa metoda ne odjavuva
        /// </summary>

        public void LogMeOut()
        {
            IWebElement logOutButton = Driver.FindElement(By.Id("logout2"));
                        
            wait.Until(Expect.ElementIsVisible(By.Id("logout2")));
            IWebElement signOutButton = Driver.FindElement(By.Id("logout2"));
            signOutButton.Click();           
        }

        /// <summary>
        /// Ovaa metoda generira random karakteri so dolzina length koja ja zema kako argument 
        /// </summary>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public static string RandomGenerateCharacters(int lenght)
        {
            const string character = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrsqtuvwxyz0123456789-_";
            return new string(Enumerable.Repeat(character, lenght).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Ovaa metoda generira random bukvi so dolzina length koja ja zema kako argument
        /// </summary>
        /// <param name="lenght"></param>
        /// <returns></returns>
        public static string RandomGenerateLetters(int lenght)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrsqtuvwxyz";
            return new string(Enumerable.Repeat(letters, lenght).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        /// <summary>
        /// Ovaa metoda odbira datum od kalendar, kako argumenti se brojot na sedmicata vo mesecot i brojot na denot vo sedmicata  
        /// </summary>
        /// <param name="calendar"></param>
        /// <param name="numberOfWeekInMonth"></param>
        /// <param name="numberOfDayInWeek"></param>
        public void DatePicker( int numberOfWeekInMonth, int numberOfDayInWeek)
        {
            wait.Until(Expect.ElementIsVisible(By.ClassName("uib-datepicker")));
            IWebElement calendar = Driver.FindElement(By.CssSelector(".uib-daypicker"));

            wait.Until(Expect.ElementIsVisible(By.CssSelector("table > tbody")));
            IWebElement calendarBody = calendar.FindElement(By.CssSelector("table > tbody"));

            wait.Until(Expect.VisibilityOfAllElementsLocatedBy(By.CssSelector("tr")));
            List<IWebElement> weeks = calendarBody.FindElements(By.TagName("tr")).ToList();

            IWebElement secondWeek = weeks[numberOfWeekInMonth];

            List<IWebElement> daysInWeek = secondWeek.FindElements(By.TagName("td")).ToList();

            IWebElement day = daysInWeek[numberOfDayInWeek].FindElement(By.CssSelector("button[ng-click='select(dt.date)']"));
            day.Click();
        }

        /// <summary>
        /// Ovoj metod odbira random payment metod vo formata za kreiranje na baranje
        /// </summary>
        public void RandomPaymentMethod()
        {
            
            wait.Until(Expect.ElementExists(By.CssSelector("#newRequestForm > div > div:nth-child(11) > .row > div")));
            IWebElement paymentForm = Driver.FindElement(By.CssSelector("#newRequestForm > div > div:nth-child(11) > .row > div"));

            wait.Until(Expect.PresenceOfAllElementsLocatedBy(By.CssSelector("div[class^='row']:not([class$='ng-hide'])")));
            List<IWebElement> paymentMethods = paymentForm.FindElements(By.CssSelector("div[class^='row']:not([class$='ng-hide'])")).ToList();
            Console.WriteLine(paymentMethods.Count);

            int indexPaymentMethod = random.Next(0, paymentMethods.Count-1);

            wait.Until(Expect.ElementIsVisible(By.CssSelector("div > .checkbox-input ")));
            IWebElement chosenPaymentMethod = paymentMethods[indexPaymentMethod].FindElement(By.CssSelector("div > .checkbox-input "));
            chosenPaymentMethod.Click();
        }

        /// <summary>
        /// Ovaa metoda kreira baranje so title koj sto go zema kako argument
        /// </summary>
        /// <param name="titleRequest"></param>
        public void CreateRequest(string titleRequest)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;

            IWebElement createRequestButton = Driver.FindElement(By.CssSelector("a[ui-sref='client-create-request']"));
            createRequestButton.Click();

            IWebElement titleRequestField = Driver.FindElement(By.CssSelector("input[ng-model='vm.request.title']"));
            titleRequestField.Click();
            titleRequestField.SendKeys(titleRequest);

            IWebElement transportField = Driver.FindElement(By.CssSelector("select[name='categoryType']"));
            transportField.Click();

            IWebElement motorbikeOption = Driver.FindElement(By.CssSelector("option[value='MOTORBIKE']"));
            motorbikeOption.Click();

            IWebElement pickUpAddress = Driver.FindElement(By.Name("pickUpAddress"));
            IWebElement adress = pickUpAddress.FindElement(By.CssSelector("input[ng-value='vm.address.formattedAddress']"));
            adress.SendKeys(RandomGenerateLetters(1));
            List<IWebElement> autoPickUpAddress = Driver.FindElements(By.CssSelector("span[class='pac-matched']")).ToList();
            autoPickUpAddress[0].Click();

            IWebElement deliveryAddress = Driver.FindElement(By.Name("deliveryAddress"));
            IWebElement adressD = deliveryAddress.FindElement(By.CssSelector("input[ng-value='vm.address.formattedAddress']"));
            adressD.SendKeys(RandomGenerateLetters(1));
            List<IWebElement> autoDeliveryAddress = Driver.FindElements(By.CssSelector("span[class='pac-matched']")).ToList();
            autoDeliveryAddress[0].Click();

            IWebElement pickUpDateCheckBox = Driver.FindElement(By.Id("setPickUpDate"));
            pickUpDateCheckBox.Click();

            wait.Until(Expect.ElementIsVisible(By.CssSelector("input[ng-click='vm.openEarliestPickUpDatePicker()']")));
            IWebElement pickUpEarliestDateInput = Driver.FindElement(By.CssSelector("input[ng-click='vm.openEarliestPickUpDatePicker()']"));
            pickUpEarliestDateInput.Click();
                        
            DatePicker(2, 4);

            wait.Until(Expect.ElementIsVisible(By.CssSelector("input[ng-click='vm.openLatestPickUpDatePicker()']")));
            IWebElement pickUpLatestDateInput = Driver.FindElement(By.CssSelector("input[ng-click='vm.openLatestPickUpDatePicker()']"));
            pickUpLatestDateInput.Click();
                        
            DatePicker(2, 5);

            IWebElement deliveryDateCheckBox = Driver.FindElement(By.Id("setDeliveryUpDate"));
            deliveryDateCheckBox.Click();

            IWebElement deliveryEarliestDateInput = Driver.FindElement(By.CssSelector("input[ng-click='vm.openEarliestDeliveryDatePicker()']"));
            deliveryEarliestDateInput.Click();
                        
            DatePicker(3, 4);

            wait.Until(Expect.ElementIsVisible(By.CssSelector("input[ng-click='vm.openLatestDeliveryDatePicker()']")));
            IWebElement deliveryLatestDateInput = Driver.FindElement(By.CssSelector("input[ng-click='vm.openLatestDeliveryDatePicker()']"));
            deliveryLatestDateInput.Click();
                        
            DatePicker(3, 4);
                        
            IWebElement removeItemButton = Driver.FindElement(By.CssSelector("a[title='Remove Item']"));
            removeItemButton.Click();
                        
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");

            RandomPaymentMethod();
                        
            wait.Until(Expect.ElementIsVisible(By.CssSelector("#newRequestForm > div > div:nth-child(14) > input")));
            IWebElement submitButton = Driver.FindElement(By.CssSelector("#newRequestForm > div > div:nth-child(14) > input"));
            submitButton.Click();                                              
        }              
        
        /// <summary>
        /// So ovaa metoda prebaruvame niz tabelata vo MyRequest ili vo AcceptedOffers
        /// </summary>
        /// <param name="sideMenuButtonCss"></param>
        /// <param name="myActiveRequestsOrAcceptedOffersButtonCss"></param>
        /// <returns></returns>
        public List<IWebElement> FindInMyRequestsOrAcceptedOffers(string sideMenuButtonCss, string myActiveRequestsOrAcceptedOffersButtonCss)
        {            
            Driver.Navigate().Refresh();

            wait.Until(Expect.ElementIsVisible(By.CssSelector(sideMenuButtonCss)));
            IWebElement myRequestsButton = Driver.FindElement(By.CssSelector(sideMenuButtonCss));
            myRequestsButton.Click();

            wait.Until(Expect.ElementIsVisible(By.CssSelector(myActiveRequestsOrAcceptedOffersButtonCss)));
            IWebElement activeMyRequestsButton = Driver.FindElement(By.CssSelector(myActiveRequestsOrAcceptedOffersButtonCss));
            activeMyRequestsButton.Click();
            Driver.Navigate().Refresh();

            wait.Until(Expect.ElementIsVisible(By.ClassName("table-body")));
            IWebElement tableBodyActiveRequests = Driver.FindElement(By.ClassName("table-body"));

            wait.Until(Expect.VisibilityOfAllElementsLocatedBy(By.TagName("tr")));
            List<IWebElement> listOfRowsActiveRequests = tableBodyActiveRequests.FindElements(By.TagName("tr")).ToList();                       

            IWebElement firstRowActiveRequests = listOfRowsActiveRequests[0];

            wait.Until(Expect.VisibilityOfAllElementsLocatedBy(By.CssSelector("td")));
            List<IWebElement> listOfColumnsMyActiveRequest = firstRowActiveRequests.FindElements(By.TagName("td")).ToList();
                        
            return listOfColumnsMyActiveRequest;
        }
    }
}
