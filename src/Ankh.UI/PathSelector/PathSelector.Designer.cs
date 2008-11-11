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
            this.suppressGroupBox = new System.Windows.Forms.GroupBox();
            this.suppressLabel = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fromPanel = new System.Windows.Forms.Panel();
            this.toPanel = new System.Windows.Forms.Panel();
            this.revisionPickerEnd = new Ankh.UI.PathSelector.VersionSelector();
            this.revisionPickerStart = new Ankh.UI.PathSelector.VersionSelector();
            this.pathSelectionTreeView = new Ankh.UI.PathSelectionTreeView();
            this.suppressGroupBox.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.fromPanel.SuspendLayout();
            this.toPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.Location = new System.Drawing.Point(3, 3);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            this.recursiveCheckBox.Size = new System.Drawing.Size(88, 24);
            this.recursiveCheckBox.TabIndex = 1;
            this.recursiveCheckBox.Text = "Recursive";
            this.recursiveCheckBox.CheckedChanged += new System.EventHandler(this.RecursiveCheckedChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(237, 4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 24);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(318, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            // 
            // suppressGroupBox
            // 
            this.suppressGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.suppressGroupBox.Controls.Add(this.suppressLabel);
            this.suppressGroupBox.Location = new System.Drawing.Point(12, 328);
            this.suppressGroupBox.Name = "suppressGroupBox";
            this.suppressGroupBox.Size = new System.Drawing.Size(396, 32);
            this.suppressGroupBox.TabIndex = 3;
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
            this.bottomPanel.Size = new System.Drawing.Size(396, 33);
            this.bottomPanel.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&From:";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(0, -1);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "&To:";
            // 
            // fromPanel
            // 
            this.fromPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fromPanel.Controls.Add(this.revisionPickerStart);
            this.fromPanel.Controls.Add(this.label1);
            this.fromPanel.Location = new System.Drawing.Point(9, 232);
            this.fromPanel.Name = "fromPanel";
            this.fromPanel.Size = new System.Drawing.Size(399, 42);
            this.fromPanel.TabIndex = 1;
            // 
            // toPanel
            // 
            this.toPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toPanel.Controls.Add(this.revisionPickerEnd);
            this.toPanel.Controls.Add(this.label2);
            this.toPanel.Location = new System.Drawing.Point(9, 280);
            this.toPanel.Name = "toPanel";
            this.toPanel.Size = new System.Drawing.Size(399, 42);
            this.toPanel.TabIndex = 2;
            // 
            // revisionPickerEnd
            // 
            this.revisionPickerEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPickerEnd.Location = new System.Drawing.Point(4, 15);
            this.revisionPickerEnd.Name = "revisionPickerEnd";
            this.revisionPickerEnd.Size = new System.Drawing.Size(395, 29);
            this.revisionPickerEnd.SvnOrigin = null;
            this.revisionPickerEnd.TabIndex = 1;
            // 
            // revisionPickerStart
            // 
            this.revisionPickerStart.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPickerStart.Location = new System.Drawing.Point(4, 13);
            this.revisionPickerStart.Name = "revisionPickerStart";
            this.revisionPickerStart.Size = new System.Drawing.Size(395, 29);
            this.revisionPickerStart.SvnOrigin = null;
            this.revisionPickerStart.TabIndex = 1;
            // 
            // pathSelectionTreeView
            // 
            this.pathSelectionTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pathSelectionTreeView.CheckBoxes = true;
            this.pathSelectionTreeView.Context = null;
            this.pathSelectionTreeView.Location = new System.Drawing.Point(9, 12);
            this.pathSelectionTreeView.Name = "pathSelectionTreeView";
            this.pathSelectionTreeView.Recursive = false;
            this.pathSelectionTreeView.SingleCheck = false;
            this.pathSelectionTreeView.Size = new System.Drawing.Size(396, 214);
            this.pathSelectionTreeView.TabIndex = 0;
            // 
            // PathSelector
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(420, 397);
            this.ControlBox = false;
            this.Controls.Add(this.toPanel);
            this.Controls.Add(this.fromPanel);
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.suppressGroupBox);
            this.Controls.Add(this.pathSelectionTreeView);
            this.MinimumSize = new System.Drawing.Size(100, 36);
            this.Name = "PathSelector";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "PathSelector";
            this.suppressGroupBox.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.fromPanel.ResumeLayout(false);
            this.fromPanel.PerformLayout();
            this.toPanel.ResumeLayout(false);
            this.toPanel.PerformLayout();
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
        private Ankh.UI.PathSelector.VersionSelector revisionPickerStart;
        private Ankh.UI.PathSelector.VersionSelector revisionPickerEnd;
        protected System.Windows.Forms.Label suppressLabel;
        protected System.Windows.Forms.GroupBox suppressGroupBox;
        private System.Windows.Forms.Panel bottomPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel fromPanel;
        private System.Windows.Forms.Panel toPanel;
    }
}
