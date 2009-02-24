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

namespace Ankh.UI.Commands
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
            this.components = new System.ComponentModel.Container();
            this.urlGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.urlBrowse = new System.Windows.Forms.Button();
            this.urlTextBox = new System.Windows.Forms.TextBox();
            this.localDirGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.localDirTextBox = new System.Windows.Forms.TextBox();
            this.nonRecursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.omitExternalsCheckBox = new System.Windows.Forms.CheckBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.revisionPicker = new Ankh.UI.PathSelector.VersionSelector();
            this.urlGroupBox.SuspendLayout();
            this.localDirGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // urlGroupBox
            // 
            this.urlGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.urlGroupBox.Controls.Add(this.label1);
            this.urlGroupBox.Controls.Add(this.revisionPicker);
            this.urlGroupBox.Controls.Add(this.urlBrowse);
            this.urlGroupBox.Controls.Add(this.urlTextBox);
            this.urlGroupBox.Location = new System.Drawing.Point(11, 12);
            this.urlGroupBox.Name = "urlGroupBox";
            this.urlGroupBox.Size = new System.Drawing.Size(515, 80);
            this.urlGroupBox.TabIndex = 0;
            this.urlGroupBox.TabStop = false;
            this.urlGroupBox.Text = "Check Out";
            this.urlGroupBox.TextChanged += new System.EventHandler(this.ControlsChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Url:";
            // 
            // urlBrowse
            // 
            this.urlBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.urlBrowse.CausesValidation = false;
            this.urlBrowse.Location = new System.Drawing.Point(480, 17);
            this.urlBrowse.Name = "urlBrowse";
            this.urlBrowse.Size = new System.Drawing.Size(28, 23);
            this.urlBrowse.TabIndex = 2;
            this.urlBrowse.Text = "...";
            this.urlBrowse.Click += new System.EventHandler(this.urlBrowse_Click);
            // 
            // urlTextBox
            // 
            this.urlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.urlTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.urlTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.urlTextBox.Location = new System.Drawing.Point(52, 19);
            this.urlTextBox.Name = "urlTextBox";
            this.urlTextBox.Size = new System.Drawing.Size(422, 20);
            this.urlTextBox.TabIndex = 1;
            this.urlTextBox.TextChanged += new System.EventHandler(this.urlTextBox_TextChanged);
            this.urlTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.urlTextBox_Validating);
            // 
            // localDirGroupBox
            // 
            this.localDirGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.localDirGroupBox.Controls.Add(this.label2);
            this.localDirGroupBox.Controls.Add(this.button1);
            this.localDirGroupBox.Controls.Add(this.localDirTextBox);
            this.localDirGroupBox.Location = new System.Drawing.Point(11, 104);
            this.localDirGroupBox.Name = "localDirGroupBox";
            this.localDirGroupBox.Size = new System.Drawing.Size(514, 49);
            this.localDirGroupBox.TabIndex = 1;
            this.localDirGroupBox.TabStop = false;
            this.localDirGroupBox.Text = "To:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Path:";
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.button1.CausesValidation = false;
            this.button1.Location = new System.Drawing.Point(480, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(28, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "...";
            this.button1.Click += new System.EventHandler(this.BrowseClicked);
            // 
            // localDirTextBox
            // 
            this.localDirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.localDirTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.localDirTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.localDirTextBox.Location = new System.Drawing.Point(52, 19);
            this.localDirTextBox.Name = "localDirTextBox";
            this.localDirTextBox.Size = new System.Drawing.Size(422, 20);
            this.localDirTextBox.TabIndex = 1;
            this.localDirTextBox.TextChanged += new System.EventHandler(this.ControlsChanged);
            this.localDirTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.localDirTextBox_Validating);
            // 
            // nonRecursiveCheckBox
            // 
            this.nonRecursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nonRecursiveCheckBox.Location = new System.Drawing.Point(13, 182);
            this.nonRecursiveCheckBox.Name = "nonRecursiveCheckBox";
            this.nonRecursiveCheckBox.Size = new System.Drawing.Size(104, 24);
            this.nonRecursiveCheckBox.TabIndex = 4;
            this.nonRecursiveCheckBox.Text = "&Non-recursive";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(362, 183);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(450, 183);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            // 
            // omitExternalsCheckBox
            // 
            this.omitExternalsCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.omitExternalsCheckBox.Location = new System.Drawing.Point(13, 160);
            this.omitExternalsCheckBox.Name = "omitExternalsCheckBox";
            this.omitExternalsCheckBox.Size = new System.Drawing.Size(104, 24);
            this.omitExternalsCheckBox.TabIndex = 3;
            this.omitExternalsCheckBox.Text = "Omit E&xternals";
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // revisionPicker
            // 
            this.revisionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPicker.Location = new System.Drawing.Point(55, 45);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(453, 29);
            this.revisionPicker.SvnOrigin = null;
            this.revisionPicker.TabIndex = 3;
            // 
            // CheckoutDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(539, 218);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.omitExternalsCheckBox);
            this.Controls.Add(this.nonRecursiveCheckBox);
            this.Controls.Add(this.localDirGroupBox);
            this.Controls.Add(this.urlGroupBox);
            this.Name = "CheckoutDialog";
            this.Text = "Checkout from Subversion";
            this.urlGroupBox.ResumeLayout(false);
            this.urlGroupBox.PerformLayout();
            this.localDirGroupBox.ResumeLayout(false);
            this.localDirGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.GroupBox urlGroupBox;
        private System.Windows.Forms.TextBox urlTextBox;
        private System.Windows.Forms.GroupBox localDirGroupBox;
        private System.Windows.Forms.TextBox localDirTextBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox nonRecursiveCheckBox;
        private System.Windows.Forms.Button urlBrowse;
        private System.Windows.Forms.CheckBox omitExternalsCheckBox;
        private Ankh.UI.PathSelector.VersionSelector revisionPicker;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}
