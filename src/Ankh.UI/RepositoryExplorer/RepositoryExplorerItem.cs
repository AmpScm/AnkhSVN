// Copyright 2008-2009 The AnkhSVN Project
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

        internal SvnDirEntry Entry
        {
            get
            {
                if (_li != null)
                    return _li.Entry;
                else if (_tn != null)
                    return _tn.DirectoryEntry;

                throw new InvalidOperationException();
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

        [DisplayName("File Name")]
        public string Name
        {
            get 
            {
                if (_origin == null)
                    return null;
                return _origin.Target.FileName; 
            }
            set
            {
                RepositoryTreeView tv = null;

                if (TreeNode != null)
                    tv = TreeNode.TreeView as RepositoryTreeView;
                else if (ListViewItem != null && ListViewItem.ListView != null)
                    tv = ListViewItem.ListView.RepositoryTreeView;

                if (tv != null)
                {
                    CancelEventArgs c = new CancelEventArgs();

                    tv.OnItemEdit(this, c);

                    if (!c.Cancel)
                        tv.OnAfterEdit(this, value, c);
                }
            }
        }

        [Category("Subversion"), DisplayName("Last Author")]
        [Description("Author of the Last Commit")]
        public string LastCommittedAuthor
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
        [Description("Revision number of the Last Commit")]
        public long LastCommittedRevision
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
        [Description("Time of the Last Commit")]
        public DateTime LastCommittedDate
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
            if (TreeNode != null)
            {
                Uri uri = null;
                if (refreshParent && null != TreeNode.Parent && null != TreeNode.Origin)
                {
                    uri = ((RepositoryTreeNode)this.TreeNode.Parent).NormalizedUri;
                }
                else
                {
                    uri = TreeNode.NormalizedUri;
                }
                if (uri != null
                    && this.TreeNode.TreeView is RepositoryTreeView)
                {
                    RepositoryTreeView rtv = (RepositoryTreeView)this.TreeNode.TreeView;
                    rtv.Reload(uri);
                }
            }
            else if (ListViewItem != null)
            {
                RepositoryExplorerControl rec = GetRepositoryExplorerControl();
                if (rec != null)
                {
                    // This always reloads the parent and its children (not only when requested)
                    Uri uri = new Uri(SvnTools.GetNormalizedUri(this.Uri), "./"); // parent uri
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
