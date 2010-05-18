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
    partial class AddProjectToSubversion
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddProjectToSubversion));
			this.markAsManaged = new System.Windows.Forms.CheckBox();
			this.writeUrlInSolution = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// bodyPanel
			// 
			resources.ApplyResources(this.bodyPanel, "bodyPanel");
			// 
			// markAsManaged
			// 
			resources.ApplyResources(this.markAsManaged, "markAsManaged");
			this.markAsManaged.Checked = true;
			this.markAsManaged.CheckState = System.Windows.Forms.CheckState.Checked;
			this.markAsManaged.Name = "markAsManaged";
			this.markAsManaged.UseVisualStyleBackColor = true;
			// 
			// writeUrlInSolution
			// 
			resources.ApplyResources(this.writeUrlInSolution, "writeUrlInSolution");
			this.writeUrlInSolution.Checked = true;
			this.writeUrlInSolution.CheckState = System.Windows.Forms.CheckState.Checked;
			this.writeUrlInSolution.Name = "writeUrlInSolution";
			this.writeUrlInSolution.UseVisualStyleBackColor = true;
			// 
			// AddProjectToSubversion
			// 
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.writeUrlInSolution);
			this.Controls.Add(this.markAsManaged);
			this.Name = "AddProjectToSubversion";
			this.Controls.SetChildIndex(this.bodyPanel, 0);
			this.Controls.SetChildIndex(this.markAsManaged, 0);
			this.Controls.SetChildIndex(this.writeUrlInSolution, 0);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox markAsManaged;
        private System.Windows.Forms.CheckBox writeUrlInSolution;
    }
}