// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using System.Collections.ObjectModel;
using Ankh.Scc;
using System.Diagnostics;

namespace Ankh.UI.RepositoryExplorer
{
    sealed class ListItemCollection : KeyedCollection<Uri, SvnListEventArgs>
    {
        protected override Uri GetKeyForItem(SvnListEventArgs item)
        {
            return item.EntryUri;
        }
    }

    sealed class RepositoryTreeNode : TreeNode
    {
        readonly Uri _uri;
        readonly SvnOrigin _origin;

        RepositoryTreeNode _dummy;
        ListItemCollection _items;
        bool _loaded;
        bool _expandAfterLoad;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryTreeNode"/> class.
        /// </summary>
        /// <param name="origin">The origin.</param>
        public RepositoryTreeNode(SvnOrigin origin)
        {
            if(origin == null)
                throw new ArgumentNullException("origin");

            _uri = origin.Uri;
            _origin = origin;
        }

        /// <summary>
        /// Initializes a dummy instance of the <see cref="RepositoryTreeNode"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="dummyNode">if set to <c>true</c> [dummy node].</param>
        internal RepositoryTreeNode(Uri uri, bool dummyNode)
        {
            Debug.Assert(dummyNode, "Must specify dummyNode");

            _uri = uri;
            _origin = null;
        }

        public int IconIndex
        {
            get { return ImageIndex; }
            set { ImageIndex = SelectedImageIndex = value; }
        }

        public Uri RawUri
        {
            get { return _uri; }
        }

        public SvnOrigin Origin
        {
            get { return _origin; }
        }

        public Uri NormalizedUri
        {
            get { return (_origin != null) ? ((SvnUriTarget) _origin.Target).Uri : null; }
        }

        public bool IsRepositoryPath
        {
            get { return (_origin != null); }
        }

        public Uri RepositoryRoot
        {
            get { return (_origin != null) ? _origin.RepositoryRoot : null; }
        }

        public new void Expand()
        {
            RemoveDummy();
            base.Expand();
        }

        public void LoadExpand()
        {
            if (Nodes.Count > 0)
            {
                _expandAfterLoad = false;
                if (_items != null && _items.Count > 0)
                    _loaded = true;
                Expand();
            }
        }

        new RepositoryTreeView TreeView
        {
            get { return (RepositoryTreeView)base.TreeView; }
        }

        internal void AddDummy()
        {
            if (Nodes.Count == 0 && _dummy == null)
            {
                _dummy = new RepositoryTreeNode(RawUri, true);
                _dummy.Name = _dummy.Text = "<dummy>";
                Nodes.Add(_dummy);
            }
        }

        internal bool ExpandAfterLoad
        {
            get { return _expandAfterLoad; }
        }

        internal void RemoveDummy()
        {
            if (_dummy != null)
            {
                Nodes.Remove(_dummy);
                _dummy = null;
            }
        }

        internal void EnsureLoaded(bool expandAfterLoading)
        {
            RemoveDummy();

            if (!_loaded)
            {
                if(RawUri != null && this.IsRepositoryPath)
                    TreeView.BrowseItem(RawUri);
                _loaded = true;
            }

            if (expandAfterLoading)
                _expandAfterLoad = true;
        }

        public ListItemCollection FolderItems
        {
            get { return _items ?? (_items = new ListItemCollection()); }
        }

        internal void AddItem(SharpSvn.SvnListEventArgs item)
        {
            if (FolderItems.Contains(item.EntryUri))
                FolderItems.Remove(item.EntryUri);

            FolderItems.Add(item);
        }

        public SvnListEventArgs DirectoryItem
        {
            get
            {
                if (FolderItems.Count > 0)
                    return FolderItems[0]; // First is folder itself

                return null;
            }
        }
    }
}
