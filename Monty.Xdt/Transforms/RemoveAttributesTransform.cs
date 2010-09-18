using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monty.Xdt.Transforms
{
    public class RemoveAttributesTransform : Transform
    {
        public override void Apply()
        {
            if (!this.Arguments.Any())
                throw new InvalidOperationException("Attributes to remove must be specified for the RemoveAttributes transform.");

            foreach (var e in this.GetTargetElements())
            {
                foreach (var arg in this.Arguments)
                {
                    var a = e.Attribute(arg);

                    if (a == null)
                        throw new InvalidOperationException(String.Format("Couldn't find attribute '{0}' to remove.", arg));
                    else
                        a.Remove();
                }
            }
        }
    }
}
