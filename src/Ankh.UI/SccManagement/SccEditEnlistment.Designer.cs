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
            this.projectLocationGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.projectLocationGroup.Controls.Add(this.projectLocationBrowse);
            this.projectLocationGroup.Controls.Add(this.projectLocationBox);
            this.projectLocationGroup.Location = new System.Drawing.Point(12, 12);
            this.projectLocationGroup.Name = "projectLocationGroup";
            this.projectLocationGroup.Size = new System.Drawing.Size(339, 48);
            this.projectLocationGroup.TabIndex = 0;
            this.projectLocationGroup.TabStop = false;
            this.projectLocationGroup.Text = "&Project Location:";
            // 
            // projectLocationBrowse
            // 
            this.projectLocationBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.projectLocationBrowse.Location = new System.Drawing.Point(301, 17);
            this.projectLocationBrowse.Name = "projectLocationBrowse";
            this.projectLocationBrowse.Size = new System.Drawing.Size(32, 23);
            this.projectLocationBrowse.TabIndex = 1;
            this.projectLocationBrowse.Text = "...";
            this.projectLocationBrowse.UseVisualStyleBackColor = true;
            this.projectLocationBrowse.Click += new System.EventHandler(this.projectLocationBrowse_Click);
            // 
            // projectLocationBox
            // 
            this.projectLocationBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.projectLocationBox.Location = new System.Drawing.Point(6, 19);
            this.projectLocationBox.Name = "projectLocationBox";
            this.projectLocationBox.Size = new System.Drawing.Size(289, 20);
            this.projectLocationBox.TabIndex = 0;
            this.projectLocationBox.TextChanged += new System.EventHandler(this.projectLocationBox_TextChanged);
            this.projectLocationBox.Validating += new System.ComponentModel.CancelEventHandler(this.projectLocationBox_Validating);
            // 
            // debugLocationGroup
            // 
            this.debugLocationGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.debugLocationGroup.Controls.Add(this.debugLocationBrowse);
            this.debugLocationGroup.Controls.Add(this.debugLocationBox);
            this.debugLocationGroup.Location = new System.Drawing.Point(12, 66);
            this.debugLocationGroup.Name = "debugLocationGroup";
            this.debugLocationGroup.Size = new System.Drawing.Size(339, 48);
            this.debugLocationGroup.TabIndex = 2;
            this.debugLocationGroup.TabStop = false;
            this.debugLocationGroup.Text = "&Debug Location:";
            // 
            // debugLocationBrowse
            // 
            this.debugLocationBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.debugLocationBrowse.Location = new System.Drawing.Point(301, 17);
            this.debugLocationBrowse.Name = "debugLocationBrowse";
            this.debugLocationBrowse.Size = new System.Drawing.Size(32, 23);
            this.debugLocationBrowse.TabIndex = 1;
            this.debugLocationBrowse.Text = "...";
            this.debugLocationBrowse.UseVisualStyleBackColor = true;
            this.debugLocationBrowse.Click += new System.EventHandler(this.debugLocationBrowse_Click);
            // 
            // debugLocationBox
            // 
            this.debugLocationBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.debugLocationBox.Location = new System.Drawing.Point(6, 19);
            this.debugLocationBox.Name = "debugLocationBox";
            this.debugLocationBox.Size = new System.Drawing.Size(289, 20);
            this.debugLocationBox.TabIndex = 0;
            this.debugLocationBox.TextChanged += new System.EventHandler(this.debugLocationBox_TextChanged);
            this.debugLocationBox.Validating += new System.ComponentModel.CancelEventHandler(this.debugLocationBox_Validating);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(189, 132);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(270, 132);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // SccEditEnlistment
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 167);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.debugLocationGroup);
            this.Controls.Add(this.projectLocationGroup);
            this.Name = "SccEditEnlistment";
            this.Text = "Select Project Location";
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
