using System;
using Ankh.UI;
using System.Collections;
using System.Threading;
using NSvn.Core;
using System.Diagnostics;
using EnvDTE;

using Thread = System.Threading.Thread;

namespace Ankh.RepositoryExplorer
{
    /// <summary>
    /// Responsible for controlling the repository explorer.
    /// </summary>
    internal class Controller
    {
        public Controller( AnkhContext context, 
            RepositoryExplorerControl repositoryExplorer, Window window )
        {
            this.repositoryExplorer = repositoryExplorer;
            this.enableBackgroundListing = repositoryExplorer.EnableBackgroundListing;

            this.context = context;
            this.window = window;

            this.repositoryExplorer.EnableBackgroundListingChanged += 
                new EventHandler( this.BackgroundListingChanged );
            this.repositoryExplorer.AddClicked += new EventHandler(AddClicked);
            this.repositoryExplorer.NodeExpanding += new NodeExpandingDelegate(NodeExpanding);
            this.repositoryExplorer.SelectionChanged +=new EventHandler(SelectionChanged);
            
            this.directories = new Hashtable();
        }

        /// <summary>
        /// The selected node in the repository explorer.
        /// </summary>
        public INode SelectedNode
        {
            get{ return (INode)this.repositoryExplorer.SelectedNode; }
        }

        /// <summary>
        /// The command bar associated with the repository explorer.
        /// </summary>
        public Microsoft.Office.Core.CommandBar CommandBar
        {
            get{ return this.repositoryExplorer.CommandBar; }
            set{ this.repositoryExplorer.CommandBar = value; }
        }

        /// <summary>
        /// Start the create directory operation.
        /// </summary>
        /// <param name="handler"></param>
        public void MakeDir( INewDirectoryHandler handler )
        {
            this.repositoryExplorer.MakeDir( handler );
        }

        /// <summary>
        /// Forces a node to refresh.
        /// </summary>
        /// <param name="node"></param>
        public void Refresh( INode node )
        {
            // first invalidate our cache
            lock( this.directories )
            {
                this.Invalidate( node.Url );
            }

            this.repositoryExplorer.RefreshNode( node );
        }

        /// <summary>
        /// Recursively removes all children beneath a node.
        /// </summary>
        /// <param name="url"></param>
        private void Invalidate( string url )
        {
            INode[] children = (INode[])this.directories[url];
            if ( children != null )
            {
                foreach( INode node in children )
                {
                    if ( node.IsDirectory )
                        this.Invalidate( node.Url );
                }
            }
            // now remove the item
            this.directories.Remove( url );
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
                lock( this.directories )
                    children = (INode[])this.directories[parent.Url];

                if ( children != null )
                {
                    Debug.WriteLine( "Repository directory listing found in cache" );
                }
                else
                {
                    // nope - we have to do the work ourselves
                    Debug.WriteLine( "Repository directory listing *NOT* found in cache" );

                    // we want to run this in a separate thread
                    ListRunner runner = new ListRunner( parent, this.context );
                    runner.Start( "Retrieving directory info." );
                    if ( !runner.Cancelled )
                    {
                        DirectoryEntry[] entries = runner.Entries;
                        children = new INode[entries.Length];
                        int i = 0;
                        foreach( DirectoryEntry entry in entries )
                            children[i++] = new Node( parent, entry );
                    }

                    if ( this.enableBackgroundListing )
                        new BackgroundLister( children, this ).Start();
                }

                // sort them nicely
                Array.Sort(children, Controller.NODECOMPARER);
                args.Children = children;
            }
            catch( Exception ex )
            {
                Error.Handle(ex);
                throw;
            }
        }

        /// <summary>
        /// The user wants to go to an URL.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AddClicked(object sender, EventArgs args )
        {
            string url = this.repositoryExplorer.Url;
            Revision revision = this.repositoryExplorer.Revision;
            INode rootNode = new RootNode(url, revision);
            this.repositoryExplorer.AddRoot( url, rootNode );
        }


