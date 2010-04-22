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

namespace Ankh.UI.OptionsPages
{
    partial class EnvironmentSettingsControl
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
            this.authenticationEdit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.proxyEdit = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.interactiveMergeOnConflict = new System.Windows.Forms.CheckBox();
            this.autoAddFiles = new System.Windows.Forms.CheckBox();
            this.flashWindowAfterOperation = new System.Windows.Forms.CheckBox();
            this.autoLockFiles = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pcDefaultDoubleClick = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // authenticationEdit
            // 
            this.authenticationEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.authenticationEdit.Location = new System.Drawing.Point(319, 51);
            this.authenticationEdit.Name = "authenticationEdit";
            this.authenticationEdit.Size = new System.Drawing.Size(75, 23);
            this.authenticationEdit.TabIndex = 3;
            this.authenticationEdit.Text = "Edit";
            this.authenticationEdit.UseVisualStyleBackColor = true;
            this.authenticationEdit.Click += new System.EventHandler(this.authenticationEdit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Au&thentication Cache:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "&Proxy Settings:";
            // 
            // proxyEdit
            // 
            this.proxyEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.proxyEdit.Location = new System.Drawing.Point(319, 18);
            this.proxyEdit.Name = "proxyEdit";
            this.proxyEdit.Size = new System.Drawing.Size(75, 23);
            this.proxyEdit.TabIndex = 1;
            this.proxyEdit.Text = "Edit";
            this.proxyEdit.UseVisualStyleBackColor = true;
            this.proxyEdit.Click += new System.EventHandler(this.proxyEdit_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.proxyEdit);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.authenticationEdit);
            this.groupBox1.Location = new System.Drawing.Point(0, 185);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(400, 83);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Subversion User Settings";
            // 
            // interactiveMergeOnConflict
            // 
            this.interactiveMergeOnConflict.AutoSize = true;
            this.interactiveMergeOnConflict.Location = new System.Drawing.Point(0, 63);
            this.interactiveMergeOnConflict.Name = "interactiveMergeOnConflict";
            this.interactiveMergeOnConflict.Size = new System.Drawing.Size(218, 17);
            this.interactiveMergeOnConflict.TabIndex = 2;
            this.interactiveMergeOnConflict.Text = "Start Interactive Merge on &Conflict (Beta)";
            this.interactiveMergeOnConflict.UseVisualStyleBackColor = true;
            // 
            // autoAddFiles
            // 
            this.autoAddFiles.AutoSize = true;
            this.autoAddFiles.Location = new System.Drawing.Point(0, 17);
            this.autoAddFiles.Name = "autoAddFiles";
            this.autoAddFiles.Size = new System.Drawing.Size(192, 17);
            this.autoAddFiles.TabIndex = 0;
            this.autoAddFiles.Text = "Directly &add new files to subversion";
            this.autoAddFiles.UseVisualStyleBackColor = true;
            // 
            // flashWindowAfterOperation
            // 
            this.flashWindowAfterOperation.AutoSize = true;
            this.flashWindowAfterOperation.Location = new System.Drawing.Point(0, 86);
            this.flashWindowAfterOperation.Name = "flashWindowAfterOperation";
            this.flashWindowAfterOperation.Size = new System.Drawing.Size(261, 17);
            this.flashWindowAfterOperation.TabIndex = 3;
            this.flashWindowAfterOperation.Text = "&Flash title bar when a lengthy operation completes";
            this.flashWindowAfterOperation.UseVisualStyleBackColor = true;
            // 
            // autoLockFiles
            // 
            this.autoLockFiles.AutoSize = true;
            this.autoLockFiles.Location = new System.Drawing.Point(0, 40);
            this.autoLockFiles.Name = "autoLockFiles";
            this.autoLockFiles.Size = new System.Drawing.Size(306, 17);
            this.autoLockFiles.TabIndex = 1;
            this.autoLockFiles.Text = "Automatically &lock files on change without user confirmation";
            this.autoLockFiles.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(238, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "When &double clicking items in Pending Changes:";
            // 
            // pcDefaultDoubleClick
            // 
            this.pcDefaultDoubleClick.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pcDefaultDoubleClick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.pcDefaultDoubleClick.FormattingEnabled = true;
            this.pcDefaultDoubleClick.Items.AddRange(new object[] {
            "Open the file in Visual Studio",
            "Show changes in the current diff tool"});
            this.pcDefaultDoubleClick.Location = new System.Drawing.Point(3, 131);
            this.pcDefaultDoubleClick.Name = "pcDefaultDoubleClick";
            this.pcDefaultDoubleClick.Size = new System.Drawing.Size(339, 21);
            this.pcDefaultDoubleClick.TabIndex = 6;
            // 
            // EnvironmentSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.autoAddFiles);
            this.Controls.Add(this.autoLockFiles);
            this.Controls.Add(this.interactiveMergeOnConflict);
            this.Controls.Add(this.flashWindowAfterOperation);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pcDefaultDoubleClick);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "EnvironmentSettingsControl";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button authenticationEdit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button proxyEdit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox interactiveMergeOnConflict;
        private System.Windows.Forms.CheckBox autoAddFiles;
        private System.Windows.Forms.CheckBox flashWindowAfterOperation;
        private System.Windows.Forms.CheckBox autoLockFiles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox pcDefaultDoubleClick;
    }
}
