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

namespace Ankh.UI.SccManagement
{
    partial class AddToSubversion
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddToSubversion));
			this.locationBox = new System.Windows.Forms.GroupBox();
			this.repositoryUrl = new System.Windows.Forms.ComboBox();
			this.createFolderButton = new System.Windows.Forms.Button();
			this.repositoryTree = new Ankh.UI.RepositoryExplorer.RepositoryTreeView();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.localFolder = new System.Windows.Forms.ComboBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.projectBox = new System.Windows.Forms.GroupBox();
			this.projectNameBox = new System.Windows.Forms.TextBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.resultUriBox = new System.Windows.Forms.TextBox();
			this.addTrunk = new System.Windows.Forms.CheckBox();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			this.bodyPanel = new System.Windows.Forms.Panel();
			this.locationBox.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.projectBox.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.bodyPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// locationBox
			// 
			resources.ApplyResources(this.locationBox, "locationBox");
			this.locationBox.Controls.Add(this.repositoryUrl);
			this.locationBox.Controls.Add(this.createFolderButton);
			this.locationBox.Controls.Add(this.repositoryTree);
			this.locationBox.Controls.Add(this.comboBox1);
			this.locationBox.Name = "locationBox";
			this.locationBox.TabStop = false;
			// 
			// repositoryUrl
			// 
			resources.ApplyResources(this.repositoryUrl, "repositoryUrl");
			this.repositoryUrl.FormattingEnabled = true;
			this.repositoryUrl.Name = "repositoryUrl";
			this.repositoryUrl.SelectedIndexChanged += new System.EventHandler(this.repositoryUrl_SelectedIndexChanged);
			this.repositoryUrl.TextUpdate += new System.EventHandler(this.repositoryUrl_TextUpdate);
			// 
			// createFolderButton
			// 
			resources.ApplyResources(this.createFolderButton, "createFolderButton");
			this.createFolderButton.Name = "createFolderButton";
			this.createFolderButton.UseVisualStyleBackColor = true;
			this.createFolderButton.Click += new System.EventHandler(this.createFolderButton_Click);
			// 
			// repositoryTree
			// 
			resources.ApplyResources(this.repositoryTree, "repositoryTree");
			this.repositoryTree.Name = "repositoryTree";
			this.repositoryTree.ShowFiles = false;
			this.repositoryTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// comboBox1
			// 
			this.comboBox1.FormattingEnabled = true;
			resources.ApplyResources(this.comboBox1, "comboBox1");
			this.comboBox1.Name = "comboBox1";
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.localFolder);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// localFolder
			// 
			resources.ApplyResources(this.localFolder, "localFolder");
			this.localFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.localFolder.FormattingEnabled = true;
			this.localFolder.Name = "localFolder";
			this.localFolder.SelectedIndexChanged += new System.EventHandler(this.localFolder_SelectedIndexChanged);
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.CausesValidation = false;
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// projectBox
			// 
			resources.ApplyResources(this.projectBox, "projectBox");
			this.projectBox.Controls.Add(this.projectNameBox);
			this.projectBox.Name = "projectBox";
			this.projectBox.TabStop = false;
			// 
			// projectNameBox
			// 
			resources.ApplyResources(this.projectNameBox, "projectNameBox");
			this.projectNameBox.Name = "projectNameBox";
			this.projectNameBox.TextChanged += new System.EventHandler(this.projectNameBox_TextChanged);
			// 
			// timer1
			// 
			this.timer1.Interval = 350;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Controls.Add(this.resultUriBox);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// resultUriBox
			// 
			resources.ApplyResources(this.resultUriBox, "resultUriBox");
			this.resultUriBox.Name = "resultUriBox";
			this.resultUriBox.ReadOnly = true;
			this.resultUriBox.TabStop = false;
			// 
			// addTrunk
			// 
			resources.ApplyResources(this.addTrunk, "addTrunk");
			this.addTrunk.Name = "addTrunk";
			this.addTrunk.UseVisualStyleBackColor = true;
			this.addTrunk.CheckedChanged += new System.EventHandler(this.addTrunk_CheckedChanged);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// bodyPanel
			// 
			resources.ApplyResources(this.bodyPanel, "bodyPanel");
			this.bodyPanel.Controls.Add(this.addTrunk);
			this.bodyPanel.Controls.Add(this.groupBox2);
			this.bodyPanel.Controls.Add(this.projectBox);
			this.bodyPanel.Controls.Add(this.label1);
			this.bodyPanel.Controls.Add(this.groupBox1);
			this.bodyPanel.Controls.Add(this.locationBox);
			this.bodyPanel.Name = "bodyPanel";
			// 
			// AddToSubversion
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.bodyPanel);
			this.Name = "AddToSubversion";
			this.locationBox.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.projectBox.ResumeLayout(false);
			this.projectBox.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.bodyPanel.ResumeLayout(false);
			this.bodyPanel.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox locationBox;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button createFolderButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox projectBox;
        private System.Windows.Forms.TextBox projectNameBox;
        private System.Windows.Forms.ComboBox repositoryUrl;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox resultUriBox;
        private System.Windows.Forms.CheckBox addTrunk;
        protected internal System.Windows.Forms.ComboBox localFolder;
        internal Ankh.UI.RepositoryExplorer.RepositoryTreeView repositoryTree;
        protected internal System.Windows.Forms.ErrorProvider errorProvider1;
        protected System.Windows.Forms.Panel bodyPanel;
    }
}
