using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.UI.Blame
{
    class BlameSection
    {
        readonly SvnBlameEventArgs _args;
        long _endLine;

        internal bool Hovered;
        public BlameSection(SvnBlameEventArgs e)
        {
            _args = e;
            _endLine = _args.LineNumber;
        }

        public long Revision
        {
            get { return _args.Revision; }
        }

        public string Author
        {
            get { return _args.Author; }
        }

        public DateTime Time
        {
            get { return _args.Time.ToLocalTime(); }
        }

        public int StartLine
        {
            get { return (int)_args.LineNumber; }
        }

        public int EndLine
        {
            get { return (int)_endLine; }
            set { _endLine = value; }
        }
    }
}
