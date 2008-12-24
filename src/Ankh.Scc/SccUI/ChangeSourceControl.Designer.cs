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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.connectButton = new System.Windows.Forms.ToolStripButton();
            this.disconnectButton = new System.Windows.Forms.ToolStripButton();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.projectRootLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.solutionRootBox = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sharedProjectUrlBrowse = new System.Windows.Forms.Button();
            this.sharedBasePathBrowse = new System.Windows.Forms.Button();
            this.sharedProjectBaseBrowse = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.sharedProjectUrlBox = new System.Windows.Forms.TextBox();
            this.sharedProjectLocationBox = new System.Windows.Forms.TextBox();
            this.sharedBasePathBox = new System.Windows.Forms.TextBox();
            this.sharedProjectBaseBox = new System.Windows.Forms.TextBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.userProjectLocationBrowse = new System.Windows.Forms.Button();
            this.userProjectBaseBrowse = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.userProjectUrlBox = new System.Windows.Forms.TextBox();
            this.userProjectLocationBox = new System.Windows.Forms.TextBox();
            this.userBasePathBox = new System.Windows.Forms.TextBox();
            this.userProjectBaseBox = new System.Windows.Forms.TextBox();
            this.solutionTab = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.ProjectColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UrlColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RegisteredColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.StatusColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.relativePathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.bindingGrid)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.solutionTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // bindingGrid
            // 
            this.bindingGrid.AllowUserToAddRows = false;
            this.bindingGrid.AllowUserToDeleteRows = false;
            this.bindingGrid.AllowUserToResizeRows = false;
            this.bindingGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bindingGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.bindingGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.bindingGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ProjectColumn,
            this.UrlColumn,
            this.RegisteredColumn,
            this.StatusColumn,
            this.PathColumn,
            this.relativePathColumn});
            this.bindingGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.bindingGrid.Location = new System.Drawing.Point(12, 28);
            this.bindingGrid.Name = "bindingGrid";
            this.bindingGrid.RowHeadersVisible = false;
            this.bindingGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.bindingGrid.ShowEditingIcon = false;
            this.bindingGrid.Size = new System.Drawing.Size(762, 261);
            this.bindingGrid.TabIndex = 1;
            this.bindingGrid.SelectionChanged += new System.EventHandler(this.bindingGrid_SelectionChanged);
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
            this.toolStrip1.Size = new System.Drawing.Size(786, 25);
            this.toolStrip1.TabIndex = 0;
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
            // projectRootLabel
            // 
            this.projectRootLabel.AutoSize = true;
            this.projectRootLabel.Location = new System.Drawing.Point(6, 18);
            this.projectRootLabel.Name = "projectRootLabel";
            this.projectRootLabel.Size = new System.Drawing.Size(74, 13);
            this.projectRootLabel.TabIndex = 2;
            this.projectRootLabel.Text = "&Solution Root:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(618, 502);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(537, 502);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 6;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(100, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(193, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "(Used for actions on the Solution Level)";
            // 
            // solutionRootBox
            // 
            this.solutionRootBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.solutionRootBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.solutionRootBox.FormattingEnabled = true;
            this.solutionRootBox.Location = new System.Drawing.Point(100, 15);
            this.solutionRootBox.Name = "solutionRootBox";
            this.solutionRootBox.Size = new System.Drawing.Size(648, 21);
            this.solutionRootBox.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(699, 502);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "&Help";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.solutionTab);
            this.tabControl1.Location = new System.Drawing.Point(12, 295);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(762, 189);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.sharedProjectUrlBrowse);
            this.tabPage1.Controls.Add(this.sharedBasePathBrowse);
            this.tabPage1.Controls.Add(this.sharedProjectBaseBrowse);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.sharedProjectUrlBox);
            this.tabPage1.Controls.Add(this.sharedProjectLocationBox);
            this.tabPage1.Controls.Add(this.sharedBasePathBox);
            this.tabPage1.Controls.Add(this.sharedProjectBaseBox);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(754, 163);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Shared Settings";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(94, 139);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(478, 13);
            this.label7.TabIndex = 17;
            this.label7.Text = "You can use the settings on the \'User Settings\' tab to only apply the settings to" +
                " the current checkout";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(94, 122);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(635, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Changes on this page are saved in the solution file. Some changes might cause oth" +
                "er workingcopies to lose track of external projects.";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 92);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "&Project Url:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 66);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "&Project Location:";
            // 
            // sharedProjectUrlBrowse
            // 
            this.sharedProjectUrlBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sharedProjectUrlBrowse.Location = new System.Drawing.Point(720, 88);
            this.sharedProjectUrlBrowse.Name = "sharedProjectUrlBrowse";
            this.sharedProjectUrlBrowse.Size = new System.Drawing.Size(28, 23);
            this.sharedProjectUrlBrowse.TabIndex = 12;
            this.sharedProjectUrlBrowse.Text = "...";
            this.sharedProjectUrlBrowse.UseVisualStyleBackColor = true;
            // 
            // sharedBasePathBrowse
            // 
            this.sharedBasePathBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sharedBasePathBrowse.Location = new System.Drawing.Point(720, 36);
            this.sharedBasePathBrowse.Name = "sharedBasePathBrowse";
            this.sharedBasePathBrowse.Size = new System.Drawing.Size(28, 23);
            this.sharedBasePathBrowse.TabIndex = 12;
            this.sharedBasePathBrowse.Text = "...";
            this.sharedBasePathBrowse.UseVisualStyleBackColor = true;
            // 
            // sharedProjectBaseBrowse
            // 
            this.sharedProjectBaseBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sharedProjectBaseBrowse.Location = new System.Drawing.Point(720, 10);
            this.sharedProjectBaseBrowse.Name = "sharedProjectBaseBrowse";
            this.sharedProjectBaseBrowse.Size = new System.Drawing.Size(28, 23);
            this.sharedProjectBaseBrowse.TabIndex = 13;
            this.sharedProjectBaseBrowse.Text = "...";
            this.sharedProjectBaseBrowse.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(59, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "&Base Path:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Project Base:";
            // 
            // sharedProjectUrlBox
            // 
            this.sharedProjectUrlBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sharedProjectUrlBox.Location = new System.Drawing.Point(97, 89);
            this.sharedProjectUrlBox.Name = "sharedProjectUrlBox";
            this.sharedProjectUrlBox.ReadOnly = true;
            this.sharedProjectUrlBox.Size = new System.Drawing.Size(617, 20);
            this.sharedProjectUrlBox.TabIndex = 7;
            // 
            // sharedProjectLocationBox
            // 
            this.sharedProjectLocationBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sharedProjectLocationBox.Location = new System.Drawing.Point(97, 63);
            this.sharedProjectLocationBox.Name = "sharedProjectLocationBox";
            this.sharedProjectLocationBox.ReadOnly = true;
            this.sharedProjectLocationBox.Size = new System.Drawing.Size(617, 20);
            this.sharedProjectLocationBox.TabIndex = 6;
            // 
            // sharedBasePathBox
            // 
            this.sharedBasePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sharedBasePathBox.Location = new System.Drawing.Point(97, 37);
            this.sharedBasePathBox.Name = "sharedBasePathBox";
            this.sharedBasePathBox.ReadOnly = true;
            this.sharedBasePathBox.Size = new System.Drawing.Size(617, 20);
            this.sharedBasePathBox.TabIndex = 9;
            // 
            // sharedProjectBaseBox
            // 
            this.sharedProjectBaseBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.sharedProjectBaseBox.Location = new System.Drawing.Point(97, 11);
            this.sharedProjectBaseBox.Name = "sharedProjectBaseBox";
            this.sharedProjectBaseBox.ReadOnly = true;
            this.sharedProjectBaseBox.Size = new System.Drawing.Size(617, 20);
            this.sharedProjectBaseBox.TabIndex = 8;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.label18);
            this.tabPage2.Controls.Add(this.label19);
            this.tabPage2.Controls.Add(this.label14);
            this.tabPage2.Controls.Add(this.label15);
            this.tabPage2.Controls.Add(this.userProjectLocationBrowse);
            this.tabPage2.Controls.Add(this.userProjectBaseBrowse);
            this.tabPage2.Controls.Add(this.label16);
            this.tabPage2.Controls.Add(this.label17);
            this.tabPage2.Controls.Add(this.userProjectUrlBox);
            this.tabPage2.Controls.Add(this.userProjectLocationBox);
            this.tabPage2.Controls.Add(this.userBasePathBox);
            this.tabPage2.Controls.Add(this.userProjectBaseBox);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(754, 163);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "User Settings";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(94, 139);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(441, 13);
            this.label18.TabIndex = 27;
            this.label18.Text = "You can use the settings on the \'Shared Settings\' tab to change the default setti" +
                "ngs globally.";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(94, 122);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(400, 13);
            this.label19.TabIndex = 26;
            this.label19.Text = "These settings only apply to the current working copy and are saved in the .suo f" +
                "ile.";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 92);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(59, 13);
            this.label14.TabIndex = 24;
            this.label14.Text = "&Project Url:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(6, 66);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(87, 13);
            this.label15.TabIndex = 25;
            this.label15.Text = "&Project Location:";
            // 
            // userProjectLocationBrowse
            // 
            this.userProjectLocationBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.userProjectLocationBrowse.Location = new System.Drawing.Point(720, 62);
            this.userProjectLocationBrowse.Name = "userProjectLocationBrowse";
            this.userProjectLocationBrowse.Size = new System.Drawing.Size(28, 23);
            this.userProjectLocationBrowse.TabIndex = 22;
            this.userProjectLocationBrowse.Text = "...";
            this.userProjectLocationBrowse.UseVisualStyleBackColor = true;
            // 
            // userProjectBaseBrowse
            // 
            this.userProjectBaseBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.userProjectBaseBrowse.Location = new System.Drawing.Point(720, 10);
            this.userProjectBaseBrowse.Name = "userProjectBaseBrowse";
            this.userProjectBaseBrowse.Size = new System.Drawing.Size(28, 23);
            this.userProjectBaseBrowse.TabIndex = 23;
            this.userProjectBaseBrowse.Text = "...";
            this.userProjectBaseBrowse.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(6, 40);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(59, 13);
            this.label16.TabIndex = 21;
            this.label16.Text = "&Base Path:";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 14);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(70, 13);
            this.label17.TabIndex = 20;
            this.label17.Text = "Project Base:";
            // 
            // userProjectUrlBox
            // 
            this.userProjectUrlBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.userProjectUrlBox.Location = new System.Drawing.Point(97, 89);
            this.userProjectUrlBox.Name = "userProjectUrlBox";
            this.userProjectUrlBox.ReadOnly = true;
            this.userProjectUrlBox.Size = new System.Drawing.Size(617, 20);
            this.userProjectUrlBox.TabIndex = 17;
            // 
            // userProjectLocationBox
            // 
            this.userProjectLocationBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.userProjectLocationBox.Location = new System.Drawing.Point(97, 63);
            this.userProjectLocationBox.Name = "userProjectLocationBox";
            this.userProjectLocationBox.ReadOnly = true;
            this.userProjectLocationBox.Size = new System.Drawing.Size(617, 20);
            this.userProjectLocationBox.TabIndex = 16;
            // 
            // userBasePathBox
            // 
            this.userBasePathBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.userBasePathBox.Location = new System.Drawing.Point(97, 37);
            this.userBasePathBox.Name = "userBasePathBox";
            this.userBasePathBox.ReadOnly = true;
            this.userBasePathBox.Size = new System.Drawing.Size(617, 20);
            this.userBasePathBox.TabIndex = 19;
            // 
            // userProjectBaseBox
            // 
            this.userProjectBaseBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.userProjectBaseBox.Location = new System.Drawing.Point(97, 11);
            this.userProjectBaseBox.Name = "userProjectBaseBox";
            this.userProjectBaseBox.ReadOnly = true;
            this.userProjectBaseBox.Size = new System.Drawing.Size(617, 20);
            this.userProjectBaseBox.TabIndex = 18;
            // 
            // solutionTab
            // 
            this.solutionTab.Controls.Add(this.label1);
            this.solutionTab.Controls.Add(this.projectRootLabel);
            this.solutionTab.Controls.Add(this.solutionRootBox);
            this.solutionTab.Location = new System.Drawing.Point(4, 22);
            this.solutionTab.Name = "solutionTab";
            this.solutionTab.Padding = new System.Windows.Forms.Padding(3);
            this.solutionTab.Size = new System.Drawing.Size(754, 163);
            this.solutionTab.TabIndex = 2;
            this.solutionTab.Text = "Solution Settings";
            this.solutionTab.UseVisualStyleBackColor = true;
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
            // textBox5
            // 
            this.textBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox5.Location = new System.Drawing.Point(97, 89);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(502, 20);
            this.textBox5.TabIndex = 7;
            // 
            // textBox6
            // 
            this.textBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox6.Location = new System.Drawing.Point(97, 63);
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(502, 20);
            this.textBox6.TabIndex = 6;
            // 
            // textBox7
            // 
            this.textBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox7.Location = new System.Drawing.Point(97, 37);
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(502, 20);
            this.textBox7.TabIndex = 9;
            // 
            // textBox8
            // 
            this.textBox8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox8.Location = new System.Drawing.Point(97, 11);
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.Size = new System.Drawing.Size(502, 20);
            this.textBox8.TabIndex = 8;
            // 
            // ProjectColumn
            // 
            this.ProjectColumn.HeaderText = "Solution/Project";
            this.ProjectColumn.Name = "ProjectColumn";
            this.ProjectColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ProjectColumn.Width = 89;
            // 
            // UrlColumn
            // 
            this.UrlColumn.HeaderText = "Url";
            this.UrlColumn.Name = "UrlColumn";
            this.UrlColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.UrlColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.UrlColumn.Width = 26;
            // 
            // RegisteredColumn
            // 
            this.RegisteredColumn.HeaderText = "Connected";
            this.RegisteredColumn.Name = "RegisteredColumn";
            this.RegisteredColumn.Width = 65;
            // 
            // StatusColumn
            // 
            this.StatusColumn.HeaderText = "Status";
            this.StatusColumn.Name = "StatusColumn";
            this.StatusColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.StatusColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.StatusColumn.Width = 43;
            // 
            // PathColumn
            // 
            this.PathColumn.HeaderText = "Base Path";
            this.PathColumn.Name = "PathColumn";
            this.PathColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.PathColumn.Width = 62;
            // 
            // relativePathColumn
            // 
            this.relativePathColumn.HeaderText = "Relative Path";
            this.relativePathColumn.Name = "relativePathColumn";
            this.relativePathColumn.Width = 96;
            // 
            // ChangeSourceControl
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(786, 537);
            this.Controls.Add(this.tabControl1);
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
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.solutionTab.ResumeLayout(false);
            this.solutionTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView bindingGrid;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton connectButton;
        private System.Windows.Forms.ToolStripButton disconnectButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.Label projectRootLabel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox solutionRootBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button sharedBasePathBrowse;
        private System.Windows.Forms.Button sharedProjectBaseBrowse;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox sharedProjectUrlBox;
        private System.Windows.Forms.TextBox sharedProjectLocationBox;
        private System.Windows.Forms.TextBox sharedBasePathBox;
        private System.Windows.Forms.TextBox sharedProjectBaseBox;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button userProjectLocationBrowse;
        private System.Windows.Forms.Button userProjectBaseBrowse;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox userProjectUrlBox;
        private System.Windows.Forms.TextBox userProjectLocationBox;
        private System.Windows.Forms.TextBox userBasePathBox;
        private System.Windows.Forms.TextBox userProjectBaseBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.Button sharedProjectUrlBrowse;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TabPage solutionTab;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProjectColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn UrlColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn RegisteredColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn StatusColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn PathColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn relativePathColumn;
    }
}
