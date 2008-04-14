using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.UI
{
    public class PathSelectorResult
    {
        readonly bool _succeeded;
        readonly List<SvnItem> _selection;
        SvnDepth _depth = SvnDepth.Unknown;
        SvnRevision _start;
        SvnRevision _end;

        public PathSelectorResult(bool succeeded, IEnumerable<SvnItem> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            _succeeded = succeeded;
            _selection = new List<SvnItem>(items);
        }

        public SvnDepth Depth
        {
            get { return _depth; }
            set { _depth = value; }
        }

        public SvnRevision RevisionStart
        {
            get { return _start; }
            set { _start = value; }
        }

        public SvnRevision RevisionEnd
        {
            get { return _end; }
            set { _end = value; }
        }


        public IList<SvnItem> Selection
        {
            get
            {
                return _selection;
            }
        }

        public bool Succeeded
        {
            get { return _succeeded; }
        }
    }
}
