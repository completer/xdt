using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monty.Xdt.Transforms
{
    public class SetAttributesTransform : Transform
    {
        public override void Apply()
        {
            if (this.Arguments.Any())
                throw new NotImplementedException("Arguments to the SetAttribute transform are not supported yet.");

            foreach (var e in this.GetTargetElements())
            {
                foreach (var a in this.TransformElement.Attributes())
                {
                    e.SetAttributeValue(a.Name, a.Value);
                }
            }
        }
    }
}
