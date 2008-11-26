using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.UI.RepositoryExplorer
{
    sealed class RepositoryExplorerItem : AnkhPropertyGridItem, ISvnRepositoryItem
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

        protected override string ComponentName
        {
            get { return Name; }
        }

        protected override string ClassName
        {
            get { return "Repository Item"; }
        }

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

        [Browsable(false)]
        public SvnOrigin Origin
        {
            get
            {
                if (TreeNode != null)
                    return TreeNode.Origin;
                if (ListViewItem != null)
                    return ListViewItem.Origin;

                throw new InvalidOperationException();
            }
        }

        SharpSvn.SvnRevision ISvnRepositoryItem.Revision
        {
            get 
            {
                SvnOrigin origin = Origin;

                if (origin != null)
                    return origin.Target.Revision;

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
