﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    partial class AddWorkingCopyExplorerRootDialog
    {
        #region InitializeComponent
        private void InitializeComponent()
        {
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.workingCopyRootTextBox = new System.Windows.Forms.TextBox();
            this.browseFolderButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(242, 61);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(323, 61);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            // 
            // workingCopyRootTextBox
            // 
            this.workingCopyRootTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.workingCopyRootTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.workingCopyRootTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.workingCopyRootTextBox.Location = new System.Drawing.Point(12, 25);
            this.workingCopyRootTextBox.Name = "workingCopyRootTextBox";
            this.workingCopyRootTextBox.Size = new System.Drawing.Size(349, 20);
            this.workingCopyRootTextBox.TabIndex = 1;
            this.workingCopyRootTextBox.TextChanged += new System.EventHandler(this.workingCopyRootTextBox_TextChanged);
            // 
            // browseFolderButton
            // 
            this.browseFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseFolderButton.Location = new System.Drawing.Point(367, 23);
            this.browseFolderButton.Name = "browseFolderButton";
            this.browseFolderButton.Size = new System.Drawing.Size(31, 23);
            this.browseFolderButton.TabIndex = 2;
            this.browseFolderButton.Text = "...";
            this.browseFolderButton.Click += new System.EventHandler(this.browseFolderButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Path:";
            // 
            // AddWorkingCopyExplorerRootDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(410, 96);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.browseFolderButton);
            this.Controls.Add(this.workingCopyRootTextBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddWorkingCopyExplorerRootDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Browse Working Copy";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private Button okButton;
        private TextBox workingCopyRootTextBox;
        private Button browseFolderButton;
        private Button cancelButton;
        private Label label1;
    }
}
