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

            this.AddToolBarImages();
           
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
        public void AddRoot( string text, IRepositoryTreeNode node)
        {
            if ( !node.IsDirectory )
                throw new ArgumentException( "The root needs to be a directory.", "node" );

            this.treeView.AddRoot( node, text );
        }

        /// <summary>
        /// Removes the root of a given node.
        /// </summary>
        /// <param name="node"></param>
        public void RemoveRoot( IRepositoryTreeNode node )
        {
            TreeNode root = (TreeNode)node.Tag;
            while( root.Parent != null )
                root = root.Parent;
            root.Remove();
        }

        /// <summary>
        /// Refreshes the node.
        /// </summary>
        /// <param name="node"></param>
        public void RefreshNode( IRepositoryTreeNode node )
        {
            this.treeView.RefreshNode( node );
        }

        /// <summary>
        /// Start the "create a new dir" operation under the current node.
        /// </summary>
        /// <param name="handler"></param>
        public void MakeDir( INewDirectoryHandler handler )
        {
            if ( this.treeView.SelectedNode == null )
                return;

            // store the handler so we have it in the callback
            this.newDirHandler = handler;
            this.treeView.AfterLabelEdit += new NodeLabelEditEventHandler(MakeDirAfterEdit);

            // create a new node.
            TreeNode node = new TreeNode( "Newdir" );
            //node.SelectedImageIndex = node.ImageIndex = this.treeView.FolderIndex;
            this.treeView.SelectedNode.Nodes.Add( node );

            // start editing it
            this.treeView.LabelEdit = true;
            node.BeginEdit();
        }

        private void AddToolBarImages()
        {
            /*this.toolbarImageList.Images.Add(
                new Icon( this.GetType().Assembly.GetManifestResourceStream(
                "Ankh.UI.Graphics.EnableBackgroundListing.ico" ) ) );
            this.toolbarImageList.Images.Add(
                new Icon(this.GetType().Assembly.GetManifestResourceStream( 
                "Ankh.UI.Graphics.AddRepoURL.ico") ) );*/
        }

        

        /// <summary>
        /// This will be called when the user finishes editing the new node.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MakeDirAfterEdit(object sender, NodeLabelEditEventArgs e)
        {
            // disconnect the event handler - we don't want it fired more than once.
            this.treeView.AfterLabelEdit -= new NodeLabelEditEventHandler(
                this.MakeDirAfterEdit );
            this.treeView.LabelEdit = false;

            bool cancel = true;
            try
            {
                // did the user actually enter a name?
                if ( this.newDirHandler != null && e.Label != null )
                {  
                    // create the new dir and refresh the parent dir.
                    cancel = !this.newDirHandler.MakeDir(
                        (IRepositoryTreeNode)e.Node.Parent.Tag, e.Label );
                    if ( !cancel )
                        this.RefreshNode( (IRepositoryTreeNode)e.Node.Parent.Tag );
                }
                // if the user cancelled, just get rid of the new node.
                if ( cancel ) 
                    e.Node.Remove();
            }
            finally
            {
                this.newDirHandler = null;
                e.CancelEdit = cancel;
            }
        }
        
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
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
        
        private INewDirectoryHandler newDirHandler;

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
