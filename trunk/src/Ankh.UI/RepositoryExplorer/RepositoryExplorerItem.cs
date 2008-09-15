using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.UI.RepositoryExplorer
{
    sealed class RepositoryExplorerItem : CustomTypeDescriptor, ISvnRepositoryItem
    {
        RepositoryTreeNode _tn;
        RepositoryListItem _li;
        string _name;
        Uri _uri;

        public RepositoryExplorerItem(RepositoryTreeNode tn)
        {
            if (tn == null)
                throw new ArgumentNullException("tn");

            _tn = tn;
            _uri = tn.RawUri;
            _name = tn.Text;
        }

        public RepositoryExplorerItem(RepositoryListItem li)
        {
            if (li == null)
                throw new ArgumentNullException("li");

            _li = li;
            _uri = li.RawUri;
            _name = li.Text;
        }

        #region Property View Conversion
        TypeConverter _rawDescriptor;
        TypeConverter Raw
        {
            get { return _rawDescriptor ?? (_rawDescriptor = TypeDescriptor.GetConverter(this, true)); }
        }

        public override PropertyDescriptorCollection GetProperties()
        {
            return Raw.GetProperties(this);
        }

        public override TypeConverter GetConverter()
        {
            return Raw;
        }

        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return Raw.GetProperties(null, null, attributes);
        }

        public override string ToString()
        {
            return Name;
        }

        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }

        public override string GetComponentName()
        {
            return Name;
        }

        public override string GetClassName()
        {
            return "Repository Item";
        }
        #endregion

        [DisplayName("Url")]
        public Uri Uri
        {
            get { return _uri; }
        }

        [DisplayName("File Name")]
        public string Name
        {
            get { return _name; }
        }

        [Browsable(false)]
        internal RepositoryListItem ListViewItem
        {
            get { return _li; }
        }

        [Browsable(false)]
        internal RepositoryTreeNode TreeNode
        {
            get { return _tn; }
        }

        #region ISvnRepositoryItem Members

        SharpSvn.SvnNodeKind ISvnRepositoryItem.NodeKind
        {
            get
            {
                Uri uri = Uri;

                if (uri != null)
                {
                    if (uri.ToString().EndsWith("/"))
                        return SvnNodeKind.Directory;
                    else
                        return SvnNodeKind.File;
                }

                return SvnNodeKind.Unknown;
            }
        }

        SharpSvn.SvnRevision ISvnRepositoryItem.Revision
        {
            get { return null; }
        }

        public void RefreshItem(bool refreshParent)
        {
            if (this.TreeNode != null)
            {
                Uri uri = null;
                if (refreshParent && this.TreeNode.Parent is RepositoryTreeNode)
                {
                    uri = ((RepositoryTreeNode)this.TreeNode.Parent).RawUri;
                }
                else
                {
                    uri = this.TreeNode.RawUri;
                }
                if (uri != null
                    && this.TreeNode.TreeView is RepositoryTreeView)
                {
                    RepositoryTreeView rtv = (RepositoryTreeView)this.TreeNode.TreeView;
                    rtv.Reload(uri);
                }
            }
            else if (this.ListViewItem != null)
            {
                RepositoryExplorerControl rec = GetRepositoryExplorerControl();
                if (rec != null)
                {
                    Uri uri = new Uri(this.Uri, "../"); // parent uri
                    rec.Reload(uri);
                }
            }
        }

        [Browsable(false)]
        public bool IsRepositoryItem
        {
            get
            {
                if (this._uri == null) { return false; }
                if (this._tn != null)
                {
                    return this._tn.IsRepositoryPath;
                }
                return true;
            }
        }
        #endregion

        //TODO better way to get to the repository explorer control
        private RepositoryExplorerControl GetRepositoryExplorerControl()
        {
            IAnkhServiceProvider ctx = null;
            if (this.TreeNode != null)
            {
                ctx = ((RepositoryTreeView)this.TreeNode.TreeView).Context;
            }
            else if (this.ListViewItem != null)
            {
                ctx = ((RepositoryListView)this.ListViewItem.ListView).Context;
            }

            if (ctx is IAnkhToolWindowHost
                && ((IAnkhToolWindowHost)ctx).Pane is Microsoft.VisualStudio.Shell.WindowPane)
            {
                Microsoft.VisualStudio.Shell.WindowPane pane = (Microsoft.VisualStudio.Shell.WindowPane)((IAnkhToolWindowHost)ctx).Pane;
                if (pane.Window is RepositoryExplorerControl)
                {
                    return (RepositoryExplorerControl)pane.Window;
                }
            }
            return null;
        }
    }
}
