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
    partial class ExternalsPropertyEditor
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
            this.externalGrid = new System.Windows.Forms.DataGridView();
            this.urlColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.revisionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.externalGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // externalGrid
            // 
            this.externalGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.externalGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.externalGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.urlColumn,
            this.revisionColumn,
            this.nameColumn});
            this.externalGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.externalGrid.Location = new System.Drawing.Point(0, 0);
            this.externalGrid.MultiSelect = false;
            this.externalGrid.Name = "externalGrid";
            this.externalGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.externalGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.externalGrid.Size = new System.Drawing.Size(348, 196);
            this.externalGrid.TabIndex = 0;
            // 
            // urlColumn
            // 
            this.urlColumn.HeaderText = "Url";
            this.urlColumn.Name = "urlColumn";
            this.urlColumn.Width = 45;
            // 
            // revisionColumn
            // 
            this.revisionColumn.FillWeight = 1F;
            this.revisionColumn.HeaderText = "Revision";
            this.revisionColumn.Name = "revisionColumn";
            this.revisionColumn.Width = 73;
            // 
            // nameColumn
            // 
            this.nameColumn.FillWeight = 30F;
            this.nameColumn.HeaderText = "Name";
            this.nameColumn.Name = "nameColumn";
            this.nameColumn.Width = 60;
            // 
            // ExternalsPropertyEditor
            // 
            this.Controls.Add(this.externalGrid);
            this.Name = "ExternalsPropertyEditor";
            ((System.ComponentModel.ISupportInitialize)(this.externalGrid)).EndInit();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.ToolTip conflictToolTip;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.DataGridView externalGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn urlColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn revisionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
    }
}
