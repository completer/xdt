using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Monty.Xdt.Transforms
{
    public class RemoveTransform : Transform
    {
        public override void Apply()
        {
            if (this.Arguments.Any())
                throw new InvalidOperationException("Arguments to the Remove transform are not supported.");

            this.GetTargetElements().First().Remove();
        }
    }
}
