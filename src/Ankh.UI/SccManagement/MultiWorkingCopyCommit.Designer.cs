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

namespace Ankh.UI.SccManagement
{
    partial class MultiWorkingCopyCommit
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiWorkingCopyCommit));
			this.label1 = new System.Windows.Forms.Label();
			this.wcList = new Ankh.UI.VSSelectionControls.SmartListView();
			this.columnWorkingCopy = new System.Windows.Forms.ColumnHeader();
			this.columnChanges = new System.Windows.Forms.ColumnHeader();
			this.columnRepository = new System.Windows.Forms.ColumnHeader();
			this.button1 = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// wcList
			// 
			resources.ApplyResources(this.wcList, "wcList");
			this.wcList.CheckBoxes = true;
			this.wcList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnWorkingCopy,
            this.columnChanges,
            this.columnRepository});
			this.wcList.Name = "wcList";
			this.wcList.Resize += new System.EventHandler(this.listView1_Resize);
			// 
			// columnWorkingCopy
			// 
			resources.ApplyResources(this.columnWorkingCopy, "columnWorkingCopy");
			// 
			// columnChanges
			// 
			resources.ApplyResources(this.columnChanges, "columnChanges");
			// 
			// columnRepository
			// 
			resources.ApplyResources(this.columnRepository, "columnRepository");
			// 
			// button1
			// 
			resources.ApplyResources(this.button1, "button1");
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Name = "button1";
			this.button1.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// MultiWorkingCopyCommit
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.wcList);
			this.Controls.Add(this.label1);
			this.Name = "MultiWorkingCopyCommit";
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Ankh.UI.VSSelectionControls.SmartListView wcList;
        private System.Windows.Forms.ColumnHeader columnWorkingCopy;
        private System.Windows.Forms.ColumnHeader columnChanges;
        private System.Windows.Forms.ColumnHeader columnRepository;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button okButton;
    }
}