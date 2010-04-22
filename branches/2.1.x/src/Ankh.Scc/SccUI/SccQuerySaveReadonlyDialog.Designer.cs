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
            this.lblFile.Location = new System.Drawing.Point(48, 12);
            this.lblFile.Name = "lblFile";
            this.lblFile.Size = new System.Drawing.Size(431, 13);
            this.lblFile.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(48, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(431, 50);
            this.label1.TabIndex = 1;
            this.label1.Text = "You can either save the file in a different location or Microsoft Visual Studio c" +
                "an attempt to remove the write-protection and overwrite the file in its current " +
                "location.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnSaveAs.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnSaveAs.Location = new System.Drawing.Point(119, 119);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(75, 23);
            this.btnSaveAs.TabIndex = 2;
            this.btnSaveAs.Text = "Save &As...";
            this.btnSaveAs.UseVisualStyleBackColor = true;
            // 
            // btnOverwrite
            // 
            this.btnOverwrite.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOverwrite.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnOverwrite.Location = new System.Drawing.Point(208, 119);
            this.btnOverwrite.Name = "btnOverwrite";
            this.btnOverwrite.Size = new System.Drawing.Size(75, 23);
            this.btnOverwrite.TabIndex = 3;
            this.btnOverwrite.Text = "&Overwrite";
            this.btnOverwrite.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(297, 119);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // pWarning
            // 
            this.pWarning.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.pWarning.Image = ((System.Drawing.Image)(resources.GetObject("pWarning.Image")));
            this.pWarning.Location = new System.Drawing.Point(1, 31);
            this.pWarning.Name = "pWarning";
            this.pWarning.Size = new System.Drawing.Size(44, 50);
            this.pWarning.TabIndex = 7;
            this.pWarning.TabStop = false;
            // 
            // SccQuerySaveReadonlyDialog
            // 
            this.AcceptButton = this.btnSaveAs;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(491, 154);
            this.Controls.Add(this.pWarning);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOverwrite);
            this.Controls.Add(this.btnSaveAs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblFile);
            this.Name = "SccQuerySaveReadonlyDialog";
            this.Text = "Save of Read-Only File";
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