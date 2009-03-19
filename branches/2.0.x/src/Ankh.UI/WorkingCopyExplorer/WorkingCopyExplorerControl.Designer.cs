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
using System.Windows.Forms;

namespace Ankh.UI.WorkingCopyExplorer
{
    partial class WorkingCopyExplorerControl
    {
        #region InitializeComponent
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.explorerPanel = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.folderTree = new Ankh.UI.WorkingCopyExplorer.FileSystemTreeView();
            this.foldersStrip = new System.Windows.Forms.ToolStrip();
            this.foldersLabel = new System.Windows.Forms.ToolStripLabel();
            this.fileList = new Ankh.UI.WorkingCopyExplorer.FileSystemDetailsView();
            this.explorerPanel.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.foldersStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // explorerPanel
            // 
            this.explorerPanel.Controls.Add(this.splitContainer);
            this.explorerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerPanel.Location = new System.Drawing.Point(0, 0);
            this.explorerPanel.Name = "explorerPanel";
            this.explorerPanel.Size = new System.Drawing.Size(902, 633);
            this.explorerPanel.TabIndex = 1;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.folderTree);
            this.splitContainer.Panel1.Controls.Add(this.foldersStrip);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.fileList);
            this.splitContainer.Size = new System.Drawing.Size(902, 633);
            this.splitContainer.SplitterDistance = 300;
            this.splitContainer.TabIndex = 0;
            // 
            // folderTree
            // 
            this.folderTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.folderTree.HideSelection = false;
            this.folderTree.Location = new System.Drawing.Point(0, 25);
            this.folderTree.Name = "folderTree";
            this.folderTree.Size = new System.Drawing.Size(300, 608);
            this.folderTree.TabIndex = 2;
            // 
            // foldersStrip
            // 
            this.foldersStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.foldersStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.foldersLabel});
            this.foldersStrip.Location = new System.Drawing.Point(0, 0);
            this.foldersStrip.Name = "foldersStrip";
            this.foldersStrip.Size = new System.Drawing.Size(300, 25);
            this.foldersStrip.TabIndex = 1;
            this.foldersStrip.Text = "toolStrip1";
            // 
            // foldersLabel
            // 
            this.foldersLabel.Name = "foldersLabel";
            this.foldersLabel.Size = new System.Drawing.Size(124, 22);
            this.foldersLabel.Text = "Working Copy Folders";
            // 
            // fileList
            // 
            this.fileList.AllowColumnReorder = true;
            this.fileList.Context = null;
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.HideSelection = false;
            this.fileList.Location = new System.Drawing.Point(0, 0);
            this.fileList.Name = "fileList";
            this.fileList.Size = new System.Drawing.Size(598, 633);
            this.fileList.TabIndex = 0;
            // 
            // WorkingCopyExplorerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.explorerPanel);
            this.Name = "WorkingCopyExplorerControl";
            this.Size = new System.Drawing.Size(902, 633);
            this.explorerPanel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.foldersStrip.ResumeLayout(false);
            this.foldersStrip.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private Panel explorerPanel;
        private FileSystemDetailsView fileList;
        private SplitContainer splitContainer;
        private ToolStrip foldersStrip;
        private ToolStripLabel foldersLabel;
        private FileSystemTreeView folderTree;
        private System.ComponentModel.IContainer components;
    }
}
