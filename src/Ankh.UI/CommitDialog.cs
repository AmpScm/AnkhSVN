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
            add{ this.diffView.DiffWanted += value; }
            remove{ this.diffView.DiffWanted -= value; }
        }


        public CommitDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.CreateToolTips();

            this.commitItemsTree.GetPathInfo += new GetPathInfoDelegate(commitItemsTree_GetPathInfo);
            this.commitItemsTree.AfterCheck += new TreeViewEventHandler(ItemChecked);
            
            this.diffViewVisible = true;
            this.ToggleDiffView();
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

        public IList CommitItems
        {
            get{ return this.commitItemsTree.CheckedItems; }
            set
            { 
                this.commitItemsTree.Items = value;
                this.commitItemsTree.CheckedItems = value;
                foreach( object item in value )
                    this.diffView.AddPage( item.ToString() );
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
            this.ToggleDiffView();
        }

        private void ToggleDiffView()
        {
            if ( this.diffViewVisible )
            {
                this.diffViewHeight = this.diffView.Height;
                this.diffView.Height = 0;
                this.Height -= this.diffViewHeight;
            }
            else
            {
                this.Height += this.diffViewHeight;
                this.diffView.Height += this.diffViewHeight;
            }

            this.diffViewVisible = ! this.diffViewVisible;

//            if ( this.diffView.Visible )
//            {                
//                this.diffView.Visible = false;
//                this.Height = this.okButton.Top + this.okButton.Height + 40;
//                this.showDiffButton.Text = "Show diff";
//  
//                // reanchor these things to the bottom
//                this.showDiffButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
//                this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
//                this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
//                this.logMessageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
//                    | System.Windows.Forms.AnchorStyles.Left) 
//                    | System.Windows.Forms.AnchorStyles.Right)));
//
//            }
//            else
//            {
//                // these items can no longer anchor to the bottom
//                this.logMessageBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
//                this.showDiffButton.Anchor = AnchorStyles.Left | AnchorStyles.Top;
//                this.okButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
//                this.cancelButton.Anchor = AnchorStyles.Right | AnchorStyles.Top;
//                this.diffView.Top = this.okButton.Top + this.okButton.Height + 10;
//                this.Height += 400;
//                this.diffView.Visible = true;
//                this.showDiffButton.Text = "Hide diff";
//            }

            // Center the dialog vertically
            this.Top = (Screen.FromControl(this).Bounds.Height / 2) - (this.Height/2);

        }

        private void CommitDialog_VisibleChanged(object sender, System.EventArgs e)
        {
            if ( this.Visible && this.logMessageBox.Text == string.Empty )
            {
                ArrayList arr = new ArrayList();
                foreach( object item in this.commitItemsTree.CheckedItems )
                    arr.Add( item.ToString() );
            
                this.LogMessageTemplate.UrlPaths = this.commitItemsTree.UrlPaths;
                this.logMessageBox.Text = this.LogMessageTemplate.PreProcess( arr );
                this.preprocessed = true;
            }
        }

        private void ItemChecked(object sender, TreeViewEventArgs e )
        {
            // don't bother if the 
            if ( ! this.preprocessed )
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

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.diffView = new Ankh.UI.DiffTab();
            this.bottomSplitter = new System.Windows.Forms.Splitter();
            this.commitItemsTree = new Ankh.UI.PathSelectionTreeView();
            this.topSplitter = new System.Windows.Forms.Splitter();
            this.logMessagePanel = new System.Windows.Forms.Panel();
            this.showDiffButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.logMessageBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.logMessagePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // diffView
            // 
            this.diffView.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.diffView.Location = new System.Drawing.Point(0, 341);
            this.diffView.Name = "diffView";
            this.diffView.SelectedIndex = 0;
            this.diffView.Size = new System.Drawing.Size(816, 296);
            this.diffView.TabIndex = 0;
            // 
            // bottomSplitter
            // 
            this.bottomSplitter.BackColor = System.Drawing.SystemColors.Control;
            this.bottomSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bottomSplitter.Location = new System.Drawing.Point(0, 338);
            this.bottomSplitter.MinSize = 100;
            this.bottomSplitter.Name = "bottomSplitter";
            this.bottomSplitter.Size = new System.Drawing.Size(816, 3);
            this.bottomSplitter.TabIndex = 1;
            this.bottomSplitter.TabStop = false;
            this.bottomSplitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitterMoved);
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
            this.commitItemsTree.Size = new System.Drawing.Size(816, 136);
            this.commitItemsTree.TabIndex = 2;
            this.commitItemsTree.UrlPaths = false;
            // 
            // topSplitter
            // 
            this.topSplitter.BackColor = System.Drawing.SystemColors.Control;
            this.topSplitter.Dock = System.Windows.Forms.DockStyle.Top;
            this.topSplitter.Location = new System.Drawing.Point(0, 136);
            this.topSplitter.MinSize = 100;
            this.topSplitter.Name = "topSplitter";
            this.topSplitter.Size = new System.Drawing.Size(816, 3);
            this.topSplitter.TabIndex = 3;
            this.topSplitter.TabStop = false;
            this.topSplitter.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.SplitterMoved);
            // 
            // logMessagePanel
            // 
            this.logMessagePanel.Controls.Add(this.label1);
            this.logMessagePanel.Controls.Add(this.logMessageBox);
            this.logMessagePanel.Controls.Add(this.cancelButton);
            this.logMessagePanel.Controls.Add(this.okButton);
            this.logMessagePanel.Controls.Add(this.showDiffButton);
            this.logMessagePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMessagePanel.Location = new System.Drawing.Point(0, 139);
            this.logMessagePanel.Name = "logMessagePanel";
            this.logMessagePanel.Size = new System.Drawing.Size(816, 199);
            this.logMessagePanel.TabIndex = 4;
            // 
            // showDiffButton
            // 
            this.showDiffButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.showDiffButton.Location = new System.Drawing.Point(16, 168);
            this.showDiffButton.Name = "showDiffButton";
            this.showDiffButton.TabIndex = 0;
            this.showDiffButton.Text = "Show diff";
            this.showDiffButton.Click += new System.EventHandler(this.showDiffButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(632, 168);
            this.okButton.Name = "okButton";
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(728, 168);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            // 
            // logMessageBox
            // 
            this.logMessageBox.AcceptsReturn = true;
            this.logMessageBox.AcceptsTab = true;
            this.logMessageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.logMessageBox.Location = new System.Drawing.Point(0, 24);
            this.logMessageBox.Multiline = true;
            this.logMessageBox.Name = "logMessageBox";
            this.logMessageBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.logMessageBox.Size = new System.Drawing.Size(816, 128);
            this.logMessageBox.TabIndex = 3;
            this.logMessageBox.Text = "";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 4;
            this.label1.Text = "Log message:";
            // 
            // CommitDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(816, 637);
            this.ControlBox = false;
            this.Controls.Add(this.logMessagePanel);
            this.Controls.Add(this.topSplitter);
            this.Controls.Add(this.commitItemsTree);
            this.Controls.Add(this.bottomSplitter);
            this.Controls.Add(this.diffView);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(390, 320);
            this.Name = "CommitDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Commit";
            this.logMessagePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private LogMessageTemplate logMessageTemplate;
       
        private bool preprocessed = false;

        private bool diffViewVisible;
        private int diffViewHeight;
        private Ankh.UI.DiffTab diffView;
        private Ankh.UI.PathSelectionTreeView commitItemsTree;
        private System.Windows.Forms.Button showDiffButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox logMessageBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Splitter bottomSplitter;
        private System.Windows.Forms.Splitter topSplitter;
        private System.Windows.Forms.Panel logMessagePanel;

        private const int MINLOGMESSAGEPANELHEIGHT = 180;
       
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;


        private void commitItemsTree_GetPathInfo(object sender, GetPathInfoEventArgs args)
        {
            args.Path = args.Item.ToString();
        }

        private void SplitterMoved(object sender, System.Windows.Forms.SplitterEventArgs e)
        {
            if ( this.logMessagePanel.Height < MINLOGMESSAGEPANELHEIGHT )
            {
                int heightChange = (MINLOGMESSAGEPANELHEIGHT - this.logMessagePanel.Height) / 2;
                this.diffView.Height -= heightChange;
                this.commitItemsTree.Height -= heightChange;
            }
        }

        [STAThread] 
        public static void Main()
        {
            CommitDialog the = new CommitDialog();
            the.LogMessageTemplate = new LogMessageTemplate();
            the.ShowDialog();
        }
    }
}



