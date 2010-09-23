using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Monty.Xdt.Transforms
{
    public class InsertTransform : Transform
    {
        public override IEnumerable<XElement> GetTargetElements()
        {
            // use the parent in case there are no selected elements in the input doc
            string xpath = Transform.GetTargetXPath(this.TransformElement.Parent);
            return this.WorkingDoc.XPathSelectElements(xpath);
        }

        public override void Apply()
        {
            var targets = this.GetTargetElements();

            if (this.Arguments.Any())
                throw new InvalidOperationException("Arguments to the Insert transform are not supported.");

            if (targets.Any())
            {
                targets.First().Add(this.TransformElement);
            }
        }
    }
}
