using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using SharpSvn;
using EnvDTE;
using Utils;

using Ankh.ContextServices;
using Ankh.UI;
using Ankh.VS;
using Thread = System.Threading.Thread;
using Ankh.WorkingCopyExplorer;

namespace Ankh.RepositoryExplorer
{
    /// <summary>
    /// Responsible for controlling the repository explorer.
    /// </summary>
    public class Controller : AnkhService
    {
        static readonly NodeComparer NODECOMPARER = new NodeComparer();
        readonly Dictionary<string, INode[]> _directories;

        RepositoryExplorerControl repositoryExplorer;


        public Controller(IAnkhServiceProvider context)
            : base(context)
        {
            if(Shell != null && Shell.RepositoryExplorer != null)
                SetControl(Shell.RepositoryExplorer);


            this._directories = new Dictionary<string, INode[]>();
        }

        IExplorersShell _shell;
        IExplorersShell Shell
        {
            get { return _shell ?? (_shell = GetService<IExplorersShell>()); }
        }

        internal void SetControl(RepositoryExplorerControl value)
        {
            this.repositoryExplorer = Shell.RepositoryExplorer;

            if (repositoryExplorer != null)
            {
                this.repositoryExplorer.EnableBackgroundListingChanged +=
                    new EventHandler(this.BackgroundListingChanged);
                this.repositoryExplorer.NodeExpanding += new NodeExpandingDelegate(NodeExpanding);
                this.repositoryExplorer.SelectionChanged += new EventHandler(SelectionChanged);
                this.repositoryExplorer.AddRepoButtonClicked += new EventHandler(AddRepoButtonClicked);
            }
        }

        /// <summary>
        /// The selected node in the repository explorer.
        /// </summary>
        public INode SelectedNode
        {
            get
            {
                if (this.repositoryExplorer != null)
                    return (INode)this.repositoryExplorer.SelectedNode;
                else
                    return null;
            }
        }

        /// <summary>
        /// Start the create directory operation.
        /// </summary>
        /// <param name="handler"></param>
        public void MakeDir(INewDirectoryHandler handler)
        {
            this.repositoryExplorer.MakeDir(handler);
        }

        /// <summary>
        /// Add a new root node to the repository explorer.
        /// </summary>
        /// <param name="info"></param>
        public void AddRoot(RepositoryRootInfo info)
        {
            INode rootNode = new RootNode(info.Url, info.Revision);
            string label = String.Format("{0} [{1}]",
                rootNode.Url, rootNode.Revision);
            this.repositoryExplorer.AddRoot(label, rootNode);
        }

        public void RemoveRoot(INode node)
        {
            if (!(node is RootNode))
                throw new ArgumentException("Must be a root node");

            this.repositoryExplorer.RemoveRoot(node);
        }

        public bool IsRootNode(INode node)
        {
            return node is RootNode;
        }

        /// <summary>
        /// Forces a node to refresh.
        /// </summary>
        /// <param name="node"></param>
        public void Refresh(INode node)
        {
            // first invalidate our cache
            lock (this._directories)
            {
                this.Invalidate(node.Url);
            }

            this.repositoryExplorer.RefreshNode(node);
        }

        /// <summary>
        /// Recursively removes all children beneath a node.
        /// </summary>
        /// <param name="url"></param>
        private void Invalidate(string url)
        {
            INode[] children;
            if (_directories.TryGetValue(url, out children))
            {
                foreach (INode node in children)
                {
                    if (node.IsDirectory)
                        this.Invalidate(node.Url);
                }
            }
            // now remove the item
            this._directories.Remove(url);
        }

