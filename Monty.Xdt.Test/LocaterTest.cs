using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monty.Xdt;
using System.Collections;

namespace Monty.Xdt.Test
{
    /// <summary>
    /// Summary description for LocaterTest
    /// </summary>
    [TestClass]
    public class LocaterTest
    {
        public LocaterTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        XDocument doc;

        [TestMethod]
        public void TestRootElement()
        {
            string xpath = this.doc.Root.GetImplicitXPath();
            Assert.IsTrue(xpath == "/configuration");

            var element = this.doc.XPathSelectElement(xpath);
            Assert.IsNotNull(element);
            Assert.IsTrue(element == this.doc.Root);
        }

        [TestMethod]
        public void TestImplicitXPath()
        {
            var element = doc.Element("configuration")
                .Element("system.net")
                .Element("mailSettings")
                .Element("smtp")
                .Element("network");

            string xpath = element.GetImplicitXPath();
            Assert.AreEqual(element, doc.XPathSelectElement(xpath));
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.doc = XDocument.Parse(@"
                <configuration>
                  <system.net>
                    <something />
                    <mailSettings>
                      <smtp>
                        <network host=""127.0.0.1"" />
                      </smtp>
                    </mailSettings>
                  </system.net>
                </configuration>
                ");

        }

    }
}
