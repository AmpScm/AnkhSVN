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
            this.newFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newFolderButton.Enabled = false;
            this.newFolderButton.Location = new System.Drawing.Point(11, 319);
            this.newFolderButton.Name = "newFolderButton";
            this.newFolderButton.Size = new System.Drawing.Size(104, 23);
            this.newFolderButton.TabIndex = 0;
            this.newFolderButton.Text = "&Make New Folder";
            this.newFolderButton.UseVisualStyleBackColor = true;
            this.newFolderButton.Visible = false;
            this.newFolderButton.Click += new System.EventHandler(this.newFolderButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(242, 319);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(323, 319);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // urlLabel
            // 
            this.urlLabel.AutoSize = true;
            this.urlLabel.Location = new System.Drawing.Point(12, 12);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(23, 13);
            this.urlLabel.TabIndex = 2;
            this.urlLabel.Text = "&Url:";
            this.urlLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // urlBox
            // 
            this.urlBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.urlBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.urlBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.urlBox.FormattingEnabled = true;
            this.urlBox.Location = new System.Drawing.Point(38, 9);
            this.urlBox.Name = "urlBox";
            this.urlBox.Size = new System.Drawing.Size(360, 21);
            this.urlBox.TabIndex = 3;
            this.urlBox.Leave += new System.EventHandler(this.urlBox_Leave);
            this.urlBox.TextUpdate += new System.EventHandler(this.urlBox_TextUpdate);
            this.urlBox.TextChanged += new System.EventHandler(this.urlBox_TextChanged);
            // 
            // reposBrowser
            // 
            this.reposBrowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.reposBrowser.Context = null;
            this.reposBrowser.Location = new System.Drawing.Point(12, 36);
            this.reposBrowser.Name = "reposBrowser";
            this.reposBrowser.Size = new System.Drawing.Size(386, 269);
            this.reposBrowser.TabIndex = 2;
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(410, 354);
            this.Controls.Add(this.newFolderButton);
            this.Controls.Add(this.reposBrowser);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.urlBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.urlLabel);
            this.Name = "RepositoryFolderBrowserDialog";
            this.Text = "Select Url";
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
