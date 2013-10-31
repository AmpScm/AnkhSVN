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

namespace Ankh.UI.RepositoryExplorer
{
    partial class RepositoryFolderBrowserDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepositoryFolderBrowserDialog));
			this.newFolderButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.urlLabel = new System.Windows.Forms.Label();
			this.urlBox = new System.Windows.Forms.ComboBox();
			this.reposBrowser = new Ankh.UI.RepositoryExplorer.RepositoryTreeView();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// newFolderButton
			// 
			resources.ApplyResources(this.newFolderButton, "newFolderButton");
			this.newFolderButton.Name = "newFolderButton";
			this.newFolderButton.UseVisualStyleBackColor = true;
			this.newFolderButton.Click += new System.EventHandler(this.newFolderButton_Click);
			// 
			// okButton
			// 
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// urlLabel
			// 
			resources.ApplyResources(this.urlLabel, "urlLabel");
			this.urlLabel.Name = "urlLabel";
			// 
			// urlBox
			// 
			resources.ApplyResources(this.urlBox, "urlBox");
			this.urlBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.urlBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.urlBox.FormattingEnabled = true;
			this.urlBox.Name = "urlBox";
			this.urlBox.Leave += new System.EventHandler(this.urlBox_Leave);
			this.urlBox.TextUpdate += new System.EventHandler(this.urlBox_TextUpdate);
			this.urlBox.TextChanged += new System.EventHandler(this.urlBox_TextChanged);
			// 
			// reposBrowser
			// 
			resources.ApplyResources(this.reposBrowser, "reposBrowser");
			this.reposBrowser.Name = "reposBrowser";
			this.reposBrowser.ShowFiles = false;
			this.reposBrowser.RetrievingChanged += new System.EventHandler(this.reposBrowser_RetrievingChanged);
			this.reposBrowser.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.reposBrowser_AfterSelect);
			// 
			// timer
			// 
			this.timer.Interval = 500;
			this.timer.Tick += new System.EventHandler(this.timer_Tick);
			// 
			// RepositoryFolderBrowserDialog
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.newFolderButton);
			this.Controls.Add(this.reposBrowser);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.urlBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.urlLabel);
			this.Name = "RepositoryFolderBrowserDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button newFolderButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private Ankh.UI.RepositoryExplorer.RepositoryTreeView reposBrowser;
        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.ComboBox urlBox;
        private System.Windows.Forms.Timer timer;
    }
}