        /// <summary>
        /// The background listing checkbox' state has changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void BackgroundListingChanged( object sender, EventArgs args )
        {
            this.enableBackgroundListing = this.repositoryExplorer.EnableBackgroundListing;
        }

        private object[] selection = new object[]{ null };
        private void SelectionChanged(object sender, EventArgs e)
        {
            this.selection[0] = this.repositoryExplorer.SelectedNode;
            this.window.SetSelectionContainer( ref this.selection );
        }

        /// <summary>
        /// Used for running the list action in a separate thread.
        /// </summary>
        private class ListRunner : ProgressRunner
        {
            public ListRunner( INode node, AnkhContext context ) : 
                base(context)
            {
                this.node = node;
            }

            /// <summary>
            /// The entries returned.
            /// </summary>
            public DirectoryEntry[] Entries
            {
                get{ return this.entries; }
            }

            protected override void DoRun()
            {
                this.entries = this.Context.Client.List( this.node.Url, 
                    this.node.Revision, false );
            }

            private INode node;
            private DirectoryEntry[] entries;
        }

        /// <summary>
        /// Used for doing a breadth first listing of a 
        /// repository recursively in the background.
        /// </summary>
        private class BackgroundLister
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="root">The parent directory for which to list children</param>
            /// <param name="parent"></param>
            public BackgroundLister( INode root, Controller parent )
            {
                this.parent = parent;
                this.queue = new Queue( 50 );
                this.queue.Enqueue( root );
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="children">An array of children. The thread
            /// will list the directories in this array.</param>
            /// <param name="parent"></param>
            public BackgroundLister( INode[] children, Controller parent )
            {
                this.parent = parent;
                this.queue = new Queue( 50 );
                foreach ( INode node in children )
                    if ( node.IsDirectory )
                        this.queue.Enqueue(node);
            }

            /// <summary>
            /// Call this method to start the thread.
            /// </summary>
            public void Start()
            {
                Thread thread = new Thread( new ThreadStart(this.Work) );
                thread.Name = "Background lister " + BackgroundLister.threadCount;
                thread.Start();

                Debug.WriteLine( "Starting " + 
                    thread.Name, "Ankh" );

                Interlocked.Increment( ref BackgroundLister.threadCount );
            }

            /// <summary>
            /// The worker method, which does the actual listing.
            /// </summary>
            private void Work()
            {
                // run as long as there are items in the queue or until the user
                // cancels background listing
                while( queue.Count > 0 && this.parent.enableBackgroundListing )
                {
                    INode node = (INode)queue.Dequeue();
                    Debug.WriteLine( Thread.CurrentThread.Name + " listing " + node.Url, 
                        "Ankh" );
                    DirectoryEntry[] entries = 
                        this.parent.context.Client.List( 
                        node.Url, node.Revision, false );
                    INode[] children = new INode[entries.Length];
                    for( int i=0; i < entries.Length; i++ )
                    {
                        children[i] = new Node( node, entries[i] );

                        // we put the directories on the queue
                        lock( this.parent.directories )
                        {
                            if ( children[i].IsDirectory &&
                                !this.parent.directories.Contains(
                                children[i].Url )                                 
                                )
                            {
                                this.queue.Enqueue( children[i] );
                            }
                        }
                    }

                    // store the list in the hashtable
                    lock( this.parent.directories )
                        this.parent.directories[ node.Url ] = children;
                }
            }

            private Controller parent;
            private Queue queue;
            private static int threadCount = 1;
        }

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
                if( (n1.IsDirectory && n2.IsDirectory) || 
                    (!n1.IsDirectory && !n2.IsDirectory) )
                    return n1.Name.CompareTo(n2.Name);
                else if ( n1.IsDirectory && !n2.IsDirectory )
                    return -1;
                else 
                    return 1;
            }
            #endregion
        }


        private static readonly NodeComparer NODECOMPARER = new NodeComparer();
        private RepositoryExplorerControl repositoryExplorer;
        private Hashtable directories;
        private AnkhContext context;
        private Window window;

        private bool enableBackgroundListing = false;

        
    }
}
