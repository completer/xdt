using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;

namespace Monty.Xdt.Test
{
    [TestClass]
    public class XmlNamespacePrefixManagerTests
    {
        [TestMethod]
        public void Returns_empty_prefix_for_empty_namespace()
        {
            var manager = new XmlNamespacePrefixManager(new XmlNamespaceManager(new NameTable()));
            string ns = "";

            string prefix = manager.AddNamespace(ns);
            Assert.AreEqual("", prefix);
        }

        [TestMethod]
        public void Returns_distinct_prefixes_for_different_namespaces()
        {
            var manager = new XmlNamespacePrefixManager(new XmlNamespaceManager(new NameTable()));
            string ns1 = "http://test1.com";
            string ns2 = "http://test2.com";

            string prefix1 = manager.AddNamespace(ns1);
            string prefix2 = manager.AddNamespace(ns2);
            Assert.AreNotEqual(prefix1, prefix2);
        }
    }
}
