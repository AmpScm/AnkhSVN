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
using System.Windows.Forms;

namespace Ankh.UI.WorkingCopyExplorer
{
    partial class AddWorkingCopyExplorerRootDialog
    {
        #region InitializeComponent
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddWorkingCopyExplorerRootDialog));
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.workingCopyRootTextBox = new System.Windows.Forms.TextBox();
			this.browseFolderButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
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
			// workingCopyRootTextBox
			// 
			resources.ApplyResources(this.workingCopyRootTextBox, "workingCopyRootTextBox");
			this.workingCopyRootTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.workingCopyRootTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystem;
			this.workingCopyRootTextBox.Name = "workingCopyRootTextBox";
			this.workingCopyRootTextBox.TextChanged += new System.EventHandler(this.workingCopyRootTextBox_TextChanged);
			// 
			// browseFolderButton
			// 
			resources.ApplyResources(this.browseFolderButton, "browseFolderButton");
			this.browseFolderButton.Name = "browseFolderButton";
			this.browseFolderButton.Click += new System.EventHandler(this.browseFolderButton_Click);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// AddWorkingCopyExplorerRootDialog
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.label1);
			this.Controls.Add(this.browseFolderButton);
			this.Controls.Add(this.workingCopyRootTextBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Name = "AddWorkingCopyExplorerRootDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        #endregion

        private Button okButton;
        private TextBox workingCopyRootTextBox;
        private Button browseFolderButton;
        private Button cancelButton;
        private Label label1;
    }
}
