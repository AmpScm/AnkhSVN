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
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(579, 75);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // wcList
            // 
            this.wcList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.wcList.CheckBoxes = true;
            this.wcList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnWorkingCopy,
            this.columnChanges,
            this.columnRepository});
            this.wcList.Location = new System.Drawing.Point(12, 102);
            this.wcList.Name = "wcList";
            this.wcList.Size = new System.Drawing.Size(579, 165);
            this.wcList.TabIndex = 1;
            this.wcList.Resize += new System.EventHandler(this.listView1_Resize);
            // 
            // columnWorkingCopy
            // 
            this.columnWorkingCopy.Text = "Working Copy";
            this.columnWorkingCopy.Width = 200;
            // 
            // columnChanges
            // 
            this.columnChanges.Text = "Changes";
            // 
            // columnRepository
            // 
            this.columnRepository.Text = "Repository";
            this.columnRepository.Width = 200;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(516, 290);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(435, 290);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // MultiWorkingCopyCommit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 325);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.wcList);
            this.Controls.Add(this.label1);
            this.Name = "MultiWorkingCopyCommit";
            this.Text = "Commit from Multiple Working Copies";
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