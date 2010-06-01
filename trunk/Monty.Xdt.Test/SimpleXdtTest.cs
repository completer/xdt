using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Xml.Linq;

namespace Monty.Xdt.Test
{
    [TestClass]
    public class SimpleXdtTest
    {
        public SimpleXdtTest()
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

        [TestMethod]
        public void TestIdentityTransform()
        {
            var input = GetInputDocument();
            var transform = GetIdentityTransformDocument();

            var output = new XdtTransformer().Transform(input, transform);

            Assert.IsTrue(XDocument.DeepEquals(input, output));
        }

        [TestMethod]
        public void TestRemoveTransform()
        {
            // remove the entire system.web element

            var input = GetInputDocument();
            var transform = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <system.web xdt:Transform=""Remove"" />
                </configuration>
                ");
            var output = new XdtTransformer().Transform(input, transform);

            Assert.IsFalse(output.Descendants("system.web").Any());
            Assert.IsTrue(output.Descendants("configuration").Any());
        }

        [TestMethod]
        public void TestSetAttributesTransform()
        {
            // change the "debug" attribute to false

            var input = GetInputDocument();
            var transform = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <system.web>
                    <compilation debug=""false"" xdt:Transform=""SetAttributes"" />
                  </system.web>
                </configuration>
                ");
            var output = new XdtTransformer().Transform(input, transform);

            var element = output
                .Element("configuration")
                .Element("system.web")
                .Element("compilation");

            Assert.IsTrue(element.Attribute("debug").Value == "false");

            // ensure we haven't accidentally added attributes from the transform document
            Assert.IsFalse(output.Descendants().Any(e =>
                e.Attributes().Any(a =>
                    a.Name.NamespaceName == Namespaces.Xdt)));
        }

        XDocument GetInputDocument()
        {
            return XDocument.Load(@"..\..\..\Monty.Xdt.Test\SimpleInputDocument.xml");            
        }

        XDocument GetTransformDocument()
        {
            return XDocument.Load(@"..\..\..\Monty.Xdt.Test\SimpleTransformDocument.xml");
        }

        XDocument GetIdentityTransformDocument()
        {
            return XDocument.Load(@"..\..\..\Monty.Xdt.Test\IdentityTransformDocument.xml");
        }
    }
}
