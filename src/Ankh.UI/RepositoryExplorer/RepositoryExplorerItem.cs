using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.Scc;
using SharpSvn;
using System.Diagnostics;

namespace Ankh.UI.RepositoryExplorer
{
    sealed class RepositoryExplorerItem : AnkhPropertyGridItem, ISvnRepositoryItem
    {
        readonly IAnkhServiceProvider _context;
        readonly RepositoryTreeNode _tn;
        readonly RepositoryListItem _li;        
        readonly string _name;
        readonly SvnOrigin _origin;

        public RepositoryExplorerItem(IAnkhServiceProvider context, SvnOrigin origin, RepositoryTreeNode tn)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (tn == null)
                throw new ArgumentNullException("tn");

            _context = context;
            _origin = origin;
            _tn = tn;
            _name = tn.Text;
        }

        public RepositoryExplorerItem(IAnkhServiceProvider context, SvnOrigin origin, RepositoryListItem li)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (li == null)
                throw new ArgumentNullException("li");

            _context = context;
            _origin = origin;
            _li = li;            
        }

        protected override string ComponentName
        {
            get { return Name; }
        }

        protected override string ClassName
        {
            get { return "Repository Item"; }
        }

        SvnListEventArgs Info
        {
            get
            {
                if (_li != null)
                    return _li.Info;
                else if (_tn != null)
                    return _tn.DirectoryItem;

                throw new InvalidOperationException();
            }
        }


        SvnDirEntry Entry
        {
            get
            {
                SvnListEventArgs info = Info;

                if(info != null)
                    return info.Entry;

                return null;
            }
        }

        [DisplayName("Url")]
        public Uri Uri
        {
            get 
            {
                if (_origin == null)
                    return null;
                return _origin.Uri; 
            }
        }

        [DisplayName("Name")]
        public string Name
        {
            get 
            {
                if (_origin == null)
                    return null;
                return _origin.Target.FileName; 
            }
        }

        [Category("Subversion"), DisplayName("Last Author")]
        public string LastAuthor
        {
            get
            {
                SvnDirEntry entry = Entry;
                if (entry != null)
                    return entry.Author;
                else
                    return null;
            }
        }  

        [Category("Subversion"), DisplayName("Last Revision")]
        public long LastRevision
        {
            get
            {
                SvnDirEntry entry = Entry;
                if (entry != null)
                    return entry.Revision;
                else
                    return -1;
            }
        }

        [Category("Subversion"), DisplayName("Last Committed")]
        public DateTime LastCommitted
        {
            get
            {
                SvnDirEntry entry = Entry;
                if (entry != null)
                    return entry.Time.ToLocalTime();
                else
                    return DateTime.MinValue;
            }
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
            get { return _origin; }
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
