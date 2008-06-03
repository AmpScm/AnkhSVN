using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using System.Collections.ObjectModel;

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
        RepositoryTreeNode _dummy;
        ListItemCollection _items;
        bool _loaded;
        bool _expandAfterLoad;

        public RepositoryTreeNode(Uri uri)
        {
            _uri = uri;
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
                _dummy = new RepositoryTreeNode(RawUri);
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
                if(RawUri != null)
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
