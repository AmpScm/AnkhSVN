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
            this.button1 = new System.Windows.Forms.Button();
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
            this.userProjectLocationBrowse = new System.Windows.Forms.Button();
            this.userProjectBaseBrowse = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.usProjectUrl = new System.Windows.Forms.TextBox();
            this.usRelativePath = new System.Windows.Forms.TextBox();
            this.usBindPath = new System.Windows.Forms.TextBox();
            this.usProjectLocation = new System.Windows.Forms.TextBox();
            this.solutionSettingsTab = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
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
            this.bindingGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
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
            this.bindingGrid.Location = new System.Drawing.Point(12, 25);
            this.bindingGrid.Name = "bindingGrid";
            this.bindingGrid.ReadOnly = true;
            this.bindingGrid.RowHeadersVisible = false;
            this.bindingGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.bindingGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.bindingGrid.ShowEditingIcon = false;
            this.bindingGrid.Size = new System.Drawing.Size(763, 197);
            this.bindingGrid.StandardTab = true;
            this.bindingGrid.TabIndex = 0;
            this.bindingGrid.SelectionChanged += new System.EventHandler(this.bindingGrid_SelectionChanged);
            // 
            // RegisteredColumn
            // 
            this.RegisteredColumn.HeaderText = "Connected";
            this.RegisteredColumn.Name = "RegisteredColumn";
            this.RegisteredColumn.ReadOnly = true;
            this.RegisteredColumn.Width = 65;
            // 
            // ProjectColumn
            // 
            this.ProjectColumn.HeaderText = "Solution/Project";
            this.ProjectColumn.Name = "ProjectColumn";
            this.ProjectColumn.ReadOnly = true;
            this.ProjectColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ProjectColumn.Width = 89;
            // 
            // RepositoryColumn
            // 
            this.RepositoryColumn.HeaderText = "Repository";
            this.RepositoryColumn.Name = "RepositoryColumn";
            this.RepositoryColumn.ReadOnly = true;
            this.RepositoryColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.RepositoryColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.RepositoryColumn.Width = 63;
            // 
            // BindingColumn
            // 
            this.BindingColumn.HeaderText = "SCC Binding";
            this.BindingColumn.Name = "BindingColumn";
            this.BindingColumn.ReadOnly = true;
            this.BindingColumn.Width = 91;
            // 
            // StatusColumn
            // 
            this.StatusColumn.HeaderText = "Status";
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.ReadOnly = true;
            this.StatusColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.StatusColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.StatusColumn.Width = 43;
            // 
            // relativePathColumn
            // 
            this.relativePathColumn.HeaderText = "Relative Path";
            this.relativePathColumn.Name = "relativePathColumn";
            this.relativePathColumn.ReadOnly = true;
            this.relativePathColumn.Width = 96;
            // 
            // BindingPathColumn
            // 
            this.BindingPathColumn.HeaderText = "Binding Path";
            this.BindingPathColumn.Name = "BindingPathColumn";
            this.BindingPathColumn.ReadOnly = true;
            this.BindingPathColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.BindingPathColumn.Width = 73;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectButton,
            this.disconnectButton,
            this.refreshButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(787, 25);
            this.toolStrip1.TabIndex = 5;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // connectButton
            // 
            this.connectButton.Image = ((System.Drawing.Image)(resources.GetObject("connectButton.Image")));
            this.connectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(72, 22);
            this.connectButton.Text = "C&onnect";
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Image = ((System.Drawing.Image)(resources.GetObject("disconnectButton.Image")));
            this.disconnectButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(86, 22);
            this.disconnectButton.Text = "&Disconnect";
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.Image = ((System.Drawing.Image)(resources.GetObject("refreshButton.Image")));
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(66, 22);
            this.refreshButton.Text = "&Refresh";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(619, 415);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(538, 415);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(107, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(473, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "The Binding specified here is used for actions on the solution level (including o" +
                "pen from subversion)";
            // 
            // slnBindPath
            // 
            this.slnBindPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.slnBindPath.Location = new System.Drawing.Point(107, 37);
            this.slnBindPath.Name = "slnBindPath";
            this.slnBindPath.ReadOnly = true;
            this.slnBindPath.Size = new System.Drawing.Size(569, 20);
            this.slnBindPath.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(700, 415);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "&Help";
            // 
            // settingsTabControl
            // 
            this.settingsTabControl.Alignment = System.Windows.Forms.TabAlignment.Left;
            this.settingsTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.settingsTabControl.Controls.Add(this.sharedSettingsTab);
            this.settingsTabControl.Controls.Add(this.userSettingsTab);
            this.settingsTabControl.Controls.Add(this.solutionSettingsTab);
            this.settingsTabControl.Location = new System.Drawing.Point(12, 224);
            this.settingsTabControl.Multiline = true;
            this.settingsTabControl.Name = "settingsTabControl";
            this.settingsTabControl.SelectedIndex = 0;
            this.settingsTabControl.Size = new System.Drawing.Size(763, 185);
            this.settingsTabControl.TabIndex = 1;
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
            this.sharedSettingsTab.Location = new System.Drawing.Point(42, 4);
            this.sharedSettingsTab.Name = "sharedSettingsTab";
            this.sharedSettingsTab.Padding = new System.Windows.Forms.Padding(3);
            this.sharedSettingsTab.Size = new System.Drawing.Size(717, 177);
            this.sharedSettingsTab.TabIndex = 0;
            this.sharedSettingsTab.Text = "Shared Settings";
            this.sharedSettingsTab.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(107, 137);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(499, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "You can use the settings on the \'User Settings\' tab to only apply the settings to" +
                " the current working copy.";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(107, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(574, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Changes on this page are saved in the solution file. Some changes will be applied" +
                " to other working copies after updating.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(85, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "SCC Binding &Url:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "SCC &Relative Path:";
            // 
            // sharedProjectUrlBrowse
            // 
            this.sharedProjectUrlBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sharedProjectUrlBrowse.Location = new System.Drawing.Point(683, 88);
            this.sharedProjectUrlBrowse.Name = "sharedProjectUrlBrowse";
            this.sharedProjectUrlBrowse.Size = new System.Drawing.Size(28, 23);
            this.sharedProjectUrlBrowse.TabIndex = 9;
            this.sharedProjectUrlBrowse.Text = "...";
            this.sharedProjectUrlBrowse.UseVisualStyleBackColor = true;
            // 
            // sharedBasePathBrowse
            // 
            this.sharedBasePathBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sharedBasePathBrowse.Location = new System.Drawing.Point(683, 36);
            this.sharedBasePathBrowse.Name = "sharedBasePathBrowse";
            this.sharedBasePathBrowse.Size = new System.Drawing.Size(28, 23);
            this.sharedBasePathBrowse.TabIndex = 4;
            this.sharedBasePathBrowse.Text = "...";
            this.sharedBasePathBrowse.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(94, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "SCC &Binding Path:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Project &Location:";
            // 
            // shProjectUrl
            // 
            this.shProjectUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.shProjectUrl.Location = new System.Drawing.Point(107, 89);
            this.shProjectUrl.Name = "shProjectUrl";
            this.shProjectUrl.ReadOnly = true;
            this.shProjectUrl.Size = new System.Drawing.Size(570, 20);
            this.shProjectUrl.TabIndex = 8;
            // 
            // shRelativePath
            // 
            this.shRelativePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.shRelativePath.Location = new System.Drawing.Point(107, 63);
            this.shRelativePath.Name = "shRelativePath";
            this.shRelativePath.ReadOnly = true;
            this.shRelativePath.Size = new System.Drawing.Size(570, 20);
            this.shRelativePath.TabIndex = 6;
            // 
            // shBindPath
            // 
            this.shBindPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.shBindPath.Location = new System.Drawing.Point(107, 37);
            this.shBindPath.Name = "shBindPath";
            this.shBindPath.ReadOnly = true;
            this.shBindPath.Size = new System.Drawing.Size(570, 20);
            this.shBindPath.TabIndex = 3;
            // 
            // shProjectLocation
            // 
            this.shProjectLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.shProjectLocation.Location = new System.Drawing.Point(107, 11);
            this.shProjectLocation.Name = "shProjectLocation";
            this.shProjectLocation.ReadOnly = true;
            this.shProjectLocation.Size = new System.Drawing.Size(570, 20);
            this.shProjectLocation.TabIndex = 1;
            // 
            // userSettingsTab
            // 
            this.userSettingsTab.Controls.Add(this.label18);
            this.userSettingsTab.Controls.Add(this.label19);
            this.userSettingsTab.Controls.Add(this.label14);
            this.userSettingsTab.Controls.Add(this.label15);
            this.userSettingsTab.Controls.Add(this.userProjectLocationBrowse);
            this.userSettingsTab.Controls.Add(this.userProjectBaseBrowse);
            this.userSettingsTab.Controls.Add(this.label16);
            this.userSettingsTab.Controls.Add(this.label17);
            this.userSettingsTab.Controls.Add(this.usProjectUrl);
            this.userSettingsTab.Controls.Add(this.usRelativePath);
            this.userSettingsTab.Controls.Add(this.usBindPath);
            this.userSettingsTab.Controls.Add(this.usProjectLocation);
            this.userSettingsTab.Location = new System.Drawing.Point(42, 4);
            this.userSettingsTab.Name = "userSettingsTab";
            this.userSettingsTab.Padding = new System.Windows.Forms.Padding(3);
            this.userSettingsTab.Size = new System.Drawing.Size(717, 177);
            this.userSettingsTab.TabIndex = 1;
            this.userSettingsTab.Text = "User Settings";
            this.userSettingsTab.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(107, 137);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(441, 13);
            this.label18.TabIndex = 11;
            this.label18.Text = "You can use the settings on the \'Shared Settings\' tab to change the default setti" +
                "ngs globally.";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(107, 117);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(400, 13);
            this.label19.TabIndex = 10;
            this.label19.Text = "These settings only apply to the current working copy and are saved in the .suo f" +
                "ile.";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 92);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(85, 13);
            this.label14.TabIndex = 8;
            this.label14.Text = "SCC Binding &Url:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 66);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(98, 13);
            this.label15.TabIndex = 6;
            this.label15.Text = "SCC &Relative Path:";
            // 
            // userProjectLocationBrowse
            // 
            this.userProjectLocationBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.userProjectLocationBrowse.Location = new System.Drawing.Point(682, 36);
            this.userProjectLocationBrowse.Name = "userProjectLocationBrowse";
            this.userProjectLocationBrowse.Size = new System.Drawing.Size(28, 23);
            this.userProjectLocationBrowse.TabIndex = 5;
            this.userProjectLocationBrowse.Text = "...";
            this.userProjectLocationBrowse.UseVisualStyleBackColor = true;
            // 
            // userProjectBaseBrowse
            // 
            this.userProjectBaseBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.userProjectBaseBrowse.Location = new System.Drawing.Point(682, 10);
            this.userProjectBaseBrowse.Name = "userProjectBaseBrowse";
            this.userProjectBaseBrowse.Size = new System.Drawing.Size(28, 23);
            this.userProjectBaseBrowse.TabIndex = 2;
            this.userProjectBaseBrowse.Text = "...";
            this.userProjectBaseBrowse.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 40);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(94, 13);
            this.label16.TabIndex = 3;
            this.label16.Text = "SCC &Binding Path:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 14);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(87, 13);
            this.label17.TabIndex = 0;
            this.label17.Text = "Project &Location:";
            // 
            // usProjectUrl
            // 
            this.usProjectUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.usProjectUrl.Location = new System.Drawing.Point(107, 89);
            this.usProjectUrl.Name = "usProjectUrl";
            this.usProjectUrl.ReadOnly = true;
            this.usProjectUrl.Size = new System.Drawing.Size(569, 20);
            this.usProjectUrl.TabIndex = 9;
            // 
            // usRelativePath
            // 
            this.usRelativePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.usRelativePath.Location = new System.Drawing.Point(107, 63);
            this.usRelativePath.Name = "usRelativePath";
            this.usRelativePath.ReadOnly = true;
            this.usRelativePath.Size = new System.Drawing.Size(569, 20);
            this.usRelativePath.TabIndex = 7;
            // 
            // usBindPath
            // 
            this.usBindPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.usBindPath.Location = new System.Drawing.Point(107, 37);
            this.usBindPath.Name = "usBindPath";
            this.usBindPath.ReadOnly = true;
            this.usBindPath.Size = new System.Drawing.Size(569, 20);
            this.usBindPath.TabIndex = 4;
            // 
            // usProjectLocation
            // 
            this.usProjectLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.usProjectLocation.Location = new System.Drawing.Point(107, 11);
            this.usProjectLocation.Name = "usProjectLocation";
            this.usProjectLocation.ReadOnly = true;
            this.usProjectLocation.Size = new System.Drawing.Size(569, 20);
            this.usProjectLocation.TabIndex = 1;
            // 
            // solutionSettingsTab
            // 
            this.solutionSettingsTab.Controls.Add(this.button2);
            this.solutionSettingsTab.Controls.Add(this.label22);
            this.solutionSettingsTab.Controls.Add(this.label23);
            this.solutionSettingsTab.Controls.Add(this.label24);
            this.solutionSettingsTab.Controls.Add(this.label25);
            this.solutionSettingsTab.Controls.Add(this.slnRelativePath);
            this.solutionSettingsTab.Controls.Add(this.slnBindUrl);
            this.solutionSettingsTab.Controls.Add(this.slnProjectLocation);
            this.solutionSettingsTab.Controls.Add(this.label1);
            this.solutionSettingsTab.Controls.Add(this.slnBindPath);
            this.solutionSettingsTab.Location = new System.Drawing.Point(42, 4);
            this.solutionSettingsTab.Name = "solutionSettingsTab";
            this.solutionSettingsTab.Padding = new System.Windows.Forms.Padding(3);
            this.solutionSettingsTab.Size = new System.Drawing.Size(717, 177);
            this.solutionSettingsTab.TabIndex = 2;
            this.solutionSettingsTab.Text = "Solution Settings";
            this.solutionSettingsTab.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(682, 36);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(28, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "...";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 92);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(85, 13);
            this.label22.TabIndex = 7;
            this.label22.Text = "SCC Binding &Url:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(6, 66);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(98, 13);
            this.label23.TabIndex = 5;
            this.label23.Text = "SCC &Relative Path:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(6, 40);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(94, 13);
            this.label24.TabIndex = 2;
            this.label24.Text = "SCC &Binding Path:";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(6, 14);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(92, 13);
            this.label25.TabIndex = 0;
            this.label25.Text = "Solution &Location:";
            // 
            // slnRelativePath
            // 
            this.slnRelativePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.slnRelativePath.Location = new System.Drawing.Point(107, 63);
            this.slnRelativePath.Name = "slnRelativePath";
            this.slnRelativePath.ReadOnly = true;
            this.slnRelativePath.Size = new System.Drawing.Size(569, 20);
            this.slnRelativePath.TabIndex = 6;
            // 
            // slnBindUrl
            // 
            this.slnBindUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.slnBindUrl.Location = new System.Drawing.Point(107, 89);
            this.slnBindUrl.Name = "slnBindUrl";
            this.slnBindUrl.ReadOnly = true;
            this.slnBindUrl.Size = new System.Drawing.Size(569, 20);
            this.slnBindUrl.TabIndex = 8;
            // 
            // slnProjectLocation
            // 
            this.slnProjectLocation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.slnProjectLocation.Location = new System.Drawing.Point(107, 11);
            this.slnProjectLocation.Name = "slnProjectLocation";
            this.slnProjectLocation.ReadOnly = true;
            this.slnProjectLocation.Size = new System.Drawing.Size(569, 20);
            this.slnProjectLocation.TabIndex = 1;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(94, 142);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(478, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "You can use the settings on the \'User Settings\' tab to only apply the settings to" +
                " the current checkout";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(94, 125);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(410, 13);
            this.label9.TabIndex = 16;
            this.label9.Text = "Committing changes of these settings may cause recheckouts in other workingcopies" +
                ".";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 92);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(59, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "&Project Url:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 66);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(87, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "&Project Location:";
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.Location = new System.Drawing.Point(605, 35);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(28, 23);
            this.button4.TabIndex = 12;
            this.button4.Text = "...";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button5.Location = new System.Drawing.Point(605, 9);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(28, 23);
            this.button5.TabIndex = 13;
            this.button5.Text = "...";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 40);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(59, 13);
            this.label12.TabIndex = 11;
            this.label12.Text = "&Base Path:";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(6, 14);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(70, 13);
            this.label13.TabIndex = 10;
            this.label13.Text = "Project Base:";
            // 
            // ChangeSourceControl
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(787, 450);
            this.Controls.Add(this.settingsTabControl);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.bindingGrid);
            this.Name = "ChangeSourceControl";
            this.Text = "Change Source Control";
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
        private System.Windows.Forms.Button button1;
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
        private System.Windows.Forms.Button userProjectLocationBrowse;
        private System.Windows.Forms.Button userProjectBaseBrowse;
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
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn RegisteredColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProjectColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn RepositoryColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BindingColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn relativePathColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn BindingPathColumn;
    }
}
