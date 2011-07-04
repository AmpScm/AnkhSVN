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

namespace Ankh.Scc.SccUI
{
    partial class ChangeSourceControl
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeSourceControl));
			this.bindingGrid = new System.Windows.Forms.DataGridView();
			this.RegisteredColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.ProjectColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.RepositoryColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.BindingColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.relativePathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.BindingPathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.connectButton = new System.Windows.Forms.ToolStripButton();
			this.disconnectButton = new System.Windows.Forms.ToolStripButton();
			this.refreshButton = new System.Windows.Forms.ToolStripButton();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.slnBindPath = new System.Windows.Forms.TextBox();
			this.settingsTabControl = new System.Windows.Forms.TabControl();
			this.sharedSettingsTab = new System.Windows.Forms.TabPage();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.sharedProjectUrlBrowse = new System.Windows.Forms.Button();
			this.sharedBasePathBrowse = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.shProjectUrl = new System.Windows.Forms.TextBox();
			this.shRelativePath = new System.Windows.Forms.TextBox();
			this.shBindPath = new System.Windows.Forms.TextBox();
			this.shProjectLocation = new System.Windows.Forms.TextBox();
			this.userSettingsTab = new System.Windows.Forms.TabPage();
			this.label18 = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.label15 = new System.Windows.Forms.Label();
			this.usProjectLocationBrowse = new System.Windows.Forms.Button();
			this.label16 = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.usProjectUrl = new System.Windows.Forms.TextBox();
			this.usRelativePath = new System.Windows.Forms.TextBox();
			this.usBindPath = new System.Windows.Forms.TextBox();
			this.usProjectLocation = new System.Windows.Forms.TextBox();
			this.solutionSettingsTab = new System.Windows.Forms.TabPage();
			this.slnBindBrowse = new System.Windows.Forms.Button();
			this.label22 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.label25 = new System.Windows.Forms.Label();
			this.slnRelativePath = new System.Windows.Forms.TextBox();
			this.slnBindUrl = new System.Windows.Forms.TextBox();
			this.slnProjectLocation = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.button4 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.bindingGrid)).BeginInit();
			this.toolStrip1.SuspendLayout();
			this.settingsTabControl.SuspendLayout();
			this.sharedSettingsTab.SuspendLayout();
			this.userSettingsTab.SuspendLayout();
			this.solutionSettingsTab.SuspendLayout();
			this.SuspendLayout();
			// 
			// bindingGrid
			// 
			this.bindingGrid.AllowUserToAddRows = false;
			this.bindingGrid.AllowUserToDeleteRows = false;
			this.bindingGrid.AllowUserToResizeColumns = false;
			this.bindingGrid.AllowUserToResizeRows = false;
			resources.ApplyResources(this.bindingGrid, "bindingGrid");
			this.bindingGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
			this.bindingGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
			this.bindingGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.RegisteredColumn,
            this.ProjectColumn,
            this.RepositoryColumn,
            this.BindingColumn,
            this.StatusColumn,
            this.relativePathColumn,
            this.BindingPathColumn});
			this.bindingGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
			this.bindingGrid.Name = "bindingGrid";
			this.bindingGrid.ReadOnly = true;
			this.bindingGrid.RowHeadersVisible = false;
			this.bindingGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.bindingGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.bindingGrid.ShowEditingIcon = false;
			this.bindingGrid.StandardTab = true;
			this.bindingGrid.SelectionChanged += new System.EventHandler(this.bindingGrid_SelectionChanged);
			// 
			// RegisteredColumn
			// 
			resources.ApplyResources(this.RegisteredColumn, "RegisteredColumn");
			this.RegisteredColumn.Name = "RegisteredColumn";
			this.RegisteredColumn.ReadOnly = true;
			// 
			// ProjectColumn
			// 
			resources.ApplyResources(this.ProjectColumn, "ProjectColumn");
			this.ProjectColumn.Name = "ProjectColumn";
			this.ProjectColumn.ReadOnly = true;
			this.ProjectColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// RepositoryColumn
			// 
			resources.ApplyResources(this.RepositoryColumn, "RepositoryColumn");
			this.RepositoryColumn.Name = "RepositoryColumn";
			this.RepositoryColumn.ReadOnly = true;
			this.RepositoryColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.RepositoryColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// BindingColumn
			// 
			resources.ApplyResources(this.BindingColumn, "BindingColumn");
			this.BindingColumn.Name = "BindingColumn";
			this.BindingColumn.ReadOnly = true;
			// 
			// StatusColumn
			// 
			resources.ApplyResources(this.StatusColumn, "StatusColumn");
			this.StatusColumn.Name = "StatusColumn";
			this.StatusColumn.ReadOnly = true;
			this.StatusColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.StatusColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// relativePathColumn
			// 
			resources.ApplyResources(this.relativePathColumn, "relativePathColumn");
			this.relativePathColumn.Name = "relativePathColumn";
			this.relativePathColumn.ReadOnly = true;
			// 
			// BindingPathColumn
			// 
			resources.ApplyResources(this.BindingPathColumn, "BindingPathColumn");
			this.BindingPathColumn.Name = "BindingPathColumn";
			this.BindingPathColumn.ReadOnly = true;
			this.BindingPathColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// toolStrip1
			// 
			resources.ApplyResources(this.toolStrip1, "toolStrip1");
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectButton,
            this.disconnectButton,
            this.refreshButton});
			this.toolStrip1.Name = "toolStrip1";
			// 
			// connectButton
			// 
			resources.ApplyResources(this.connectButton, "connectButton");
			this.connectButton.Name = "connectButton";
			this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
			// 
			// disconnectButton
			// 
			resources.ApplyResources(this.disconnectButton, "disconnectButton");
			this.disconnectButton.Name = "disconnectButton";
			this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
			// 
			// refreshButton
			// 
			resources.ApplyResources(this.refreshButton, "refreshButton");
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			// 
			// okButton
			// 
			resources.ApplyResources(this.okButton, "okButton");
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Name = "okButton";
			this.okButton.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// slnBindPath
			// 
			resources.ApplyResources(this.slnBindPath, "slnBindPath");
			this.slnBindPath.Name = "slnBindPath";
			this.slnBindPath.ReadOnly = true;
			// 
			// settingsTabControl
			// 
			resources.ApplyResources(this.settingsTabControl, "settingsTabControl");
			this.settingsTabControl.Controls.Add(this.sharedSettingsTab);
			this.settingsTabControl.Controls.Add(this.userSettingsTab);
			this.settingsTabControl.Controls.Add(this.solutionSettingsTab);
			this.settingsTabControl.Multiline = true;
			this.settingsTabControl.Name = "settingsTabControl";
			this.settingsTabControl.SelectedIndex = 0;
			// 
			// sharedSettingsTab
			// 
			this.sharedSettingsTab.Controls.Add(this.label7);
			this.sharedSettingsTab.Controls.Add(this.label6);
			this.sharedSettingsTab.Controls.Add(this.label5);
			this.sharedSettingsTab.Controls.Add(this.label2);
			this.sharedSettingsTab.Controls.Add(this.sharedProjectUrlBrowse);
			this.sharedSettingsTab.Controls.Add(this.sharedBasePathBrowse);
			this.sharedSettingsTab.Controls.Add(this.label4);
			this.sharedSettingsTab.Controls.Add(this.label3);
			this.sharedSettingsTab.Controls.Add(this.shProjectUrl);
			this.sharedSettingsTab.Controls.Add(this.shRelativePath);
			this.sharedSettingsTab.Controls.Add(this.shBindPath);
			this.sharedSettingsTab.Controls.Add(this.shProjectLocation);
			resources.ApplyResources(this.sharedSettingsTab, "sharedSettingsTab");
			this.sharedSettingsTab.Name = "sharedSettingsTab";
			this.sharedSettingsTab.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// sharedProjectUrlBrowse
			// 
			resources.ApplyResources(this.sharedProjectUrlBrowse, "sharedProjectUrlBrowse");
			this.sharedProjectUrlBrowse.Name = "sharedProjectUrlBrowse";
			this.sharedProjectUrlBrowse.UseVisualStyleBackColor = true;
			// 
			// sharedBasePathBrowse
			// 
			resources.ApplyResources(this.sharedBasePathBrowse, "sharedBasePathBrowse");
			this.sharedBasePathBrowse.Name = "sharedBasePathBrowse";
			this.sharedBasePathBrowse.UseVisualStyleBackColor = true;
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// shProjectUrl
			// 
			resources.ApplyResources(this.shProjectUrl, "shProjectUrl");
			this.shProjectUrl.Name = "shProjectUrl";
			this.shProjectUrl.ReadOnly = true;
			// 
			// shRelativePath
			// 
			resources.ApplyResources(this.shRelativePath, "shRelativePath");
			this.shRelativePath.Name = "shRelativePath";
			this.shRelativePath.ReadOnly = true;
			// 
			// shBindPath
			// 
			resources.ApplyResources(this.shBindPath, "shBindPath");
			this.shBindPath.Name = "shBindPath";
			this.shBindPath.ReadOnly = true;
			// 
			// shProjectLocation
			// 
			resources.ApplyResources(this.shProjectLocation, "shProjectLocation");
			this.shProjectLocation.Name = "shProjectLocation";
			this.shProjectLocation.ReadOnly = true;
			// 
			// userSettingsTab
			// 
			this.userSettingsTab.Controls.Add(this.label18);
			this.userSettingsTab.Controls.Add(this.label19);
			this.userSettingsTab.Controls.Add(this.label14);
			this.userSettingsTab.Controls.Add(this.label15);
			this.userSettingsTab.Controls.Add(this.usProjectLocationBrowse);
			this.userSettingsTab.Controls.Add(this.label16);
			this.userSettingsTab.Controls.Add(this.label17);
			this.userSettingsTab.Controls.Add(this.usProjectUrl);
			this.userSettingsTab.Controls.Add(this.usRelativePath);
			this.userSettingsTab.Controls.Add(this.usBindPath);
			this.userSettingsTab.Controls.Add(this.usProjectLocation);
			resources.ApplyResources(this.userSettingsTab, "userSettingsTab");
			this.userSettingsTab.Name = "userSettingsTab";
			this.userSettingsTab.UseVisualStyleBackColor = true;
			// 
			// label18
			// 
			resources.ApplyResources(this.label18, "label18");
			this.label18.Name = "label18";
			// 
			// label19
			// 
			resources.ApplyResources(this.label19, "label19");
			this.label19.Name = "label19";
			// 
			// label14
			// 
			resources.ApplyResources(this.label14, "label14");
			this.label14.Name = "label14";
			// 
			// label15
			// 
			resources.ApplyResources(this.label15, "label15");
			this.label15.Name = "label15";
			// 
			// usProjectLocationBrowse
			// 
			resources.ApplyResources(this.usProjectLocationBrowse, "usProjectLocationBrowse");
			this.usProjectLocationBrowse.Name = "usProjectLocationBrowse";
			this.usProjectLocationBrowse.UseVisualStyleBackColor = true;
			// 
			// label16
			// 
			resources.ApplyResources(this.label16, "label16");
			this.label16.Name = "label16";
			// 
			// label17
			// 
			resources.ApplyResources(this.label17, "label17");
			this.label17.Name = "label17";
			// 
			// usProjectUrl
			// 
			resources.ApplyResources(this.usProjectUrl, "usProjectUrl");
			this.usProjectUrl.Name = "usProjectUrl";
			this.usProjectUrl.ReadOnly = true;
			// 
			// usRelativePath
			// 
			resources.ApplyResources(this.usRelativePath, "usRelativePath");
			this.usRelativePath.Name = "usRelativePath";
			this.usRelativePath.ReadOnly = true;
			// 
			// usBindPath
			// 
			resources.ApplyResources(this.usBindPath, "usBindPath");
			this.usBindPath.Name = "usBindPath";
			this.usBindPath.ReadOnly = true;
			// 
			// usProjectLocation
			// 
			resources.ApplyResources(this.usProjectLocation, "usProjectLocation");
			this.usProjectLocation.Name = "usProjectLocation";
			this.usProjectLocation.ReadOnly = true;
			// 
			// solutionSettingsTab
			// 
			this.solutionSettingsTab.Controls.Add(this.slnBindBrowse);
			this.solutionSettingsTab.Controls.Add(this.label22);
			this.solutionSettingsTab.Controls.Add(this.label23);
			this.solutionSettingsTab.Controls.Add(this.label24);
			this.solutionSettingsTab.Controls.Add(this.label25);
			this.solutionSettingsTab.Controls.Add(this.slnRelativePath);
			this.solutionSettingsTab.Controls.Add(this.slnBindUrl);
			this.solutionSettingsTab.Controls.Add(this.slnProjectLocation);
			this.solutionSettingsTab.Controls.Add(this.label1);
			this.solutionSettingsTab.Controls.Add(this.slnBindPath);
			resources.ApplyResources(this.solutionSettingsTab, "solutionSettingsTab");
			this.solutionSettingsTab.Name = "solutionSettingsTab";
			this.solutionSettingsTab.UseVisualStyleBackColor = true;
			// 
			// slnBindBrowse
			// 
			resources.ApplyResources(this.slnBindBrowse, "slnBindBrowse");
			this.slnBindBrowse.Name = "slnBindBrowse";
			this.slnBindBrowse.UseVisualStyleBackColor = true;
			this.slnBindBrowse.Click += new System.EventHandler(this.slnBindBrowse_Click);
			// 
			// label22
			// 
			resources.ApplyResources(this.label22, "label22");
			this.label22.Name = "label22";
			// 
			// label23
			// 
			resources.ApplyResources(this.label23, "label23");
			this.label23.Name = "label23";
			// 
			// label24
			// 
			resources.ApplyResources(this.label24, "label24");
			this.label24.Name = "label24";
			// 
			// label25
			// 
			resources.ApplyResources(this.label25, "label25");
			this.label25.Name = "label25";
			// 
			// slnRelativePath
			// 
			resources.ApplyResources(this.slnRelativePath, "slnRelativePath");
			this.slnRelativePath.Name = "slnRelativePath";
			this.slnRelativePath.ReadOnly = true;
			// 
			// slnBindUrl
			// 
			resources.ApplyResources(this.slnBindUrl, "slnBindUrl");
			this.slnBindUrl.Name = "slnBindUrl";
			this.slnBindUrl.ReadOnly = true;
			// 
			// slnProjectLocation
			// 
			resources.ApplyResources(this.slnProjectLocation, "slnProjectLocation");
			this.slnProjectLocation.Name = "slnProjectLocation";
			this.slnProjectLocation.ReadOnly = true;
			// 
			// label8
			// 
			resources.ApplyResources(this.label8, "label8");
			this.label8.Name = "label8";
			// 
			// label9
			// 
			resources.ApplyResources(this.label9, "label9");
			this.label9.Name = "label9";
			// 
			// label10
			// 
			resources.ApplyResources(this.label10, "label10");
			this.label10.Name = "label10";
			// 
			// label11
			// 
			resources.ApplyResources(this.label11, "label11");
			this.label11.Name = "label11";
			// 
			// button4
			// 
			resources.ApplyResources(this.button4, "button4");
			this.button4.Name = "button4";
			this.button4.UseVisualStyleBackColor = true;
			// 
			// button5
			// 
			resources.ApplyResources(this.button5, "button5");
			this.button5.Name = "button5";
			this.button5.UseVisualStyleBackColor = true;
			// 
			// label12
			// 
			resources.ApplyResources(this.label12, "label12");
			this.label12.Name = "label12";
			// 
			// label13
			// 
			resources.ApplyResources(this.label13, "label13");
			this.label13.Name = "label13";
			// 
			// ChangeSourceControl
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.settingsTabControl);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.bindingGrid);
			this.Name = "ChangeSourceControl";
			((System.ComponentModel.ISupportInitialize)(this.bindingGrid)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.settingsTabControl.ResumeLayout(false);
			this.sharedSettingsTab.ResumeLayout(false);
			this.sharedSettingsTab.PerformLayout();
			this.userSettingsTab.ResumeLayout(false);
			this.userSettingsTab.PerformLayout();
			this.solutionSettingsTab.ResumeLayout(false);
			this.solutionSettingsTab.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView bindingGrid;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton connectButton;
        private System.Windows.Forms.ToolStripButton disconnectButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox slnBindPath;
        private System.Windows.Forms.TabControl settingsTabControl;
        private System.Windows.Forms.TabPage sharedSettingsTab;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button sharedBasePathBrowse;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox shProjectUrl;
        private System.Windows.Forms.TextBox shRelativePath;
        private System.Windows.Forms.TextBox shBindPath;
        private System.Windows.Forms.TextBox shProjectLocation;
        private System.Windows.Forms.TabPage userSettingsTab;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button usProjectLocationBrowse;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox usProjectUrl;
        private System.Windows.Forms.TextBox usRelativePath;
        private System.Windows.Forms.TextBox usBindPath;
        private System.Windows.Forms.TextBox usProjectLocation;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button sharedProjectUrlBrowse;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TabPage solutionSettingsTab;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox slnRelativePath;
        private System.Windows.Forms.TextBox slnBindUrl;
        private System.Windows.Forms.TextBox slnProjectLocation;
        private System.Windows.Forms.Button slnBindBrowse;
        private System.Windows.Forms.DataGridViewCheckBoxColumn RegisteredColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProjectColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn RepositoryColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BindingColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn relativePathColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BindingPathColumn;
    }
}
