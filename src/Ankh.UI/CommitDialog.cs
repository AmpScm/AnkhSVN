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

            
            this.diffView.Visible = false;
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
                this.commitItemsTree.Paths = value;
                this.commitItemsTree.CheckedItems = value;
                foreach( object item in value )
                    this.diffView.AddPage( item.ToString() );
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
            if ( this.diffView.Visible )
            {                
                this.diffView.Visible = false;
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
                this.diffView.Top = this.okButton.Top + this.okButton.Height + 10;
                this.Height += 400;
                this.diffView.Visible = true;
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
                foreach( object item in this.commitItemsTree.CheckedItems )
                    arr.Add( item.ToString() );
            
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.logLabel = new System.Windows.Forms.Label();
            this.showDiffButton = new System.Windows.Forms.Button();
            this.diffView = new Ankh.UI.DiffTab();
            this.logMessageBox = new System.Windows.Forms.RichTextBox();
            this.commitItemsTree = new PathSelectionTreeView();
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
            this.diffView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            this.diffView.Font = new System.Drawing.Font("Courier New", 10F);
            this.diffView.Location = new System.Drawing.Point(0, 312);
            this.diffView.Name = "diffView";
            this.diffView.TabIndex = 10;
            this.diffView.Size = new System.Drawing.Size(814,0);
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
            // commitItemsTree
            // 
            this.commitItemsTree.Location = new System.Drawing.Point(0, 0);
            this.commitItemsTree.Name = "commitItemsTree";
            this.commitItemsTree.Size = new System.Drawing.Size(816, 112);
            this.commitItemsTree.TabIndex = 9;
            // 
            // CommitDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(816, 315);
            this.ControlBox = false;
            this.Controls.Add(this.commitItemsTree);
            this.Controls.Add(this.logMessageBox);
            this.Controls.Add(this.diffView);
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
        private System.Windows.Forms.RichTextBox logMessageBox;
        private LogMessageTemplate logMessageTemplate;
       
        private bool preprocessed = false;
        private Ankh.UI.DiffTab diffView;
        private PathSelectionTreeView commitItemsTree;
       
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

        private void commitItemsTree_GetPathInfo(object sender, GetPathInfoEventArgs args)
        {
            args.Path = args.Item.ToString();
        }
    }

}



