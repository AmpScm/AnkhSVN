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
    partial class IgnorePropertyEditor
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IgnorePropertyEditor));
			this.ignoreGroupBox = new System.Windows.Forms.GroupBox();
			this.ignoreTextBox = new System.Windows.Forms.TextBox();
			this.conflictToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.ignoreGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// ignoreGroupBox
			// 
			this.ignoreGroupBox.Controls.Add(this.ignoreTextBox);
			resources.ApplyResources(this.ignoreGroupBox, "ignoreGroupBox");
			this.ignoreGroupBox.Name = "ignoreGroupBox";
			this.ignoreGroupBox.TabStop = false;
			// 
			// ignoreTextBox
			// 
			this.ignoreTextBox.AcceptsReturn = true;
			this.ignoreTextBox.AcceptsTab = true;
			resources.ApplyResources(this.ignoreTextBox, "ignoreTextBox");
			this.ignoreTextBox.Name = "ignoreTextBox";
			this.ignoreTextBox.TextChanged += new System.EventHandler(this.ignoreTextBox_TextChanged);
			// 
			// IgnorePropertyEditor
			// 
			this.Controls.Add(this.ignoreGroupBox);
			this.Name = "IgnorePropertyEditor";
			this.ignoreGroupBox.ResumeLayout(false);
			this.ignoreGroupBox.PerformLayout();
			this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.GroupBox ignoreGroupBox;
        private System.Windows.Forms.ToolTip conflictToolTip;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TextBox ignoreTextBox;
        
    }
}
