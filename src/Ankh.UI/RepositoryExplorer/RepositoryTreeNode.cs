using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using System.Collections.ObjectModel;

namespace Ankh.UI.RepositoryExplorer
{


    /// <summary>
    /// Represents a node in the tree.
    /// </summary>
    public interface IRepositoryTreeNode
    {
        /// <summary>
        /// The filename.
        /// </summary>
        string Name
        {
            get;
        }

        bool IsDirectory
        {
            get;
        }

        object Tag
        {
            get;
            set;
        }
    }

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

        internal void AddDummy()
        {
            if (Nodes.Count == 0 && _dummy == null)
            {
                _dummy = new RepositoryTreeNode(RawUri);
                _dummy.Name = _dummy.Text = "<dummy>";
                Nodes.Add(_dummy);
            }
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
