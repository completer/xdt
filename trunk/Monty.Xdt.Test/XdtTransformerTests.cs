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
    public class XdtTransformerTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void TestIdentityTransform()
        {
            // supply an empty transform document

            var input = GetInputDocument();
            var transform = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"" />
                ");

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
        public void TestReplaceTransform()
        {
            // replace the entire system.web element

            var input = GetInputDocument();
            var transform = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <system.web xdt:Transform=""Replace"">
                    <extra content=""here"" />
                  </system.web>
                </configuration>
                ");
            var output = new XdtTransformer().Transform(input, transform);

            var element = output
                .Element("configuration")
                .Element("system.web")
                .Element("extra");

            Assert.IsTrue(element.Attribute("content").Value == "here");

            // ensure there are no xdt attributes from the transform doc!
            Assert.IsFalse(output.Descendants().Attributes().Any(a => a.Name.NamespaceName == Namespaces.Xdt));
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

        [TestMethod]
        public void TestConditionLocator()
        {
            var input = GetInputDocument();
            var transform = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <appSettings>
                    <add key=""key2"" value=""value2-live"" xdt:Locator=""Condition(@key='key2')"" xdt:Transform=""SetAttributes"" />
                  </appSettings>
                </configuration>
                ");
            var output = new XdtTransformer().Transform(input, transform);

            var settings = output
                .Element("configuration")
                .Element("appSettings")
                .Elements("add");
            var setting1 = settings.Single(e => e.Attribute("key").Value == "key1");
            var setting2 = settings.Single(e => e.Attribute("key").Value == "key2");

            Assert.IsTrue(setting1.Attribute("value").Value == "value1");
            Assert.IsTrue(setting2.Attribute("value").Value == "value2-live");
        }

        [TestMethod]
        public void TestSetAttributesTransformWithLocator()
        {
            var input = GetInputDocument();
            var transform = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <appSettings>
                    <add key=""key2"" value=""value2-live"" xdt:Locator=""Match(key)"" xdt:Transform=""SetAttributes"" />
                  </appSettings>
                </configuration>
                ");
            var output = new XdtTransformer().Transform(input, transform);

            var settings = output
                .Element("configuration")
                .Element("appSettings")
                .Elements("add");
            var setting1 = settings.Single(e => e.Attribute("key").Value == "key1");
            var setting2 = settings.Single(e => e.Attribute("key").Value == "key2");

            Assert.IsTrue(setting1.Attribute("value").Value == "value1");
            Assert.IsTrue(setting2.Attribute("value").Value == "value2-live");
        }

        [TestMethod]
        public void TestMultipleElementsAreTransformed()
        {
            var input = GetInputDocument();
            var transform = XDocument.Parse(@"
                <configuration xmlns:xdt=""http://schemas.microsoft.com/XML-Document-Transform"">
                  <appSettings>
                    <add value=""ha"" xdt:Transform=""SetAttributes"" />
                  </appSettings>
                </configuration>
                ");
            var output = new XdtTransformer().Transform(input, transform);

            var settings = output
                .Element("configuration")
                .Element("appSettings")
                .Elements("add");

            Assert.IsTrue(settings.Count() == 3);
            Assert.IsTrue(settings.SelectMany(e => e.Attributes("value")).All(a => a.Value == "ha"));
        }

        XDocument GetInputDocument()
        {
            return XDocument.Load(@"..\..\..\Monty.Xdt.Test\SimpleInputDocument.xml");            
        }
    }
}
