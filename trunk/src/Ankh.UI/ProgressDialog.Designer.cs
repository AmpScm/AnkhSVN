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

namespace Ankh.UI
{
    partial class ProgressDialog
    {
        private System.ComponentModel.IContainer components = null;


        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressDialog));
			this.cancelButton = new System.Windows.Forms.Button();
			this.actionList = new Ankh.UI.VSSelectionControls.SmartListView();
			this.actionColumn = new System.Windows.Forms.ColumnHeader();
			this.pathColumn = new System.Windows.Forms.ColumnHeader();
			this.progressLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Click += new System.EventHandler(this.CancelClick);
			// 
			// actionList
			// 
			resources.ApplyResources(this.actionList, "actionList");
			this.actionList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.actionColumn,
            this.pathColumn});
			this.actionList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.actionList.Name = "actionList";
			// 
			// actionColumn
			// 
			resources.ApplyResources(this.actionColumn, "actionColumn");
			// 
			// pathColumn
			// 
			resources.ApplyResources(this.pathColumn, "pathColumn");
			// 
			// progressLabel
			// 
			resources.ApplyResources(this.progressLabel, "progressLabel");
			this.progressLabel.Name = "progressLabel";
			// 
			// ProgressDialog
			// 
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.cancelButton;
			this.ControlBox = false;
			this.Controls.Add(this.progressLabel);
			this.Controls.Add(this.actionList);
			this.Controls.Add(this.cancelButton);
			this.Name = "ProgressDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.Button cancelButton;
        private Ankh.UI.VSSelectionControls.SmartListView actionList;
        private System.Windows.Forms.ColumnHeader actionColumn;
        private System.Windows.Forms.ColumnHeader pathColumn;
        private System.Windows.Forms.Label progressLabel;
    }
}
