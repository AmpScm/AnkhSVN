// $Id$
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using NSvn;
using NSvn.Core;
using System.Text.RegularExpressions;

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

        public RepositoryExplorerControl( )
        {
            //This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            //Set revision choices in combobox
            this.revisionPicker.WorkingEnabled = false;
            this.revisionPicker.BaseEnabled = false;
            this.revisionPicker.CommittedEnabled = false;
            this.revisionPicker.PreviousEnabled = false;
			
            this.components = new System.ComponentModel.Container();
            this.SetToolTips();

            this.ValidateRevision();
        }

        /// <summary>
        /// The NSvnContext to use for authentication, notifications etc.
        /// </summary>
        public NSvnContext Context
        {
            get{ return this.context; }
            set{ this.context = value; }
        }
        
        public void AddMenuItem( MenuItem item, int position )
        {
            this.treeView.ContextMenu.MenuItems.Add( position, item );
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
            try
            {
                this.Cursor = Cursors.WaitCursor;

                RepositoryDirectory dir = new RepositoryDirectory(
                    this.urlTextBox.Text, this.revisionPicker.Revision );

                if ( this.context != null )
                    dir.Context = this.context;

                this.treeView.RepositoryRoot = dir;
                this.treeView.Enabled = true;
            }
            catch( AuthorizationFailedException )
            {
                MessageBox.Show( "Could not authorize against repository " + 
                    this.urlTextBox.Text, "Authorization failed", MessageBoxButtons.OK,
                    MessageBoxIcon.Warning );
            }
            catch( SvnClientException ex )
            {
                MessageBox.Show( "An error occurred: " + ex.Message, "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error );
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

		#region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.urlLabel = new System.Windows.Forms.Label();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.revisionLabel = new System.Windows.Forms.Label();
            this.goButton = new System.Windows.Forms.Button();
            this.treeView = new Ankh.UI.RepositoryTreeView();
            this.revisionPicker = new Ankh.UI.RevisionPicker();
            this.SuspendLayout();
            // 
            // urlLabel
            // 
            this.urlLabel.Location = new System.Drawing.Point(1, 8);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(24, 23);
            this.urlLabel.TabIndex = 0;
            this.urlLabel.Text = "Url:";
            // 
            // urlTextBox
            // 
            this.urlTextBox.Location = new System.Drawing.Point(28, 5);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(220, 20);
            this.urlTextBox.TabIndex = 1;
            this.urlTextBox.Text = "http://www.ankhsvn.com:8088/svn/test";
            // 
            // revisionLabel
            // 
            this.revisionLabel.Location = new System.Drawing.Point(1, 29);
            this.revisionLabel.Name = "revisionLabel";
            this.revisionLabel.TabIndex = 2;
            this.revisionLabel.Text = "Select a revision or manually type the revision number:";
            this.revisionLabel.Size = new System.Drawing.Size( 350, 20 );
            // 
            // goButton
            // 
            this.goButton.Enabled = false;
            this.goButton.Location = new System.Drawing.Point(256, 4);
            this.goButton.Name = "goButton";
            this.goButton.TabIndex = 5;
            this.goButton.Text = "Go";
            this.goButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // treeView
            // 
            this.treeView.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right);
            this.treeView.Enabled = false;
            this.treeView.ImageIndex = -1;
            this.treeView.Location = new System.Drawing.Point(0, 80);
            this.treeView.Name = "treeView";
            this.treeView.RepositoryRoot = null;
            this.treeView.SelectedImageIndex = -1;
            this.treeView.Size = new System.Drawing.Size(368, 288);
            this.treeView.TabIndex = 6;
            // 
            // revisionPicker
            // 
            this.revisionPicker.Location = new System.Drawing.Point(8, 48);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(336, 24);
            this.revisionPicker.TabIndex = 7;
            this.revisionPicker.Changed += new System.EventHandler(this.revisionPicker_Changed);
            // 
            // RepositoryExplorerControl
            // 
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.revisionPicker,
                                                                          this.treeView,
                                                                          this.goButton,
                                                                          this.revisionLabel,
                                                                          this.urlTextBox,
                                                                          this.urlLabel});
            this.Name = "RepositoryExplorerControl";
            this.Size = new System.Drawing.Size(376, 376);
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.Label revisionLabel;
        private Ankh.UI.RepositoryTreeView treeView;
        private System.Windows.Forms.Button goButton;
        private Ankh.UI.RevisionPicker revisionPicker;
        private NSvnContext context;
		
						
    }
}



