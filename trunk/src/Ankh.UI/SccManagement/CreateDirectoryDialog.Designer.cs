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

namespace Ankh.UI.SccManagement
{
    partial class CreateDirectoryDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateDirectoryDialog));
			this.directoryNameBox = new System.Windows.Forms.TextBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.logMessage = new Ankh.UI.PendingChanges.LogMessageEditor(this.components);
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// directoryNameBox
			// 
			resources.ApplyResources(this.directoryNameBox, "directoryNameBox");
			this.directoryNameBox.Name = "directoryNameBox";
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// btnOk
			// 
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOk.Name = "btnOk";
			this.btnOk.UseVisualStyleBackColor = true;
			// 
			// logMessage
			// 
			resources.ApplyResources(this.logMessage, "logMessage");
			this.logMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.logMessage.Name = "logMessage";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// CreateDirectoryDialog
			// 
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.logMessage);
			this.Controls.Add(this.directoryNameBox);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Name = "CreateDirectoryDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox directoryNameBox;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private Ankh.UI.PendingChanges.LogMessageEditor logMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
