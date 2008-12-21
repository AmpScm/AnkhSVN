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

namespace Ankh.UI.OptionsPages
{
    partial class AnkhSettingsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.diffExeBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.diffBrowseBtn = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.mergeExeBox = new System.Windows.Forms.ComboBox();
            this.mergeBrowseBtn = new System.Windows.Forms.Button();
            this.interactiveMergeOnConflict = new System.Windows.Forms.CheckBox();
            this.patchBrowseBtn = new System.Windows.Forms.Button();
            this.patchExeBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // diffExeBox
            // 
            this.diffExeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.diffExeBox.DisplayMember = "DisplayName";
            this.diffExeBox.Location = new System.Drawing.Point(0, 15);
            this.diffExeBox.Name = "diffExeBox";
            this.diffExeBox.Size = new System.Drawing.Size(355, 21);
            this.diffExeBox.TabIndex = 1;
            this.diffExeBox.SelectionChangeCommitted += new System.EventHandler(this.tool_selectionCommitted);
            this.diffExeBox.TextChanged += new System.EventHandler(this.diffExeBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(91, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "External &Diff Tool:";
            // 
            // diffBrowseBtn
            // 
            this.diffBrowseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.diffBrowseBtn.Location = new System.Drawing.Point(366, 13);
            this.diffBrowseBtn.Name = "diffBrowseBtn";
            this.diffBrowseBtn.Size = new System.Drawing.Size(26, 23);
            this.diffBrowseBtn.TabIndex = 2;
            this.diffBrowseBtn.Text = "...";
            this.diffBrowseBtn.UseVisualStyleBackColor = true;
            this.diffBrowseBtn.Click += new System.EventHandler(this.diffBrowseBtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "External &Merge Tool:";
            // 
            // mergeExeBox
            // 
            this.mergeExeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.mergeExeBox.DisplayMember = "DisplayName";
            this.mergeExeBox.Location = new System.Drawing.Point(0, 58);
            this.mergeExeBox.Name = "mergeExeBox";
            this.mergeExeBox.Size = new System.Drawing.Size(355, 21);
            this.mergeExeBox.TabIndex = 4;
            this.mergeExeBox.ValueMember = "ToolTemplate";
            this.mergeExeBox.SelectionChangeCommitted += new System.EventHandler(this.tool_selectionCommitted);
            // 
            // mergeBrowseBtn
            // 
            this.mergeBrowseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.mergeBrowseBtn.Location = new System.Drawing.Point(366, 55);
            this.mergeBrowseBtn.Name = "mergeBrowseBtn";
            this.mergeBrowseBtn.Size = new System.Drawing.Size(26, 23);
            this.mergeBrowseBtn.TabIndex = 5;
            this.mergeBrowseBtn.Text = "...";
            this.mergeBrowseBtn.UseVisualStyleBackColor = true;
            this.mergeBrowseBtn.Click += new System.EventHandler(this.mergeBrowseBtn_Click);
            // 
            // interactiveMergeOnConflict
            // 
            this.interactiveMergeOnConflict.AutoSize = true;
            this.interactiveMergeOnConflict.Location = new System.Drawing.Point(0, 124);
            this.interactiveMergeOnConflict.Name = "interactiveMergeOnConflict";
            this.interactiveMergeOnConflict.Size = new System.Drawing.Size(218, 17);
            this.interactiveMergeOnConflict.TabIndex = 10;
            this.interactiveMergeOnConflict.Text = "Start Interactive Merge on &Conflict (Beta)";
            this.interactiveMergeOnConflict.UseVisualStyleBackColor = true;
            // 
            // patchBrowseBtn
            // 
            this.patchBrowseBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.patchBrowseBtn.Location = new System.Drawing.Point(366, 94);
            this.patchBrowseBtn.Name = "patchBrowseBtn";
            this.patchBrowseBtn.Size = new System.Drawing.Size(26, 23);
            this.patchBrowseBtn.TabIndex = 8;
            this.patchBrowseBtn.Text = "...";
            this.patchBrowseBtn.UseVisualStyleBackColor = true;
            this.patchBrowseBtn.Click += new System.EventHandler(this.patchBrowseBtn_Click);
            // 
            // patchExeBox
            // 
            this.patchExeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.patchExeBox.DisplayMember = "DisplayName";
            this.patchExeBox.Location = new System.Drawing.Point(0, 97);
            this.patchExeBox.Name = "patchExeBox";
            this.patchExeBox.Size = new System.Drawing.Size(355, 21);
            this.patchExeBox.TabIndex = 7;
            this.patchExeBox.ValueMember = "ToolTemplate";
            this.patchExeBox.SelectionChangeCommitted += new System.EventHandler(this.tool_selectionCommitted);
            this.patchExeBox.SelectedIndexChanged += new System.EventHandler(this.tool_selectionCommitted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-3, 82);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "External &Patch Tool:";
            // 
            // AnkhSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.patchBrowseBtn);
            this.Controls.Add(this.patchExeBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.interactiveMergeOnConflict);
            this.Controls.Add(this.mergeBrowseBtn);
            this.Controls.Add(this.mergeExeBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.diffBrowseBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.diffExeBox);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "AnkhSettingsControl";
            this.Size = new System.Drawing.Size(400, 271);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox diffExeBox;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button diffBrowseBtn;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox mergeExeBox;
		private System.Windows.Forms.Button mergeBrowseBtn;
        private System.Windows.Forms.CheckBox interactiveMergeOnConflict;
        private System.Windows.Forms.Button patchBrowseBtn;
        private System.Windows.Forms.ComboBox patchExeBox;
        private System.Windows.Forms.Label label3;
    }
}
