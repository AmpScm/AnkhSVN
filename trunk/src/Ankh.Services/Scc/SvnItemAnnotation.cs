using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SharpSvn;

namespace Ankh.Scc
{
    /// <summary>
    /// Some versioned files might have a temporary/transient annotation from a previous
    /// operation. E.g.
    /// <list type="unordered">
    ///     <item>Files merged and/or conflicted during update have an original version</item>
    ///     <item>Conflicts might have url information that isn't stored in the Subversion store</item>
    /// </list>
    /// </summary>
    public class SvnItemAnnotation
    {
        readonly IFileStatusCache _context;
        readonly string _fullPath;
        IDictionary<string, string> _justStored;

        public SvnItemAnnotation(IFileStatusCache context, string fullPath)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (string.IsNullOrEmpty(fullPath))
                throw new ArgumentNullException("fullPath");

            _context = context;
            _fullPath = fullPath;
        }

        const string ElemNode = "node";
        const string AttrName = "name";

        SvnItemAnnotation(IFileStatusCache context, string fullPath, XmlReader from)
            : this (context, fullPath)
        {
            if (!from.MoveToFirstAttribute())
                throw new InvalidOperationException();

            Dictionary<string, string> rest = null;
            do
            {
                switch (from.Name)
                {
                    case AttrName:
                        break;

                    // TODO: Read explicit values

                    default:
                        if (rest == null)
                            rest = new Dictionary<string,string>(StringComparer.Ordinal);

                        rest[from.Name] = from.Value;
                        break;
                }
            }
            while (from.MoveToNextAttribute());
            _justStored = rest;
            from.MoveToElement();
        }

        public void Write(IFileStatusCache context, XmlWriter to, string wcRoot)
        {
            to.WriteStartElement(ElemNode);
            to.WriteAttributeString(AttrName, new Uri(wcRoot).MakeRelativeUri(new Uri(wcRoot)).ToString());

            // TODO: Write explicit values

            if (_justStored != null)
                foreach (KeyValuePair<string, string> kv in _justStored)
                {
                    to.WriteAttributeString(kv.Key, kv.Value);
                }
            to.WriteEndElement();
        }

        public static SvnItemAnnotation Load(IFileStatusCache context, XmlReader from, string wcRoot)
        {
            if (from.Name != ElemNode)
                return null;

            string name = from.GetAttribute(AttrName);

            return new SvnItemAnnotation(context, Path.Combine(wcRoot, name), from);
        }
    }
}
