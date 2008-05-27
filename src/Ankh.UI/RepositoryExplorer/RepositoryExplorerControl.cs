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

namespace Ankh.UI
{
    /// <summary>
    /// Gives a tree view of the repository based on revision.
    /// </summary>
    public partial class RepositoryExplorerControl : System.Windows.Forms.UserControl
    {
        IAnkhUISite _uiSite;

        public RepositoryExplorerControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
 
            this.components = new Container();
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
            treeView.SelectionPublishServiceProvider = _uiSite;
        }     
        
        /// <summary>
        /// Fired when the selection changes.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// The selected node. Will be null if there is no selection.
        /// </summary>
        public IRepositoryTreeNode SelectedNode
        {
            get
            { 
                return 
                    this.treeView.SelectedNode != null ? 
                    this.treeView.SelectedNode.Tag as IRepositoryTreeNode :
                    null;
            }
        }

        /// <summary>
        /// The current roots in the treeview.
        /// </summary>
        public IRepositoryTreeNode[] Roots
        {
            get
            {
                ArrayList list = new ArrayList();
                foreach( TreeNode node in this.treeView.Nodes )
                    list.Add( node.Tag );
                return (IRepositoryTreeNode[])
                    list.ToArray(typeof(IRepositoryTreeNode));
            }
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

        private void TreeViewMouseDown( object sender, MouseEventArgs args )
        {
            if(UISite == null || args.Button != MouseButtons.Right )
                return;

            // make sure right click causes a selection
            this.treeView.SelectedNode = this.treeView.GetNodeAt( args.X, args.Y );

            Point screen = this.treeView.PointToScreen( new Point(args.X, args.Y) );

            UISite.ShowContextMenu(AnkhCommandMenu.RepositoryExplorerContextMenu, screen.X, screen.Y);
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ( this.SelectionChanged != null )
                this.SelectionChanged( this, EventArgs.Empty );
        }
        
        private void treeView_RetrievingChanged(object sender, EventArgs e)
        {
            busyProgress.Enabled = busyProgress.Visible = treeView.Retrieving;
        }
    }

    /// <summary>
    /// An object that handles the actual creation of a directory.
    /// </summary>
    public interface INewDirectoryHandler
    {
        bool MakeDir( IRepositoryTreeNode parent, string dirname );
    }
}
