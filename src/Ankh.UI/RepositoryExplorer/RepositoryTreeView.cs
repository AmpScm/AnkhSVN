// $Id$
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Windows.Forms;

using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using SharpSvn;
using System.Collections.ObjectModel;

namespace Ankh.UI.RepositoryExplorer
{
    /// <summary>
    /// Treeview that shows the layout of a SVN repository
    /// </summary>
    partial class RepositoryTreeView : TreeViewWithSelection<RepositoryTreeNode>
    {
        public RepositoryTreeView()
        {
            InitializeComponent();
            ShowRootLines = false;
        }

        IAnkhServiceProvider _context;

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
        }

        IFileIconMapper _iconMapper;

        IFileIconMapper IconMapper
        {
            get
            {
                if (_iconMapper == null)
                {
                    if (Context != null)
                        _iconMapper = Context.GetService<IFileIconMapper>();
                }
                return _iconMapper;
            }
        }

        RepositoryTreeNode _rootNode;

        protected RepositoryTreeNode RootNode
        {
            get { return _rootNode ?? (_rootNode = CreateRootNode()); }
        }

        private RepositoryTreeNode CreateRootNode()
        {
            RepositoryTreeNode rootNode;

            rootNode = new RepositoryTreeNode(null);
            rootNode.Text = RepositoryStrings.RootName;

            if (IconMapper != null)
                rootNode.IconIndex = IconMapper.GetSpecialIcon(SpecialIcon.Servers);

            SortedAddNode(Nodes, rootNode);

            return rootNode;
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (DesignMode)
                return;

            if (ImageList == null && IconMapper != null)
                ImageList = IconMapper.ImageList;

            GC.KeepAlive(RootNode);
        }

        public void AddRoot(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            if (ImageList == null && IconMapper != null)
                ImageList = IconMapper.ImageList;

            RepositoryTreeNode serverNode = EnsureServerOf(uri);

            if (!_rootNode.IsExpanded)
                _rootNode.Expand();

            if (!serverNode.IsExpanded)
                serverNode.Expand();

            BrowseRoot(serverNode, uri);
        }

        private RepositoryTreeNode EnsureServerOf(Uri uri)
        {
            Uri serverUri;
            RepositoryTreeNode serverNode = FindServer(uri, out serverUri);

            if (serverNode == null)
            {
                serverNode = new RepositoryTreeNode(serverUri);
                serverNode.Text = serverUri.ToString();

                if (IconMapper != null)
                    serverNode.IconIndex = IconMapper.GetSpecialIcon(SpecialIcon.Server);


                if (serverNode.Text.ToString() == "file:///")
                    serverNode.Text = RepositoryStrings.LocalRepositories;

                SortedAddNode(RootNode.Nodes, serverNode);

                if (!RootNode.IsExpanded)
                    RootNode.Expand();
            }
            return serverNode;
        }

        private void SortedAddNode(TreeNodeCollection nodeCollection, RepositoryTreeNode newNode)
        {
            int n = 0;
            foreach (RepositoryTreeNode tn in nodeCollection)
            {
                if (StringComparer.OrdinalIgnoreCase.Compare(tn.Text, newNode.Text) >= 0)
                {
                    nodeCollection.Insert(n, newNode);
                    return;
                }
                n++;
            }
            nodeCollection.Add(newNode);
        }

        private RepositoryTreeNode FindServer(Uri uri, out Uri serverUri)
        {
            serverUri = new Uri(uri.GetComponents(UriComponents.SchemeAndServer, UriFormat.UriEscaped));

            foreach (RepositoryTreeNode tn in RootNode.Nodes)
            {
                if (serverUri == tn.RawUri)
                {
                    return tn;
                }
            }
            return null;
        }

        int FolderIndex
        {
            get { return IconMapper.DirectoryIcon; }
        }

        delegate void DoSomething();


        readonly Dictionary<Uri, RepositoryTreeNode> _nodeMap = new Dictionary<Uri, RepositoryTreeNode>();

