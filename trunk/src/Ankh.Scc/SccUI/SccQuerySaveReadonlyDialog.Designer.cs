// $Id$
//
// Copyright 2009 The AnkhSVN Project
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

namespace Ankh.Scc.SccUI
{
    partial class SccQuerySaveReadonlyDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SccQuerySaveReadonlyDialog));
			this.lblFile = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.btnSaveAs = new System.Windows.Forms.Button();
			this.btnOverwrite = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.pWarning = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pWarning)).BeginInit();
			this.SuspendLayout();
			// 
			// lblFile
			// 
			resources.ApplyResources(this.lblFile, "lblFile");
			this.lblFile.Name = "lblFile";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// btnSaveAs
			// 
			resources.ApplyResources(this.btnSaveAs, "btnSaveAs");
			this.btnSaveAs.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.btnSaveAs.Name = "btnSaveAs";
			this.btnSaveAs.UseVisualStyleBackColor = true;
			// 
			// btnOverwrite
			// 
			resources.ApplyResources(this.btnOverwrite, "btnOverwrite");
			this.btnOverwrite.DialogResult = System.Windows.Forms.DialogResult.No;
			this.btnOverwrite.Name = "btnOverwrite";
			this.btnOverwrite.UseVisualStyleBackColor = true;
			// 
			// btnCancel
			// 
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// pWarning
			// 
			resources.ApplyResources(this.pWarning, "pWarning");
			this.pWarning.Name = "pWarning";
			this.pWarning.TabStop = false;
			// 
			// SccQuerySaveReadonlyDialog
			// 
			this.AcceptButton = this.btnSaveAs;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.pWarning);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOverwrite);
			this.Controls.Add(this.btnSaveAs);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.lblFile);
			this.Name = "SccQuerySaveReadonlyDialog";
			((System.ComponentModel.ISupportInitialize)(this.pWarning)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSaveAs;
        private System.Windows.Forms.Button btnOverwrite;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PictureBox pWarning;
    }
}