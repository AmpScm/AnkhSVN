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
    partial class PropertyEditorDialog
    {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertyEditorDialog));
            this.editButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.propGroupBox = new System.Windows.Forms.GroupBox();
            this.svnItemLabel = new System.Windows.Forms.Label();
            this.revertButton = new System.Windows.Forms.Button();
            this.propListView = new Ankh.UI.VSSelectionControls.SmartListView();
            this.nameColumn = new System.Windows.Forms.ColumnHeader();
            this.stateColumn = new System.Windows.Forms.ColumnHeader();
            this.baseValueColumn = new System.Windows.Forms.ColumnHeader();
            this.valueColumn = new System.Windows.Forms.ColumnHeader();
            this.propGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // editButton
            // 
            resources.ApplyResources(this.editButton, "editButton");
            this.editButton.Name = "editButton";
            this.editButton.Click += new System.EventHandler(this.editButton_Click);
            // 
            // deleteButton
            // 
            resources.ApplyResources(this.deleteButton, "deleteButton");
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            // 
            // addButton
            // 
            resources.ApplyResources(this.addButton, "addButton");
            this.addButton.Name = "addButton";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // propGroupBox
            // 
            resources.ApplyResources(this.propGroupBox, "propGroupBox");
            this.propGroupBox.Controls.Add(this.svnItemLabel);
            this.propGroupBox.Name = "propGroupBox";
            this.propGroupBox.TabStop = false;
            // 
            // svnItemLabel
            // 
            resources.ApplyResources(this.svnItemLabel, "svnItemLabel");
            this.svnItemLabel.AutoEllipsis = true;
            this.svnItemLabel.Name = "svnItemLabel";
            // 
            // revertButton
            // 
            resources.ApplyResources(this.revertButton, "revertButton");
            this.revertButton.Name = "revertButton";
            this.revertButton.Click += new System.EventHandler(this.revertButton_Click);
            // 
            // propListView
            // 
            resources.ApplyResources(this.propListView, "propListView");
            this.propListView.AutoArrange = false;
            this.propListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn,
            this.stateColumn,
            this.baseValueColumn,
            this.valueColumn});
            this.propListView.GridLines = true;
            this.propListView.HideSelection = false;
            this.propListView.Name = "propListView";
            this.propListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.propListView_MouseDoubleClick);
            this.propListView.SelectedIndexChanged += new System.EventHandler(this.propListView_SelectedIndexChanged);
            // 
            // nameColumn
            // 
            resources.ApplyResources(this.nameColumn, "nameColumn");
            // 
            // stateColumn
            // 
            resources.ApplyResources(this.stateColumn, "stateColumn");
            // 
            // baseValueColumn
            // 
            resources.ApplyResources(this.baseValueColumn, "baseValueColumn");
            // 
            // valueColumn
            // 
            resources.ApplyResources(this.valueColumn, "valueColumn");
            // 
            // PropertyEditorDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.propGroupBox);
            this.Controls.Add(this.propListView);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.revertButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.editButton);
            this.Name = "PropertyEditorDialog";
            this.propGroupBox.ResumeLayout(false);
            this.propGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }
		#endregion

        private System.Windows.Forms.ColumnHeader nameColumn;
        private System.Windows.Forms.ColumnHeader baseValueColumn;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button deleteButton;
        private Ankh.UI.VSSelectionControls.SmartListView propListView;
        private System.Windows.Forms.Button editButton;
        private System.Windows.Forms.Button addButton;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.GroupBox propGroupBox;
        private System.Windows.Forms.Label svnItemLabel;
        private System.Windows.Forms.ColumnHeader stateColumn;
        private System.Windows.Forms.ColumnHeader valueColumn;
        private System.Windows.Forms.Button revertButton;
    }
}
