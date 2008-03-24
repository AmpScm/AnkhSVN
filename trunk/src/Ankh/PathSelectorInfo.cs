// $Id$
using System;
using System.Collections;


using SharpSvn;

namespace Ankh
{
    /// <summary>
    /// Represents the parameters passed to UIShell.ShowPathSelector
    /// </summary>
    public class PathSelectorInfo
    {
        public PathSelectorInfo( string caption, IList items, IList checkedItems )
        {
            this.caption = caption;
            this.items = items;
            this.checkedItems = checkedItems;
        }

        public IList Items
        {
            get{ return this.items; }
            set{ this.items = value; }
        }

        public IList CheckedItems
        {
            get{ return this.checkedItems; }
            set{ this.checkedItems = value; }
        }

        public bool EnableRecursive
        {
            get{ return this.enableRecursive; }
            set{ this.enableRecursive = value; }
        }

        public SvnDepth Depth
        {
            get { return this.depth; }
            set { this.depth = value; }
        }

        public SvnRevision RevisionStart
        {
            get{ return this.revisionStart; }
            set{ this.revisionStart = value; }
        }

        public SvnRevision RevisionEnd
        {
            get{ return this.revisionEnd; }
            set{ this.revisionEnd = value; }
        }

        public bool SingleSelection
        {
            get{ return this.singleSelection; }
            set{ this.singleSelection = value; }
        }

        public string Caption
        {
            get{ return this.caption; }
            set{ this.caption = value; }
        }

        private string caption = "Someone obviously forgot to put a caption here.";
        private bool singleSelection = false;
        private bool enableRecursive = false;
        private SvnDepth depth = SvnDepth.Empty;
        private IList items = new object[]{};
        private IList checkedItems = new object[]{};
        private SvnRevision revisionStart, revisionEnd;
    }
}
