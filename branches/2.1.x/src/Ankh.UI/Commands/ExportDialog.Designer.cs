// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.Commands
{
    partial class ExportDialog
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.localDirGroupBox = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.toDirBrowseButton = new System.Windows.Forms.Button();
            this.toBox = new System.Windows.Forms.TextBox();
            this.nonRecursiveCheckBox = new System.Windows.Forms.CheckBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.radioButtonGroupbox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.originBox = new System.Windows.Forms.TextBox();
            this.revisionPicker = new Ankh.UI.PathSelector.VersionSelector();
            this.localDirGroupBox.SuspendLayout();
            this.radioButtonGroupbox.SuspendLayout();
            this.SuspendLayout();
            // 
            // localDirGroupBox
            // 
            this.localDirGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.localDirGroupBox.Controls.Add(this.label2);
            this.localDirGroupBox.Controls.Add(this.toDirBrowseButton);
            this.localDirGroupBox.Controls.Add(this.toBox);
            this.localDirGroupBox.Location = new System.Drawing.Point(8, 96);
            this.localDirGroupBox.Name = "localDirGroupBox";
            this.localDirGroupBox.Size = new System.Drawing.Size(508, 56);
            this.localDirGroupBox.TabIndex = 1;
            this.localDirGroupBox.TabStop = false;
            this.localDirGroupBox.Text = "Export &To:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Pa&th:";
            // 
            // toDirBrowseButton
            // 
            this.toDirBrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.toDirBrowseButton.Location = new System.Drawing.Point(474, 18);
            this.toDirBrowseButton.Name = "toDirBrowseButton";
            this.toDirBrowseButton.Size = new System.Drawing.Size(24, 23);
            this.toDirBrowseButton.TabIndex = 2;
            this.toDirBrowseButton.Text = "&...";
            this.toDirBrowseButton.Click += new System.EventHandler(this.BrowseClicked);
            // 
            // toBox
            // 
            this.toBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.toBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.toBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.toBox.Location = new System.Drawing.Point(51, 20);
            this.toBox.Name = "toBox";
            this.toBox.Size = new System.Drawing.Size(417, 20);
            this.toBox.TabIndex = 1;
            this.toBox.TextChanged += new System.EventHandler(this.ControlsChanged);
            // 
            // nonRecursiveCheckBox
            // 
            this.nonRecursiveCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.nonRecursiveCheckBox.Location = new System.Drawing.Point(24, 165);
            this.nonRecursiveCheckBox.Name = "nonRecursiveCheckBox";
            this.nonRecursiveCheckBox.Size = new System.Drawing.Size(104, 24);
            this.nonRecursiveCheckBox.TabIndex = 2;
            this.nonRecursiveCheckBox.Text = "&Non-recursive";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(360, 166);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(441, 166);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            // 
            // radioButtonGroupbox
            // 
            this.radioButtonGroupbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.radioButtonGroupbox.Controls.Add(this.label1);
            this.radioButtonGroupbox.Controls.Add(this.revisionPicker);
            this.radioButtonGroupbox.Controls.Add(this.originBox);
            this.radioButtonGroupbox.Location = new System.Drawing.Point(8, 12);
            this.radioButtonGroupbox.Name = "radioButtonGroupbox";
            this.radioButtonGroupbox.Size = new System.Drawing.Size(508, 78);
            this.radioButtonGroupbox.TabIndex = 0;
            this.radioButtonGroupbox.TabStop = false;
            this.radioButtonGroupbox.Text = "&From:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Path:";
            // 
            // originBox
            // 
            this.originBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.originBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.originBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.originBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.originBox.Location = new System.Drawing.Point(51, 19);
            this.originBox.Name = "originBox";
            this.originBox.ReadOnly = true;
            this.originBox.Size = new System.Drawing.Size(447, 13);
            this.originBox.TabIndex = 1;
            this.originBox.TextChanged += new System.EventHandler(this.ControlsChanged);
            // 
            // revisionPicker
            // 
            this.revisionPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.revisionPicker.Location = new System.Drawing.Point(9, 41);
            this.revisionPicker.Name = "revisionPicker";
            this.revisionPicker.Size = new System.Drawing.Size(493, 29);
            this.revisionPicker.SvnOrigin = null;
            this.revisionPicker.TabIndex = 3;
            this.revisionPicker.Changed += new System.EventHandler(this.ControlsChanged);
            // 
            // ExportDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(526, 201);
            this.Controls.Add(this.radioButtonGroupbox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.nonRecursiveCheckBox);
            this.Controls.Add(this.localDirGroupBox);
            this.Name = "ExportDialog";
            this.Text = "Export";
            this.localDirGroupBox.ResumeLayout(false);
            this.localDirGroupBox.PerformLayout();
            this.radioButtonGroupbox.ResumeLayout(false);
            this.radioButtonGroupbox.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion



        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private Ankh.UI.PathSelector.VersionSelector revisionPicker;
        private System.Windows.Forms.GroupBox localDirGroupBox;
        private System.Windows.Forms.TextBox toBox;
        private System.Windows.Forms.Button toDirBrowseButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox nonRecursiveCheckBox;
        private System.Windows.Forms.GroupBox radioButtonGroupbox;
        private System.Windows.Forms.TextBox originBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}
