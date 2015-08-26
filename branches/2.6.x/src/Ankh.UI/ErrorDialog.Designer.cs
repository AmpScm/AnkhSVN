// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ErrorDialog));
            this.messageLabel = new System.Windows.Forms.Label();
            this.headerLabel = new System.Windows.Forms.Label();
            this.stackTraceTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.showStackTraceBtn = new System.Windows.Forms.Button();
            this.errorReportButton = new System.Windows.Forms.Button();
            this.whitePanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.whitePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // messageLabel
            // 
            resources.ApplyResources(this.messageLabel, "messageLabel");
            this.messageLabel.Name = "messageLabel";
            // 
            // headerLabel
            // 
            resources.ApplyResources(this.headerLabel, "headerLabel");
            this.headerLabel.Name = "headerLabel";
            // 
            // stackTraceTextBox
            // 
            resources.ApplyResources(this.stackTraceTextBox, "stackTraceTextBox");
            this.stackTraceTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.stackTraceTextBox.Name = "stackTraceTextBox";
            this.stackTraceTextBox.ReadOnly = true;
            this.stackTraceTextBox.TabStop = false;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // showStackTraceBtn
            // 
            resources.ApplyResources(this.showStackTraceBtn, "showStackTraceBtn");
            this.showStackTraceBtn.Name = "showStackTraceBtn";
            this.showStackTraceBtn.Click += new System.EventHandler(this.showStackTraceBtn_Click);
            // 
            // errorReportButton
            // 
            resources.ApplyResources(this.errorReportButton, "errorReportButton");
            this.errorReportButton.DialogResult = System.Windows.Forms.DialogResult.Retry;
            this.errorReportButton.Name = "errorReportButton";
            // 
            // whitePanel
            // 
            resources.ApplyResources(this.whitePanel, "whitePanel");
            this.whitePanel.BackColor = System.Drawing.SystemColors.Window;
            this.whitePanel.Controls.Add(this.pictureBox1);
            this.whitePanel.Controls.Add(this.stackTraceTextBox);
            this.whitePanel.Controls.Add(this.messageLabel);
            this.whitePanel.Controls.Add(this.headerLabel);
            this.whitePanel.Name = "whitePanel";
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // ErrorDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.okButton;
            this.Controls.Add(this.errorReportButton);
            this.Controls.Add(this.showStackTraceBtn);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.whitePanel);
            this.Name = "ErrorDialog";
            this.whitePanel.ResumeLayout(false);
            this.whitePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Label messageLabel;
        private System.Windows.Forms.Label headerLabel;
        private System.Windows.Forms.TextBox stackTraceTextBox;
        private System.Windows.Forms.Button showStackTraceBtn;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button errorReportButton;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Panel whitePanel;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}
