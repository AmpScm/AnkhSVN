// $Id$
//
// Copyright 2003-2009 The AnkhSVN Project
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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Windows.Forms;

using Ankh.UI.VSSelectionControls;
using Ankh.VS;
using SharpSvn;
using System.Collections.ObjectModel;
using Ankh.Scc;
using Ankh.UI.RepositoryExplorer.Dialogs;
using Ankh.Commands;

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
            HideSelection = false;
        }

        IAnkhServiceProvider _context;

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;

                if (ImageList == null && IconMapper != null)
                {
                    ImageList = IconMapper.ImageList;

                    if (_rootNode != null)
                        _rootNode.IconIndex = IconMapper.GetSpecialIcon(SpecialIcon.Servers);
                }
            }
        }

        SvnRevision _revision = SvnRevision.Head;
        [Localizable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
        public SvnRevision SvnRevision
        {
            get { return _revision; }
            set { _revision = value ?? SvnRevision.Head; UpdateLabelEdit(); }
        }

        bool _allowRename;

        [DefaultValue(false), Localizable(false)]
        public bool AllowRenames
        {
            get { return _allowRename; }
            set { _allowRename = value; UpdateLabelEdit(); }
        }

        private void UpdateLabelEdit()
        {
            if (AllowRenames && SvnRevision == SvnRevision.Head)
                LabelEdit = true;
            else
                LabelEdit = false;
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

            rootNode = new RepositoryTreeNode(null, true);
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

        protected override string GetCanonicalName(RepositoryTreeNode item)
        {
            Uri uri = item.RawUri;

            if (uri != null)
                return uri.AbsoluteUri;
            else
                return null;
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

        public void RemoveRootOf(Uri uri)
        {
            if (this.Retrieving || uri == null) { return; }
            Uri serverUri;
            RepositoryTreeNode serverNode = FindServer(uri, out serverUri);
            if (serverNode != null)
            {
                CleanupCacheFor(serverUri, true);
                this.RootNode.Nodes.Remove(serverNode);
            }
        }

        private RepositoryTreeNode EnsureServerOf(Uri uri)
        {
            Uri serverUri;
            RepositoryTreeNode serverNode = FindServer(uri, out serverUri);

            if (serverNode == null)
            {
                serverNode = new RepositoryTreeNode(serverUri, true);
                serverNode.Text = serverUri.ToString();

                if (IconMapper != null)
                    serverNode.IconIndex = IconMapper.GetSpecialIcon(SpecialIcon.Server);


                if (serverNode.Text.ToString() == "file:///")
                {
                    serverNode.IconIndex = IconMapper.GetSpecialFolderIcon(Environment.SpecialFolder.MyComputer);
                    serverNode.Text = RepositoryStrings.LocalRepositories;
                }

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

        readonly Dictionary<Uri, RepositoryTreeNode> _nodeMap = new Dictionary<Uri, RepositoryTreeNode>();

        public void BrowseRoot(RepositoryTreeNode parent, Uri uri)
        {
            uri = SvnTools.GetNormalizedUri(uri);

            RepositoryTreeNode itemNode;
            if (_nodeMap.TryGetValue(uri, out itemNode))
            {
                SelectedNode = itemNode;
                itemNode.EnsureVisible();
                return;
            }

            BrowseTo(uri);
        }

        SvnDirEntryItems _retrieveItems = SvnDirEntryItems.SvnListDefault | SvnDirEntryItems.Kind | SvnDirEntryItems.Revision;
        [Localizable(false), DefaultValue(SvnDirEntryItems.SvnListDefault | SvnDirEntryItems.Kind | SvnDirEntryItems.Revision)]
        public SvnDirEntryItems RetrieveItems
        {
            get { return _retrieveItems; }
            set { _retrieveItems = value | SvnDirEntryItems.Kind | SvnDirEntryItems.Revision; }
        }

        bool _retrieveLocks;
        [Localizable(false), DefaultValue(false), Description("Boolean indicating whether to retrieve lock information")]
        public bool RetrieveLocks
        {
            get { return _retrieveLocks; }
            set { _retrieveLocks = value; }
        }

        List<Uri> _running = new List<Uri>();

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new RepositoryTreeNode SelectedNode
        {
            get { return base.SelectedNode as RepositoryTreeNode; }
            set { base.SelectedNode = value; }
        }

        string _expandTo;
        public void BrowseTo(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            Uri nUri;
            try
            {
                nUri = SvnTools.GetNormalizedUri(uri);
            }
            catch (UriFormatException)
            {
                return;
            }

            RepositoryTreeNode tn;
            if (_nodeMap.TryGetValue(nUri, out tn))
            {
                TreeNode parent = tn;
                while (parent != null)
                {
                    if (!parent.IsExpanded)
                        parent.Expand();
                    parent = parent.Parent;
                }
                SelectedNode = tn;
                tn.EnsureVisible();
                return;
            }

            if (uri.IsAbsoluteUri)
                _expandTo = SvnTools.GetNormalizedUri(uri).AbsoluteUri;
            else
                _expandTo = null;

            BrowseItem(uri);
        }

        internal void BrowseItem(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            if (DesignMode)
                return;

            Uri nUri = SvnTools.GetNormalizedUri(uri);

            if (_running.Contains(nUri))
                return;

            _running.Add(nUri);

            if (_running.Count == 1)
                OnRetrievingChanged(EventArgs.Empty);

            AnkhAction d = delegate()
            {
                bool ok = false;
                try
                {
                    SvnListArgs la = new SvnListArgs();
                    la.RetrieveEntries = RetrieveItems;
                    la.RetrieveLocks = RetrieveLocks;
                    la.Depth = SvnDepth.Children;
                    la.ThrowOnError = false;

                    Collection<SvnListEventArgs> items;
                    using (SvnClient client = Context.GetService<ISvnClientPool>().GetClient())
                    {
                        client.GetList(uri, la, out items);
                    }

                    AnkhAction addItems = (AnkhAction)delegate()
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

                            _running.Remove(nUri);

                            if (_running.Count == 0)
                                OnRetrievingChanged(EventArgs.Empty);
                        };

                    if (IsHandleCreated)
                        BeginInvoke(addItems);
                    else
                        addItems();

                    ok = true;
                }
                finally
                {
                    if (!ok)
                        BeginInvoke((AnkhAction)delegate()
                        {
                            _running.Remove(nUri);

                            if (_running.Count == 0)
                                OnRetrievingChanged(EventArgs.Empty);
                        });
                }
            };

            d.BeginInvoke(null, null);
        }

        public void Reload(Uri uri)
        {
            if (uri == null)
            {
                // This might be useful when repository roots are persisted across sessions
                // and might be used to trigger the re-read of the persisted repositories
            }
            else
            {
                Uri nUri = SvnTools.GetNormalizedUri(uri);
                RepositoryTreeNode tn = null;
                if (_nodeMap.TryGetValue(nUri, out tn))
                {
                    CleanupCacheFor(uri, false);
                    tn.Nodes.Clear();
                    this.BrowseItem(uri);
                }
            }
        }

        [DefaultValue(false)]
        public new bool ShowRootLines
        {
            get { return base.ShowRootLines; }
            set { base.ShowRootLines = value; }
        }

        [DefaultValue(false)]
        public new bool HideSelection
        {
            get { return base.HideSelection; }
            set { base.HideSelection = value; }
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
            if (_nodeMap.TryGetValue(uri, out tn))
            {
                if (tn.ExpandAfterLoad || IsLoading(uri))
                    tn.LoadExpand();

                if (SelectedNode == tn)
                {
                    OnSelectedNodeRefresh(EventArgs.Empty);
                }
            }
        }

        private bool IsLoading(Uri uri)
        {
            if (!uri.IsAbsoluteUri || string.IsNullOrEmpty(_expandTo))
                return false;

            string t = uri.AbsoluteUri;

            if (t.Length < _expandTo.Length)
            {
                if (!t.EndsWith("/"))
                    t += '/';

                if (_expandTo.StartsWith(t))
                    return true;
            }
            else if (t == _expandTo)
            {
                //_expandTo = null;
                return true;
            }

            return false;
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);

            TreeViewHitTestInfo hti = HitTest(PointToClient(MousePosition));

            RepositoryTreeNode rtn = hti.Node as RepositoryTreeNode;
            if (rtn != null && hti.Location != TreeViewHitTestLocations.None)
            {
                // Make sure the node is expanded after its loaded (Was set to false on first click)
                rtn.EnsureLoaded(true);
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

        /// <summary>
        /// The ensure the repository root
        /// </summary>
        /// <param name="uri">Always the repository root</param>
        /// <returns></returns>
        private RepositoryTreeNode EnsureRoot(Uri uri)
        {
            EnsureServerOf(uri);

            Uri serverUri;
            Uri nUri = SvnTools.GetNormalizedUri(uri);

            RepositoryTreeNode serverNode = FindServer(uri, out serverUri);

            if (serverNode == null)
                return null;

            if (!serverNode.IsExpanded && IsLoading(nUri))
                serverNode.LoadExpand();

            foreach (RepositoryTreeNode reposRoot in serverNode.Nodes)
            {
                if (reposRoot.NormalizedUri == nUri)
                    return reposRoot;
            }

            // uri is always the repos root here
            RepositoryTreeNode rtn = new RepositoryTreeNode(new SvnOrigin(uri, uri));
            rtn.Text = uri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped);
            if (IconMapper != null)
                rtn.IconIndex = IconMapper.GetSpecialIcon(SpecialIcon.Db);

            serverNode.Nodes.Add(rtn);

            if (!_nodeMap.ContainsKey(rtn.NormalizedUri))
                _nodeMap.Add(rtn.NormalizedUri, rtn);

            if (!serverNode.IsExpanded || IsLoading(nUri))
                serverNode.LoadExpand();

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

            RepositoryTreeNode s = EnsureFolderUri(folderUri, item.RepositoryRoot);

            if (s != null)
            {
                s.AddItem(item);

                if (s.ExpandAfterLoad)
                {
                    s.LoadExpand();

                    Uri nUri = SvnTools.GetNormalizedUri(folderUri);

                    if (IsLoading(nUri))
                    {
                        TreeNode tn = SelectedNode;
                        while (tn != null && tn != s)
                            tn = tn.Parent;

                        if (tn != s)
                        {
                            SelectedNode = s;
                            s.EnsureVisible();
                        }
                    }
                }
            }
        }

        private RepositoryTreeNode EnsureFolderUri(Uri uri, Uri repositoryUri)
        {
            Uri nUri = SvnTools.GetNormalizedUri(uri);
            RepositoryTreeNode tn;

            if (!_nodeMap.TryGetValue(nUri, out tn))
            {
                Uri parentUri = new Uri(uri, "../");

                if (parentUri == uri)
                    return null;

                RepositoryTreeNode parent = EnsureFolderUri(parentUri, repositoryUri);

                if (parent != null)
                {
                    tn = new RepositoryTreeNode(new SvnOrigin(uri, repositoryUri));
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

                    if (!parent.IsExpanded && IsLoading(nUri))
                        parent.LoadExpand();
                }
            }

            return tn;
        }

        [Browsable(false)]
        public bool Retrieving
        {
            get { return _running.Count > 0; }
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
            e.SelectionItem = new RepositoryExplorerItem(Context, e.Item.Origin, e.Item);
            base.OnRetrieveSelection(e);
        }

        private void CleanupCacheFor(Uri uri, bool includeBase)
        {
            if (uri == null)
                throw new InvalidOperationException();

            Uri nUri = SvnTools.GetNormalizedUri(uri);
            string start = nUri.AbsoluteUri;

            if (start[start.Length - 1] != '/')
                start += '/';

            List<Uri> removeUris = new List<Uri>();
            foreach (Uri cachedUri in _nodeMap.Keys)
            {
                if (cachedUri.MakeRelativeUri(nUri).IsAbsoluteUri)
                    continue;

                if (cachedUri.AbsoluteUri.StartsWith(start))
                    removeUris.Add(cachedUri);
                else if (includeBase && cachedUri == nUri)
                    removeUris.Add(cachedUri);
            }

            foreach (Uri removeUri in removeUris)
            {
                _nodeMap.Remove(removeUri);
            }

        }

        RepositoryExplorerItem _editItem;
        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            base.OnBeforeLabelEdit(e);

            _editItem = null;
            RepositoryTreeNode item = e.Node as RepositoryTreeNode;
            if (item == null || !AllowRenames || SvnRevision != SvnRevision.Head)
                e.CancelEdit = true;
            else
            {
                CancelEventArgs ce = new CancelEventArgs(e.CancelEdit);
                OnItemEdit(_editItem = new RepositoryExplorerItem(Context, item.Origin, item), ce);

                if (ce.Cancel)
                    e.CancelEdit = true;
            }
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            base.OnAfterLabelEdit(e);

            try
            {
                RepositoryTreeNode item = e.Node as RepositoryTreeNode;
                if (_editItem != null && _editItem.TreeNode == item)
                {
                    CancelEventArgs c = new CancelEventArgs();
                    OnAfterEdit(_editItem, e.Label, c);

                    if (c.Cancel)
                        e.CancelEdit = true;
                }
            }
            finally
            {
                _editItem = null;
            }
        }

        internal void OnItemEdit(RepositoryExplorerItem item, CancelEventArgs e)
        {
            if (!AllowRenames || SvnRevision != SvnRevision.Head || item.Origin == null
                || item.Origin.IsRepositoryRoot)
            {
                e.Cancel = true;
                return;
            }
            //e.Cancel = true;
        }

        internal void OnAfterEdit(RepositoryExplorerItem item, string newName, CancelEventArgs e)
        {
            if (!AllowRenames || SvnRevision != SvnRevision.Head || string.IsNullOrEmpty(newName))
            {
                e.Cancel = true;
                return;
            }

            Context.GetService<IAnkhCommandService>().DirectlyExecCommand(AnkhCommand.RenameRepositoryItem, new string[] { newName });
        }

        internal void DoCreateDirectory()
        {
            RepositoryTreeNode tn = SelectedNode;

            if (tn == null)
                return;

            Uri u = tn.RawUri;

            using (Ankh.UI.SccManagement.CreateDirectoryDialog dialog = new Ankh.UI.SccManagement.CreateDirectoryDialog())
            {
                if (dialog.ShowDialog(Context) != DialogResult.OK)
                    return;

                Uri newDir = SvnTools.AppendPathSuffix(u, dialog.NewDirectoryName);

                Context.GetService<IProgressRunner>().RunModal(
                    "Creating Directory",
                    delegate(object sender, ProgressWorkerArgs a)
                    {
                        SvnCreateDirectoryArgs args = new SvnCreateDirectoryArgs();
                        args.CreateParents = true;

                        args.LogMessage = dialog.LogMessage;
                        a.Client.RemoteCreateDirectory(newDir, args);
                    });

                AddRoot(newDir);
            }
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool CanCreateDirectory
        {
            get
            {
                RepositoryTreeNode tn = SelectedNode;
                return (tn != null) && (tn.NormalizedUri != null) && tn.IsRepositoryPath;
            }
        }
    }
}
