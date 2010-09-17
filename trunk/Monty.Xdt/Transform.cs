using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Monty.Xdt
{
    public abstract class Transform
    {
        public IEnumerable<XElement> TargetElements   { get; private set; }
        public XElement              TransformElement { get; private set; }

        public string ArgumentString { get; private set; }

        public IEnumerable<string> Arguments
        {
            get { return this.ArgumentString.Split(',').Where(s => !String.IsNullOrEmpty(s)); }
        }

        public abstract void Apply();

        public static Transform Create(XElement transformElement, IEnumerable<XElement> targetElements)
        {
            string raw = transformElement.Attribute(Namespaces.Xdt + "Transform").Value;
            var match = Regex.Match(raw, @"(\w*)(\((.*)\))?");

            if (!match.Success)
                throw new InvalidOperationException(String.Format("Invalid Transform attribute '{0}'.", raw));

            string type = match.Groups[1].Value;
            string args = match.Groups[3].Value;

            // todo: make extensible!
            var classType = Assembly.GetExecutingAssembly().GetType("Monty.Xdt.Transforms." + type + "Transform");
            var transform = (Transform) Activator.CreateInstance(classType);

            transform.TransformElement = transformElement;
            transform.TargetElements = targetElements;
            transform.ArgumentString = args;
            
            return transform;
        }
    }
}
