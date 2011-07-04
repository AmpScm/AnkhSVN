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
    partial class ExportDialog
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExportDialog));
			this.localDirGroupBox = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.toDirBrowseButton = new System.Windows.Forms.Button();
			this.toBox = new System.Windows.Forms.TextBox();
			this.nonRecursiveCheckBox = new System.Windows.Forms.CheckBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.radioButtonGroupbox = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.revisionPicker = new Ankh.UI.PathSelector.VersionSelector();
			this.originBox = new System.Windows.Forms.TextBox();
			this.localDirGroupBox.SuspendLayout();
			this.radioButtonGroupbox.SuspendLayout();
			this.SuspendLayout();
			// 
			// localDirGroupBox
			// 
			resources.ApplyResources(this.localDirGroupBox, "localDirGroupBox");
			this.localDirGroupBox.Controls.Add(this.label2);
			this.localDirGroupBox.Controls.Add(this.toDirBrowseButton);
			this.localDirGroupBox.Controls.Add(this.toBox);
			this.localDirGroupBox.Name = "localDirGroupBox";
			this.localDirGroupBox.TabStop = false;
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// toDirBrowseButton
			// 
			resources.ApplyResources(this.toDirBrowseButton, "toDirBrowseButton");
			this.toDirBrowseButton.Name = "toDirBrowseButton";
			this.toDirBrowseButton.Click += new System.EventHandler(this.BrowseClicked);
			// 
			// toBox
			// 
			resources.ApplyResources(this.toBox, "toBox");
			this.toBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.toBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this.toBox.Name = "toBox";
			this.toBox.TextChanged += new System.EventHandler(this.ControlsChanged);
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
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			// 
			// radioButtonGroupbox
			// 
			resources.ApplyResources(this.radioButtonGroupbox, "radioButtonGroupbox");
			this.radioButtonGroupbox.Controls.Add(this.label1);
			this.radioButtonGroupbox.Controls.Add(this.revisionPicker);
			this.radioButtonGroupbox.Controls.Add(this.originBox);
			this.radioButtonGroupbox.Name = "radioButtonGroupbox";
			this.radioButtonGroupbox.TabStop = false;
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
			this.revisionPicker.Changed += new System.EventHandler(this.ControlsChanged);
			// 
			// originBox
			// 
			resources.ApplyResources(this.originBox, "originBox");
			this.originBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
			this.originBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
			this.originBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.originBox.Name = "originBox";
			this.originBox.ReadOnly = true;
			this.originBox.TextChanged += new System.EventHandler(this.ControlsChanged);
			// 
			// ExportDialog
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.radioButtonGroupbox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.nonRecursiveCheckBox);
			this.Controls.Add(this.localDirGroupBox);
			this.Name = "ExportDialog";
			this.localDirGroupBox.ResumeLayout(false);
			this.localDirGroupBox.PerformLayout();
			this.radioButtonGroupbox.ResumeLayout(false);
			this.radioButtonGroupbox.PerformLayout();
			this.ResumeLayout(false);

        }
        #endregion



        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private Ankh.UI.PathSelector.VersionSelector revisionPicker;
        private System.Windows.Forms.GroupBox localDirGroupBox;
        private System.Windows.Forms.TextBox toBox;
        private System.Windows.Forms.Button toDirBrowseButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox nonRecursiveCheckBox;
        private System.Windows.Forms.GroupBox radioButtonGroupbox;
        private System.Windows.Forms.TextBox originBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
