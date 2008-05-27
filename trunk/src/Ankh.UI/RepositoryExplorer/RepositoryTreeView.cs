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
        public void AddRoot(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            if(ImageList == null && IconMapper != null)
                ImageList = IconMapper.ImageList;

            if (Nodes.Count == 0)
            {
                _rootNode = new RepositoryTreeNode(null);
                _rootNode.Text = RepositoryStrings.RootName;

                if (IconMapper != null)
                    _rootNode.IconIndex = IconMapper.GetSpecialIcon(SpecialIcon.Servers);

                SortedAddNode(Nodes, _rootNode);
            }

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

                SortedAddNode(_rootNode.Nodes, serverNode);
            }

            if(!_rootNode.IsExpanded)
                _rootNode.Expand();

            if (!serverNode.IsExpanded)
                serverNode.Expand();

            BrowseRoot(serverNode, uri);
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

            foreach (RepositoryTreeNode tn in _rootNode.Nodes)
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
            // uri = SvnTools.NormalizeUri(uri); // TODO: Implement in SharpSvn

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
        public SvnDirEntryItems RetrieveItems
        {
            get { return _retrieveItems; }
            set { _retrieveItems = value | SvnDirEntryItems.Kind | SvnDirEntryItems.Revision; }
        }

        int nRunning;

        private void BrowseItem(Uri uri)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            nRunning++;
            if (nRunning == 1)
                OnRetrievingChanged(EventArgs.Empty);
            
            DoSomething d = delegate()
            {
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

                    if (items != null && items.Count > 0)
                        Invoke((DoSomething)delegate()
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
                        });
                }
                finally
                {
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
        
        private RepositoryTreeNode EnsureRoot(Uri uri)
        {
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
            _nodeMap.Add(rtn.RawUri, rtn);

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

            if (first)
            {
                EnsureFolderUri(folderUri);
            }
        }

        private RepositoryTreeNode EnsureFolderUri(Uri uri)
        {
            RepositoryTreeNode tn;

            if (!_nodeMap.TryGetValue(uri, out tn))
            {
                Uri parentUri = new Uri(uri, "../");

                if (parentUri == uri)
                    return null;

                RepositoryTreeNode parent = EnsureFolderUri(parentUri);

                if (parent != null)
                {
                    tn = new RepositoryTreeNode(uri);
                    string name = uri.ToString();
                    int nS = name.LastIndexOf('/', name.Length - 1);

                    if (nS >= 0)
                        tn.Text = name.Substring(nS, name.Length - nS - 1);
                    else
                        tn.Text = name;

                    if (IconMapper != null)
                        tn.IconIndex = IconMapper.DirectoryIcon;

                    _nodeMap.Add(tn.RawUri, tn);
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
            if(RetrievingChanged != null)
                RetrievingChanged(this, e);
        }
     
        public void AddRoot( IRepositoryTreeNode node, string label )
        {
            AddRoot(new Uri(node.Name));
            /*TreeNode root = new TreeNode( label, this.FolderIndex, this.FolderIndex );

            root.Tag = node;
            node.Tag = root;          

            this.Nodes.Add( root );

            TreeNode dummy = new TreeNode("");
            dummy.Tag = DUMMY_NODE;

            root.Nodes.Add( dummy );*/
        }
        
        /// <summary>
        /// Refresh the contents of this node.
        /// </summary>
        /// <param name="n"></param>
        public void RefreshNode( IRepositoryTreeNode n )
        {
            // get rid of the subnodes
            TreeNode node = (TreeNode)n.Tag;
            node.Nodes.Clear();

            // now add the dummy child.
            TreeNode dummy = new TreeNode();
            dummy.Tag = DUMMY_NODE;
            node.Nodes.Add( dummy );

            // make sure it gets refilled.
            node.Collapse();
            node.Expand();
        }

/*        private void BuildSubTree( TreeNodeCollection nodes, IList nodeList )
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                // we have set a new root, so get rid of any existing nodes
                nodes.Clear();

                foreach( IRepositoryTreeNode child in nodeList )
                {
                    TreeNode newNode;

                    if ( child.IsDirectory )
                    {
                        TreeNode dummy = new TreeNode( "" );
                        dummy.Tag = DUMMY_NODE;

                        newNode = new TreeNode( child.Name, 
                            new TreeNode[]{ dummy } );

                        // start with the closed folder icon
                        newNode.SelectedImageIndex = newNode.ImageIndex = this.FolderIndex;
                    }
                    else
                    {
                        newNode = new TreeNode( child.Name);

                        // set the icon
                        this.SetIcon( newNode, child.Name );
                    }

                    newNode.Tag = child;
                    child.Tag = newNode;
                    nodes.Add( newNode );

                } // foreach

            }
            catch( ApplicationException )
            {
                this.Nodes.Clear();
                this.Nodes.Add( new TreeNode( "An error occurred",  
                    this.FolderIndex, this.FolderIndex ) );
            }
            finally
            {           
                this.Cursor = Cursors.Default;
            }
        }*/

        public void SetIcon(TreeNode node, string name)
        {
            if (IconMapper != null)
                node.SelectedImageIndex = node.ImageIndex = IconMapper.GetIconForExtension(Path.GetExtension(name));            
        }

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        public static readonly object DUMMY_NODE = new object();
    }

    
}
