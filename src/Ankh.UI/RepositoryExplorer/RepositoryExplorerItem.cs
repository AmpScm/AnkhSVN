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
        readonly IAnkhServiceProvider _context;
        readonly RepositoryTreeNode _tn;
        readonly RepositoryListItem _li;        
        readonly string _name;
        readonly Uri _uri;

        public RepositoryExplorerItem(IAnkhServiceProvider context, RepositoryTreeNode tn)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (tn == null)
                throw new ArgumentNullException("tn");

            _context = context;
            _tn = tn;
            _uri = tn.RawUri;
            _name = tn.Text;
        }

        public RepositoryExplorerItem(IAnkhServiceProvider context, RepositoryListItem li)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (li == null)
                throw new ArgumentNullException("li");

            _context = context;
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
            get 
            {
                if (ListViewItem != null)
                    return ListViewItem.Info.Entry.Revision;
                if (TreeNode != null)
                    return TreeNode.Revision;

                return null;
            }
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
        [Obsolete]
        public bool IsRepositoryItem
        {
            get { return RepositoryRoot != null; }
        }

        public Uri RepositoryRoot
        {
            get
            {
                if (TreeNode != null)
                    return TreeNode.RepositoryRoot;
                if (ListViewItem != null)
                    return ListViewItem.Info.RepositoryRoot;

                return null;
            }
        }
        #endregion

        private RepositoryExplorerControl GetRepositoryExplorerControl()
        {
            // TODO: Implement a better way to get to the repository explorer control
            // For now we just find the parent control        

            Control control = null;
            if (TreeNode != null)
            {
                control = this.TreeNode.TreeView;
            }
            else if (ListViewItem != null)
            {
                control = ListViewItem.ListView;
            }

            RepositoryExplorerControl rc = null;

            while (control != null && (null == (rc = control as RepositoryExplorerControl)))
                control = control.Parent;

            return rc;
        }
    }
}
