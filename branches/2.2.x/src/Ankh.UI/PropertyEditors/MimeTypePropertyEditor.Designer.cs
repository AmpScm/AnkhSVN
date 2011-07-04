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
    partial class MimeTypePropertyEditor
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MimeTypePropertyEditor));
			this.mimeTextBox = new System.Windows.Forms.TextBox();
			this.mimeGroupBox = new System.Windows.Forms.GroupBox();
			this.conflictToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.mimeGroupBox.SuspendLayout();
			this.SuspendLayout();
			// 
			// mimeTextBox
			// 
			resources.ApplyResources(this.mimeTextBox, "mimeTextBox");
			this.mimeTextBox.Name = "mimeTextBox";
			this.mimeTextBox.TextChanged += new System.EventHandler(this.mimeTextBox_TextChanged);
			// 
			// mimeGroupBox
			// 
			this.mimeGroupBox.Controls.Add(this.mimeTextBox);
			resources.ApplyResources(this.mimeGroupBox, "mimeGroupBox");
			this.mimeGroupBox.Name = "mimeGroupBox";
			this.mimeGroupBox.TabStop = false;
			// 
			// MimeTypePropertyEditor
			// 
			this.Controls.Add(this.mimeGroupBox);
			this.Name = "MimeTypePropertyEditor";
			this.mimeGroupBox.ResumeLayout(false);
			this.mimeGroupBox.PerformLayout();
			this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TextBox mimeTextBox;
        private System.Windows.Forms.GroupBox mimeGroupBox;
        private System.Windows.Forms.ToolTip conflictToolTip;
        private System.ComponentModel.IContainer components;

    }
}
