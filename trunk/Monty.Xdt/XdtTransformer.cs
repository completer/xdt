using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Text.RegularExpressions;

namespace Monty.Xdt
{
    /// <summary>
    /// Implements a subset of the Microsoft XML document transform language.
    /// See http://msdn.microsoft.com/en-us/library/dd465326(VS.100,lightweight).aspx
    /// </summary>
    public class XdtTransformer
    {
        public XDocument Transform(XDocument inputDoc, XDocument transformDoc)
        {
            var workingDoc = new XDocument(inputDoc);

            // (1) pair each "Transform" element with the element(s)
            // it targets in the working document
            var ts = from e in transformDoc.Descendants()
                     where e.Attributes(Namespaces.Xdt + "Transform").Any()
                     let xpath = GetTargetXPath(e)
                     let targetElements = workingDoc.XPathSelectElements(xpath)
                     select Monty.Xdt.Transform.Create(e, targetElements);

            // (2) apply each transform to its target elements
            foreach (var t in ts)
            {
                t.Apply();
            }

            // (3) remove any xdt attributes copied from the transform doc
            RemoveXdtAttributes(workingDoc);

            return workingDoc;
        }

        static string GetTargetXPath(XElement element)
        {
            var locator = Locator.Parse(element);

            if (locator != null && locator.Type == "XPath")
                return locator.Arguments;
            else
                return GetImplicitXPathRecursive(element);
        }

        static string GetImplicitXPathRecursive(XElement element)
        {
            string xpath = "/" + element.Name.LocalName + GetLocatorPredicate(element);
            
            if (element == element.Document.Root)
                return xpath;
            else
                return GetImplicitXPathRecursive(element.Parent) + xpath;
        }

        static string GetLocatorPredicate(XElement element)
        {
            var locatorAttribute = element.Attributes(Namespaces.Xdt + "Locator").FirstOrDefault();

            if (locatorAttribute == null)
            {
                return String.Empty;
            }
            else
            {
                var locator = Locator.Parse(locatorAttribute.Value);

                if (locator.Type == "Condition")
                {
                    // use the user-defined value as an xpath predicate
                    return "[" + locator.Arguments + "]";
                }
                else if (locator.Type == "Match")
                {
                    // convenience case of the Condition locator, build the xpath
                    // predicate for the user by matching on all specified attributes
                    
                    var attributeNames = locator.Arguments.Split(',').Select(s => s.Trim());
                    var attributes = element.Attributes().Where(a => attributeNames.Contains(a.Name.LocalName));

                    return "[" + attributes.ToConcatenatedString(a =>
                        "@" + a.Name.LocalName + "='" + a.Value + "'", " and ") + "]";
                }
                else
                {
                    throw new NotImplementedException(String.Format("The Locator '{0}' is not supported", locator.Type));
                }
            }
        }

        static void RemoveXdtAttributes(XDocument doc)
        {
            var xdtAttributes = from a in doc.Descendants().Attributes()
                                where a.Name.NamespaceName == Namespaces.Xdt
                                select a;

            foreach (var a in xdtAttributes.ToList())
            {
                a.Remove();
            }
        }
    }
}
