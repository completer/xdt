using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Monty.Xdt.Transforms
{
    public class InsertAfterTransform : Transform
    {
        public override IEnumerable<XElement> GetTargetElements()
        {
            // the argument string is an absolute xpath expression to the target element
            string xpath = this.ArgumentString;
            return this.WorkingDoc.XPathSelectElements(xpath);
        }

        public override void Apply()
        {
            if (this.GetTargetElements().Count() != 1)
                throw new InvalidOperationException("You must select exactly one target element for the InsertBefore transform.");

            this.GetTargetElements().Single().AddAfterSelf(this.TransformElement);
        }
    }
}
