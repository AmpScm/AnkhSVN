using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using NSvn.Core;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog that allows the user to perform the Switch command.
    /// </summary>
    public class SwitchDialog : PathSelector
    {
        

        public SwitchDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.SingleSelection = true;
            this.TreeView.AfterCheck += new TreeViewEventHandler(AfterCheck);
            this.RevisionPickerStart.Changed += new EventHandler(RevisionPickerStart_Changed);
        }

        public string ToUrl
        {
            get{ return this.urlTextBox.Text; }
        }

        public string SelectedPath
        {
            get
            { 
                if ( this.TreeView.CheckedItems.Count > 0 )
                    return this.TreeView.CheckedItems[0].ToString();
                else 
                    return null;
                
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

        private void AfterCheck(object sender, TreeViewEventArgs e)
        {
            IList items = this.TreeView.CheckedItems;
            if ( items.Count > 0 )
            {
                GetPathInfoEventArgs args = new GetPathInfoEventArgs( items[0] );
                this.RaiseGetPathInfo( args );
                this.currentUrl = args.Path;
                this.urlTextBox.Text = this.currentUrl;
                this.urlTextBox.Enabled = true;
            }
            else
            {
                this.currentUrl = null;
                this.urlTextBox.Enabled = false;             
            }

            this.ValidateDialog();
    
        }
       
        private void urlTextBox_TextChanged(object sender, System.EventArgs e)
        {
            this.ValidateDialog();
        }

        private void RevisionPickerStart_Changed(object sender, EventArgs e)
        {
            this.ValidateDialog();
        }

        private void ValidateDialog()
        {
            bool hasUrl = this.currentUrl != null;
            bool userEnteredUrl = (this.currentUrl != this.urlTextBox.Text &&
                        this.urlTextBox.Text != String.Empty);
            bool hasNewRevision = this.RevisionPickerStart.Revision != Revision.Head; 
            if ( hasUrl && (userEnteredUrl || hasNewRevision) )
            {
                this.OkButton.Enabled = true;
            }
            else
            {
                this.OkButton.Enabled = false;
            }
        }

        
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.urlGroupBox = new System.Windows.Forms.GroupBox();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.suppressGroupBox.SuspendLayout();
            this.urlGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // pathSelectionTreeView
            // 
            this.pathSelectionTreeView.Name = "pathSelectionTreeView";
            this.pathSelectionTreeView.Size = new System.Drawing.Size(362, 240);
            this.pathSelectionTreeView.TabIndex = 6;
            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.recursiveCheckBox.Location = new System.Drawing.Point(5, 424);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.okButton.Location = new System.Drawing.Point(205, 424);
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cancelButton.Location = new System.Drawing.Point(285, 424);
            this.cancelButton.Name = "cancelButton";
            // 
            // revisionStartGroupBox
            // 
            this.revisionStartGroupBox.Name = "revisionStartGroupBox";
            this.revisionStartGroupBox.Size = new System.Drawing.Size(362, 48);
            this.revisionStartGroupBox.TabIndex = 1;
            // 
            // revisionEndGroupBox
            // 
            this.revisionEndGroupBox.Name = "revisionEndGroupBox";
            this.revisionEndGroupBox.Size = new System.Drawing.Size(362, 48);
            // 
            // suppressGroupBox
            // 
            this.suppressGroupBox.Location = new System.Drawing.Point(0, 384);
            this.suppressGroupBox.Name = "suppressGroupBox";
            this.suppressGroupBox.Size = new System.Drawing.Size(370, 32);
            this.suppressGroupBox.Visible = false;
            // 
            // suppressLabel
            // 
            this.suppressLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.suppressLabel.Location = new System.Drawing.Point(5, 8);
            this.suppressLabel.Name = "suppressLabel";
            // 
            // urlGroupBox
            // 
            this.urlGroupBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.urlGroupBox.Controls.Add(this.urlTextBox);
            this.urlGroupBox.Location = new System.Drawing.Point(-3, 341);
            this.urlGroupBox.Name = "urlGroupBox";
            this.urlGroupBox.Size = new System.Drawing.Size(368, 48);
            this.urlGroupBox.TabIndex = 2;
            this.urlGroupBox.TabStop = false;
            this.urlGroupBox.Text = "Switch to";
            // 
            // urlTextBox
            // 
            this.urlTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.urlTextBox.Location = new System.Drawing.Point(3, 16);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(362, 20);
            this.urlTextBox.TabIndex = 0;
            this.urlTextBox.Text = "";
            this.urlTextBox.TextChanged += new EventHandler(urlTextBox_TextChanged);
            // 
            // SwitchDialog
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.Caption = "Switch";
            this.ClientSize = new System.Drawing.Size(362, 455);
            this.Controls.Add(this.urlGroupBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimumSize = new System.Drawing.Size(0, 0);
            this.Name = "SwitchDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Switch";
            this.Controls.SetChildIndex(this.revisionEndGroupBox, 0);
            this.Controls.SetChildIndex(this.suppressGroupBox, 0);
            this.Controls.SetChildIndex(this.revisionStartGroupBox, 0);
            this.Controls.SetChildIndex(this.pathSelectionTreeView, 0);
            this.Controls.SetChildIndex(this.recursiveCheckBox, 0);
            this.Controls.SetChildIndex(this.okButton, 0);
            this.Controls.SetChildIndex(this.cancelButton, 0);
            this.Controls.SetChildIndex(this.urlGroupBox, 0);
            this.suppressGroupBox.ResumeLayout(false);
            this.urlGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.GroupBox urlGroupBox;
        private System.Windows.Forms.TextBox urlTextBox;
        private string currentUrl;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        
    }
}
