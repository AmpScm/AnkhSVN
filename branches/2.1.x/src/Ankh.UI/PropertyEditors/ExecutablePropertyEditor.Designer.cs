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
    partial class ExecutablePropertyEditor
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.conflictToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.executableTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // executableTextBox
            // 
            this.executableTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.executableTextBox.Location = new System.Drawing.Point(0, 0);
            this.executableTextBox.Multiline = true;
            this.executableTextBox.Name = "executableTextBox";
            this.executableTextBox.ReadOnly = true;
            this.executableTextBox.Size = new System.Drawing.Size(348, 196);
            this.executableTextBox.TabIndex = 0;
            this.executableTextBox.Text = "File is executable.";
            // 
            // ExecutablePropertyEditor
            // 
            this.Controls.Add(this.executableTextBox);
            this.Name = "ExecutablePropertyEditor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ToolTip conflictToolTip;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TextBox executableTextBox;
    }
}