        public void BrowseRoot(RepositoryTreeNode parent, Uri uri)
        {
            uri = SvnTools.GetNormalizedUri(uri);

            RepositoryTreeNode itemNode;
            if (_nodeMap.TryGetValue(uri, out itemNode))
            {
                //itemNode.Select();
                itemNode.EnsureVisible();
                return;
            }

            BrowseItem(uri);
        }

        SvnDirEntryItems _retrieveItems = SvnDirEntryItems.SvnListDefault | SvnDirEntryItems.Kind | SvnDirEntryItems.Revision;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnDirEntryItems RetrieveItems
        {
            get { return _retrieveItems; }
            set { _retrieveItems = value | SvnDirEntryItems.Kind | SvnDirEntryItems.Revision; }
        }

        int nRunning;

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new RepositoryTreeNode SelectedNode
        {
            get { return base.SelectedNode as RepositoryTreeNode; }
            set { base.SelectedNode = value; }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                base.SelectedNode = GetNodeAt(e.X, e.Y);
            }

            base.OnMouseDown(e);
        }


        internal void BrowseItem(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            if (DesignMode)
                return;

            nRunning++;
            if (nRunning == 1)
                OnRetrievingChanged(EventArgs.Empty);

            DoSomething d = delegate()
            {
                bool ok = false;
                try
                {
                    SvnListArgs la = new SvnListArgs();
                    la.RetrieveEntries = RetrieveItems;
                    la.Depth = SvnDepth.Children;
                    la.ThrowOnError = false;

                    Collection<SvnListEventArgs> items;
                    using (SvnClient client = Context.GetService<ISvnClientPool>().GetClient())
                    {
                        client.GetList(uri, la, out items);
                    }

                    BeginInvoke((DoSomething)delegate()
                    {
                        if (items != null && items.Count > 0)
                        {
                            bool first = true;
                            foreach (SvnListEventArgs a in items)
                            {
                                if (first)
                                {
                                    if (a.RepositoryRoot != null)
                                        EnsureRoot(a.RepositoryRoot);
                                }

                                AddItem(a, first);
                                first = false;
                            }

                            MaybeExpand(uri);
                        }

                        nRunning--;

                        if (nRunning == 0)
                            OnRetrievingChanged(EventArgs.Empty);
                    });

                    ok = true;
                }
                finally
                {
                    if (!ok)
                        BeginInvoke((DoSomething)delegate()
                        {
                            nRunning--;

                            if (nRunning == 0)
                                OnRetrievingChanged(EventArgs.Empty);
                        });
                }
            };

            d.BeginInvoke(null, null);
        }

        [DefaultValue(false)]
        public new bool ShowRootLines
        {
            get { return base.ShowRootLines; }
            set { base.ShowRootLines = value; }
        }

        /// <summary>
        /// Occurs when the selected node was refreshed
        /// </summary>
        public event EventHandler SelectedNodeRefresh;

        /// <summary>
        /// Raises the <see cref="E:SelectedNodeRefresh"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        protected virtual void OnSelectedNodeRefresh(EventArgs e)
        {
            if (SelectedNodeRefresh != null)
                SelectedNodeRefresh(this, e);
        }

        private void MaybeExpand(Uri uri)
        {
            uri = SvnTools.GetNormalizedUri(uri);
            RepositoryTreeNode tn;
            if(_nodeMap.TryGetValue(uri, out tn))
            {
                if(tn.ExpandAfterLoad)
                    tn.LoadExpand();

                if (SelectedNode == tn)
                {
                    OnSelectedNodeRefresh(EventArgs.Empty);
                }
            }            
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.TreeView.BeforeExpand"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.TreeViewCancelEventArgs"/> that contains the event data.</param>
        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);

            if (!e.Cancel)
            {
                RepositoryTreeNode rtn = e.Node as RepositoryTreeNode;
                rtn.EnsureLoaded(true);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.TreeView.AfterExpand"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.TreeViewEventArgs"/> that contains the event data.</param>
        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            RepositoryTreeNode rtn = e.Node as RepositoryTreeNode;

            if (rtn != null)
            {
                rtn.EnsureLoaded(true);
            }
        }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);

            RepositoryTreeNode rtn = e.Node as RepositoryTreeNode;

            if (rtn != null)
            {
                rtn.EnsureLoaded(false);
            }
        }

        private RepositoryTreeNode EnsureRoot(Uri uri)
        {
            EnsureServerOf(uri);

            Uri serverUri;

            RepositoryTreeNode serverNode = FindServer(uri, out serverUri);

            if (serverNode == null)
                return null;

            foreach (RepositoryTreeNode reposRoot in serverNode.Nodes)
            {
                if (reposRoot.RawUri == uri)
                    return reposRoot;
            }

            RepositoryTreeNode rtn = new RepositoryTreeNode(uri);
            rtn.Text = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
            if (IconMapper != null)
                rtn.IconIndex = IconMapper.GetSpecialIcon(SpecialIcon.Db);

            serverNode.Nodes.Add(rtn);
            _nodeMap.Add(SvnTools.GetNormalizedUri(rtn.RawUri), rtn);

            if (!serverNode.IsExpanded)
                serverNode.Expand();

            return rtn;
        }

        private void AddItem(SvnListEventArgs item, bool first)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            Uri uri = item.EntryUri;

            Uri folderUri;

            if (item.Entry.NodeKind == SvnNodeKind.File)
                folderUri = new Uri(uri, "./");
            else
                folderUri = uri;

            RepositoryTreeNode s = EnsureFolderUri(folderUri);

            if (s != null)
            {
                s.AddItem(item);

                if(s.ExpandAfterLoad)
                    s.LoadExpand();
            }
        }

        internal static string UriItemName(Uri uri)
        {
            string v = uri.AbsoluteUri;

            if (v.EndsWith("/"))
                v = v.TrimEnd('/');

            int lS = v.LastIndexOf('/');

            if (lS >= 0)
                return Uri.UnescapeDataString(v.Substring(lS + 1));
            else
                return Uri.UnescapeDataString(v);
        }

        private RepositoryTreeNode EnsureFolderUri(Uri uri)
        {
            Uri nUri = SvnTools.GetNormalizedUri(uri);
            RepositoryTreeNode tn;

            if (!_nodeMap.TryGetValue(nUri, out tn))
            {
                Uri parentUri = new Uri(uri, "../");

                if (parentUri == uri)
                    return null;

                RepositoryTreeNode parent = EnsureFolderUri(parentUri);

                if (parent != null)
                {
                    tn = new RepositoryTreeNode(uri);
                    string name = uri.ToString();
                    int nS = name.LastIndexOf('/', name.Length - 2);

                    if (nS >= 0)
                        tn.Text = name.Substring(nS + 1, name.Length - nS - 2);
                    else
                        tn.Text = name;

                    tn.Text = Uri.UnescapeDataString(tn.Text); // Unescape special characters like '#' and ' '

                    if (IconMapper != null)
                        tn.IconIndex = IconMapper.DirectoryIcon;

                    _nodeMap.Add(nUri, tn);

                    tn.AddDummy();

                    SortedAddNode(parent.Nodes, tn);
                }
            }

            return tn;
        }

        public bool Retrieving
        {
            get { return nRunning > 0; }
        }

        public event EventHandler RetrievingChanged;
        private void OnRetrievingChanged(EventArgs e)
        {
            if (RetrievingChanged != null)
                RetrievingChanged(this, e);
        }

        protected override void OnResolveItem(ResolveItemEventArgs e)
        {
            e.Item = ((RepositoryExplorerItem)e.SelectionItem).TreeNode;
            base.OnResolveItem(e);
        }

        protected override void OnRetrieveSelection(TreeViewWithSelection<RepositoryTreeNode>.RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new RepositoryExplorerItem((RepositoryTreeNode)e.SelectionItem);
            base.OnRetrieveSelection(e);
        }
    }


}
