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
    public enum CommitDialogResult
    {
        Cancel,
        Commit
    }
    
    /// <summary>
    /// Dialog that lets a user enter a log message for a commit.
    /// </summary>
    public class CommitDialog : UserControl
    {
        public event EventHandler Proceed;

        

        public CommitDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.CreateToolTips();

            this.commitItemsTree.GetPathInfo += new GetPathInfoDelegate(commitItemsTree_GetPathInfo);
            this.commitItemsTree.AfterCheck += new TreeViewEventHandler(ItemChecked);
            
            //this.diffViewVisible = true;
            //this.ToggleDiffView();
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

        /// <summary>
        /// The raw log message, with commented lines still embedded.
        /// </summary>
        public string RawLogMessage
        {
            get
            {
                return this.logMessageBox.Text;
            }
            set
            {
                this.logMessageBox.Text = value;
            }
        }

        /// <summary>
        /// The template to use for log messages.
        /// </summary>
        public LogMessageTemplate LogMessageTemplate
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.logMessageTemplate; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.logMessageTemplate = value; }
        }

        public IList CommitItems
        {
            get{ return this.commitItemsTree.CheckedItems; }
            set
            { 
                this.commitItemsTree.Items = value;
                this.commitItemsTree.CheckedItems = value;
               
            }
        }

        public bool UrlPaths
        {
            get
            { return this.commitItemsTree.UrlPaths; }
            set
            { 
                this.commitItemsTree.UrlPaths = value;
            }
        }

        public CommitDialogResult CommitDialogResult
        {
            get{ return this.dialogResult; }
        }

        /// <summary>
        /// Whether the Commit/Cancel buttons should be enabled.
        /// </summary>
        public bool ButtonsEnabled
        {
            get{ return this.commitButton.Enabled || this.cancelButton.Enabled; }
            set
            {
                this.commitButton.Enabled = this.cancelButton.Enabled = value;
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
            commitToolTip.SetToolTip( this.commitButton, 
                "Perform the commit." ); 
            commitToolTip.SetToolTip( this.cancelButton, "Cancel the commit." );  
        }
        

        /// <summary>
        /// Initialize the log message in the text box.
        /// </summary>
        public void Initialize()
        {   
            if ( this.logMessageBox.Text.Trim() == String.Empty )
            {
                ArrayList arr = new ArrayList();
                foreach( object item in this.commitItemsTree.CheckedItems )
                    arr.Add( item.ToString() );
                this.logMessageBox.Text = this.LogMessageTemplate.PreProcess( arr );
        
                this.LogMessageTemplate.UrlPaths = this.commitItemsTree.UrlPaths; 
            }

            this.loaded = true;
        }

        /// <summary>
        /// Reset the log message
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Reset()
        {
            this.logMessageBox.Text = "";
        }

        private void ItemChecked(object sender, TreeViewEventArgs e )
        {
            // don't bother if we haven't been loaded
            if ( ! this.loaded )
                return;
            
            if ( e.Node.Checked )
            {    
                this.logMessageBox.Text = this.LogMessageTemplate.AddItem(
                    this.logMessageBox.Text, e.Node.Tag.ToString() );
            }
            else 
            {
                this.logMessageBox.Text = this.logMessageTemplate.RemoveItem(
                    this.logMessageBox.Text, e.Node.Tag.ToString() );                
            }        
        }

        private void commitItemsTree_GetPathInfo(object sender, GetPathInfoEventArgs args)
        {
            args.Path = args.Item.ToString();
        }

        private void RaiseProceed(object sender, System.EventArgs e)
        {
            if ( sender == this.cancelButton )
                this.dialogResult = CommitDialogResult.Cancel;
            else if ( sender == this.commitButton )
                this.dialogResult = CommitDialogResult.Commit;

            if ( this.Proceed != null )
                this.Proceed( this, EventArgs.Empty );

            this.loaded = false;
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.commitItemsTree = new Ankh.UI.PathSelectionTreeView();
            this.topSplitter = new System.Windows.Forms.Splitter();
            this.logMessagePanel = new System.Windows.Forms.Panel();
            this.logMessageBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.commitButton = new System.Windows.Forms.Button();
            this.logMessagePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // commitItemsTree
            // 
            this.commitItemsTree.CheckBoxes = true;
            this.commitItemsTree.Dock = System.Windows.Forms.DockStyle.Top;
            this.commitItemsTree.ImageIndex = -1;
            this.commitItemsTree.Items = new object[0];
            this.commitItemsTree.Location = new System.Drawing.Point(0, 0);
            this.commitItemsTree.Name = "commitItemsTree";
            this.commitItemsTree.Recursive = false;
            this.commitItemsTree.SelectedImageIndex = -1;
            this.commitItemsTree.SingleCheck = false;
            this.commitItemsTree.Size = new System.Drawing.Size(808, 112);
            this.commitItemsTree.TabIndex = 2;
            this.commitItemsTree.UrlPaths = false;
            // 
            // topSplitter
            // 
            this.topSplitter.BackColor = System.Drawing.SystemColors.Control;
            this.topSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.topSplitter.Location = new System.Drawing.Point(0, 112);
            this.topSplitter.MinSize = 100;
            this.topSplitter.Name = "topSplitter";
            this.topSplitter.Size = new System.Drawing.Size(808, 3);
            this.topSplitter.TabIndex = 3;
            this.topSplitter.TabStop = false;
            // 
            // logMessagePanel
            // 
            this.logMessagePanel.Controls.Add(this.logMessageBox);
            this.logMessagePanel.Controls.Add(this.cancelButton);
            this.logMessagePanel.Controls.Add(this.commitButton);
            this.logMessagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMessagePanel.Location = new System.Drawing.Point(0, 115);
            this.logMessagePanel.Name = "logMessagePanel";
            this.logMessagePanel.Size = new System.Drawing.Size(808, 149);
            this.logMessagePanel.TabIndex = 4;
            // 
            // logMessageBox
            // 
            this.logMessageBox.AcceptsReturn = true;
            this.logMessageBox.AcceptsTab = true;
            this.logMessageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.logMessageBox.Location = new System.Drawing.Point(0, 0);
            this.logMessageBox.Multiline = true;
            this.logMessageBox.Name = "logMessageBox";
            this.logMessageBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logMessageBox.Size = new System.Drawing.Size(808, 112);
            this.logMessageBox.TabIndex = 3;
            this.logMessageBox.Text = "";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(720, 118);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler(this.RaiseProceed);
            // 
            // commitButton
            // 
            this.commitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.commitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.commitButton.Location = new System.Drawing.Point(640, 118);
            this.commitButton.Name = "commitButton";
            this.commitButton.TabIndex = 0;
            this.commitButton.Text = "Commit";
            this.commitButton.Click += new System.EventHandler(this.RaiseProceed);
            // 
            // CommitDialog
            // 
            this.Controls.Add(this.logMessagePanel);
            this.Controls.Add(this.topSplitter);
            this.Controls.Add(this.commitItemsTree);
            this.Name = "CommitDialog";
            this.Size = new System.Drawing.Size(808, 264);
            this.logMessagePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private LogMessageTemplate logMessageTemplate;
       
        private bool loaded = false;

        private Ankh.UI.PathSelectionTreeView commitItemsTree;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox logMessageBox;
        private System.Windows.Forms.Splitter topSplitter;
        private System.Windows.Forms.Panel logMessagePanel;
        private System.Windows.Forms.Button commitButton;
        private CommitDialogResult dialogResult = CommitDialogResult.Cancel;

       
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
    }
}



