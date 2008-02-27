// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Utils;
using System.Text.RegularExpressions;
using System.Diagnostics;
using Utils.Win32;
using System.Reflection;
using System.Runtime.InteropServices;
using SharpSvn;

namespace Ankh.UI
{
    
    /// <summary>
    /// Gives a tree view of the repository based on revision.
    /// </summary>
    [ComVisible(true)]
    [Guid("23CBDF36-E528-4581-A59A-EA212A783D4E")]
    public class RepositoryExplorerControl : System.Windows.Forms.UserControl
    {
        public RepositoryExplorerControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
 

            this.components = new Container();

            this.AddToolBarImages();
           
        }

        /// <summary>
        /// The command bar to be used for a context menu.
        /// </summary>
        public object CommandBar
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.commandBar; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.commandBar = value; }
        }     

        
        
        /// <summary>
        /// Fired if the background listing checkbox' state is changed.
        /// </summary>
        public event EventHandler EnableBackgroundListingChanged;

        /// <summary>
        /// Fired when the selection changes.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Fired whenever a directory node is expanded.
        /// </summary>
        public event NodeExpandingDelegate NodeExpanding;

        /// <summary>
        /// Fired whenever the Add Rep button is clicked
        /// </summary>
        public event EventHandler AddRepoButtonClicked;

        

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
        /// Whether the "Enable background listing" checkbox is checked.
        /// </summary>
        public bool EnableBackgroundListing
        {
            get{ return this.enableBackgroundListingButton.Pushed; }
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
            node.SelectedImageIndex = this.treeView.ClosedFolderIndex;
            node.ImageIndex = this.treeView.ClosedFolderIndex;
            this.treeView.SelectedNode.Nodes.Add( node );

            // start editing it
            this.treeView.LabelEdit = true;
            node.BeginEdit();
        }

        private void AddToolBarImages()
        {
            this.toolbarImageList.Images.Add(
                new Icon( this.GetType().Assembly.GetManifestResourceStream(
                "Ankh.UI.Graphics.EnableBackgroundListing.ico" ) ) );
            this.toolbarImageList.Images.Add(
                new Icon(this.GetType().Assembly.GetManifestResourceStream( 
                "Ankh.UI.Graphics.AddRepoURL.ico") ) );
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
        /// The user wants to expand a node. Let him, but we have to list whats under it
        /// first.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeView_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            // don't bother going to the server unless it's the dummy child
            if ( e.Node.Nodes.Count > 0 &&
                e.Node.Nodes[0].Tag != RepositoryTreeView.DUMMY_NODE )
                return;

            // now see if any event handlers want to provide the children
            IRepositoryTreeNode node = (IRepositoryTreeNode)e.Node.Tag;
            NodeExpandingEventArgs args = new NodeExpandingEventArgs( node );

            try
            {
                if ( this.NodeExpanding != null )
                    this.NodeExpanding( this, args );
                this.treeView.AddChildren( node, args.Children );
            }
            catch( SvnException )
            {
                // don't open it - the user can click on the plus to try again
                e.Cancel = true;
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
            Debug.Assert( this.commandBar != null, "commandBar is null" );
            
            if ( args.Button != MouseButtons.Right )
                return;

            // make sure right click causes a selection
            this.treeView.SelectedNode = this.treeView.GetNodeAt( args.X, args.Y );

            Point screen = this.treeView.PointToScreen( new Point(args.X, args.Y) );
            this.commandBar.GetType().InvokeMember(
                "ShowPopup", BindingFlags.InvokeMethod, null, 
                this.commandBar, new object[]{ screen.X, screen.Y } );
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ( this.SelectionChanged != null )
                this.SelectionChanged( this, EventArgs.Empty );
        }

        private void ToolBarButtonClicked(object sender, 
            System.Windows.Forms.ToolBarButtonClickEventArgs e)
        {
            if (e.Button == this.enableBackgroundListingButton)
            {
                if (this.EnableBackgroundListingChanged != null)
                    this.EnableBackgroundListingChanged(this, EventArgs.Empty);
                e.Button.ToolTipText = e.Button.Pushed ? "Disable background listing" : "Enable background listing";
            }
            else if (e.Button == this.addRepoURLButton)
            {
                if (this.AddRepoButtonClicked != null)
                    this.AddRepoButtonClicked(this, EventArgs.Empty);
            }
        }



        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.backgroundListingCheck = new System.Windows.Forms.CheckBox();
            this.toolBar = new System.Windows.Forms.ToolBar();
            this.enableBackgroundListingButton = new System.Windows.Forms.ToolBarButton();
            this.toolbarImageList = new System.Windows.Forms.ImageList(this.components);
            this.addRepoURLButton = new System.Windows.Forms.ToolBarButton();
            this.treeView = new Ankh.UI.RepositoryTreeView();
            this.SuspendLayout();
            // 
            // backgroundListingCheck
            // 
            this.backgroundListingCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.backgroundListingCheck.Location = new System.Drawing.Point(80, 160);
            this.backgroundListingCheck.Name = "backgroundListingCheck";
            this.backgroundListingCheck.Size = new System.Drawing.Size(184, 16);
            this.backgroundListingCheck.TabIndex = 6;
            this.backgroundListingCheck.Text = "Enable background listing";
            // 
            // toolBar
            // 
            this.toolBar.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
            this.toolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] {
            this.enableBackgroundListingButton,
            this.addRepoURLButton});
            this.toolBar.ButtonSize = new System.Drawing.Size(16, 16);
            this.toolBar.DropDownArrows = true;
            this.toolBar.ImageList = this.toolbarImageList;
            this.toolBar.Location = new System.Drawing.Point(0, 0);
            this.toolBar.Name = "toolBar";
            this.toolBar.ShowToolTips = true;
            this.toolBar.Size = new System.Drawing.Size(296, 28);
            this.toolBar.TabIndex = 10;
            this.toolBar.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.ToolBarButtonClicked);
            // 
            // enableBackgroundListingButton
            // 
            this.enableBackgroundListingButton.ImageIndex = 0;
            this.enableBackgroundListingButton.Style = System.Windows.Forms.ToolBarButtonStyle.ToggleButton;
            this.enableBackgroundListingButton.ToolTipText = "Enable background listing";
            // 
            // toolbarImageList
            // 
            this.toolbarImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.toolbarImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.toolbarImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // addRepoURLButton
            // 
            this.addRepoURLButton.ImageIndex = 1;
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.Location = new System.Drawing.Point(0, 28);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(296, 268);
            this.treeView.TabIndex = 9;
            this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeExpand);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeViewMouseDown);
            // 
            // RepositoryExplorerControl
            // 
            this.Controls.Add(this.toolBar);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.backgroundListingCheck);
            this.Name = "RepositoryExplorerControl";
            this.Size = new System.Drawing.Size(296, 296);
            this.ResumeLayout(false);

        }
        #endregion

        private object commandBar;
        private System.Windows.Forms.CheckBox backgroundListingCheck;
        private Ankh.UI.RepositoryTreeView treeView;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolBar toolBar;
        private System.Windows.Forms.ToolBarButton enableBackgroundListingButton;
        private System.Windows.Forms.ImageList toolbarImageList;
        private ToolBarButton addRepoURLButton;

        private INewDirectoryHandler newDirHandler;

        
    }

    /// <summary>
    /// An object that handles the actual creation of a directory.
    /// </summary>
    public interface INewDirectoryHandler
    {
        bool MakeDir( IRepositoryTreeNode parent, string dirname );
    }
}
