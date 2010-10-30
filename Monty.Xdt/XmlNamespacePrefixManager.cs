using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Monty.Xdt
{
    public class XmlNamespacePrefixManager
    {
        public XmlNamespaceManager Manager { get; private set; }

        List<string> set = new List<string>(); // an OrderedSet would be better

        public XmlNamespacePrefixManager(XmlNamespaceManager manager)
        {
            this.Manager = manager;
        }

        /// <summary>
        /// Adds the namespace to the wrapped XmlNamespaceManager and returns a prefix.
        /// </summary>
        /// <returns>
        /// The prefix will be the empty string for the empty namespace, otherwise some
        /// string different to the prefixes given to previously added namespaces.
        /// </returns>
        public string AddNamespace(string ns)
        {
            // add any new namespace to the namespace set
            if (!this.set.Contains(ns))
                this.set.Add(ns);
            
            // determine a unique prefix for the namespace
            // (based on its index in the set)
            string prefix = String.IsNullOrEmpty(ns)
                ? String.Empty
                : new String('a', this.set.IndexOf(ns) + 1);

            // ensure the prefix -> namespace mapping is registered
            // with the wrapped namespace manager/resolver
            this.Manager.AddNamespace(prefix, ns);

            return prefix;
        }
    }
}
