using System;
using Ankh.UI;
using NSvn;
using NSvn.Core;

namespace Ankh.RepositoryExplorer
{
	/// <summary>
	/// Summary description for Controller.
	/// </summary>
	internal class Controller : IRepositoryTreeController
	{
        public event EventHandler RootChanged;

        public Controller( AnkhContext context )
        {
            this.context = context;
        }

        public string RootText
        {
            get
            {
                if ( this.rootNode != null ) 
                     return this.rootNode.Resource.Url + " @ " + this.rootNode.Resource.Revision.ToString();
                else 
                     return "";
            }
        }        

        public IRepositoryTreeNode RootNode
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.rootNode; }
        }

        public IRepositoryTreeView TreeView
        {
            set{ this.treeView = value; }
        }

        public void SetRepository( string url, Revision revision )
        {
            RepositoryDirectory dir = new RepositoryDirectory( url, revision );
            dir.Context = this.context.Context;

            this.rootNode = new Node( dir );
            this.OnRootChanged();
        }     
   
        /// <summary>
        /// Visits the nodes selected in the repository explorer treeview.
        /// </summary>
        /// <param name="visitor"></param>
        public void VisitSelectedNodes( IRepositoryResourceVisitor visitor )
        {
            if ( this.treeView.SelectionCount > 0 )
            {
                IRepositoryTreeNode[] nodes = this.treeView.SelectedNodes;
                foreach( Node node in nodes )
                {
                    node.Resource.Accept( visitor );
                }
            }
        }

        /// <summary>
        /// Dispatches the RootChanged event.
        /// </summary>
        protected virtual void OnRootChanged()
        {
            if ( this.RootChanged != null )
                this.RootChanged( this, EventArgs.Empty );
        }


        private Node rootNode;
        private AnkhContext context;
        private string rootText;		
        private IRepositoryTreeView treeView;
	}
}
