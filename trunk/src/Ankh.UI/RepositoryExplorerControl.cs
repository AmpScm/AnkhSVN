// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NSvn.Core;
using Utils;
using System.Text.RegularExpressions;
using Microsoft.Office.Core;
using System.Diagnostics;
using System.Web;
using Utils.Win32;

namespace Ankh.UI
{
    
    /// <summary>
    /// Gives a tree view of the repository based on revision.
    /// </summary>
    public class RepositoryExplorerControl : System.Windows.Forms.UserControl
    {
        public RepositoryExplorerControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
 
            //Set revision choices in combobox
            this.revisionPicker.WorkingEnabled = false;
            this.revisionPicker.BaseEnabled = false;
            this.revisionPicker.CommittedEnabled = false;
            this.revisionPicker.PreviousEnabled = false;

            this.components = new Container();
            this.SetToolTips();

            Win32.SHAutoComplete( this.urlTextBox.Handle, 
                Shacf.UrlAll | Shacf.Filesystem );

            this.ValidateAdd();
        }

        /// <summary>
        /// The command bar to be used for a context menu.
        /// </summary>
        public CommandBar CommandBar
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.commandBar; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.commandBar = value; }
        }     

        /// <summary>
        /// The revision selected by the user.
        /// </summary>
        public Revision Revision
        {
            get{ return this.revisionPicker.Revision; }
        }
        
        /// <summary>
        /// Fired whenever the user clicks Go.
        /// </summary>
        public event EventHandler AddClicked;

        public event EventHandler RemoveClicked 
        {
            add{ this.removeButton.Click += value; }
            remove{ this.removeButton.Click -= value; }
        }

        /// <summary>
        /// Fired if the background listing checkbox' state is changed.
        /// </summary>
        public event EventHandler EnableBackgroundListingChanged
        {
            add{ this.backgroundListingCheck.CheckedChanged += value; }
            remove{ this.backgroundListingCheck.CheckedChanged -= value; }
        }

        /// <summary>
        /// Fired when the selection changes.
        /// </summary>
        public event EventHandler SelectionChanged;

        /// <summary>
        /// Fired whenever a directory node is expanded.
        /// </summary>
        public event NodeExpandingDelegate NodeExpanding;

        /// <summary>
        /// The URL entered in the text box.
        /// </summary>
        public string Url
        {
            get
            {
                return this.urlTextBox.Text;
            }
        }

        /// <summary>
        /// The selected node. Will be null if there is no selection.
        /// </summary>
        public IRepositoryTreeNode SelectedNode
        {
            get
            { 
                return 
                    this.treeView.SelectedNode != null ? 
                    (IRepositoryTreeNode)this.treeView.SelectedNode.Tag :
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
            get{ return this.backgroundListingCheck.Checked; }
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
            this.removeButton.Enabled = this.SelectedNode != null;
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

        private INewDirectoryHandler newDirHandler;

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
            catch( SvnClientException )
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

        private void SetToolTips()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip ToolTip = new ToolTip(this.components);

            // Set up the delays in milliseconds for the ToolTip.
            ToolTip.AutoPopDelay = 5000;
            ToolTip.InitialDelay = 1000;
            ToolTip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            ToolTip.ShowAlways = true;
         
            // Set up the ToolTip text for the Button and Checkbox.
            ToolTip.SetToolTip( this.urlTextBox, 
                "Write the url to your repository" );
            ToolTip.SetToolTip( this.treeView, 
                "Select a date from the calendar" );
        }

        //Gives a tree view of repository if valid revision is selected
        private void addButton_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if ( this.AddClicked != null ) 
                    this.AddClicked( this, EventArgs.Empty );  
                this.treeView.Enabled = true;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void revisionPicker_Changed(object sender, System.EventArgs e)
        {
            this.ValidateAdd();            
        }

        private void ValidateAdd()
        {
            this.addButton.Enabled = this.revisionPicker.Valid && 
                UriUtils.ValidUrl.IsMatch( this.urlTextBox.Text );
        }

        private void TreeViewMouseDown( object sender, MouseEventArgs args )
        {
            Debug.Assert( this.commandBar != null, "commandBar is null" );
            
            if ( args.Button != MouseButtons.Right )
                return;

            // make sure right click causes a selection
            this.treeView.SelectedNode = this.treeView.GetNodeAt( args.X, args.Y );

            Point screen = this.treeView.PointToScreen( new Point(args.X, args.Y) );
            this.commandBar.ShowPopup( screen.X, screen.Y );
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ( this.SelectionChanged != null )
                this.SelectionChanged( this, EventArgs.Empty );
            this.removeButton.Enabled = this.SelectedNode != null;
        }


        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.addButton = new System.Windows.Forms.Button();
            this.revisionLabel = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.urlLabel = new System.Windows.Forms.Label();
            this.backgroundListingCheck = new System.Windows.Forms.CheckBox();
            this.removeButton = new System.Windows.Forms.Button();
            this.treeView = new Ankh.UI.RepositoryTreeView();
            this.revisionPicker = new Ankh.UI.RevisionPicker();
            this.SuspendLayout();
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.urlTextBox.Location = new System.Drawing.Point(4, 72);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(368, 20);
            this.urlTextBox.TabIndex = 1;
            this.urlTextBox.Text = "";
            this.urlTextBox.TextChanged += new System.EventHandler(this.urlTextBox_TextChanged);
            // 
            // addButton
            // 
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(4, 99);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(64, 23);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "Add";
            this.addButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // revisionLabel
            // 
            this.revisionLabel.Location = new System.Drawing.Point(4, 8);
            this.revisionLabel.Name = "revisionLabel";
            this.revisionLabel.Size = new System.Drawing.Size(292, 18);
            this.revisionLabel.TabIndex = 3;
            this.revisionLabel.Text = "Select a revision or manually type the revision number:";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(73, 99);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(72, 23);
            this.browseButton.TabIndex = 4;
            this.browseButton.Text = "Browse...";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // urlLabel
            // 
            this.urlLabel.Location = new System.Drawing.Point(4, 56);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(24, 16);
            this.urlLabel.TabIndex = 5;
            this.urlLabel.Text = "Url:";
            // 
            // backgroundListingCheck
            // 
            this.backgroundListingCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.backgroundListingCheck.Location = new System.Drawing.Point(0, 328);
            this.backgroundListingCheck.Name = "backgroundListingCheck";
            this.backgroundListingCheck.Size = new System.Drawing.Size(184, 16);
            this.backgroundListingCheck.TabIndex = 6;
            this.backgroundListingCheck.Text = "Enable background listing";
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Enabled = false;
            this.removeButton.Location = new System.Drawing.Point(283, 99);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(88, 23);
            this.removeButton.TabIndex = 8;
            this.removeButton.Text = "Remove";
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.ImageIndex = -1;
            this.treeView.Location = new System.Drawing.Point(0, 128);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = -1;
            this.treeView.Size = new System.Drawing.Size(376, 192);
            this.treeView.TabIndex = 9;
            this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeViewMouseDown);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView_BeforeExpand);
            // 
            // revisionPicker
            // 
            this.revisionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPicker.Location = new System.Drawing.Point(4, 24);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(368, 20);
            this.revisionPicker.TabIndex = 10;
            this.revisionPicker.Changed += new System.EventHandler(this.revisionPicker_Changed);
            // 
            // RepositoryExplorerControl
            // 
            this.Controls.Add(this.revisionPicker);
            this.Controls.Add(this.treeView);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.backgroundListingCheck);
            this.Controls.Add(this.urlLabel);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.revisionLabel);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.urlTextBox);
            this.Name = "RepositoryExplorerControl";
            this.Size = new System.Drawing.Size(376, 352);
            this.ResumeLayout(false);

        }
        #endregion

        private void browseButton_Click(object sender, System.EventArgs e)
        {
            // Browse to a local repository

            FolderBrowser browser = new FolderBrowser();

            //convert the returned directory path to a URL - for a local path URL no need for encoding
            if ( browser.ShowDialog() == DialogResult.OK) 
                urlTextBox.Text ="file:///" +  browser.DirectoryPath.Replace( '\\', '/');

        }

        private void urlTextBox_TextChanged(object sender, System.EventArgs e)
        {
            this.ValidateAdd();
        }


        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Label revisionLabel;
        private System.Windows.Forms.Button browseButton;
        private CommandBar commandBar;
        private System.Windows.Forms.CheckBox backgroundListingCheck;
        private System.Windows.Forms.Label urlLabel;
        

        private System.Windows.Forms.Button addButton;
        private Ankh.UI.RepositoryTreeView treeView;
        private Ankh.UI.RevisionPicker revisionPicker;
        private System.Windows.Forms.Button removeButton;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
    }

    /// <summary>
    /// An object that handles the actual creation of a directory.
    /// </summary>
    public interface INewDirectoryHandler
    {
        bool MakeDir( IRepositoryTreeNode parent, string dirname );
    }
}
