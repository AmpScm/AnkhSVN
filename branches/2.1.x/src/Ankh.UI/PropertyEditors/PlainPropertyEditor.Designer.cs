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

using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.PropertyEditors
{
    partial class PlainPropertyEditor
    {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.valueTextBox = new System.Windows.Forms.TextBox();
            this.plainGroupBox = new System.Windows.Forms.GroupBox();
            this.conflictToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.plainGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // valueTextBox
            // 
            this.valueTextBox.AcceptsReturn = true;
            this.valueTextBox.AcceptsTab = true;
            this.valueTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.valueTextBox.Location = new System.Drawing.Point(6, 19);
            this.valueTextBox.Multiline = true;
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.valueTextBox.Size = new System.Drawing.Size(336, 174);
            this.valueTextBox.TabIndex = 0;
            this.valueTextBox.TextChanged += new System.EventHandler(this.valueTextBox_TextChanged);
            // 
            // plainGroupBox
            // 
            this.plainGroupBox.Controls.Add(this.valueTextBox);
            this.plainGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plainGroupBox.Location = new System.Drawing.Point(0, 0);
            this.plainGroupBox.Name = "plainGroupBox";
            this.plainGroupBox.Size = new System.Drawing.Size(348, 196);
            this.plainGroupBox.TabIndex = 2;
            this.plainGroupBox.TabStop = false;
            this.plainGroupBox.Text = "Enter values";
            // 
            // PlainPropertyEditor
            // 
            this.Controls.Add(this.plainGroupBox);
            this.Name = "PlainPropertyEditor";
            this.plainGroupBox.ResumeLayout(false);
            this.plainGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TextBox valueTextBox;
        private System.Windows.Forms.GroupBox plainGroupBox;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ToolTip conflictToolTip;

    }
}
