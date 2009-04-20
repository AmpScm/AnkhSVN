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

namespace Ankh.UI.PendingChanges
{
    partial class PendingCommitsPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.topLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.issueLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.logMessageEditor = new Ankh.UI.PendingChanges.LogMessageEditor(this.components);
            this.issueNumberBox = new System.Windows.Forms.TextBox();
            this.lastRevLabel = new System.Windows.Forms.Label();
            this.lastRevBox = new System.Windows.Forms.TextBox();
            this.pendingCommits = new Ankh.UI.PendingChanges.Commits.PendingCommitsView(this.components);
            this.pathColumn = new System.Windows.Forms.ColumnHeader();
            this.projectColumn = new System.Windows.Forms.ColumnHeader();
            this.changeColumn = new System.Windows.Forms.ColumnHeader();
            this.fullPathColumn = new System.Windows.Forms.ColumnHeader();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.topLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.panel1);
            this.splitContainer.Panel1MinSize = 10;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pendingCommits);
            this.splitContainer.Panel2MinSize = 10;
            this.splitContainer.Size = new System.Drawing.Size(768, 300);
            this.splitContainer.SplitterDistance = 83;
            this.splitContainer.SplitterWidth = 3;
            this.splitContainer.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.topLayoutPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(768, 83);
            this.panel1.TabIndex = 1;
            // 
            // topLayoutPanel
            // 
            this.topLayoutPanel.ColumnCount = 5;
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 145F));
            this.topLayoutPanel.Controls.Add(this.issueLabel, 3, 0);
            this.topLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.topLayoutPanel.Controls.Add(this.logMessageEditor, 0, 1);
            this.topLayoutPanel.Controls.Add(this.issueNumberBox, 4, 0);
            this.topLayoutPanel.Controls.Add(this.lastRevLabel, 1, 0);
            this.topLayoutPanel.Controls.Add(this.lastRevBox, 2, 0);
            this.topLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.topLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.topLayoutPanel.Name = "topLayoutPanel";
            this.topLayoutPanel.RowCount = 2;
            this.topLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.topLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topLayoutPanel.Size = new System.Drawing.Size(766, 81);
            this.topLayoutPanel.TabIndex = 0;
            // 
            // issueLabel
            // 
            this.issueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.issueLabel.AutoSize = true;
            this.issueLabel.Enabled = false;
            this.issueLabel.Location = new System.Drawing.Point(583, 3);
            this.issueLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.issueLabel.Name = "issueLabel";
            this.issueLabel.Size = new System.Drawing.Size(35, 13);
            this.issueLabel.TabIndex = 3;
            this.issueLabel.Text = "&Issue:";
            this.issueLabel.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Message:";
            // 
            // logMessageEditor
            // 
            this.topLayoutPanel.SetColumnSpan(this.logMessageEditor, 5);
            this.logMessageEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMessageEditor.Location = new System.Drawing.Point(0, 20);
            this.logMessageEditor.Margin = new System.Windows.Forms.Padding(0);
            this.logMessageEditor.Name = "logMessageEditor";
            this.logMessageEditor.Size = new System.Drawing.Size(766, 61);
            this.logMessageEditor.TabIndex = 4;
            this.logMessageEditor.Text = null;
            // 
            // issueNumberBox
            // 
            this.issueNumberBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.issueNumberBox.Enabled = false;
            this.issueNumberBox.Location = new System.Drawing.Point(621, 0);
            this.issueNumberBox.Margin = new System.Windows.Forms.Padding(0);
            this.issueNumberBox.Name = "issueNumberBox";
            this.issueNumberBox.Size = new System.Drawing.Size(144, 20);
            this.issueNumberBox.TabIndex = 0;
            this.issueNumberBox.Visible = false;
            this.issueNumberBox.TextChanged += new System.EventHandler(this.issueNumberBox_TextChanged);
            this.issueNumberBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.issueNumberBox_KeyPress);
            // 
            // lastRevLabel
            // 
            this.lastRevLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lastRevLabel.AutoSize = true;
            this.lastRevLabel.Location = new System.Drawing.Point(237, 3);
            this.lastRevLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.lastRevLabel.Name = "lastRevLabel";
            this.lastRevLabel.Size = new System.Drawing.Size(69, 13);
            this.lastRevLabel.TabIndex = 5;
            this.lastRevLabel.Text = "Last revision:";
            this.lastRevLabel.Visible = false;
            // 
            // lastRevBox
            // 
            this.lastRevBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lastRevBox.Enabled = false;
            this.lastRevBox.Location = new System.Drawing.Point(312, 3);
            this.lastRevBox.Name = "lastRevBox";
            this.lastRevBox.ReadOnly = true;
            this.lastRevBox.Size = new System.Drawing.Size(74, 13);
            this.lastRevBox.TabIndex = 6;
            this.lastRevBox.Visible = false;
            // 
            // pendingCommits
            // 
            this.pendingCommits.AllowColumnReorder = true;
            this.pendingCommits.CheckBoxes = true;
            this.pendingCommits.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pendingCommits.HideSelection = false;
            this.pendingCommits.Location = new System.Drawing.Point(0, 0);
            this.pendingCommits.Name = "pendingCommits";
            this.pendingCommits.ShowItemToolTips = true;
            this.pendingCommits.Size = new System.Drawing.Size(768, 214);
            this.pendingCommits.TabIndex = 0;
            this.pendingCommits.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pendingCommits_MouseDoubleClick);
            this.pendingCommits.ResolveItem += new System.EventHandler<Ankh.UI.VSSelectionControls.ListViewWithSelection<Ankh.UI.PendingChanges.Commits.PendingCommitItem>.ResolveItemEventArgs>(this.pendingCommits_ResolveItem);
            // 
            // PendingCommitsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "PendingCommitsPage";
            this.Text = "Source Files";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.topLayoutPanel.ResumeLayout(false);
            this.topLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader pathColumn;
        private System.Windows.Forms.ColumnHeader projectColumn;
        private System.Windows.Forms.ColumnHeader changeColumn;
        private System.Windows.Forms.ColumnHeader fullPathColumn;
        private System.Windows.Forms.TableLayoutPanel topLayoutPanel;
        private System.Windows.Forms.TextBox issueNumberBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label issueLabel;
        private Ankh.UI.PendingChanges.LogMessageEditor logMessageEditor;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel panel1;
        private Ankh.UI.PendingChanges.Commits.PendingCommitsView pendingCommits;
        private System.Windows.Forms.Label lastRevLabel;
        private System.Windows.Forms.TextBox lastRevBox;
    }
}
