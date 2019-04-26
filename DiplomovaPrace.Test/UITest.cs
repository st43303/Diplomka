using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace DiplomovaPrace.Test
{
    [TestClass]
    public class UITest
    {
        private IWebDriver driver = new ChromeDriver(@"D:\Users\Miloslav Moravec\source\repos\Diplomka\Diplomka\packages\Selenium.WebDriver.ChromeDriver.73.0.3683.68\driver\win32");

        [TestMethod]
        public void TestLogin()
        {
            driver.Navigate().GoToUrl("http://www.milamoravec.cz");
            driver.FindElement(By.Id("Username")).SendKeys("Skipper");
            driver.FindElement(By.Id("Password")).SendKeys("špatné heslo");
            driver.FindElement(By.TagName("button")).Click();
            string message = driver.FindElement(By.TagName("strong")).Text;
            string expectedMessage = "Zadané heslo není správné.";
            Assert.AreEqual(expectedMessage, message);
        }


        public void TestCreateProject()
        {
            driver.Navigate().GoToUrl("http://www.milamoravec.cz/Project/Create");
            driver.FindElement(By.Id("Name")).SendKeys("Projekt vytvořený přes Selenium");
            driver.FindElement(By.Id("Description")).SendKeys("Tento projekt je vytvořený přes Selenium a slouží k testování uživatelského rozhraní této diplomové práce.");
            driver.FindElement(By.Id("btnAllRight")).Click();
            driver.FindElement(By.Id("ProjectForm")).Submit();
        }

        public void TestCreateTask()
        {
            driver.Navigate().GoToUrl("http://www.milamoravec.cz/Task/Create");
            driver.FindElement(By.Id("Text")).SendKeys("Udělat test");
            driver.FindElement(By.Id("datepicker")).SendKeys("20.04.2019");
            IWebElement priority = driver.FindElement(By.Id("ID_Priority"));
            SelectElement selectPriority = new SelectElement(priority);
            selectPriority.SelectByValue("2");
            driver.FindElement(By.ClassName("btn-success")).Click();
        }

        public void TestChangeStateTask()
        {
            IWebElement btn = driver.FindElement(By.ClassName("fa-eye"));
            btn.Click();
            Thread.Sleep(5000);
            IWebElement state = driver.FindElement(By.Id("ID_State"));
            SelectElement selectState = new SelectElement(state);
            selectState.SelectByValue("2");
            driver.FindElement(By.Id("btnState")).Click();
        }

    }

}