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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PendingCommitsPage));
            this.splitContainer = new Ankh.UI.VSSelectionControls.SmartSplitContainer();
            this.borderPanel = new System.Windows.Forms.Panel();
            this.topLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.issueLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.logMessageEditor = new Ankh.UI.PendingChanges.LogMessageEditor(this.components);
            this.pendingCommits = new Ankh.UI.PendingChanges.Commits.PendingCommitsView(this.components);
            this.issueNumberBox = new System.Windows.Forms.TextBox();
            this.lastRevLabel = new System.Windows.Forms.Label();
            this.lastRevBox = new System.Windows.Forms.TextBox();
            this.pathColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.projectColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.changeColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fullPathColumn = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.borderPanel.SuspendLayout();
            this.topLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            resources.ApplyResources(this.splitContainer, "splitContainer");
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.borderPanel);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pendingCommits);
            // 
            // borderPanel
            // 
            this.borderPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.borderPanel.Controls.Add(this.topLayoutPanel);
            resources.ApplyResources(this.borderPanel, "borderPanel");
            this.borderPanel.Name = "borderPanel";
            // 
            // topLayoutPanel
            // 
            resources.ApplyResources(this.topLayoutPanel, "topLayoutPanel");
            this.topLayoutPanel.Controls.Add(this.issueLabel, 3, 0);
            this.topLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.topLayoutPanel.Controls.Add(this.logMessageEditor, 0, 1);
            this.topLayoutPanel.Controls.Add(this.issueNumberBox, 4, 0);
            this.topLayoutPanel.Controls.Add(this.lastRevLabel, 1, 0);
            this.topLayoutPanel.Controls.Add(this.lastRevBox, 2, 0);
            this.topLayoutPanel.Name = "topLayoutPanel";
            // 
            // issueLabel
            // 
            resources.ApplyResources(this.issueLabel, "issueLabel");
            this.issueLabel.Name = "issueLabel";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // logMessageEditor
            // 
            this.topLayoutPanel.SetColumnSpan(this.logMessageEditor, 5);
            resources.ApplyResources(this.logMessageEditor, "logMessageEditor");
            this.logMessageEditor.HideHorizontalScrollBar = true;
            this.logMessageEditor.Name = "logMessageEditor";
            this.logMessageEditor.PasteSource = this.pendingCommits;
            // 
            // pendingCommits
            // 
            this.pendingCommits.AllowColumnReorder = true;
            this.pendingCommits.CheckBoxes = true;
            resources.ApplyResources(this.pendingCommits, "pendingCommits");
            this.pendingCommits.HideSelection = false;
            this.pendingCommits.Name = "pendingCommits";
            this.pendingCommits.ShowItemToolTips = true;
            this.pendingCommits.ShowSelectAllCheckBox = true;
            this.pendingCommits.ResolveItem += new System.EventHandler<Ankh.UI.VSSelectionControls.ListViewWithSelection<Ankh.UI.PendingChanges.Commits.PendingCommitItem>.ResolveItemEventArgs>(this.pendingCommits_ResolveItem);
            this.pendingCommits.KeyUp += new System.Windows.Forms.KeyEventHandler(this.pendingCommits_KeyUp);
            // 
            // issueNumberBox
            // 
            this.issueNumberBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.issueNumberBox, "issueNumberBox");
            this.issueNumberBox.Name = "issueNumberBox";
            this.issueNumberBox.TextChanged += new System.EventHandler(this.issueNumberBox_TextChanged);
            this.issueNumberBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.issueNumberBox_KeyPress);
            // 
            // lastRevLabel
            // 
            resources.ApplyResources(this.lastRevLabel, "lastRevLabel");
            this.lastRevLabel.Name = "lastRevLabel";
            // 
            // lastRevBox
            // 
            this.lastRevBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.lastRevBox, "lastRevBox");
            this.lastRevBox.Name = "lastRevBox";
            this.lastRevBox.ReadOnly = true;
            // 
            // PendingCommitsPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "PendingCommitsPage";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.borderPanel.ResumeLayout(false);
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
        private Ankh.UI.VSSelectionControls.SmartSplitContainer splitContainer;
        private System.Windows.Forms.Panel borderPanel;
        private Ankh.UI.PendingChanges.Commits.PendingCommitsView pendingCommits;
        private System.Windows.Forms.Label lastRevLabel;
        private System.Windows.Forms.TextBox lastRevBox;
    }
}
