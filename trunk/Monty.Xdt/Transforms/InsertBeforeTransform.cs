using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;


namespace Monty.Xdt.Transforms
{
    public class InsertBeforeTransform : Transform
    {
        public override IEnumerable<XElement> GetTargetElements()
        {
            // the argument string is an absolute xpath expression to the target element
            string xpath = this.ArgumentString;
            return this.WorkingDoc.XPathSelectElements(xpath);
        }

        public override void Apply()
        {
            var targets = this.GetTargetElements();

            if (targets.Count() != 1)
                throw new InvalidOperationException("You must select exactly one target element for the InsertBefore transform.");

            targets.Single().AddBeforeSelf(this.TransformElement);
        }
    }
}
