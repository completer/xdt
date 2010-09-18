using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monty.Xdt.Transforms
{
    public class RemoveAll : Transform
    {
        public override void Apply()
        {
            if (this.Arguments.Any())
                throw new InvalidOperationException("Arguments to the RemoveAll transform are not supported.");

            foreach (var e in this.GetTargetElements())
            {
                e.Remove();
            }
        }
    }
}
