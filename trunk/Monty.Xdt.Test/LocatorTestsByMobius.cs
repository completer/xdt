using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Monty.Xdt.Test
{
    /// <summary>
    /// Locator tests based on http://blog.hmobius.com/post/2010/02/17/ASPNET-40-Part-4-Config-Transformation-Files.aspx
    /// </summary>
    [TestClass]
    public class LocatorTestsByMobius
    {
        public TestContext TestContext { get; set; }

        XDocument inputDoc;
        XDocument desiredDoc;

        [TestInitialize]
        public void TestInitialize()
        {
            this.inputDoc = XDocument.Parse(@"
                <configuration>
                  <system.net>
                    <mailSettings>
                      <smtp deliveryMethod=""SpecifiedPickupDirectory"" from=""dan@test.com"">
                        <specifiedPickupDirectory pickupDirectoryLocation=""D:\Temp""/>
                      </smtp>
                    </mailSettings>
                  </system.net>
                </configuration>
                ");

            this.desiredDoc = XDocument.Parse(@"
                <configuration>
                  <system.net>
                    <mailSettings>
                      <smtp deliveryMethod=""Network"" from=""dan@test.com"">
                        <network host=""liveMailServer"" />
                      </smtp>
                    </mailSettings>
                  </system.net>
                </configuration>
                ");
        }

        [TestMethod]
        public void TestConditionLocator()
        {
            var transformDoc = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <system.net>
                    <mailSettings>
                      <smtp deliveryMethod=""Network"" from=""dan@test.com""
                            xdt:Locator=""Condition(@deliveryMethod='SpecifiedPickupDirectory')""
                            xdt:Transform=""Replace"">
                        <network host=""liveMailServer""/>
                      </smtp>
                    </mailSettings>
                  </system.net>
                </configuration>
                ");

            var output = new XdtTransformer().Transform(this.inputDoc, transformDoc);
            Assert.IsTrue(XNode.DeepEquals(this.desiredDoc, output));
        }

        [TestMethod]
        public void TestMatchLocator()
        {
            var transformDoc = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <system.net>
                    <mailSettings>
                      <smtp deliveryMethod=""Network"" from=""dan@test.com""
                            xdt:Locator=""Match(from)""
                            xdt:Transform=""Replace"">
                        <network host=""liveMailServer""/>
                      </smtp>
                    </mailSettings>
                  </system.net>
                </configuration>
                ");

            var output = new XdtTransformer().Transform(this.inputDoc, transformDoc);
            Assert.IsTrue(XNode.DeepEquals(this.desiredDoc, output));
        }

        [TestMethod]
        public void TestXPathLocator()
        {
            var transformDoc = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <system.net>
                    <mailSettings>
                      <smtp deliveryMethod=""Network"" from=""dan@test.com""
                            xdt:Locator=""XPath(configuration/system.net/mailSettings/smtp
                               [@deliveryMethod='SpecifiedPickupDirectory'])""
                            xdt:Transform=""Replace"">
                        <network host=""liveMailServer""/>
                      </smtp>
                    </mailSettings>
                  </system.net>
                </configuration>
                ");

            var output = new XdtTransformer().Transform(this.inputDoc, transformDoc);
            Assert.IsTrue(XNode.DeepEquals(this.desiredDoc, output));
        }
    }
}
