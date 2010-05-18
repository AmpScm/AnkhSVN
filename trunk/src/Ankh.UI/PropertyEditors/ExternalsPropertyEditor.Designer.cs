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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExternalsPropertyEditor));
			this.externalGrid = new System.Windows.Forms.DataGridView();
			this.urlColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.buttonColumn = new System.Windows.Forms.DataGridViewButtonColumn();
			this.revisionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.revButtonColumn = new System.Windows.Forms.DataGridViewButtonColumn();
			this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(this.externalGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// externalGrid
			// 
			this.externalGrid.AllowUserToResizeRows = false;
			this.externalGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.externalGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
			this.externalGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			this.externalGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.externalGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.urlColumn,
            this.buttonColumn,
            this.revisionColumn,
            this.revButtonColumn,
            this.nameColumn});
			resources.ApplyResources(this.externalGrid, "externalGrid");
			this.externalGrid.MultiSelect = false;
			this.externalGrid.Name = "externalGrid";
			this.externalGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.externalGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
			this.externalGrid.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.externalGrid_RowValidating);
			this.externalGrid.RowValidated += new System.Windows.Forms.DataGridViewCellEventHandler(this.externalGrid_RowValidated);
			this.externalGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.externalGrid_CellContentClick);
			// 
			// urlColumn
			// 
			resources.ApplyResources(this.urlColumn, "urlColumn");
			this.urlColumn.Name = "urlColumn";
			// 
			// buttonColumn
			// 
			this.buttonColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
			this.buttonColumn.FillWeight = 30F;
			resources.ApplyResources(this.buttonColumn, "buttonColumn");
			this.buttonColumn.Name = "buttonColumn";
			this.buttonColumn.Text = "...";
			this.buttonColumn.UseColumnTextForButtonValue = true;
			// 
			// revisionColumn
			// 
			this.revisionColumn.FillWeight = 1F;
			resources.ApplyResources(this.revisionColumn, "revisionColumn");
			this.revisionColumn.Name = "revisionColumn";
			// 
			// revButtonColumn
			// 
			this.revButtonColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
			this.revButtonColumn.FillWeight = 30F;
			resources.ApplyResources(this.revButtonColumn, "revButtonColumn");
			this.revButtonColumn.Name = "revButtonColumn";
			this.revButtonColumn.Text = "...";
			this.revButtonColumn.UseColumnTextForButtonValue = true;
			// 
			// nameColumn
			// 
			this.nameColumn.FillWeight = 30F;
			resources.ApplyResources(this.nameColumn, "nameColumn");
			this.nameColumn.Name = "nameColumn";
			// 
			// ExternalsPropertyEditor
			// 
			this.Controls.Add(this.externalGrid);
			this.Name = "ExternalsPropertyEditor";
			((System.ComponentModel.ISupportInitialize)(this.externalGrid)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.DataGridView externalGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn urlColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn revisionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
        private System.Windows.Forms.DataGridViewButtonColumn buttonColumn;
        private System.Windows.Forms.DataGridViewButtonColumn revButtonColumn;
    }
}
