// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NSvn;
using NSvn.Core;
using Utils;
using System.Text.RegularExpressions;
using Microsoft.Office.Core;
using System.Diagnostics;

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
        /// The IRepositoryTreeController that controls the view.
        /// </summary>
        public IRepositoryTreeController Controller
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.treeView.Controller; }

            set
            { 
                this.treeView.Controller = value; 
                value.TreeView = this.treeView;
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

                this.treeView.Controller.SetRepository( this.urlTextBox.Text, this.revisionPicker.Revision );
                this.treeView.Go();
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
            this.SuspendLayout();
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left);
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
            // RepositoryExplorerControl
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.urlLabel,
                                                                          this.browseButton,
                                                                          this.revisionLabel,
                                                                          this.goButton,
                                                                          this.urlTextBox});
            this.Name = "RepositoryExplorerControl";
            this.Size = new System.Drawing.Size(408, 392);
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
            this.treeView.Controller = null;
            this.treeView.Enabled = false;
            this.treeView.ImageIndex = -1;
            this.treeView.Location = new System.Drawing.Point(0, 88);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = -1;
            this.treeView.Size = new System.Drawing.Size(400, 296);
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
            browser.ShowDialog();
            //convert the returned directory path to a URL 
            if (browser.DirectoryPath.Length != 0) 
                urlTextBox.Text = "file:///" + browser.DirectoryPath.Replace( '\\', '/');

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
        private System.Windows.Forms.Label urlLabel;

 

    }
}
