using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Monty.Xdt
{
    public class Locator
    {
        /// <summary>
        /// The type of locator, "Condition", "Match" or "XPath".
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The string specified by the user in parentheses after the type.
        /// </summary>
        public string Arguments { get; set; }

        public static Locator Parse(XElement element)
        {
            var locatorAttribute = element.Attributes(Namespaces.Xdt + "Locator").FirstOrDefault();

            if (locatorAttribute != null)
                return Locator.Parse(locatorAttribute.Value);
            else
                return null;
        }

        public static Locator Parse(string locator)
        {
            var match = Regex.Match(locator, @"(\w*)\((.*)\)");

            if (!match.Success)
                throw new InvalidOperationException(String.Format("Invalid Locator attribute '{0}'.", locator));

            return new Locator
            {
                Type = match.Groups[1].Value,
                Arguments = match.Groups[2].Value
            };
        }

        public static string GetLocatorPredicate(XElement element)
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
    }
}
