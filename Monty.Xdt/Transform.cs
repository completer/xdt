using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace Monty.Xdt
{
    public abstract class Transform
    {
        public XElement  TransformElement { get; private set; }
        public XDocument WorkingDoc       { get; private set; }

        public string ArgumentString { get; private set; }

        public IEnumerable<string> Arguments
        {
            get { return this.ArgumentString.Split(',').Where(s => !String.IsNullOrEmpty(s)); }
        }

        public abstract void Apply();

        public static Transform Create(XElement transformElement, XDocument workingDoc)
        {
            string raw = transformElement.Attribute(Namespaces.Xdt + "Transform").Value;
            var match = Regex.Match(raw, @"(\w*)(\((.*)\))?");

            if (!match.Success)
                throw new InvalidOperationException(String.Format("Invalid Transform attribute '{0}'.", raw));

            string type = match.Groups[1].Value;
            string args = match.Groups[3].Value;

            // todo: make extensible!
            var classType = Assembly.GetExecutingAssembly().GetType("Monty.Xdt.Transforms." + type + "Transform");

            if (classType == null)
                throw new NotSupportedException(String.Format("The transform '{0}' is not supported or could not be found.", type));

            var transform = (Transform) Activator.CreateInstance(classType);

            transform.TransformElement = transformElement;
            transform.WorkingDoc = workingDoc;
            transform.ArgumentString = args;
            
            return transform;
        }

        public virtual IEnumerable<XElement> GetTargetElements()
        {
            var xpath = Transform.GetTargetXPath(this.TransformElement);
            return this.WorkingDoc.XPathSelectElements(xpath.Expression, xpath.Resolver);
        }

        protected static XPathResult GetTargetXPath(XElement element)
        {
            var locator = Locator.Parse(element);

            if (locator != null && locator.Type == "XPath")
            {
                return new XPathResult
                {
                    Expression = locator.Arguments,
                    Resolver = new XmlNamespaceManager(new NameTable())
                };
            }
            else
            {
                var nsManager = new XmlNamespacePrefixManager(new XmlNamespaceManager(new NameTable()));
                string expression = Transform.GetTargetXPathRecursive(element, nsManager);

                return new XPathResult
                {
                    Expression = expression,
                    Resolver = nsManager.Manager
                };
            }
        }

        protected static string GetTargetXPathRecursive(
            XElement element,
            XmlNamespacePrefixManager nsManager)
        {
            string prefix = nsManager.AddNamespace(element.Name.NamespaceName);

            if (prefix != String.Empty)
                prefix += ":";

            string xpath = "/" + prefix + element.Name.LocalName + Locator.GetLocatorPredicate(element);

            if (element == element.Document.Root)
                return xpath;
            else
                return Transform.GetTargetXPathRecursive(element.Parent, nsManager) + xpath;
        }
    }
}
