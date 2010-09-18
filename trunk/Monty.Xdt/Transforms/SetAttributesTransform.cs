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
            foreach (var e in this.GetTargetElements())
            {
                if (this.Arguments.Any())
                {
                    foreach (string arg in this.Arguments)
                    {
                        var a = e.Attribute(arg);
                        var b = this.TransformElement.Attribute(arg);

                        if (a == null || b == null)
                            throw new InvalidOperationException(String.Format("Couldn't find attribute '{0}' to set.", arg));
                        else
                            a.SetValue(b.Value);
                    }
                }
                else
                {
                    foreach (var a in this.TransformElement.Attributes())
                    {
                        e.SetAttributeValue(a.Name, a.Value);
                    }
                }
            }
        }
    }
}
