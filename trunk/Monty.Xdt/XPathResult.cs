using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Monty.Xdt
{
    public class XPathResult
    {
        public string                Expression { get; set; }
        public IXmlNamespaceResolver Resolver   { get; set; }
    }
}
