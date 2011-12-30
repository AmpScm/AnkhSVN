// $Id$
//
// Copyright 2005-2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections;

using SharpSvn;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Ankh.UI;
using Ankh.Scc;

namespace Ankh.UI.PathSelector
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
        SelectableFilter _checkableFilter;

        bool _evaluated;

        public PathSelectorInfo(string caption, IEnumerable<SvnItem> items)
        {
            if (string.IsNullOrEmpty(caption))
                throw new ArgumentNullException("caption");
            if (items == null)
                throw new ArgumentNullException("items");

            _caption = caption;
            _items = new List<SvnItem>(items);
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

        public event SelectableFilter CheckableFilter
        {
            add
            {
                _evaluated = false;
                _checkableFilter += value;
            }
            remove
            {
                _evaluated = false;
                _checkableFilter -= value;
            }
        }

        public bool EvaluateChecked(SvnItem item)
        {
            return EvaluateFilter(item, _checkedFilter);
        }

        public bool EvaluateCheckable(SvnItem item, SvnRevision from, SvnRevision to)
        {
            if (_checkableFilter == null)
                return true;

            foreach (SelectableFilter i in _checkableFilter.GetInvocationList())
            {
                if (!i(item, from, to))
                    return false;
            }
            return true;
        }

        void EnsureFiltered()
        {
            if (!_evaluated)
            {
                _checkedItems.Clear();
                _visibleItems.Clear();

                foreach (SvnItem i in _items)
                {
                    if (EvaluateFilter(i, _visibleFilter))
                    {
                        if (!_visibleItems.ContainsKey(i.FullPath))
                            _visibleItems.Add(i.FullPath, i);

                        if (EvaluateFilter(i, _checkedFilter)
                            // make sure all the checked items are suitable for the revisions
                            && EvaluateCheckable(i, RevisionStart, RevisionEnd))
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

        ICollection<SvnItem> CheckedItems
        {
            get
            {
                EnsureFiltered();
                return _checkedItems.Values;
            }
        }

        public bool EnableRecursive
        {
            get { return _enableRecursive; }
            set { _enableRecursive = value; }
        }

        public SvnDepth Depth
        {
            get { return _depth; }
            set { _depth = value; }
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
            get { return _singleSelection; }
            set { _singleSelection = value; }
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

        public delegate bool SelectableFilter(SvnItem item, SvnRevision from, SvnRevision to);

        public static bool EvaluateFilter(SvnItem item, Predicate<SvnItem> filter)
        {
            if (item == null)
                return false;
            if (filter == null)
                return true;

            foreach (Predicate<SvnItem> i in filter.GetInvocationList())
            {
                if (!i(item))
                    return false;
            }
            return true;
        }
    }
}
