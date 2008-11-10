using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.PathSelector
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
            this.recursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.revisionStartGroupBox = new System.Windows.Forms.GroupBox();
            this.revisionPickerStart = new Ankh.UI.PathSelector.VersionSelector();
            this.revisionEndGroupBox = new System.Windows.Forms.GroupBox();
            this.revisionPickerEnd = new Ankh.UI.PathSelector.VersionSelector();
            this.suppressGroupBox = new System.Windows.Forms.GroupBox();
            this.suppressLabel = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.pathSelectionTreeView = new Ankh.UI.PathSelectionTreeView();
            this.revisionStartGroupBox.SuspendLayout();
            this.revisionEndGroupBox.SuspendLayout();
            this.suppressGroupBox.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.Location = new System.Drawing.Point(3, 3);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            this.recursiveCheckBox.Size = new System.Drawing.Size(88, 24);
            this.recursiveCheckBox.TabIndex = 2;
            this.recursiveCheckBox.Text = "Recursive";
            this.recursiveCheckBox.CheckedChanged += new System.EventHandler(this.RecursiveCheckedChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(177, 4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 24);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(258, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            // 
            // revisionStartGroupBox
            // 
            this.revisionStartGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionStartGroupBox.Controls.Add(this.revisionPickerStart);
            this.revisionStartGroupBox.Location = new System.Drawing.Point(12, 220);
            this.revisionStartGroupBox.Name = "revisionStartGroupBox";
            this.revisionStartGroupBox.Size = new System.Drawing.Size(336, 48);
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
            this.revisionPickerStart.Size = new System.Drawing.Size(330, 29);
            this.revisionPickerStart.TabIndex = 0;
            // 
            // revisionEndGroupBox
            // 
            this.revisionEndGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionEndGroupBox.Controls.Add(this.revisionPickerEnd);
            this.revisionEndGroupBox.Location = new System.Drawing.Point(12, 274);
            this.revisionEndGroupBox.Name = "revisionEndGroupBox";
            this.revisionEndGroupBox.Size = new System.Drawing.Size(336, 48);
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
            this.revisionPickerEnd.Size = new System.Drawing.Size(330, 29);
            this.revisionPickerEnd.TabIndex = 1;
            // 
            // suppressGroupBox
            // 
            this.suppressGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.suppressGroupBox.Controls.Add(this.suppressLabel);
            this.suppressGroupBox.Location = new System.Drawing.Point(12, 328);
            this.suppressGroupBox.Name = "suppressGroupBox";
            this.suppressGroupBox.Size = new System.Drawing.Size(336, 32);
            this.suppressGroupBox.TabIndex = 2;
            this.suppressGroupBox.TabStop = false;
            // 
            // suppressLabel
            // 
            this.suppressLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.suppressLabel.Location = new System.Drawing.Point(6, 13);
            this.suppressLabel.Name = "suppressLabel";
            this.suppressLabel.Size = new System.Drawing.Size(324, 16);
            this.suppressLabel.TabIndex = 0;
            this.suppressLabel.Text = "You can suppress this dialog by holding down the Shift key";
            // 
            // bottomPanel
            // 
            this.bottomPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bottomPanel.Controls.Add(this.recursiveCheckBox);
            this.bottomPanel.Controls.Add(this.okButton);
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Location = new System.Drawing.Point(12, 362);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(336, 33);
            this.bottomPanel.TabIndex = 6;
            // 
            // pathSelectionTreeView
            // 
            this.pathSelectionTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pathSelectionTreeView.CheckBoxes = true;
            this.pathSelectionTreeView.Context = null;
            this.pathSelectionTreeView.Location = new System.Drawing.Point(12, 12);
            this.pathSelectionTreeView.Name = "pathSelectionTreeView";
            this.pathSelectionTreeView.Recursive = false;
            this.pathSelectionTreeView.SingleCheck = false;
            this.pathSelectionTreeView.Size = new System.Drawing.Size(336, 202);
            this.pathSelectionTreeView.TabIndex = 3;
            // 
            // PathSelector
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(360, 397);
            this.ControlBox = false;
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.suppressGroupBox);
            this.Controls.Add(this.revisionStartGroupBox);
            this.Controls.Add(this.pathSelectionTreeView);
            this.Controls.Add(this.revisionEndGroupBox);
            this.MinimumSize = new System.Drawing.Size(100, 36);
            this.Name = "PathSelector";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PathSelector";
            this.revisionStartGroupBox.ResumeLayout(false);
            this.revisionEndGroupBox.ResumeLayout(false);
            this.suppressGroupBox.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
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
        private Ankh.UI.PathSelector.VersionSelector revisionPickerStart;
        protected System.Windows.Forms.GroupBox revisionEndGroupBox;
        private Ankh.UI.PathSelector.VersionSelector revisionPickerEnd;
        protected System.Windows.Forms.Label suppressLabel;
        protected System.Windows.Forms.GroupBox suppressGroupBox;
        private System.Windows.Forms.Panel bottomPanel;
    }
}
