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
    partial class CheckoutDialog
    {

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.revisionGroupBox = new System.Windows.Forms.GroupBox();
            this.urlGroupBox = new System.Windows.Forms.GroupBox();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.localDirGroupBox = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.localDirTextBox = new System.Windows.Forms.TextBox();
            this.nonRecursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.urlBrowse = new System.Windows.Forms.Button();
            this.revisionPicker = new Ankh.UI.PathSelector.VersionSelector();
            this.revisionGroupBox.SuspendLayout();
            this.urlGroupBox.SuspendLayout();
            this.localDirGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // revisionGroupBox
            // 
            this.revisionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionGroupBox.Controls.Add(this.revisionPicker);
            this.revisionGroupBox.Location = new System.Drawing.Point(10, 122);
            this.revisionGroupBox.Name = "revisionGroupBox";
            this.revisionGroupBox.Size = new System.Drawing.Size(483, 58);
            this.revisionGroupBox.TabIndex = 2;
            this.revisionGroupBox.TabStop = false;
            this.revisionGroupBox.Text = "&Revision:";
            // 
            // urlGroupBox
            // 
            this.urlGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.urlGroupBox.Controls.Add(this.urlBrowse);
            this.urlGroupBox.Controls.Add(this.urlTextBox);
            this.urlGroupBox.Location = new System.Drawing.Point(11, 12);
            this.urlGroupBox.Name = "urlGroupBox";
            this.urlGroupBox.Size = new System.Drawing.Size(483, 49);
            this.urlGroupBox.TabIndex = 0;
            this.urlGroupBox.TabStop = false;
            this.urlGroupBox.Text = "&Url:";
            this.urlGroupBox.TextChanged += new System.EventHandler(this.ControlsChanged);
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.urlTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.urlTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.urlTextBox.Location = new System.Drawing.Point(6, 19);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(436, 20);
            this.urlTextBox.TabIndex = 0;
            this.urlTextBox.TextChanged += new System.EventHandler(this.urlTextBox_TextChanged);
            // 
            // localDirGroupBox
            // 
            this.localDirGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.localDirGroupBox.Controls.Add(this.button1);
            this.localDirGroupBox.Controls.Add(this.localDirTextBox);
            this.localDirGroupBox.Location = new System.Drawing.Point(11, 67);
            this.localDirGroupBox.Name = "localDirGroupBox";
            this.localDirGroupBox.Size = new System.Drawing.Size(482, 49);
            this.localDirGroupBox.TabIndex = 1;
            this.localDirGroupBox.TabStop = false;
            this.localDirGroupBox.Text = "&Folder:";
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button1.Location = new System.Drawing.Point(448, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(28, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "&...";
            this.button1.Click += new System.EventHandler(this.BrowseClicked);
            // 
            // localDirTextBox
            // 
            this.localDirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.localDirTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.localDirTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.localDirTextBox.Location = new System.Drawing.Point(6, 16);
            this.localDirTextBox.Name = "localDirTextBox";
            this.localDirTextBox.Size = new System.Drawing.Size(436, 20);
            this.localDirTextBox.TabIndex = 0;
            this.localDirTextBox.TextChanged += new System.EventHandler(this.ControlsChanged);
            // 
            // nonRecursiveCheckBox
            // 
            this.nonRecursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nonRecursiveCheckBox.Location = new System.Drawing.Point(13, 186);
            this.nonRecursiveCheckBox.Name = "nonRecursiveCheckBox";
            this.nonRecursiveCheckBox.Size = new System.Drawing.Size(104, 24);
            this.nonRecursiveCheckBox.TabIndex = 3;
            this.nonRecursiveCheckBox.Text = "&Non-recursive";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(330, 187);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(418, 187);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            // 
            // urlBrowse
            // 
            this.urlBrowse.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.urlBrowse.Location = new System.Drawing.Point(448, 17);
            this.urlBrowse.Name = "urlBrowse";
            this.urlBrowse.Size = new System.Drawing.Size(28, 23);
            this.urlBrowse.TabIndex = 2;
            this.urlBrowse.Text = "&...";
            this.urlBrowse.Click += new System.EventHandler(this.urlBrowse_Click);
            // 
            // revisionPicker
            // 
            this.revisionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPicker.Location = new System.Drawing.Point(7, 19);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(470, 33);
            this.revisionPicker.SvnOrigin = null;
            this.revisionPicker.TabIndex = 0;
            this.revisionPicker.Changed += new System.EventHandler(this.ControlsChanged);
            // 
            // CheckoutDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(507, 222);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.revisionGroupBox);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.nonRecursiveCheckBox);
            this.Controls.Add(this.localDirGroupBox);
            this.Controls.Add(this.urlGroupBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckoutDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Checkout from Subversion";
            this.revisionGroupBox.ResumeLayout(false);
            this.urlGroupBox.ResumeLayout(false);
            this.urlGroupBox.PerformLayout();
            this.localDirGroupBox.ResumeLayout(false);
            this.localDirGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.GroupBox revisionGroupBox;
        private Ankh.UI.PathSelector.VersionSelector revisionPicker;
        private System.Windows.Forms.GroupBox urlGroupBox;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.GroupBox localDirGroupBox;
        private System.Windows.Forms.TextBox localDirTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox nonRecursiveCheckBox;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.Button urlBrowse;
    }
}
