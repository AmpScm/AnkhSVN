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
    partial class CreateBranchDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CreateBranchDialog));
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnOk = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.versionBox = new System.Windows.Forms.NumericUpDown();
			this.versionBrowse = new System.Windows.Forms.Button();
			this.specificVersionRadio = new System.Windows.Forms.RadioButton();
			this.headVersionRadio = new System.Windows.Forms.RadioButton();
			this.wcVersionRadio = new System.Windows.Forms.RadioButton();
			this.label3 = new System.Windows.Forms.Label();
			this.fromUrlBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.fromFolderBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.toUrlBrowse = new System.Windows.Forms.Button();
			this.toUrlBox = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.switchBox = new System.Windows.Forms.CheckBox();
			this.logMessage = new Ankh.UI.PendingChanges.LogMessageEditor(this.components);
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.versionBox)).BeginInit();
			this.groupBox1.SuspendLayout();
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
			// groupBox3
			// 
			resources.ApplyResources(this.groupBox3, "groupBox3");
			this.groupBox3.Controls.Add(this.versionBox);
			this.groupBox3.Controls.Add(this.versionBrowse);
			this.groupBox3.Controls.Add(this.specificVersionRadio);
			this.groupBox3.Controls.Add(this.headVersionRadio);
			this.groupBox3.Controls.Add(this.wcVersionRadio);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.fromUrlBox);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Controls.Add(this.fromFolderBox);
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// versionBox
			// 
			resources.ApplyResources(this.versionBox, "versionBox");
			this.versionBox.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
			this.versionBox.Name = "versionBox";
			this.versionBox.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
			// 
			// versionBrowse
			// 
			resources.ApplyResources(this.versionBrowse, "versionBrowse");
			this.versionBrowse.Name = "versionBrowse";
			this.versionBrowse.UseVisualStyleBackColor = true;
			this.versionBrowse.Click += new System.EventHandler(this.versionBrowse_Click);
			// 
			// specificVersionRadio
			// 
			resources.ApplyResources(this.specificVersionRadio, "specificVersionRadio");
			this.specificVersionRadio.Name = "specificVersionRadio";
			this.specificVersionRadio.UseVisualStyleBackColor = true;
			// 
			// headVersionRadio
			// 
			resources.ApplyResources(this.headVersionRadio, "headVersionRadio");
			this.headVersionRadio.Checked = true;
			this.headVersionRadio.Name = "headVersionRadio";
			this.headVersionRadio.TabStop = true;
			this.headVersionRadio.UseVisualStyleBackColor = true;
			// 
			// wcVersionRadio
			// 
			resources.ApplyResources(this.wcVersionRadio, "wcVersionRadio");
			this.wcVersionRadio.Name = "wcVersionRadio";
			this.wcVersionRadio.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// fromUrlBox
			// 
			resources.ApplyResources(this.fromUrlBox, "fromUrlBox");
			this.fromUrlBox.Name = "fromUrlBox";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// fromFolderBox
			// 
			resources.ApplyResources(this.fromFolderBox, "fromFolderBox");
			this.fromFolderBox.Name = "fromFolderBox";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.toUrlBrowse);
			this.groupBox1.Controls.Add(this.toUrlBox);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
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
			this.toUrlBox.TextChanged += new System.EventHandler(this.toUrlBox_TextChanged);
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
			// switchBox
			// 
			resources.ApplyResources(this.switchBox, "switchBox");
			this.switchBox.Name = "switchBox";
			this.switchBox.UseVisualStyleBackColor = true;
			// 
			// logMessage
			// 
			resources.ApplyResources(this.logMessage, "logMessage");
			this.logMessage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.logMessage.Name = "logMessage";
			// 
			// CreateBranchDialog
			// 
			this.AcceptButton = this.btnOk;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.switchBox);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.logMessage);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnCancel);
			this.Name = "CreateBranchDialog";
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.versionBox)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox fromUrlBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fromFolderBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton specificVersionRadio;
        private System.Windows.Forms.RadioButton headVersionRadio;
        private System.Windows.Forms.RadioButton wcVersionRadio;
        private System.Windows.Forms.Label label3;
        private Ankh.UI.PendingChanges.LogMessageEditor logMessage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button toUrlBrowse;
        private System.Windows.Forms.TextBox toUrlBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button versionBrowse;
        private System.Windows.Forms.CheckBox switchBox;
        private System.Windows.Forms.NumericUpDown versionBox;
    }
}
