using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using Ankh.Scc;
using System.ComponentModel;

namespace Ankh.UI.Blame
{
    public class BlameSection : IBlameSection
    {
        readonly SvnBlameEventArgs _args;
        readonly SvnOrigin _origin;
        long _endLine;

        internal bool Hovered;

        public BlameSection(SvnBlameEventArgs e, SvnOrigin origin)
        {
            _args = e;
            _endLine = _args.LineNumber;
            _origin = origin;
        }

        [Category("Subversion")]
        public long Revision
        {
            get { return _args.Revision; }
        }

        [Category("Subversion")]
        public string Author
        {
            get { return _args.Author; }
        }

        [Category("Subversion")]
        public DateTime Time
        {
            get { return _args.Time.ToLocalTime(); }
        }

        internal int StartLine
        {
            get { return (int)_args.LineNumber; }
        }

        internal int EndLine
        {
            get { return (int)_endLine; }
            set { _endLine = value; }
        }

        [Browsable(false)]
        public SvnOrigin Origin
        {
            get { return _origin; }
        }
    }
}
