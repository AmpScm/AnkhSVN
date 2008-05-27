using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

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


    class RepositoryTreeNode : TreeNode
    {
        readonly Uri _uri;

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
    }
}
