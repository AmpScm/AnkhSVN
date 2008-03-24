using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    partial class UpdateDialog
    {

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.pathSelectionTreeView = new Ankh.UI.PathSelectionTreeView();
            this.revisionGroupBox = new System.Windows.Forms.GroupBox();
            this.revisionPicker = new Ankh.UI.RevisionPicker();
            this.recursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.revisionGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(208, 386);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "&Update";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(294, 386);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "&Cancel";
            // 
            // pathSelectionTreeView
            // 
            this.pathSelectionTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pathSelectionTreeView.CheckBoxes = true;
            this.pathSelectionTreeView.Items = new object[0];
            this.pathSelectionTreeView.Location = new System.Drawing.Point(0, 28);
            this.pathSelectionTreeView.Name = "pathSelectionTreeView";
            this.pathSelectionTreeView.Recursive = false;
            this.pathSelectionTreeView.SingleCheck = false;
            this.pathSelectionTreeView.Size = new System.Drawing.Size(376, 214);
            this.pathSelectionTreeView.TabIndex = 1;
            this.pathSelectionTreeView.UrlPaths = false;
            // 
            // revisionGroupBox
            // 
            this.revisionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionGroupBox.Controls.Add(this.revisionPicker);
            this.revisionGroupBox.Location = new System.Drawing.Point(8, 250);
            this.revisionGroupBox.Name = "revisionGroupBox";
            this.revisionGroupBox.Size = new System.Drawing.Size(360, 72);
            this.revisionGroupBox.TabIndex = 2;
            this.revisionGroupBox.TabStop = false;
            this.revisionGroupBox.Text = "&Revision";
            // 
            // revisionPicker
            // 
            this.revisionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPicker.Location = new System.Drawing.Point(8, 32);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(344, 24);
            this.revisionPicker.TabIndex = 0;
            // 
            // recursiveCheckBox
            // 
            this.recursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.recursiveCheckBox.Location = new System.Drawing.Point(10, 386);
            this.recursiveCheckBox.Name = "recursiveCheckBox";
            this.recursiveCheckBox.Size = new System.Drawing.Size(78, 24);
            this.recursiveCheckBox.TabIndex = 3;
            this.recursiveCheckBox.Text = "Recursi&ve";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(8, 338);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(360, 40);
            this.label1.TabIndex = 5;
            this.label1.Text = "You can suppress this dialog by holding down the Shift key while clicking Update";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Paths";
            // 
            // UpdateDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(376, 415);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.recursiveCheckBox);
            this.Controls.Add(this.revisionGroupBox);
            this.Controls.Add(this.pathSelectionTreeView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.MinimumSize = new System.Drawing.Size(266, 280);
            this.Name = "UpdateDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Update";
            this.revisionGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.GroupBox revisionGroupBox;
        private Ankh.UI.RevisionPicker revisionPicker;
        private Ankh.UI.PathSelectionTreeView pathSelectionTreeView;
        private System.Windows.Forms.CheckBox recursiveCheckBox;
        private System.Windows.Forms.Label label1;
        private Label label2;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
    }
}
