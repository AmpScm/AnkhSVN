using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class LockDialog
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.stealLocksCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.suppressGroupBox = new System.Windows.Forms.GroupBox();
            this.suppressLabel = new System.Windows.Forms.Label();
            this.bottomPanel = new System.Windows.Forms.Panel();
            this.messageTextBox = new System.Windows.Forms.TextBox();
            this.lockMessageGroupBox = new System.Windows.Forms.GroupBox();
            this.pathSelectionTreeView = new Ankh.UI.PathSelectionTreeView();
            this.suppressGroupBox.SuspendLayout();
            this.bottomPanel.SuspendLayout();
            this.lockMessageGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // stealLocksCheckBox
            // 
            this.stealLocksCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.stealLocksCheckBox.Location = new System.Drawing.Point(11, 8);
            this.stealLocksCheckBox.Name = "stealLocksCheckBox";
            this.stealLocksCheckBox.Size = new System.Drawing.Size(129, 24);
            this.stealLocksCheckBox.TabIndex = 5;
            this.stealLocksCheckBox.Text = "&Steal Locks";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(182, 7);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 24);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(263, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 24);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            // 
            // suppressGroupBox
            // 
            this.suppressGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.suppressGroupBox.Controls.Add(this.suppressLabel);
            this.suppressGroupBox.Location = new System.Drawing.Point(12, 3);
            this.suppressGroupBox.Name = "suppressGroupBox";
            this.suppressGroupBox.Size = new System.Drawing.Size(349, 32);
            this.suppressGroupBox.TabIndex = 0;
            this.suppressGroupBox.TabStop = false;
            // 
            // suppressLabel
            // 
            this.suppressLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.suppressLabel.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.suppressLabel.Location = new System.Drawing.Point(8, 11);
            this.suppressLabel.Name = "suppressLabel";
            this.suppressLabel.Size = new System.Drawing.Size(298, 18);
            this.suppressLabel.TabIndex = 0;
            this.suppressLabel.Text = "You can suppress this dialog by holding down the Shift key";
            // 
            // bottomPanel
            // 
            this.bottomPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bottomPanel.Controls.Add(this.stealLocksCheckBox);
            this.bottomPanel.Controls.Add(this.okButton);
            this.bottomPanel.Controls.Add(this.cancelButton);
            this.bottomPanel.Location = new System.Drawing.Point(12, 359);
            this.bottomPanel.Name = "bottomPanel";
            this.bottomPanel.Size = new System.Drawing.Size(349, 36);
            this.bottomPanel.TabIndex = 4;
            // 
            // messageTextBox
            // 
            this.messageTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.messageTextBox.Location = new System.Drawing.Point(11, 18);
            this.messageTextBox.Multiline = true;
            this.messageTextBox.Name = "messageTextBox";
            this.messageTextBox.Size = new System.Drawing.Size(327, 91);
            this.messageTextBox.TabIndex = 3;
            // 
            // lockMessageGroupBox
            // 
            this.lockMessageGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lockMessageGroupBox.Controls.Add(this.messageTextBox);
            this.lockMessageGroupBox.Location = new System.Drawing.Point(12, 240);
            this.lockMessageGroupBox.Name = "lockMessageGroupBox";
            this.lockMessageGroupBox.Size = new System.Drawing.Size(349, 113);
            this.lockMessageGroupBox.TabIndex = 2;
            this.lockMessageGroupBox.TabStop = false;
            this.lockMessageGroupBox.Text = "Lock message";
            // 
            // pathSelectionTreeView
            // 
            this.pathSelectionTreeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pathSelectionTreeView.CheckBoxes = true;
            this.pathSelectionTreeView.Context = null;
            this.pathSelectionTreeView.Location = new System.Drawing.Point(12, 45);
            this.pathSelectionTreeView.Name = "pathSelectionTreeView";
            this.pathSelectionTreeView.Recursive = false;
            this.pathSelectionTreeView.SingleCheck = false;
            this.pathSelectionTreeView.Size = new System.Drawing.Size(349, 183);
            this.pathSelectionTreeView.TabIndex = 1;
            this.pathSelectionTreeView.UrlPaths = false;
            this.pathSelectionTreeView.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.pathSelectionTreeView_AfterCheck);
            // 
            // LockDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(373, 407);
            this.ControlBox = false;
            this.Controls.Add(this.bottomPanel);
            this.Controls.Add(this.lockMessageGroupBox);
            this.Controls.Add(this.suppressGroupBox);
            this.Controls.Add(this.pathSelectionTreeView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MinimumSize = new System.Drawing.Size(100, 36);
            this.Name = "LockDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Lock";
            this.suppressGroupBox.ResumeLayout(false);
            this.bottomPanel.ResumeLayout(false);
            this.lockMessageGroupBox.ResumeLayout(false);
            this.lockMessageGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        protected Ankh.UI.PathSelectionTreeView pathSelectionTreeView;
        private System.Windows.Forms.CheckBox stealLocksCheckBox;
        [CLSCompliant(false)]
        protected System.Windows.Forms.Button okButton;
        [CLSCompliant(false)]
        protected System.Windows.Forms.Button cancelButton;

        protected System.Windows.Forms.Label suppressLabel;
        protected System.Windows.Forms.GroupBox suppressGroupBox;
        private System.Windows.Forms.Panel bottomPanel;

        private System.Windows.Forms.TextBox messageTextBox;
        private System.Windows.Forms.GroupBox lockMessageGroupBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
    }
}
