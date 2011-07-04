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
    partial class SccEditEnlistment
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SccEditEnlistment));
			this.projectLocationGroup = new System.Windows.Forms.GroupBox();
			this.projectLocationBrowse = new System.Windows.Forms.Button();
			this.projectLocationBox = new System.Windows.Forms.TextBox();
			this.debugLocationGroup = new System.Windows.Forms.GroupBox();
			this.debugLocationBrowse = new System.Windows.Forms.Button();
			this.debugLocationBox = new System.Windows.Forms.TextBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.projectLocationGroup.SuspendLayout();
			this.debugLocationGroup.SuspendLayout();
			this.SuspendLayout();
			// 
			// projectLocationGroup
			// 
			resources.ApplyResources(this.projectLocationGroup, "projectLocationGroup");
			this.projectLocationGroup.Controls.Add(this.projectLocationBrowse);
			this.projectLocationGroup.Controls.Add(this.projectLocationBox);
			this.projectLocationGroup.Name = "projectLocationGroup";
			this.projectLocationGroup.TabStop = false;
			// 
			// projectLocationBrowse
			// 
			resources.ApplyResources(this.projectLocationBrowse, "projectLocationBrowse");
			this.projectLocationBrowse.Name = "projectLocationBrowse";
			this.projectLocationBrowse.UseVisualStyleBackColor = true;
			this.projectLocationBrowse.Click += new System.EventHandler(this.projectLocationBrowse_Click);
			// 
			// projectLocationBox
			// 
			resources.ApplyResources(this.projectLocationBox, "projectLocationBox");
			this.projectLocationBox.Name = "projectLocationBox";
			this.projectLocationBox.TextChanged += new System.EventHandler(this.projectLocationBox_TextChanged);
			this.projectLocationBox.Validating += new System.ComponentModel.CancelEventHandler(this.projectLocationBox_Validating);
			// 
			// debugLocationGroup
			// 
			resources.ApplyResources(this.debugLocationGroup, "debugLocationGroup");
			this.debugLocationGroup.Controls.Add(this.debugLocationBrowse);
			this.debugLocationGroup.Controls.Add(this.debugLocationBox);
			this.debugLocationGroup.Name = "debugLocationGroup";
			this.debugLocationGroup.TabStop = false;
			// 
			// debugLocationBrowse
			// 
			resources.ApplyResources(this.debugLocationBrowse, "debugLocationBrowse");
			this.debugLocationBrowse.Name = "debugLocationBrowse";
			this.debugLocationBrowse.UseVisualStyleBackColor = true;
			this.debugLocationBrowse.Click += new System.EventHandler(this.debugLocationBrowse_Click);
			// 
			// debugLocationBox
			// 
			resources.ApplyResources(this.debugLocationBox, "debugLocationBox");
			this.debugLocationBox.Name = "debugLocationBox";
			this.debugLocationBox.TextChanged += new System.EventHandler(this.debugLocationBox_TextChanged);
			this.debugLocationBox.Validating += new System.ComponentModel.CancelEventHandler(this.debugLocationBox_Validating);
			// 
			// okButton
			// 
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// SccEditEnlistment
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.debugLocationGroup);
			this.Controls.Add(this.projectLocationGroup);
			this.Name = "SccEditEnlistment";
			this.projectLocationGroup.ResumeLayout(false);
			this.projectLocationGroup.PerformLayout();
			this.debugLocationGroup.ResumeLayout(false);
			this.debugLocationGroup.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox projectLocationGroup;
        private System.Windows.Forms.Button projectLocationBrowse;
        private System.Windows.Forms.TextBox projectLocationBox;
        private System.Windows.Forms.GroupBox debugLocationGroup;
        private System.Windows.Forms.Button debugLocationBrowse;
        private System.Windows.Forms.TextBox debugLocationBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}
