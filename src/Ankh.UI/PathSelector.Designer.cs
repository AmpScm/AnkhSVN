using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class PathSelector
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pathSelectionTreeView = new Ankh.UI.PathSelectionTreeView();
            this.recursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.revisionStartGroupBox = new System.Windows.Forms.GroupBox();
            this.revisionPickerStart = new Ankh.UI.RevisionPicker();
            this.revisionEndGroupBox = new System.Windows.Forms.GroupBox();
            this.revisionPickerEnd = new Ankh.UI.RevisionPicker();
            this.suppressGroupBox = new System.Windows.Forms.GroupBox();
            this.suppressLabel = new System.Windows.Forms.Label();
            this.revisionStartGroupBox.SuspendLayout();
            this.revisionEndGroupBox.SuspendLayout();
            this.suppressGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // pathSelectionTreeView
            // 
            this.pathSelectionTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.pathSelectionTreeView.CheckBoxes = true;
            this.pathSelectionTreeView.ImageIndex = -1;
            this.pathSelectionTreeView.Items = new object[0];
            this.pathSelectionTreeView.Location = new System.Drawing.Point(0, 0);
            this.pathSelectionTreeView.Name = "pathSelectionTreeView";
            this.pathSelectionTreeView.Recursive = false;
            this.pathSelectionTreeView.SelectedImageIndex = -1;
            this.pathSelectionTreeView.SingleCheck = false;
            this.pathSelectionTreeView.Size = new System.Drawing.Size(368, 240);
            this.pathSelectionTreeView.TabIndex = 3;
            this.pathSelectionTreeView.UrlPaths = false;
            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.recursiveCheckBox.Location = new System.Drawing.Point(8, 388);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            this.recursiveCheckBox.Size = new System.Drawing.Size(88, 24);
            this.recursiveCheckBox.TabIndex = 2;
            this.recursiveCheckBox.Text = "Recursive";
            this.recursiveCheckBox.CheckedChanged += new System.EventHandler(this.RecursiveCheckedChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(208, 392);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 24);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(288, 392);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            // 
            // revisionStartGroupBox
            // 
            this.revisionStartGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionStartGroupBox.Controls.Add(this.revisionPickerStart);
            this.revisionStartGroupBox.Location = new System.Drawing.Point(0, 248);
            this.revisionStartGroupBox.Name = "revisionStartGroupBox";
            this.revisionStartGroupBox.Size = new System.Drawing.Size(368, 48);
            this.revisionStartGroupBox.TabIndex = 0;
            this.revisionStartGroupBox.TabStop = false;
            this.revisionStartGroupBox.Text = "Revision";
            // 
            // revisionPickerStart
            // 
            this.revisionPickerStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPickerStart.Location = new System.Drawing.Point(3, 16);
            this.revisionPickerStart.Name = "revisionPickerStart";
            this.revisionPickerStart.Size = new System.Drawing.Size(362, 29);
            this.revisionPickerStart.TabIndex = 0;
            // 
            // revisionEndGroupBox
            // 
            this.revisionEndGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionEndGroupBox.Controls.Add(this.revisionPickerEnd);
            this.revisionEndGroupBox.Location = new System.Drawing.Point(0, 304);
            this.revisionEndGroupBox.Name = "revisionEndGroupBox";
            this.revisionEndGroupBox.Size = new System.Drawing.Size(368, 48);
            this.revisionEndGroupBox.TabIndex = 1;
            this.revisionEndGroupBox.TabStop = false;
            this.revisionEndGroupBox.Text = "Revision end";
            // 
            // revisionPickerEnd
            // 
            this.revisionPickerEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPickerEnd.Location = new System.Drawing.Point(3, 16);
            this.revisionPickerEnd.Name = "revisionPickerEnd";
            this.revisionPickerEnd.Size = new System.Drawing.Size(362, 29);
            this.revisionPickerEnd.TabIndex = 1;
            // 
            // suppressGroupBox
            // 
            this.suppressGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.suppressGroupBox.Controls.Add(this.suppressLabel);
            this.suppressGroupBox.Location = new System.Drawing.Point(0, 352);
            this.suppressGroupBox.Name = "suppressGroupBox";
            this.suppressGroupBox.Size = new System.Drawing.Size(368, 32);
            this.suppressGroupBox.TabIndex = 2;
            this.suppressGroupBox.TabStop = false;
            // 
            // suppressLabel
            // 
            this.suppressLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.suppressLabel.Location = new System.Drawing.Point(8, 12);
            this.suppressLabel.Name = "suppressLabel";
            this.suppressLabel.Size = new System.Drawing.Size(344, 16);
            this.suppressLabel.TabIndex = 0;
            this.suppressLabel.Text = "You can suppress this dialog by holding down the Shift key";
            // 
            // PathSelector
            // 
            this.AcceptButton = this.okButton;
            this.CancelButton = this.cancelButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(368, 421);
            this.ControlBox = false;
            this.Controls.Add(this.suppressGroupBox);
            this.Controls.Add(this.revisionStartGroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.recursiveCheckBox);
            this.Controls.Add(this.pathSelectionTreeView);
            this.Controls.Add(this.revisionEndGroupBox);
            this.MinimumSize = new System.Drawing.Size(376, 0);
            this.Name = "PathSelector";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PathSelector";
            this.revisionStartGroupBox.ResumeLayout(false);
            this.revisionEndGroupBox.ResumeLayout(false);
            this.suppressGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        protected Ankh.UI.PathSelectionTreeView pathSelectionTreeView;
        protected System.Windows.Forms.CheckBox recursiveCheckBox;
        [CLSCompliant(false)]
        protected System.Windows.Forms.Button okButton;
        [CLSCompliant(false)]
        protected System.Windows.Forms.Button cancelButton;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        protected System.Windows.Forms.GroupBox revisionStartGroupBox;
        private Ankh.UI.RevisionPicker revisionPickerStart;
        protected System.Windows.Forms.GroupBox revisionEndGroupBox;
        private Ankh.UI.RevisionPicker revisionPickerEnd;
        protected System.Windows.Forms.Label suppressLabel;
        protected System.Windows.Forms.GroupBox suppressGroupBox;
    }
}
