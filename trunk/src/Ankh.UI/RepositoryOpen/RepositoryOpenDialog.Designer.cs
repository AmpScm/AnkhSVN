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
            this.openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openButton.Location = new System.Drawing.Point(489, 255);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(75, 23);
            this.openButton.TabIndex = 4;
            this.openButton.Text = "&Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.OnOkClicked);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(489, 284);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.urlLabel,
            this.urlBox,
            this.addButton,
            this.refreshButton,
            this.dirUpButton,
            this.toolStripSeparator,
            this.versionButton});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStrip1.Size = new System.Drawing.Size(573, 25);
            this.toolStrip1.TabIndex = 6;
            this.toolStrip1.Text = "Url:";
            // 
            // urlLabel
            // 
            this.urlLabel.AutoSize = false;
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(64, 22);
            this.urlLabel.Text = "&Url:";
            // 
            // urlBox
            // 
            this.urlBox.AutoToolTip = true;
            this.urlBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.urlBox.Name = "urlBox";
            this.urlBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.urlBox.Size = new System.Drawing.Size(380, 25);
            this.urlBox.SelectedIndexChanged += new System.EventHandler(this.urlBox_SelectedIndexChanged);
            // 
            // addButton
            // 
            this.addButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addButton.Image = ((System.Drawing.Image)(resources.GetObject("addButton.Image")));
            this.addButton.ImageTransparentColor = System.Drawing.Color.White;
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(23, 22);
            this.addButton.Text = "Add url";
            this.addButton.ToolTipText = "Add repository url";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Enabled = false;
            this.refreshButton.Image = ((System.Drawing.Image)(resources.GetObject("refreshButton.Image")));
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 22);
            this.refreshButton.Text = "&Refresh";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // dirUpButton
            // 
            this.dirUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.dirUpButton.Image = ((System.Drawing.Image)(resources.GetObject("dirUpButton.Image")));
            this.dirUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.dirUpButton.Name = "dirUpButton";
            this.dirUpButton.Size = new System.Drawing.Size(23, 22);
            this.dirUpButton.Text = "Up";
            this.dirUpButton.Click += new System.EventHandler(this.dirUpButton_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // versionButton
            // 
            this.versionButton.Image = ((System.Drawing.Image)(resources.GetObject("versionButton.Image")));
            this.versionButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.versionButton.Name = "versionButton";
            this.versionButton.Size = new System.Drawing.Size(55, 20);
            this.versionButton.Text = "Head";
            this.versionButton.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel1.Location = new System.Drawing.Point(12, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(57, 276);
            this.panel1.TabIndex = 7;
            // 
            // dirView
            // 
            this.dirView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dirView.Location = new System.Drawing.Point(77, 31);
            this.dirView.MultiSelect = false;
            this.dirView.Name = "dirView";
            this.dirView.Size = new System.Drawing.Size(484, 218);
            this.dirView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.dirView.TabIndex = 8;
            this.dirView.UseCompatibleStateImageBehavior = false;
            this.dirView.View = System.Windows.Forms.View.List;
            this.dirView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dirView_MouseDoubleClick);
            this.dirView.SelectedIndexChanged += new System.EventHandler(this.dirView_SelectedIndexChanged);
            this.dirView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dirView_KeyDown);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(76, 260);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "File &name:";
            // 
            // fileNameBox
            // 
            this.fileNameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.fileNameBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.fileNameBox.Location = new System.Drawing.Point(151, 257);
            this.fileNameBox.Name = "fileNameBox";
            this.fileNameBox.Size = new System.Drawing.Size(332, 20);
            this.fileNameBox.TabIndex = 1;
            this.toolTip.SetToolTip(this.fileNameBox, "Please type the url of a repository or file here");
            this.fileNameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fileNameBox_KeyDown);
            this.fileNameBox.Enter += new System.EventHandler(this.fileNameBox_Enter);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(76, 284);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Files of &type:";
            // 
            // fileTypeBox
            // 
            this.fileTypeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fileTypeBox.FormattingEnabled = true;
            this.fileTypeBox.Items.AddRange(new object[] {
            "All Files (*.*)"});
            this.fileTypeBox.Location = new System.Drawing.Point(151, 284);
            this.fileTypeBox.Name = "fileTypeBox";
            this.fileTypeBox.Size = new System.Drawing.Size(332, 21);
            this.fileTypeBox.TabIndex = 3;
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
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(573, 319);
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
            this.Text = "Open from Subversion";
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
