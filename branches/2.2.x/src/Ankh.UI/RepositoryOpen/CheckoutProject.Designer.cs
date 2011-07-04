// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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

namespace Ankh.UI.RepositoryOpen
{
    partial class CheckoutProject
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckoutProject));
			this.projectBox = new System.Windows.Forms.GroupBox();
			this.appendBranch = new System.Windows.Forms.CheckBox();
			this.version = new Ankh.UI.PathSelector.VersionSelector();
			this.checkOutFrom = new System.Windows.Forms.ComboBox();
			this.projectRootLabel = new System.Windows.Forms.Label();
			this.projectLabel = new System.Windows.Forms.Label();
			this.projectUrl = new System.Windows.Forms.TextBox();
			this.projectIcon = new System.Windows.Forms.PictureBox();
			this.locationBox = new System.Windows.Forms.GroupBox();
			this.directory = new System.Windows.Forms.TextBox();
			this.browseDirectoryButton = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.projectBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.projectIcon)).BeginInit();
			this.locationBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// projectBox
			// 
			resources.ApplyResources(this.projectBox, "projectBox");
			this.projectBox.Controls.Add(this.appendBranch);
			this.projectBox.Controls.Add(this.version);
			this.projectBox.Controls.Add(this.checkOutFrom);
			this.projectBox.Controls.Add(this.projectRootLabel);
			this.projectBox.Controls.Add(this.projectLabel);
			this.projectBox.Controls.Add(this.projectUrl);
			this.projectBox.Controls.Add(this.projectIcon);
			this.projectBox.Name = "projectBox";
			this.projectBox.TabStop = false;
			// 
			// appendBranch
			// 
			resources.ApplyResources(this.appendBranch, "appendBranch");
			this.appendBranch.Name = "appendBranch";
			this.appendBranch.UseVisualStyleBackColor = true;
			this.appendBranch.CheckedChanged += new System.EventHandler(this.appendBranch_CheckedChanged);
			// 
			// version
			// 
			resources.ApplyResources(this.version, "version");
			this.version.Name = "version";
			this.version.SvnOrigin = null;
			// 
			// checkOutFrom
			// 
			resources.ApplyResources(this.checkOutFrom, "checkOutFrom");
			this.checkOutFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.checkOutFrom.FormattingEnabled = true;
			this.checkOutFrom.Name = "checkOutFrom";
			// 
			// projectRootLabel
			// 
			resources.ApplyResources(this.projectRootLabel, "projectRootLabel");
			this.projectRootLabel.Name = "projectRootLabel";
			// 
			// projectLabel
			// 
			resources.ApplyResources(this.projectLabel, "projectLabel");
			this.projectLabel.Name = "projectLabel";
			// 
			// projectUrl
			// 
			resources.ApplyResources(this.projectUrl, "projectUrl");
			this.projectUrl.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.projectUrl.Name = "projectUrl";
			this.projectUrl.ReadOnly = true;
			// 
			// projectIcon
			// 
			resources.ApplyResources(this.projectIcon, "projectIcon");
			this.projectIcon.Name = "projectIcon";
			this.projectIcon.TabStop = false;
			// 
			// locationBox
			// 
			resources.ApplyResources(this.locationBox, "locationBox");
			this.locationBox.Controls.Add(this.directory);
			this.locationBox.Controls.Add(this.browseDirectoryButton);
			this.locationBox.Controls.Add(this.label2);
			this.locationBox.Name = "locationBox";
			this.locationBox.TabStop = false;
			// 
			// directory
			// 
			resources.ApplyResources(this.directory, "directory");
			this.directory.Name = "directory";
			// 
			// browseDirectoryButton
			// 
			resources.ApplyResources(this.browseDirectoryButton, "browseDirectoryButton");
			this.browseDirectoryButton.Name = "browseDirectoryButton";
			this.browseDirectoryButton.UseVisualStyleBackColor = true;
			this.browseDirectoryButton.Click += new System.EventHandler(this.browseDirectory_Click);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// CheckoutProject
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.locationBox);
			this.Controls.Add(this.projectBox);
			this.Name = "CheckoutProject";
			this.projectBox.ResumeLayout(false);
			this.projectBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.projectIcon)).EndInit();
			this.locationBox.ResumeLayout(false);
			this.locationBox.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox projectBox;
        private System.Windows.Forms.TextBox projectUrl;
        private System.Windows.Forms.PictureBox projectIcon;
        private System.Windows.Forms.GroupBox locationBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label projectRootLabel;
        private System.Windows.Forms.TextBox directory;
        private System.Windows.Forms.Button browseDirectoryButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox checkOutFrom;
        private System.Windows.Forms.Label projectLabel;
        private Ankh.UI.PathSelector.VersionSelector version;
        private System.Windows.Forms.CheckBox appendBranch;
    }
}