        /// <summary>
        /// Handles the event fired when a directory node is expanded and 
        /// the treeview does not have a listing for that directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void NodeExpanding(object sender, NodeExpandingEventArgs args)
        {
            try
            {
                INode parent = (INode)args.Node;
                INode[] children;

                // first see if it has been found by the background thread
                lock (this._directories)
                {
                    _directories.TryGetValue(parent.Url, out children);
                }

                if (children != null)
                {
                    Debug.WriteLine("Repository directory listing found in cache");
                }
                else
                {
                    // nope - we have to do the work ourselves
                    Debug.WriteLine("Repository directory listing *NOT* found in cache");

                    // we want to run this in a separate thread
                    ListRunner runner = new ListRunner(parent);
                    bool completed = GetService<IProgressRunner>().Run(
                        "Retrieving directory info.",
                        runner.Work).Succeeded;
                    if (completed)
                    {
                        ICollection<SvnListEventArgs> entries = runner.Entries;
                        children = new INode[entries.Count];
                        int i = 0;
                        foreach (SvnListEventArgs entry in entries)
                            children[i++] = new Node(parent, entry);
                    }

                    if (EnableBackgroundListing)
                        new BackgroundLister(children, this).Start();
                }

                // sort them nicely
                Array.Sort(children, Controller.NODECOMPARER);
                args.Children = children;
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex);
                else
                    throw;
            }
        }



//        /// <summary>
//        /// Load the stored roots in the config dir.
//        /// </summary>
//        private void LoadReposRoots()
//        {
//            IContext context = GetService<IContext>();

//            if (context == null)
//                return;
//            string[] roots;
//            try
//            {
//                roots = context.Configuration.LoadReposExplorerRoots();
//            }
//            catch (Ankh.Configuration.ConfigException ex)
//            {
//                string msg = ex.Message;
//                if (ex.InnerException != null)
//                    msg += Environment.NewLine + ex.InnerException.Message;

//                MessageBox.Show(context.GetService<IAnkhDialogOwner>().DialogOwner,
//                    @"Unable to load the %APPDATA%\AnkhSVN\reposroots.xml file."
//                    + Environment.NewLine +
//                    "The file may be corrupt. Edit it or delete it to have it recreated." +
//                    Environment.NewLine + Environment.NewLine +
//                    msg,
//                    "Unable to load repository roots");
//                return;
//            }

//            if (roots == null)
//                return;

//            foreach (string root in roots)
//            {
//                if (root == null)
//                    continue;
//                string[] components = root.Split('|');

//                // silently ignore invalid entries
//                INode node;
//                if (components.Length == 2)
//                    node = new RootNode(components[0], Parse(components[1]));
//                else if (components.Length == 1)
//                    node = new RootNode(components[0], SvnRevision.Head);
//                else
//                    continue;

//                string label = String.Format("{0} [{1}]", node.Url, node.Revision);
//#warning Needs fix!
//                //this.repositoryExplorer.AddRoot( label, node );
//            }
//        }

        static SvnRevision Parse(string s)
        {
            if (s.Equals("head", StringComparison.OrdinalIgnoreCase))
                return SvnRevision.Head;
            if (s.Equals("base", StringComparison.OrdinalIgnoreCase))
                return SvnRevision.Base;
            if (s.Equals("committed", StringComparison.OrdinalIgnoreCase))
                return SvnRevision.Committed;
            if (s.Equals("working", StringComparison.OrdinalIgnoreCase))
                return SvnRevision.Working;
            if (s.Equals("none", StringComparison.OrdinalIgnoreCase) ||
                s.Equals("unspecified", StringComparison.OrdinalIgnoreCase))
                return SvnRevision.None;
            if (s.Equals("previous", StringComparison.OrdinalIgnoreCase))
                return SvnRevision.Previous;

            long revision;
            if (long.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out revision))
                return new SvnRevision(revision);

            DateTime dt;
            if (DateTime.TryParse(s, out dt))
                return new SvnRevision(dt);

