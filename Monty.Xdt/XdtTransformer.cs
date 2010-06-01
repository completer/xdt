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
    /// Supports: xdt:Locator="Match(element-name)"
    ///           xdt:Transform="Replace"
    ///           xdt:Transform="SetAttributes"
    /// See http://msdn.microsoft.com/en-us/library/dd465326(VS.100,lightweight).aspx
    /// </summary>
    public class XdtTransformer
    {
        public XDocument Transform(XDocument inputDocument, XDocument transformDocument)
        {
            var workingDocument = new XDocument(inputDocument);

            // pair each "Transform" element with the element to be transformed
            var xs = from e in transformDocument.Descendants()
                     where e.Attributes(Namespaces.Xdt + "Transform").Any()
                     let xpath = GetTargetXPath(e)
                     select new
                     {
                         Transformer = e,
                         Target = workingDocument.XPathSelectElement(xpath)
                     };

            // todo: XPathSelectElement could be null from these user-defined conditions

            foreach (var x in xs)
            {
                TransformElement(x.Target, x.Transformer);
            }
            
            return workingDocument;
        }

        static string GetTargetXPath(XElement element)
        {
            string xpath = "/" + element.Name.LocalName;

            // possible locators: Condition, Match, XPath
            var locator = element.Attributes(Namespaces.Xdt + "Locator").FirstOrDefault();
            if (locator != null)
            {
                var l = ParseLocator(locator.Value);

                if (l.Kind == "Condition")
                {
                    // simply use the specified value as an xpath predicate

                    xpath = xpath + "[" + l.Value + "]";
                }
                else if (l.Kind == "Match")
                {
                    // a convenience case of the Condition locator, we build the xpath
                    // predicate for the user by matching on all specified attributes
                    
                    var attributeNames = l.Value.Split(',').Select(s => s.Trim());
                    var attributes = element.Attributes().Where(a => attributeNames.Contains(a.Name.LocalName));

                    xpath = xpath + "[" + attributes.ToConcatenatedString(a =>
                            "@" + a.Name.LocalName + "='" + a.Value + "'", " and ") + "]";
                }
                else
                {
                    throw new NotImplementedException(String.Format("The Locator '{0}' is not supported", l.Kind));
                }
            }

            if (element == element.Document.Root)
                return xpath;
            else
                return GetTargetXPath(element.Parent) + xpath;
        }

        static Locator ParseLocator(string locator)
        {
            var match = Regex.Match(locator, @"(\w*)\((.*)\)");

            if (!match.Success)
                throw new InvalidOperationException(String.Format("Invalid Locator attribute '{0}'.", locator));

            return new Locator
            {
                Kind = match.Groups[1].Value,
                Value = match.Groups[2].Value
            };
        }

        static void TransformElement(XElement e, XElement transformer)
        {
            string transform = transformer.Attribute(Namespaces.Xdt + "Transform").Value;

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
                var attributes = from a in transformer.Attributes()
                                 where a.Name.NamespaceName != Namespaces.Xdt
                                 select a;

                foreach (var a in attributes)
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
