// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NSvn.Core;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;

namespace Ankh.UI
{
    


    /// <summary>
    /// Dialog that lets a user enter a log message for a commit.
    /// </summary>
    public class CommitDialog : System.Windows.Forms.Form
    {
        public event DiffWantedDelegate DiffWanted
        {
            add{ this.diffTab.DiffWanted += value; }
            remove{ this.diffTab.DiffWanted -= value; }
        }


        public CommitDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.CreateToolTips();

            this.commitItems = new ArrayList();
            
            this.diffTab.Visible = false;
            
            this.pathColumnHeader.Width = this.commitItemsView.Width - this.actionColumnHeader.Width - 5;
        }

        /// <summary>
        /// The log message to be used for this commit.
        /// </summary>
        public string LogMessage
        {
            get
            { 
                return this.LogMessageTemplate.PostProcess( this.logMessageBox.Text );
            }
            set
            {
                this.logMessageBox.Text = value;
            }
        }

        public LogMessageTemplate LogMessageTemplate
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.logMessageTemplate; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.logMessageTemplate = value; }
        }

        public void AddCommitItem( CommitAction action, string path, 
            object tag )
        {
            ListViewItem item = new ListViewItem( new string[]{ path, action.ToString() } );
            item.Checked = true;
            item.Tag = tag;

            this.commitItemsView.Items.Add( item );

            // we wanna see a diff too
            this.diffTab.AddPage( path );
        }

        public Array GetSelectedTags( Type type )
        {           
            ArrayList arr = new ArrayList();
            foreach( ListViewItem i in this.commitItemsView.CheckedItems )
                arr.Add( i.Tag );
            return arr.ToArray( type );
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

        /// <summary>
        /// Makes tooltips on buttons and fields. 
        /// </summary>
        private void CreateToolTips()
        {
            // Create the ToolTip and associate with the Form container.
            ToolTip commitToolTip = new ToolTip();

            // Set up the delays in milliseconds for the ToolTip.
            commitToolTip.AutoPopDelay = 5000;
            commitToolTip.InitialDelay = 1000;
            commitToolTip.ReshowDelay = 500;
            // Force the ToolTip text to be displayed whether or not the form is active.
            commitToolTip.ShowAlways = true;
         
            // Set up the ToolTip text for the Button and Checkbox.
            //            commitToolTip.SetToolTip( this.fileTreeView, 
            //                "Three view of files you attempt to publish/commit. Files will be added into the repository." ); 
            //Doesn't work:(. Don't understand why.
            //            commitToolTip.SetToolTip( this.logMessageControl, 
            //                "Write changes you have performed since last revision or update" ); 
            commitToolTip.SetToolTip( this.okButton, 
                "Perform the commit." ); 
            commitToolTip.SetToolTip( this.cancelButton, "Cancel the commit." );  
        }

        private void showDiffButton_Click(object sender, System.EventArgs e)
        {
            if ( this.diffTab.Visible )
            {                
                this.diffTab.Visible = false;
                this.Height = this.okButton.Top + this.okButton.Height + 40;
                this.showDiffButton.Text = "Show diff";
  
                // reanchor these things to the bottom
                this.showDiffButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
                this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
                this.logMessageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                    | System.Windows.Forms.AnchorStyles.Left) 
                    | System.Windows.Forms.AnchorStyles.Right)));

            }
            else
            {
                // these items can no longer anchor to the bottom
                this.logMessageBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                this.showDiffButton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                this.okButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                this.cancelButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                this.diffTab.Top = this.okButton.Top + this.okButton.Height + 10;
                this.Height += 400;
                this.diffTab.Visible = true;
                this.showDiffButton.Text = "Hide diff";
            }

            // Center the dialog vertically
            this.Top = (Screen.FromControl(this).Bounds.Height / 2) - (this.Height/2);

        }

        private void CommitDialog_VisibleChanged(object sender, System.EventArgs e)
        {
            if ( this.Visible && this.logMessageBox.Text == string.Empty )
            {
                ArrayList arr = new ArrayList();
                foreach( ListViewItem item in this.commitItemsView.Items )
                    arr.Add( item.SubItems[0].Text );
            
                this.logMessageBox.Text = this.LogMessageTemplate.PreProcess( arr );
                this.preprocessed = true;
            }
        }

        private void ItemChecked(object sender, System.Windows.Forms.ItemCheckEventArgs e)
        {
            // don't bother if the 
            if ( ! this.preprocessed )
                return;

            ListViewItem item = this.commitItemsView.Items[e.Index];
            if ( e.CurrentValue == CheckState.Checked )
            {                
                this.logMessageBox.Text = this.logMessageTemplate.RemoveItem(
                    this.logMessageBox.Text, item.SubItems[0].Text );
            }
            else if ( e.CurrentValue == CheckState.Unchecked )
            {
                this.logMessageBox.Text = this.LogMessageTemplate.AddItem(
                    this.logMessageBox.Text, item.SubItems[0].Text );
            }        
        }


        


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.logLabel = new System.Windows.Forms.Label();
            this.showDiffButton = new System.Windows.Forms.Button();
            this.diffTab = new Ankh.UI.DiffTab();
            this.logMessageBox = new System.Windows.Forms.RichTextBox();
            this.commitItemsView = new System.Windows.Forms.ListView();
            this.pathColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.actionColumnHeader = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(710, 280);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(622, 280);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 2;
            this.okButton.Text = "Ok";
            // 
            // logLabel
            // 
            this.logLabel.Location = new System.Drawing.Point(0, 112);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(296, 23);
            this.logLabel.TabIndex = 4;
            this.logLabel.Text = "Write log message:";
            // 
            // showDiffButton
            // 
            this.showDiffButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.showDiffButton.Location = new System.Drawing.Point(8, 280);
            this.showDiffButton.Name = "showDiffButton";
            this.showDiffButton.TabIndex = 6;
            this.showDiffButton.Text = "Show diff";
            this.showDiffButton.Click += new System.EventHandler(this.showDiffButton_Click);
            // 
            // diffView
            // 
            this.diffTab.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.diffTab.Font = new System.Drawing.Font("Courier New", 10F);
            this.diffTab.Location = new System.Drawing.Point(0, 312);
            this.diffTab.Name = "diffView";
            this.diffTab.Size = new System.Drawing.Size(814, 0);
            // 
            // logMessageBox
            // 
            this.logMessageBox.AcceptsTab = true;
            this.logMessageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.logMessageBox.DetectUrls = false;
            this.logMessageBox.Location = new System.Drawing.Point(0, 136);
            this.logMessageBox.Name = "logMessageBox";
            this.logMessageBox.Size = new System.Drawing.Size(816, 128);
            this.logMessageBox.TabIndex = 8;
            this.logMessageBox.Text = "";
            // 
            // commitItemsView
            // 
            this.commitItemsView.CheckBoxes = true;
            this.commitItemsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                                                                                              this.pathColumnHeader,
                                                                                              this.actionColumnHeader});
            this.commitItemsView.Dock = System.Windows.Forms.DockStyle.Top;
            this.commitItemsView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.commitItemsView.Location = new System.Drawing.Point(0, 0);
            this.commitItemsView.Name = "commitItemsView";
            this.commitItemsView.Size = new System.Drawing.Size(816, 112);
            this.commitItemsView.TabIndex = 9;
            this.commitItemsView.View = System.Windows.Forms.View.Details;
            this.commitItemsView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ItemChecked);
            // 
            // pathColumnHeader
            // 
            this.pathColumnHeader.Text = "Path";
            this.pathColumnHeader.Width = 398;
            // 
            // actionColumnHeader
            // 
            this.actionColumnHeader.Text = "Action";
            this.actionColumnHeader.Width = 85;
            // 
            // CommitDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(816, 315);
            this.ControlBox = false;
            this.Controls.Add(this.commitItemsView);
            this.Controls.Add(this.logMessageBox);
            this.Controls.Add(this.diffTab);
            this.Controls.Add(this.showDiffButton);
            this.Controls.Add(this.logLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(390, 320);
            this.Name = "CommitDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Commit";
            this.VisibleChanged += new System.EventHandler(this.CommitDialog_VisibleChanged);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label logLabel;
        private System.Windows.Forms.Button showDiffButton;
        private Ankh.UI.DiffTab diffTab;
        private System.Windows.Forms.RichTextBox logMessageBox;
        private LogMessageTemplate logMessageTemplate;
       
        private ArrayList commitItems;
        private System.Windows.Forms.ListView commitItemsView;
        private System.Windows.Forms.ColumnHeader pathColumnHeader;
        private System.Windows.Forms.ColumnHeader actionColumnHeader;

        private bool preprocessed = false;
       
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        

        

        //        [STAThread] 
        //        public static void Main()
        //        {
        //            CommitDialog the = new CommitDialog();
        //            the.ShowDialog();
        //        }



    }

    /// <summary>
    /// The various types of actions.
    /// </summary>
    public enum CommitAction
    {
        Added,
        Deleted,
        Modified,
        None
    }
}



