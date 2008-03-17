using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI
{
    partial class ErrorDialog
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.messageLabel = new System.Windows.Forms.Label();
            this.headerLabel = new System.Windows.Forms.Label();
            this.stackTraceTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.errorReportButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // messageLabel
            // 
            this.messageLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.messageLabel.Location = new System.Drawing.Point(64, 32);
            this.messageLabel.Name = "messageLabel";
            this.messageLabel.Size = new System.Drawing.Size(288, 88);
            this.messageLabel.TabIndex = 50;
            // 
            // headerLabel
            // 
            this.headerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.headerLabel.Location = new System.Drawing.Point(64, 0);
            this.headerLabel.Name = "headerLabel";
            this.headerLabel.Size = new System.Drawing.Size(280, 23);
            this.headerLabel.TabIndex = 1;
            this.headerLabel.Text = "An error occurred";
            // 
            // stackTraceTextBox
            // 
            this.stackTraceTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.stackTraceTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.stackTraceTextBox.Location = new System.Drawing.Point(0, 128);
            this.stackTraceTextBox.Multiline = true;
            this.stackTraceTextBox.Name = "stackTraceTextBox";
            this.stackTraceTextBox.ReadOnly = true;
            this.stackTraceTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.stackTraceTextBox.Size = new System.Drawing.Size(352, 8);
            this.stackTraceTextBox.TabIndex = 2;
            this.stackTraceTextBox.TabStop = false;
            this.stackTraceTextBox.Text = "textBox1";
            this.stackTraceTextBox.Visible = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(272, 152);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(8, 152);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "&Stack trace";
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // errorReportButton
            // 
            this.errorReportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.errorReportButton.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.errorReportButton.Enabled = false;
            this.errorReportButton.Location = new System.Drawing.Point(144, 152);
            this.errorReportButton.Name = "errorReportButton";
            this.errorReportButton.Size = new System.Drawing.Size(112, 23);
            this.errorReportButton.TabIndex = 5;
            this.errorReportButton.Text = "Send error &report...";
            this.errorReportButton.Visible = false;
            // 
            // ErrorDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(352, 183);
            this.ControlBox = false;
            this.Controls.Add(this.errorReportButton);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.stackTraceTextBox);
            this.Controls.Add(this.headerLabel);
            this.Controls.Add(this.messageLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ErrorDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AnkhSVN error";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.TextBox stackTraceTextBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button errorReportButton;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
    }
}
