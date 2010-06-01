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
    public class LocatorTests
    {
        public TestContext TestContext { get; set; }

        XDocument doc;

        [TestInitialize]
        public void TestInitialize()
        {
            this.doc = XDocument.Parse(@"
                <configuration>
                  <appSettings>
                    <add key=""key1"" value=""value1"" />
                    <add key=""key2"" value=""value2"" />
                  </appSettings>
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

        [TestMethod]
        public void TestRootElement()
        {
            string xpath = XdtTransformer_Accessor.GetTargetXPath(this.doc.Root);
            Assert.IsTrue(xpath == "/configuration");

            var element = this.doc.XPathSelectElement(xpath);
            Assert.IsNotNull(element);
            Assert.IsTrue(element == this.doc.Root);
        }

        [TestMethod]
        public void TestImplicitXPath()
        {
            var element = this.doc.Element("configuration")
                .Element("system.net")
                .Element("mailSettings")
                .Element("smtp")
                .Element("network");

            string xpath = XdtTransformer_Accessor.GetTargetXPath(element);
            Assert.AreEqual(element, this.doc.XPathSelectElement(xpath));
        }

        [TestMethod]
        public void TestConditionLocator()
        {
            var transform = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <appSettings>
                    <add xdt:Locator=""Condition(@key='key1' or @anotherAttribute='something')"" />
                  </appSettings>
                </configuration>
                ");

            var transformElement = transform
                .Element("configuration")
                .Element("appSettings")
                .Element("add");

            string xpath = XdtTransformer_Accessor.GetTargetXPath(transformElement);
            Assert.AreEqual(xpath, "/configuration/appSettings/add[@key='key1' or @anotherAttribute='something']");

            var workingElement = this.doc
                .Element("configuration")
                .Element("appSettings")
                .Elements("add")
                .Single(e => e.Attribute("key").Value == "key1");
            Assert.AreEqual(workingElement, this.doc.XPathSelectElement(xpath));
        }

        [TestMethod]
        public void TestConditionLocatorRecursive()
        {
            var transform = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <appSettings xdt:Locator=""Condition(@attribute!='value')"">
                    <add xdt:Locator=""Condition(@key='key1')"" />
                  </appSettings>
                </configuration>
                ");

            var transformElement = transform
                .Element("configuration")
                .Element("appSettings")
                .Element("add");

            string xpath = XdtTransformer_Accessor.GetTargetXPath(transformElement);
            Assert.AreEqual(xpath, "/configuration/appSettings[@attribute!='value']/add[@key='key1']");
        }

        [TestMethod]
        public void TestMatchLocator()
        {
            var transform = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <appSettings>
                    <add key=""key1"" value=""value1"" xdt:Locator=""Match(key)"" />
                  </appSettings>
                </configuration>
                ");

            var transformElement = transform
                .Element("configuration")
                .Element("appSettings")
                .Element("add");

            string xpath = XdtTransformer_Accessor.GetTargetXPath(transformElement);
            Assert.AreEqual(xpath, "/configuration/appSettings/add[@key='key1']");

            var workingElement = this.doc
                .Element("configuration")
                .Element("appSettings")
                .Elements("add")
                .Single(e => e.Attribute("key").Value == "key1");
            Assert.AreEqual(workingElement, this.doc.XPathSelectElement(xpath));
        }
    }
}
