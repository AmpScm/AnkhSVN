using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class ConflictDialog
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.mineFileRadioButton = new System.Windows.Forms.RadioButton();
            this.oldRevRadioButton = new System.Windows.Forms.RadioButton();
            this.newRevRadioButton = new System.Windows.Forms.RadioButton();
            this.fileRadioButton = new System.Windows.Forms.RadioButton();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mineFileRadioButton
            // 
            this.mineFileRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mineFileRadioButton.Checked = true;
            this.mineFileRadioButton.Location = new System.Drawing.Point(8, 96);
            this.mineFileRadioButton.Name = "mineFileRadioButton";
            this.mineFileRadioButton.Size = new System.Drawing.Size(280, 24);
            this.mineFileRadioButton.TabIndex = 0;
            this.mineFileRadioButton.TabStop = true;
            this.mineFileRadioButton.Text = "test.mine";
            this.toolTip.SetToolTip(this.mineFileRadioButton, "Latest local file");
            this.mineFileRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // oldRevRadioButton
            // 
            this.oldRevRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.oldRevRadioButton.Location = new System.Drawing.Point(8, 48);
            this.oldRevRadioButton.Name = "oldRevRadioButton";
            this.oldRevRadioButton.Size = new System.Drawing.Size(280, 24);
            this.oldRevRadioButton.TabIndex = 1;
            this.oldRevRadioButton.Text = "test.r1";
            this.toolTip.SetToolTip(this.oldRevRadioButton, "Latest updated revision");
            this.oldRevRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // newRevRadioButton
            // 
            this.newRevRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.newRevRadioButton.Location = new System.Drawing.Point(8, 72);
            this.newRevRadioButton.Name = "newRevRadioButton";
            this.newRevRadioButton.Size = new System.Drawing.Size(280, 24);
            this.newRevRadioButton.TabIndex = 2;
            this.newRevRadioButton.Text = "test.r2";
            this.toolTip.SetToolTip(this.newRevRadioButton, "Latest version in repository");
            this.newRevRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // fileRadioButton
            // 
            this.fileRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileRadioButton.Location = new System.Drawing.Point(8, 24);
            this.fileRadioButton.Name = "fileRadioButton";
            this.fileRadioButton.Size = new System.Drawing.Size(280, 24);
            this.fileRadioButton.TabIndex = 3;
            this.fileRadioButton.Text = "test.txt";
            this.toolTip.SetToolTip(this.fileRadioButton, "File with conflict markers");
            this.fileRadioButton.Click += new System.EventHandler(this.selectedButton);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(228, 145);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.toolTip.SetToolTip(this.cancelButton, "Cancel this dialog.");
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(148, 145);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "Resolve";
            this.toolTip.SetToolTip(this.okButton, "Resolve conflict and delete the three files that are not selected.");
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.oldRevRadioButton);
            this.groupBox1.Controls.Add(this.newRevRadioButton);
            this.groupBox1.Controls.Add(this.fileRadioButton);
            this.groupBox1.Controls.Add(this.mineFileRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(296, 128);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Select file";
            // 
            // ConflictDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(312, 176);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(312, 192);
            this.Name = "ConflictDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Resolve conflicted file";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.RadioButton mineFileRadioButton;
        private System.Windows.Forms.RadioButton fileRadioButton;
        private System.Windows.Forms.RadioButton oldRevRadioButton;
        private System.Windows.Forms.RadioButton newRevRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;

        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ToolTip toolTip;
    }
}
