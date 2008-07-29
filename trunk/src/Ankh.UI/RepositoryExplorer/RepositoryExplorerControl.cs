// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using SharpSvn;
using Ankh.UI.Services;
using Ankh.Ids;
using Ankh.UI.RepositoryExplorer;
using Ankh.VS;

namespace Ankh.UI.RepositoryExplorer
{
    /// <summary>
    /// Gives a tree view of the repository based on revision.
    /// </summary>
    public partial class RepositoryExplorerControl : AnkhToolWindowControl
    {
        IAnkhUISite _uiSite;

        public RepositoryExplorerControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            this.components = new Container();
            treeView.RetrieveItems = SvnDirEntryItems.Kind | SvnDirEntryItems.LastAuthor | SvnDirEntryItems.Revision | SvnDirEntryItems.Size | SvnDirEntryItems.Time;
        }

        protected override void OnFrameCreated(EventArgs e)
        {
            base.OnFrameCreated(e);

            ToolWindowSite.CommandContext = AnkhId.SccExplorerContextGuid;
            ToolWindowSite.KeyboardContext = AnkhId.SccExplorerContextGuid;
        }

        IAnkhUISite UISite
        {
            get { return _uiSite; }
        }

        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;
                if (value is IAnkhUISite)
                {
                    _uiSite = value as IAnkhUISite;

                    OnUISiteChanged(EventArgs.Empty);
                }
            }
        }

        private void OnUISiteChanged(EventArgs eventArgs)
        {
            treeView.Context = _uiSite;
            fileView.Context = _uiSite;
            treeView.SelectionPublishServiceProvider = _uiSite;
            fileView.SelectionPublishServiceProvider = _uiSite;
        }

        /// <summary>
        /// Add a new URL root to the tree.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="node"></param>
        public void AddRoot(Uri uri)
        {
            this.treeView.AddRoot(uri);
        }

        public Uri SelectedUri
        {
            get { return null; }
        }

        private void TreeViewMouseDown(object sender, MouseEventArgs e)
        {
            if (UISite == null || e.Button != MouseButtons.Right)
                return;

            Point screen = this.treeView.PointToScreen(new Point(e.X, e.Y));
            UISite.ShowContextMenu(AnkhCommandMenu.RepositoryExplorerContextMenu, screen.X, screen.Y);
        }

        IFileIconMapper _iconMapper;

        IFileIconMapper IconMapper
        {
            get
            {
                if (_iconMapper == null && UISite != null)
                    _iconMapper = UISite.GetService<IFileIconMapper>();

                return _iconMapper;
            }
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RefreshFileList();
        }

        void RefreshFileList()
        {
            fileView.Items.Clear();

            RepositoryTreeNode tn = treeView.SelectedNode as RepositoryTreeNode;

            if (tn != null)
            {
                foreach (RepositoryTreeNode sn in tn.Nodes)
                {
                    if (sn.FolderItems.Contains(sn.RawUri))
                    {
                        RepositoryListItem item = new RepositoryListItem(sn.FolderItems[sn.RawUri], IconMapper);

                        fileView.Items.Add(item);
                    }
                }
                foreach (SvnListEventArgs ee in tn.FolderItems)
                {
                    if (ee.EntryUri != tn.RawUri)
                    {
                        RepositoryListItem item = new RepositoryListItem(ee, IconMapper);

                        fileView.Items.Add(item);
                    }
                }

                if (fileView.Items.Count > 0)
                {
                    fileColumn.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
                }
            }
        }

        private void treeView_RetrievingChanged(object sender, EventArgs e)
        {
            busyProgress.Enabled = busyProgress.Visible = treeView.Retrieving;
        }

        private void treeView_SelectedNodeRefresh(object sender, EventArgs e)
        {
            RefreshFileList();
        }

        private void fileView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo ht = fileView.HitTest(e.X, e.Y);

            RepositoryListItem li = ht.Item as RepositoryListItem;

            if(ht.Location == ListViewHitTestLocations.None || li == null)
                return;

            if (li.Info.Entry.NodeKind == SvnNodeKind.Directory)
            {
                treeView.BrowseTo(li.RawUri);
            }
            else
            {
                // TODO: Perform default(?) action
            }
        }        
    }
}
