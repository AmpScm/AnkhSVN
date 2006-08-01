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

            // Support Ctrl-A to select everything.
            this.logMessageBox.KeyDown += new KeyEventHandler( logMessageBox_KeyDown );
            this.logMessageBox.KeyPress += new KeyPressEventHandler( logMessageBox_KeyPress );
            
            // HACK: since there is no KeyPreview on a UserControl
            this.HookUpKeyEvent(this);
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

        public bool KeepLocks
        {
            get{ return this.keepLocksCheckBox.Checked; }
            set{ this.keepLocksCheckBox.Checked = value; }
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
                this.LogMessageTemplate.UrlPaths = this.commitItemsTree.UrlPaths; 
                this.logMessageBox.Text = this.LogMessageTemplate.PreProcess( arr );
        
                
            }

            this.logMessageBox.Focus();
            this.logMessageBox.SelectionStart = this.logMessageBox.SelectionStart + this.logMessageBox.SelectionLength;
            this.logMessageBox.SelectionLength = 0;

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
            this.dialogResult = CommitDialogResult.Cancel;
            this.CommitItems = new object[]{};
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

        void logMessageBox_KeyPress( object sender, KeyPressEventArgs e )
        {
            // suppress Ctrl-A, which is ASCII 1 for some reason...
            if ( e.KeyChar == (char)1 )
            {
                e.Handled = true;
            }
        }

        void logMessageBox_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.Control && e.KeyCode == Keys.A )
            {
                this.logMessageBox.SelectAll();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handle Ctrl-Enter to commit and Esc to cancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CommitDialog_KeyDown( object sender, KeyEventArgs e )
        {
            if ( this.commitButton.Enabled && e.Control && e.KeyCode == Keys.Enter )
            {
                this.RaiseProceed( this.commitButton, EventArgs.Empty );
                e.Handled = true;
            }
            else if ( this.cancelButton.Enabled && e.KeyCode == Keys.Escape )
            {
                this.RaiseProceed( this.cancelButton, EventArgs.Empty );
                e.Handled = true;
            }
        }

        /// <summary>
        /// This method is necessary to ensure we get this event from all controls, since there
        /// is no KeyPreview on a user control.
        /// </summary>
        /// <param name="control"></param>
        private void HookUpKeyEvent( Control control )
        {
            control.KeyDown += new KeyEventHandler( CommitDialog_KeyDown );
            foreach ( Control child in control.Controls )
            {
                this.HookUpKeyEvent( child );
            }
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.topSplitter = new System.Windows.Forms.Splitter();
            this.logMessagePanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.keepLocksCheckBox = new System.Windows.Forms.CheckBox();
            this.logMessageBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.commitButton = new System.Windows.Forms.Button();
            this.mainPanel = new System.Windows.Forms.Panel();
            this.commitItemsTree = new Ankh.UI.PathSelectionTreeView();
            this.logMessagePanel.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // topSplitter
            // 
            this.topSplitter.BackColor = System.Drawing.SystemColors.Control;
            this.topSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.topSplitter.Location = new System.Drawing.Point( 0, 112 );
            this.topSplitter.Name = "topSplitter";
            this.topSplitter.Size = new System.Drawing.Size( 772, 3 );
            this.topSplitter.TabIndex = 3;
            this.topSplitter.TabStop = false;
            // 
            // logMessagePanel
            // 
            this.logMessagePanel.Controls.Add( this.label1 );
            this.logMessagePanel.Controls.Add( this.keepLocksCheckBox );
            this.logMessagePanel.Controls.Add( this.logMessageBox );
            this.logMessagePanel.Controls.Add( this.cancelButton );
            this.logMessagePanel.Controls.Add( this.commitButton );
            this.logMessagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMessagePanel.Location = new System.Drawing.Point( 0, 115 );
            this.logMessagePanel.Name = "logMessagePanel";
            this.logMessagePanel.Size = new System.Drawing.Size( 772, 177 );
            this.logMessagePanel.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point( 113, 155 );
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size( 132, 13 );
            this.label1.TabIndex = 5;
            this.label1.Text = "Press Ctrl-Enter to commit";
            this.label1.Click += new System.EventHandler( this.label1_Click );
            // 
            // keepLocksCheckBox
            // 
            this.keepLocksCheckBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left ) ) );
            this.keepLocksCheckBox.Location = new System.Drawing.Point( 3, 152 );
            this.keepLocksCheckBox.Name = "keepLocksCheckBox";
            this.keepLocksCheckBox.Size = new System.Drawing.Size( 104, 21 );
            this.keepLocksCheckBox.TabIndex = 4;
            this.keepLocksCheckBox.Text = "Keep locks";
            // 
            // logMessageBox
            // 
            this.logMessageBox.AcceptsReturn = true;
            this.logMessageBox.AcceptsTab = true;
            this.logMessageBox.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.logMessageBox.Location = new System.Drawing.Point( 0, 0 );
            this.logMessageBox.Multiline = true;
            this.logMessageBox.Name = "logMessageBox";
            this.logMessageBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logMessageBox.Size = new System.Drawing.Size( 772, 144 );
            this.logMessageBox.TabIndex = 3;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point( 694, 150 );
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size( 75, 23 );
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.Click += new System.EventHandler( this.RaiseProceed );
            // 
            // commitButton
            // 
            this.commitButton.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.commitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.commitButton.Location = new System.Drawing.Point( 616, 150 );
            this.commitButton.Name = "commitButton";
            this.commitButton.Size = new System.Drawing.Size( 75, 23 );
            this.commitButton.TabIndex = 0;
            this.commitButton.Text = "Commit";
            this.commitButton.Click += new System.EventHandler( this.RaiseProceed );
            // 
            // mainPanel
            // 
            this.mainPanel.Anchor = ( (System.Windows.Forms.AnchorStyles)( ( ( ( System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom )
                        | System.Windows.Forms.AnchorStyles.Left )
                        | System.Windows.Forms.AnchorStyles.Right ) ) );
            this.mainPanel.Controls.Add( this.logMessagePanel );
            this.mainPanel.Controls.Add( this.topSplitter );
            this.mainPanel.Controls.Add( this.commitItemsTree );
            this.mainPanel.Location = new System.Drawing.Point( 10, 10 );
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Size = new System.Drawing.Size( 772, 292 );
            this.mainPanel.TabIndex = 0;
            // 
            // commitItemsTree
            // 
            this.commitItemsTree.CheckBoxes = true;
            this.commitItemsTree.Dock = System.Windows.Forms.DockStyle.Top;
            this.commitItemsTree.Items = new object[ 0 ];
            this.commitItemsTree.Location = new System.Drawing.Point( 0, 0 );
            this.commitItemsTree.Name = "commitItemsTree";
            this.commitItemsTree.Recursive = false;
            this.commitItemsTree.SingleCheck = false;
            this.commitItemsTree.Size = new System.Drawing.Size( 772, 112 );
            this.commitItemsTree.TabIndex = 2;
            this.commitItemsTree.UrlPaths = false;
            // 
            // CommitDialog
            // 
            this.Controls.Add( this.mainPanel );
            this.Name = "CommitDialog";
            this.Size = new System.Drawing.Size( 792, 304 );
            this.logMessagePanel.ResumeLayout( false );
            this.logMessagePanel.PerformLayout();
            this.mainPanel.ResumeLayout( false );
            this.ResumeLayout( false );

        }
        #endregion

        private LogMessageTemplate logMessageTemplate;
       
        private bool loaded = false;

        private Ankh.UI.PathSelectionTreeView commitItemsTree;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox logMessageBox;
        private System.Windows.Forms.Splitter topSplitter;
        private System.Windows.Forms.Panel logMessagePanel;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.Button commitButton;
        private CommitDialogResult dialogResult = CommitDialogResult.Cancel;
        private System.Windows.Forms.CheckBox keepLocksCheckBox;
        private Label label1;

       
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private void label1_Click( object sender, EventArgs e )
        {

        }
    }
}



