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
            var targets = this.GetTargetElements();

            if (this.Arguments.Any())
                throw new InvalidOperationException("Arguments to the Remove transform are not supported.");

            if (targets.Any())
            {
                this.GetTargetElements().First().Remove();
            }
        }
    }
}