            throw new FormatException("Cannot parse string to valid revision object");
        }

        /// <summary>
        /// The background listing checkbox' state has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void BackgroundListingChanged(object sender, EventArgs args)
        {
            // Nothing to do
        }

        private object[] selection = new object[] { null };
        private void SelectionChanged(object sender, EventArgs e)
        {
            this.selection[0] = this.repositoryExplorer.SelectedNode;
            if (this.selection[0] != null)
                Shell.SetRepositoryExplorerSelection(selection);
        }

        private void AddRepoButtonClicked(object sender, EventArgs args)
        {
            RepositoryRootInfo info = Shell.ShowAddRepositoryRootDialog();
            if (info != null)
                AddRoot(info);
        }

        #region class ListRunner
        /// <summary>
        /// Used for running the list action in a separate thread.
        /// </summary>
        private class ListRunner
        {
            public ListRunner(INode node)
            {
                this.node = node;
            }

            /// <summary>
            /// The entries returned.
            /// </summary>
            public ICollection<SvnListEventArgs> Entries
            {
                get { return this.entries; }
            }

            public void Work(object sender, ProgressWorkerArgs e)
            {
                ReposListArgs args = new ReposListArgs();
                args.Depth = SvnDepth.Children;
                args.Revision = this.node.Revision;
                args.RetrieveEntries = SvnDirEntryItems.AllFieldsV15;
                e.Client.GetList(this.node.Url, args, out entries);
            }

            class ReposListArgs : SvnListArgs
            {
                protected override void OnList(SvnListEventArgs e)
                {
                    if (e.Entry != null && e.Entry.NodeKind == SvnNodeKind.Directory && string.IsNullOrEmpty(e.Path))
                        return;

                    e.Detach();
                    base.OnList(e);
                }
            }

            private INode node;
            private Collection<SvnListEventArgs> entries;
        }
        #endregion

        #region class BackgroundLister
        /// <summary>
        /// Used for doing a breadth first listing of a 
        /// repository recursively in the background.
        /// </summary>
        private class BackgroundLister
        {
            readonly Controller _parent;
            private Queue queue;
            private static int threadCount = 1;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="root">The parent directory for which to list children</param>
            /// <param name="parent"></param>
            public BackgroundLister(INode root, Controller parent)
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");
                _parent = parent;
                this.queue = new Queue(50);
                this.queue.Enqueue(root);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="children">An array of children. The thread
            /// will list the directories in this array.</param>
            /// <param name="parent"></param>
            public BackgroundLister(INode[] children, Controller parent)
            {
                this._parent = parent;
                this.queue = new Queue(50);
                foreach (INode node in children)
                    if (node.IsDirectory)
                        this.queue.Enqueue(node);
            }

            /// <summary>
            /// Call this method to start the thread.
            /// </summary>
            public void Start()
            {
                Thread thread = new Thread(new ThreadStart(this.Work));
                thread.Name = "Background lister " + BackgroundLister.threadCount;
                thread.Start();

                Debug.WriteLine("Starting " +
                    thread.Name, "Ankh");

                Interlocked.Increment(ref BackgroundLister.threadCount);
            }

            /// <summary>
            /// The worker method, which does the actual listing.
            /// </summary>
            private void Work()
            {
                using (SvnClient client = _parent.GetService<ISvnClientPool>().GetClient())
                {
                    // run as long as there are items in the queue or until the user
                    // cancels background listing
                    while (queue.Count > 0 && this._parent.EnableBackgroundListing)
                    {
                        INode node = (INode)queue.Dequeue();
                        Debug.WriteLine(Thread.CurrentThread.Name + " listing " + node.Url,
                            "Ankh");

                        SvnListArgs args = new SvnListArgs();
                        args.Revision = node.Revision;
                        args.Depth = SvnDepth.Empty;

                        List<INode> items = new List<INode>();
                        client.List(node.Url, args,
                            delegate(object sender, SvnListEventArgs e)
                            {
                                e.Detach();
                                Node n = new Node(node, e);
                                items.Add(n);

                                lock (this._parent._directories)
                                {
                                    if (n.IsDirectory && !this._parent._directories.ContainsKey(n.Url))
                                    {
                                        this.queue.Enqueue(n);
                                    }
                                }
                            });

                        lock (this._parent._directories)
                        {
                            this._parent._directories[node.Url] = items.ToArray();
                        }
                    }
                }
            }            
        }
        #endregion

        /// <summary>
        /// Used for ordering the items in the repository explorer.
        /// </summary>
        private class NodeComparer : System.Collections.IComparer
        {
            #region IComparer Members
            public int Compare(object x, object y)
            {
                IRepositoryTreeNode n1 = (IRepositoryTreeNode)x;
                IRepositoryTreeNode n2 = (IRepositoryTreeNode)y;

                // Directories first, alphabetically, then 
                // files, also alphabetically.
                if ((n1.IsDirectory && n2.IsDirectory) ||
                    (!n1.IsDirectory && !n2.IsDirectory))
                    return n1.Name.CompareTo(n2.Name);
                else if (n1.IsDirectory && !n2.IsDirectory)
                    return -1;
                else
                    return 1;
            }
            #endregion
        }

        protected bool EnableBackgroundListing
        {
            get
            {
                if (repositoryExplorer != null)
                    return repositoryExplorer.EnableBackgroundListing;
                return false;
            }
        }
    }
}
