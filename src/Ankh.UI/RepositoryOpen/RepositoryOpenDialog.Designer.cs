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

namespace Ankh.UI.RepositoryOpen
{
    partial class RepositoryOpenDialog
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepositoryOpenDialog));
			this.openButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.urlLabel = new System.Windows.Forms.ToolStripLabel();
			this.urlBox = new System.Windows.Forms.ToolStripComboBox();
			this.addButton = new System.Windows.Forms.ToolStripButton();
			this.refreshButton = new System.Windows.Forms.ToolStripButton();
			this.dirUpButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.versionButton = new System.Windows.Forms.ToolStripButton();
			this.panel1 = new System.Windows.Forms.Panel();
			this.dirView = new Ankh.UI.VSSelectionControls.SmartListView();
			this.label2 = new System.Windows.Forms.Label();
			this.fileNameBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.fileTypeBox = new System.Windows.Forms.ComboBox();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// openButton
			// 
			resources.ApplyResources(this.openButton, "openButton");
			this.openButton.Name = "openButton";
			this.openButton.UseVisualStyleBackColor = true;
			this.openButton.Click += new System.EventHandler(this.OnOkClicked);
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.UseVisualStyleBackColor = true;
			// 
			// toolStrip1
			// 
			resources.ApplyResources(this.toolStrip1, "toolStrip1");
			this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.urlLabel,
            this.urlBox,
            this.addButton,
            this.refreshButton,
            this.dirUpButton,
            this.toolStripSeparator,
            this.versionButton});
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
			// 
			// urlLabel
			// 
			resources.ApplyResources(this.urlLabel, "urlLabel");
			this.urlLabel.Name = "urlLabel";
			// 
			// urlBox
			// 
			this.urlBox.AutoToolTip = true;
			this.urlBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.urlBox.Name = "urlBox";
			this.urlBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
			resources.ApplyResources(this.urlBox, "urlBox");
			this.urlBox.SelectedIndexChanged += new System.EventHandler(this.urlBox_SelectedIndexChanged);
			// 
			// addButton
			// 
			this.addButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.addButton, "addButton");
			this.addButton.Name = "addButton";
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// refreshButton
			// 
			this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.refreshButton, "refreshButton");
			this.refreshButton.Name = "refreshButton";
			this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
			// 
			// dirUpButton
			// 
			this.dirUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.dirUpButton, "dirUpButton");
			this.dirUpButton.Name = "dirUpButton";
			this.dirUpButton.Click += new System.EventHandler(this.dirUpButton_Click);
			// 
			// toolStripSeparator
			// 
			this.toolStripSeparator.Name = "toolStripSeparator";
			resources.ApplyResources(this.toolStripSeparator, "toolStripSeparator");
			// 
			// versionButton
			// 
			resources.ApplyResources(this.versionButton, "versionButton");
			this.versionButton.Name = "versionButton";
			// 
			// panel1
			// 
			resources.ApplyResources(this.panel1, "panel1");
			this.panel1.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.panel1.Name = "panel1";
			// 
			// dirView
			// 
			resources.ApplyResources(this.dirView, "dirView");
			this.dirView.MultiSelect = false;
			this.dirView.Name = "dirView";
			this.dirView.View = System.Windows.Forms.View.List;
			this.dirView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dirView_MouseDoubleClick);
			this.dirView.SelectedIndexChanged += new System.EventHandler(this.dirView_SelectedIndexChanged);
			this.dirView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dirView_KeyDown);
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// fileNameBox
			// 
			resources.ApplyResources(this.fileNameBox, "fileNameBox");
			this.fileNameBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.fileNameBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.fileNameBox.Name = "fileNameBox";
			this.toolTip.SetToolTip(this.fileNameBox, resources.GetString("fileNameBox.ToolTip"));
			this.fileNameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fileNameBox_KeyDown);
			this.fileNameBox.Enter += new System.EventHandler(this.fileNameBox_Enter);
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// fileTypeBox
			// 
			resources.ApplyResources(this.fileTypeBox, "fileTypeBox");
			this.fileTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.fileTypeBox.FormattingEnabled = true;
			this.fileTypeBox.Items.AddRange(new object[] {
            resources.GetString("fileTypeBox.Items")});
			this.fileTypeBox.Name = "fileTypeBox";
			this.fileTypeBox.SelectedIndexChanged += new System.EventHandler(this.fileTypeBox_SelectedIndexChanged);
			// 
			// toolTip
			// 
			this.toolTip.IsBalloon = true;
			this.toolTip.StripAmpersands = true;
			this.toolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
			this.toolTip.ToolTipTitle = "Open from Subversion";
			// 
			// RepositoryOpenDialog
			// 
			this.AcceptButton = this.openButton;
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.fileTypeBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.fileNameBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.dirView);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.openButton);
			this.Name = "RepositoryOpenDialog";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.ToolStripButton dirUpButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripComboBox urlBox;
        private System.Windows.Forms.Panel panel1;
        private Ankh.UI.VSSelectionControls.SmartListView dirView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fileNameBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox fileTypeBox;
        private System.Windows.Forms.ToolStripButton versionButton;
        private System.Windows.Forms.ToolStripLabel urlLabel;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripButton addButton;
    }
}
