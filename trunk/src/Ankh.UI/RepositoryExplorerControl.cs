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

namespace Ankh.UI
{
    /// <summary>
    /// Gives a tree view of the repository based on revision.
    /// </summary>
    public class RepositoryExplorerControl : System.Windows.Forms.UserControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public RepositoryExplorerControl()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
 
            // TODO: Add any initialization after the InitForm call

            myInitializeComponents();

            //Set revision choices in combobox
            this.revisionPicker.WorkingEnabled = false;
            this.revisionPicker.BaseEnabled = false;
            this.revisionPicker.CommittedEnabled = false;
            this.revisionPicker.PreviousEnabled = false;

            this.treeView.MouseUp += new MouseEventHandler( this.TreeViewMouseUp );
            this.treeView.BeforeExpand += new TreeViewCancelEventHandler(treeView_BeforeExpand);
            this.treeView.AfterSelect += new TreeViewEventHandler(treeView_AfterSelect);
			
            this.components = new System.ComponentModel.Container();
            this.SetToolTips();

            this.ValidateRevision();
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
        public event EventHandler GoClicked;

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
        /// Whether the "Enable background listing" checkbox is checked.
        /// </summary>
        public bool EnableBackgroundListing
        {
            get{ return this.backgroundListingCheck.Checked; }
        }

        /// <summary>
        /// Add a new URL root to the tree.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="node"></param>
        public void AddRoot( string url, IRepositoryTreeNode node)
        {
            if ( !node.IsDirectory )
                throw new ArgumentException( "The root needs to be a directory.", "node" );

            this.treeView.AddRoot( node, url );
        }

        public void RefreshNode( IRepositoryTreeNode node )
        {
            this.treeView.RefreshNode( node );
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
        private void goButton_Click(object sender, System.EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            try
            {
                if ( this.GoClicked != null ) 
                    this.GoClicked( this, EventArgs.Empty );  
                this.treeView.Enabled = true;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void revisionPicker_Changed(object sender, System.EventArgs e)
        {
            this.ValidateRevision();            
        }

        private void ValidateRevision()
        {
            this.goButton.Enabled = this.revisionPicker.Valid;
        }

        private void TreeViewMouseUp( object sender, MouseEventArgs args )
        {
            Debug.Assert( this.commandBar != null, "commandBar is null" );

            if ( args.Button != MouseButtons.Right )
                return;
            Point screen = this.treeView.PointToScreen( new Point(args.X, args.Y) );

            this.commandBar.ShowPopup( screen.X, screen.Y );
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if ( this.SelectionChanged != null )
                this.SelectionChanged( this, EventArgs.Empty );
        }


		#region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.goButton = new System.Windows.Forms.Button();
            this.revisionLabel = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.Button();
            this.urlLabel = new System.Windows.Forms.Label();
            this.backgroundListingCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left)));
            this.urlTextBox.Location = new System.Drawing.Point(32, 8);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(224, 20);
            this.urlTextBox.TabIndex = 1;
            this.urlTextBox.Text = "http://www.ankhsvn.com:8088/svn/test";
            this.urlTextBox.TextChanged += new System.EventHandler(this.urlTextBox_TextChanged);
            // 
            // goButton
            // 
            this.goButton.Location = new System.Drawing.Point(264, 8);
            this.goButton.Name = "goButton";
            this.goButton.Size = new System.Drawing.Size(32, 23);
            this.goButton.TabIndex = 2;
            this.goButton.Text = "Go";
            this.goButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // revisionLabel
            // 
            this.revisionLabel.Location = new System.Drawing.Point(0, 38);
            this.revisionLabel.Name = "revisionLabel";
            this.revisionLabel.Size = new System.Drawing.Size(344, 18);
            this.revisionLabel.TabIndex = 3;
            this.revisionLabel.Text = "Select a revision or manually type the revision number:";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(305, 8);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(56, 23);
            this.browseButton.TabIndex = 4;
            this.browseButton.Text = "Browse";
            this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
            // 
            // urlLabel
            // 
            this.urlLabel.Location = new System.Drawing.Point(7, 11);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(24, 16);
            this.urlLabel.TabIndex = 5;
            this.urlLabel.Text = "Url:";
            // 
            // backgroundListingCheck
            // 
            this.backgroundListingCheck.Location = new System.Drawing.Point(8, 88);
            this.backgroundListingCheck.Name = "backgroundListingCheck";
            this.backgroundListingCheck.Size = new System.Drawing.Size(184, 16);
            this.backgroundListingCheck.TabIndex = 6;
            this.backgroundListingCheck.Text = "Enable background listing";
            // 
            // RepositoryExplorerControl
            // 
            this.Controls.Add(this.backgroundListingCheck);
            this.Controls.Add(this.urlLabel);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.revisionLabel);
            this.Controls.Add(this.goButton);
            this.Controls.Add(this.urlTextBox);
            this.Name = "RepositoryExplorerControl";
            this.Size = new System.Drawing.Size(376, 392);
            this.ResumeLayout(false);

        }
		#endregion

        
        private void myInitializeComponents()
        {
            //initialize components outside of generated code area

            this.treeView = new Ankh.UI.RepositoryTreeView();
            this.revisionPicker = new Ankh.UI.RevisionPicker();
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.Enabled = false;
            this.treeView.ImageIndex = -1;
            this.treeView.Location = new System.Drawing.Point(0, 110);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = -1;
            this.treeView.Size = new System.Drawing.Size(407, 296);
            this.treeView.TabIndex = 6;
            // 
            // revisionPicker
            // 
            this.revisionPicker.Location = new System.Drawing.Point(8, 60);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(336, 24);
            this.revisionPicker.TabIndex = 7;
            this.revisionPicker.Changed += new System.EventHandler(this.revisionPicker_Changed);
            // 
            // RepositoryExplorerControl
            // 
            this.Controls.Add(this.revisionPicker);
            this.Controls.Add(this.treeView);

        }

 
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
        
        }
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Button goButton;
        private System.Windows.Forms.Label revisionLabel;
        private System.Windows.Forms.Button browseButton;
        private Ankh.UI.RevisionPicker revisionPicker;
        private Ankh.UI.RepositoryTreeView treeView;
        private CommandBar commandBar;
        private System.Windows.Forms.CheckBox backgroundListingCheck;
        private System.Windows.Forms.Label urlLabel;

        
    }
}
