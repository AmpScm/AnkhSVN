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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.version = new Ankh.UI.PathSelector.VersionSelector();
            this.checkOutFrom = new System.Windows.Forms.ComboBox();
            this.projectRootLabel = new System.Windows.Forms.Label();
            this.projectLabel = new System.Windows.Forms.Label();
            this.projectUrl = new System.Windows.Forms.TextBox();
            this.projectIcon = new System.Windows.Forms.PictureBox();
            this.locationIcon = new System.Windows.Forms.GroupBox();
            this.directory = new System.Windows.Forms.TextBox();
            this.browseDirectoryButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.projectIcon)).BeginInit();
            this.locationIcon.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.version);
            this.groupBox1.Controls.Add(this.checkOutFrom);
            this.groupBox1.Controls.Add(this.projectRootLabel);
            this.groupBox1.Controls.Add(this.projectLabel);
            this.groupBox1.Controls.Add(this.projectUrl);
            this.groupBox1.Controls.Add(this.projectIcon);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(525, 102);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Project:";
            // 
            // version
            // 
            this.version.Location = new System.Drawing.Point(71, 63);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(449, 29);
            this.version.SvnOrigin = null;
            this.version.TabIndex = 1;
            // 
            // checkOutFrom
            // 
            this.checkOutFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.checkOutFrom.FormattingEnabled = true;
            this.checkOutFrom.Location = new System.Drawing.Point(71, 36);
            this.checkOutFrom.Name = "checkOutFrom";
            this.checkOutFrom.Size = new System.Drawing.Size(448, 21);
            this.checkOutFrom.TabIndex = 2;
            // 
            // projectRootLabel
            // 
            this.projectRootLabel.AutoSize = true;
            this.projectRootLabel.Location = new System.Drawing.Point(6, 39);
            this.projectRootLabel.Name = "projectRootLabel";
            this.projectRootLabel.Size = new System.Drawing.Size(33, 13);
            this.projectRootLabel.TabIndex = 1;
            this.projectRootLabel.Text = "&From:";
            // 
            // projectLabel
            // 
            this.projectLabel.AutoSize = true;
            this.projectLabel.Location = new System.Drawing.Point(6, 18);
            this.projectLabel.Name = "projectLabel";
            this.projectLabel.Size = new System.Drawing.Size(43, 13);
            this.projectLabel.TabIndex = 0;
            this.projectLabel.Text = "Project:";
            // 
            // projectUrl
            // 
            this.projectUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.projectUrl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.projectUrl.Location = new System.Drawing.Point(93, 16);
            this.projectUrl.Name = "projectUrl";
            this.projectUrl.ReadOnly = true;
            this.projectUrl.Size = new System.Drawing.Size(426, 13);
            this.projectUrl.TabIndex = 0;
            this.projectUrl.Text = "http://ankhsvn.open.collab.net/svn/ankhsvn/trunk/AnkhSvn.2008.sln";
            // 
            // projectIcon
            // 
            this.projectIcon.Location = new System.Drawing.Point(71, 15);
            this.projectIcon.Name = "projectIcon";
            this.projectIcon.Size = new System.Drawing.Size(16, 16);
            this.projectIcon.TabIndex = 0;
            this.projectIcon.TabStop = false;
            // 
            // locationIcon
            // 
            this.locationIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.locationIcon.Controls.Add(this.directory);
            this.locationIcon.Controls.Add(this.browseDirectoryButton);
            this.locationIcon.Controls.Add(this.label2);
            this.locationIcon.Location = new System.Drawing.Point(12, 120);
            this.locationIcon.Name = "locationIcon";
            this.locationIcon.Size = new System.Drawing.Size(525, 50);
            this.locationIcon.TabIndex = 1;
            this.locationIcon.TabStop = false;
            this.locationIcon.Text = "&Local Directory:";
            // 
            // directory
            // 
            this.directory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.directory.Location = new System.Drawing.Point(71, 19);
            this.directory.Name = "directory";
            this.directory.Size = new System.Drawing.Size(417, 20);
            this.directory.TabIndex = 1;
            this.directory.Text = "C:\\Users\\SomeOne\\My Visual Studio Projects\\AnkhSvn";
            // 
            // browseDirectoryButton
            // 
            this.browseDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseDirectoryButton.Location = new System.Drawing.Point(493, 16);
            this.browseDirectoryButton.Name = "browseDirectoryButton";
            this.browseDirectoryButton.Size = new System.Drawing.Size(27, 25);
            this.browseDirectoryButton.TabIndex = 2;
            this.browseDirectoryButton.Text = "...";
            this.browseDirectoryButton.UseVisualStyleBackColor = true;
            this.browseDirectoryButton.Click += new System.EventHandler(this.browseDirectory_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Directory:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(462, 184);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(381, 184);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // CheckoutProject
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(549, 219);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.locationIcon);
            this.Controls.Add(this.groupBox1);
            this.Name = "CheckoutProject";
            this.Text = "Open Project from Subversion";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.projectIcon)).EndInit();
            this.locationIcon.ResumeLayout(false);
            this.locationIcon.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox projectUrl;
        private System.Windows.Forms.PictureBox projectIcon;
        private System.Windows.Forms.GroupBox locationIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label projectRootLabel;
        private System.Windows.Forms.TextBox directory;
        private System.Windows.Forms.Button browseDirectoryButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox checkOutFrom;
        private System.Windows.Forms.Label projectLabel;
        private Ankh.UI.PathSelector.VersionSelector version;
    }
}
