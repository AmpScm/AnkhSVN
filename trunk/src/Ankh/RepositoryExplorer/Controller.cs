using System;
using Ankh.UI;

using NSvn.Core;

namespace Ankh.RepositoryExplorer
{
    /// <summary>
    /// Responsible for controlling the repository explorer.
    /// </summary>
    internal class Controller
    {
        public Controller( AnkhContext context, 
            RepositoryExplorerControl repositoryExplorer )
        {
            this.repositoryExplorer = repositoryExplorer;
            this.context = context;

            this.repositoryExplorer.GoClicked += new EventHandler(GoClicked);
            this.repositoryExplorer.NodeExpanding += new NodeExpandingDelegate(NodeExpanding);
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

                // we want to run this in a separate thread
                ListRunner runner = new ListRunner( parent, this.context );
                runner.Start( "Retrieving directory info." );
                if ( !runner.Cancelled )
                {
                    DirectoryEntry[] entries = runner.Entries;
                    INode[] children = new INode[entries.Length];
                    int i = 0;
                    foreach( DirectoryEntry entry in entries )
                        children[i++] = new Node( parent, entry );

                    Array.Sort(children, Controller.NODECOMPARER);
                    args.Children = children;
                }
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
        private void GoClicked(object sender, EventArgs args )
        {
            string url = this.repositoryExplorer.Url;
            Revision revision = this.repositoryExplorer.Revision;
            this.repositoryExplorer.AddRoot( url, new RootNode(url, revision) );
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
        RepositoryExplorerControl repositoryExplorer;
        AnkhContext context;
    }
}
