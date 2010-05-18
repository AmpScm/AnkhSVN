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
    partial class NeedsLockPropertyEditor
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NeedsLockPropertyEditor));
			this.needsLockToolTip = new System.Windows.Forms.ToolTip(this.components);
			this.needsLockTextBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// needsLockTextBox
			// 
			resources.ApplyResources(this.needsLockTextBox, "needsLockTextBox");
			this.needsLockTextBox.Name = "needsLockTextBox";
			this.needsLockTextBox.ReadOnly = true;
			// 
			// NeedsLockPropertyEditor
			// 
			this.Controls.Add(this.needsLockTextBox);
			this.Name = "NeedsLockPropertyEditor";
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.ToolTip needsLockToolTip;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.TextBox needsLockTextBox;
    }
}
