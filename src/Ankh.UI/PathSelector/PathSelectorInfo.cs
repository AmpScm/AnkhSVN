// $Id$
using System;
using System.Collections;


using SharpSvn;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ankh.UI;
using Ankh.Scc;

namespace Ankh
{
    /// <summary>
    /// Represents the parameters passed to UIShell.ShowPathSelector
    /// </summary>
    public class PathSelectorInfo
    {

        readonly string _caption;
        bool _singleSelection;
        bool _enableRecursive;
        SvnDepth _depth = SvnDepth.Empty;
        readonly ICollection<SvnItem> _items;
        readonly Dictionary<string, SvnItem> _checkedItems = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
        readonly Dictionary<string, SvnItem> _visibleItems = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
        SvnRevision _revisionStart;
        SvnRevision _revisionEnd;
        Predicate<SvnItem> _checkedFilter;
        Predicate<SvnItem> _visibleFilter;
        bool _evaluated;

        [Obsolete]
        public PathSelectorInfo(string caption, ICollection<SvnItem> items, Predicate<SvnItem> checkedFilter)
            : this(caption, items)
        {
            if (checkedFilter == null)
                throw new ArgumentNullException("checkedFilter");

            CheckedFilter += checkedFilter;
        }

        public PathSelectorInfo(string caption, IEnumerable<SvnItem> items)
        {
            if (string.IsNullOrEmpty(caption))
                throw new ArgumentNullException("caption");
            if (items == null)
                throw new ArgumentNullException("items");

            _caption = caption;
            _items = new List<SvnItem>(items);
        }

        public bool EvaluateChecked(SvnItem item)
        {
            return SvnItemFilters.Evaluate(item, _checkedFilter);
        }

        public event Predicate<SvnItem> CheckedFilter
        {
            add
            {
                _evaluated = false;
                _checkedFilter += value;
            }
            remove
            {
                _evaluated = false;
                _checkedFilter -= value;
            }
        }

        public event Predicate<SvnItem> VisibleFilter
        {
            add
            {
                _evaluated = false;
                _visibleFilter += value;
            }
            remove
            {
                _evaluated = false;
                _visibleFilter -= value;
            }
        }

        void EnsureFiltered()
        {
            if (!_evaluated)
            {
                _checkedItems.Clear();
                _visibleItems.Clear();

                foreach (SvnItem i in _items)
                {
                    if (Ankh.Scc.SvnItemFilters.Evaluate(i, _visibleFilter))
                    {
                        if (!_visibleItems.ContainsKey(i.FullPath))
                            _visibleItems.Add(i.FullPath, i);

                        if (Ankh.Scc.SvnItemFilters.Evaluate(i, _checkedFilter))
                        {
                            if (!_checkedItems.ContainsKey(i.FullPath))
                                _checkedItems.Add(i.FullPath, i);
                        }
                    }
                }
                _evaluated = true;
            }
        }

        public ICollection<SvnItem> VisibleItems
        {
            get
            {
                EnsureFiltered();
                return _visibleItems.Values;
            }
        }

        public ICollection<SvnItem> CheckedItems
        {
            get
            {
                EnsureFiltered();
                return _checkedItems.Values;
            }
            //internal set { _checkedItems = value; }
        }

        public bool EnableRecursive
        {
            get { return this._enableRecursive; }
            set { this._enableRecursive = value; }
        }

        public SvnDepth Depth
        {
            get { return this._depth; }
            set { this._depth = value; }
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
            get { return this._singleSelection; }
            set { this._singleSelection = value; }
        }

        public string Caption
        {
            get { return this._caption; }
            //set{ this.caption = value; }
        }

        public PathSelectorResult DefaultResult
        {
            get
            {
                PathSelectorResult result = new PathSelectorResult(true, CheckedItems);
                result.RevisionStart = RevisionStart;
                result.RevisionEnd = RevisionEnd;
                result.Depth = Depth;
                return result;
            }
        }


    }
}
