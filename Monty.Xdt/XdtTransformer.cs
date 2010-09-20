using System;
using System.Linq;
using System.Xml.Linq;

namespace Monty.Xdt
{
    /// <summary>
    /// Implements the Microsoft XML document transform language.
    /// See http://msdn.microsoft.com/en-us/library/dd465326(VS.100,lightweight).aspx
    /// </summary>
    public class XdtTransformer
    {
        public XDocument Transform(XDocument inputDoc, XDocument transformDoc)
        {
            var workingDoc = new XDocument(inputDoc);

            // (1) create a transform object for each "Transform" element
            var ts = from e in transformDoc.Descendants()
                     from a in e.Attributes(Namespaces.Xdt + "Transform")
                     select Monty.Xdt.Transform.Create(e, workingDoc);

            // (2) apply each transform to its target elements
            foreach (var t in ts)
            {
                t.Apply();
            }

            // (3) remove any xdt attributes copied from the transform doc
            RemoveXdtAttributes(workingDoc);

            return workingDoc;
        }

        static void RemoveXdtAttributes(XDocument doc)
        {
            var xdtAttributes = from a in doc.Descendants().Attributes()
                                where a.Name.NamespaceName == Namespaces.Xdt
                                select a;

            foreach (var a in xdtAttributes.ToList())
            {
                a.Remove();
            }
        }
    }
}
