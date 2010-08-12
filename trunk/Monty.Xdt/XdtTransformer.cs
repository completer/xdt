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
            var xs = from e in transformDoc.Descendants()
                     where e.Attributes(Namespaces.Xdt + "Transform").Any()
                     let xpath = GetTargetXPath(e)
                     select new
                     {
                         TransformElement = e,
                         TargetElements = workingDoc.XPathSelectElements(xpath)
                     };

            // (2) apply each transform to its target elements
            foreach (var x in xs)
            {
                ApplyTransform(x.TransformElement, x.TargetElements);
            }

            // (3) remove any xdt attributes copied from the transform doc
            RemoveXdtAttributes(workingDoc);

            return workingDoc;
        }

        static string GetTargetXPath(XElement element)
        {
            var locator = Locator.Parse(element);

            if (locator != null && locator.Kind == "XPath")
                return locator.Value;
            else
                return GetTargetXPathRecursive(element);
        }

        static string GetTargetXPathRecursive(XElement element)
        {
            string xpath = "/" + element.Name.LocalName + GetLocatorPredicate(element);
            
            if (element == element.Document.Root)
                return xpath;
            else
                return GetTargetXPathRecursive(element.Parent) + xpath;
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

                if (locator.Kind == "Condition")
                {
                    // use the user-defined value as an xpath predicate
                    return "[" + locator.Value + "]";
                }
                else if (locator.Kind == "Match")
                {
                    // convenience case of the Condition locator, build the xpath
                    // predicate for the user by matching on all specified attributes
                    
                    var attributeNames = locator.Value.Split(',').Select(s => s.Trim());
                    var attributes = element.Attributes().Where(a => attributeNames.Contains(a.Name.LocalName));

                    return "[" + attributes.ToConcatenatedString(a =>
                        "@" + a.Name.LocalName + "='" + a.Value + "'", " and ") + "]";
                }
                else
                {
                    throw new NotImplementedException(String.Format("The Locator '{0}' is not supported", locator.Kind));
                }
            }
        }

        static void ApplyTransform(XElement transformer, IEnumerable<XElement> targetElements)
        {
            if (transformer != null)
            {
                string transform = transformer.Attribute(Namespaces.Xdt + "Transform").Value;

                foreach (var e in targetElements)
                {
                    if (transform == "Remove")
                    {
                        e.Remove();
                    }
                    else if (transform == "Replace")
                    {
                        e.ReplaceWith(transformer);
                    }
                    else if (transform == "SetAttributes")
                    {
                        foreach (var a in transformer.Attributes())
                        {
                            e.SetAttributeValue(a.Name, a.Value);
                        }
                    }
                    else
                    {
                        throw new NotImplementedException(String.Format("The transform '{0}' is not supported.", transform));
                    }
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
