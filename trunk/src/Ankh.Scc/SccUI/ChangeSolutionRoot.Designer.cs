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
    partial class ChangeSolutionRoot
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeSolutionRoot));
			this.label1 = new System.Windows.Forms.Label();
			this.slnBindPath = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// slnBindPath
			// 
			resources.ApplyResources(this.slnBindPath, "slnBindPath");
			this.slnBindPath.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.slnBindPath.FormattingEnabled = true;
			this.slnBindPath.Name = "slnBindPath";
			this.slnBindPath.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// okButton
			// 
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// checkBox1
			// 
			resources.ApplyResources(this.checkBox1, "checkBox1");
			this.checkBox1.Checked = true;
			this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.UseVisualStyleBackColor = true;
			// 
			// ChangeSolutionRoot
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.slnBindPath);
			this.Controls.Add(this.label1);
			this.Name = "ChangeSolutionRoot";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox slnBindPath;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}