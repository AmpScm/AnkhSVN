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

        public string RootText
        {
            get{ return this.rootNode != null ? this.rootNode.Resource.Url : "";  }
        }        

        public IRepositoryTreeNode RootNode
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.rootNode; }
        }

        public void SetRepository( string url, Revision revision )
        {
            this.rootNode = new Node( new RepositoryDirectory( url, revision ) );
            this.OnRootChanged();
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
        private string rootText;		
	}
}
