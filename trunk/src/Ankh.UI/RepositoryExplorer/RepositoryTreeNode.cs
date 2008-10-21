﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using System.Collections.ObjectModel;
using Ankh.Scc;

namespace Ankh.UI.RepositoryExplorer
{
    sealed class ListItemCollection : KeyedCollection<Uri, SvnListEventArgs>
    {
        protected override Uri GetKeyForItem(SvnListEventArgs item)
        {
            return item.EntryUri;
        }
    }

    class RepositoryTreeNode : TreeNode
    {
        readonly Uri _uri;
        SvnOrigin _origin;
        long _revision;

        RepositoryTreeNode _dummy;
        ListItemCollection _items;
        bool _loaded;
        bool _expandAfterLoad;
        

        public RepositoryTreeNode(Uri uri, Uri repositoryUri, bool inRepository)
        {
            _uri = uri;

            if (inRepository)
            {
                _origin = new SvnOrigin(
                    SvnTools.GetNormalizedUri(uri),
                    repositoryUri);
            }
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
            get { return (_origin != null) ? _origin.Uri : null; }
        }

        public bool IsRepositoryPath
        {
            get { return (_origin != null); }
        }

        public Uri RepositoryRoot
        {
            get { return (_origin != null) ? _origin.RepositoryRoot : null; }
        }

        public long Revision
        {
            get { return _revision; }
            internal set 
            { 
                _revision = value;

                if (_origin != null)
                    _origin = new SvnOrigin(new SvnUriTarget(_origin.Uri, value), _origin.RepositoryRoot);
            }
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

        protected new RepositoryTreeView TreeView
        {
            get { return (RepositoryTreeView)base.TreeView; }
        }

        internal void AddDummy()
        {
            if (Nodes.Count == 0 && _dummy == null)
            {
                _dummy = new RepositoryTreeNode(RawUri, null, false);
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
    }
}
