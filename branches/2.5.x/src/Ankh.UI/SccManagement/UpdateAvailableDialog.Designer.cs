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

namespace Ankh.UI.SccManagement
{
    partial class UpdateAvailableDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateAvailableDialog));
			this.panel1 = new System.Windows.Forms.Panel();
			this.bodyLabel = new System.Windows.Forms.Label();
			this.linkLabel = new System.Windows.Forms.LinkLabel();
			this.versionPanel = new System.Windows.Forms.Panel();
			this.newVerLabel = new System.Windows.Forms.Label();
			this.curVerLabel = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.headLabel = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.sameCheck = new System.Windows.Forms.CheckBox();
			this.panel1.SuspendLayout();
			this.versionPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.Controls.Add(this.bodyLabel);
			this.panel1.Controls.Add(this.linkLabel);
			this.panel1.Controls.Add(this.versionPanel);
			this.panel1.Controls.Add(this.headLabel);
			this.panel1.Name = "panel1";
			// 
			// bodyLabel
			// 
			resources.ApplyResources(this.bodyLabel, "bodyLabel");
			this.bodyLabel.Name = "bodyLabel";
			// 
			// linkLabel
			// 
			resources.ApplyResources(this.linkLabel, "linkLabel");
			this.linkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
			this.linkLabel.Name = "linkLabel";
			this.linkLabel.TabStop = true;
			this.linkLabel.UseMnemonic = false;
			this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
			// 
			// versionPanel
			// 
			resources.ApplyResources(this.versionPanel, "versionPanel");
			this.versionPanel.Controls.Add(this.newVerLabel);
			this.versionPanel.Controls.Add(this.curVerLabel);
			this.versionPanel.Controls.Add(this.label2);
			this.versionPanel.Controls.Add(this.label1);
			this.versionPanel.Name = "versionPanel";
			// 
			// newVerLabel
			// 
			resources.ApplyResources(this.newVerLabel, "newVerLabel");
			this.newVerLabel.Name = "newVerLabel";
			this.newVerLabel.UseMnemonic = false;
			// 
			// curVerLabel
			// 
			resources.ApplyResources(this.curVerLabel, "curVerLabel");
			this.curVerLabel.Name = "curVerLabel";
			this.curVerLabel.UseMnemonic = false;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// headLabel
			// 
			resources.ApplyResources(this.headLabel, "headLabel");
			this.headLabel.Name = "headLabel";
			this.headLabel.UseMnemonic = false;
			// 
			// okButton
			// 
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// sameCheck
			// 
			resources.ApplyResources(this.sameCheck, "sameCheck");
			this.sameCheck.Name = "sameCheck";
			this.sameCheck.UseVisualStyleBackColor = true;
			// 
			// UpdateAvailableDialog
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.okButton;
			this.Controls.Add(this.sameCheck);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.panel1);
			this.Name = "UpdateAvailableDialog";
			this.panel1.ResumeLayout(false);
			this.versionPanel.ResumeLayout(false);
			this.versionPanel.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button okButton;
        public System.Windows.Forms.Label headLabel;
        public System.Windows.Forms.LinkLabel linkLabel;
        public System.Windows.Forms.Panel versionPanel;
        public System.Windows.Forms.Label newVerLabel;
        public System.Windows.Forms.Label curVerLabel;
        public System.Windows.Forms.Label bodyLabel;
        public System.Windows.Forms.CheckBox sameCheck;
    }
}
