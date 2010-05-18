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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckoutDialog));
			this.urlGroupBox = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.revisionPicker = new Ankh.UI.PathSelector.VersionSelector();
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
			this.urlGroupBox.SuspendLayout();
			this.localDirGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// urlGroupBox
			// 
			resources.ApplyResources(this.urlGroupBox, "urlGroupBox");
			this.urlGroupBox.Controls.Add(this.label1);
			this.urlGroupBox.Controls.Add(this.revisionPicker);
			this.urlGroupBox.Controls.Add(this.urlBrowse);
			this.urlGroupBox.Controls.Add(this.urlTextBox);
			this.urlGroupBox.Name = "urlGroupBox";
			this.urlGroupBox.TabStop = false;
			this.urlGroupBox.TextChanged += new System.EventHandler(this.ControlsChanged);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// revisionPicker
			// 
			resources.ApplyResources(this.revisionPicker, "revisionPicker");
			this.revisionPicker.Name = "revisionPicker";
			this.revisionPicker.SvnOrigin = null;
			// 
			// urlBrowse
			// 
			resources.ApplyResources(this.urlBrowse, "urlBrowse");
			this.urlBrowse.CausesValidation = false;
			this.urlBrowse.Name = "urlBrowse";
			this.urlBrowse.Click += new System.EventHandler(this.urlBrowse_Click);
			// 
			// urlTextBox
			// 
			resources.ApplyResources(this.urlTextBox, "urlTextBox");
			this.urlTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.urlTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.urlTextBox.Name = "urlTextBox";
			this.urlTextBox.TextChanged += new System.EventHandler(this.urlTextBox_TextChanged);
			this.urlTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.urlTextBox_Validating);
			// 
			// localDirGroupBox
			// 
			resources.ApplyResources(this.localDirGroupBox, "localDirGroupBox");
			this.localDirGroupBox.Controls.Add(this.label2);
			this.localDirGroupBox.Controls.Add(this.button1);
			this.localDirGroupBox.Controls.Add(this.localDirTextBox);
			this.localDirGroupBox.Name = "localDirGroupBox";
			this.localDirGroupBox.TabStop = false;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.CausesValidation = false;
			this.button1.Name = "button1";
			this.button1.Click += new System.EventHandler(this.BrowseClicked);
			// 
			// localDirTextBox
			// 
			resources.ApplyResources(this.localDirTextBox, "localDirTextBox");
			this.localDirTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.localDirTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this.localDirTextBox.Name = "localDirTextBox";
			this.localDirTextBox.TextChanged += new System.EventHandler(this.ControlsChanged);
			this.localDirTextBox.Validating += new System.ComponentModel.CancelEventHandler(this.localDirTextBox_Validating);
			// 
			// nonRecursiveCheckBox
			// 
			resources.ApplyResources(this.nonRecursiveCheckBox, "nonRecursiveCheckBox");
			this.nonRecursiveCheckBox.Name = "nonRecursiveCheckBox";
			// 
			// okButton
			// 
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Name = "okButton";
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.CausesValidation = false;
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			// 
			// omitExternalsCheckBox
			// 
			resources.ApplyResources(this.omitExternalsCheckBox, "omitExternalsCheckBox");
			this.omitExternalsCheckBox.Name = "omitExternalsCheckBox";
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// CheckoutDialog
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.omitExternalsCheckBox);
			this.Controls.Add(this.nonRecursiveCheckBox);
			this.Controls.Add(this.localDirGroupBox);
			this.Controls.Add(this.urlGroupBox);
			this.Name = "CheckoutDialog";
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
