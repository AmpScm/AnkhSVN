// $Id$
using System;
using System.Collections;


using SharpSvn;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Ankh
{
    /// <summary>
    /// Represents the parameters passed to UIShell.ShowPathSelector
    /// </summary>
    public class PathSelectorInfo
    {
        Predicate<SvnItem> _checkedFilter;
        readonly string caption;
        private bool singleSelection = false;
        private bool enableRecursive = false;
        private SvnDepth depth = SvnDepth.Empty;
        readonly ICollection<SvnItem> items;
        ICollection<SvnItem> _checkedItems = new Collection<SvnItem>();
        SvnRevision _revisionStart;
        SvnRevision _revisionEnd;

        public PathSelectorInfo(string caption, ICollection<SvnItem> items, Predicate<SvnItem> checkedFilter)
        {
            if (string.IsNullOrEmpty(caption))
                throw new ArgumentNullException("caption");
            if (items == null)
                throw new ArgumentNullException("items");
            if (checkedFilter == null)
                throw new ArgumentNullException("checkedFilter");

            this.caption = caption;
            this.items = items;
            _checkedFilter = checkedFilter;

            EvaluateFilters();
        }

        void EvaluateFilters()
        {
            foreach (SvnItem i in items)
            {
                if (Ankh.Scc.SvnItemFilters.Evaluate(i, _checkedFilter))
                    _checkedItems.Add(i);
            }
        }

        public ICollection<SvnItem> Items
        {
            get { return this.items; }
        }

        public ICollection<SvnItem> CheckedItems
        {
            get { return this._checkedItems; }
            internal set { _checkedItems = value; }
        }

        public Predicate<SvnItem> CheckedFilter
        {
            get { return _checkedFilter; }
            //set { _checkedFilter = value; }
        }

        public bool EnableRecursive
        {
            get { return this.enableRecursive; }
            set { this.enableRecursive = value; }
        }

        public SvnDepth Depth
        {
            get { return this.depth; }
            set { this.depth = value; }
        }

        public SvnRevision RevisionStart
        {
            get { return _revisionStart; }
            set { _revisionStart = value; }
        }

        public SvnRevision RevisionEnd
        {
            get { return _revisionEnd; }
            set { _revisionEnd = value; }
        }

        public bool SingleSelection
        {
            get { return this.singleSelection; }
            set { this.singleSelection = value; }
        }

        public string Caption
        {
            get { return this.caption; }
            //set{ this.caption = value; }
        }
    }
}
