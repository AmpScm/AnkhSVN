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
    partial class CopyToDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CopyToDialog));
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.toUrlBrowse = new System.Windows.Forms.Button();
			this.toUrlBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.logMessage = new Ankh.UI.PendingChanges.LogMessageEditor(this.components);
			this.SuspendLayout();
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.CausesValidation = false;
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
			// toUrlBrowse
			// 
			resources.ApplyResources(this.toUrlBrowse, "toUrlBrowse");
			this.toUrlBrowse.Name = "toUrlBrowse";
			this.toUrlBrowse.UseVisualStyleBackColor = true;
			this.toUrlBrowse.Click += new System.EventHandler(this.toUrlBrowse_Click);
			// 
			// toUrlBox
			// 
			resources.ApplyResources(this.toUrlBox, "toUrlBox");
			this.toUrlBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.toUrlBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.toUrlBox.Name = "toUrlBox";
			this.toUrlBox.TextAlignChanged += new System.EventHandler(this.toUrlBox_TextChanged);
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// logMessage
			// 
			resources.ApplyResources(this.logMessage, "logMessage");
			this.logMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.logMessage.Name = "logMessage";
			// 
			// CopyToDialog
			// 
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.toUrlBrowse);
			this.Controls.Add(this.toUrlBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.logMessage);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Name = "CopyToDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private Ankh.UI.PendingChanges.LogMessageEditor logMessage;
        private System.Windows.Forms.Button toUrlBrowse;
        private System.Windows.Forms.TextBox toUrlBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}
