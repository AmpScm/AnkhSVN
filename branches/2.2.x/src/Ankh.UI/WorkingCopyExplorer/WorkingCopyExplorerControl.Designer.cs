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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WorkingCopyExplorerControl));
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
			resources.ApplyResources(this.explorerPanel, "explorerPanel");
			this.explorerPanel.Name = "explorerPanel";
			// 
			// splitContainer
			// 
			resources.ApplyResources(this.splitContainer, "splitContainer");
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
			// 
			// folderTree
			// 
			resources.ApplyResources(this.folderTree, "folderTree");
			this.folderTree.HideSelection = false;
			this.folderTree.Name = "folderTree";
			this.folderTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.folderTree_AfterSelect);
			// 
			// foldersStrip
			// 
			this.foldersStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.foldersStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.foldersLabel});
			resources.ApplyResources(this.foldersStrip, "foldersStrip");
			this.foldersStrip.Name = "foldersStrip";
			// 
			// foldersLabel
			// 
			this.foldersLabel.Name = "foldersLabel";
			resources.ApplyResources(this.foldersLabel, "foldersLabel");
			// 
			// fileList
			// 
			this.fileList.AllowColumnReorder = true;
			this.fileList.Context = null;
			resources.ApplyResources(this.fileList, "fileList");
			this.fileList.HideSelection = false;
			this.fileList.Name = "fileList";
			// 
			// WorkingCopyExplorerControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.explorerPanel);
			this.Name = "WorkingCopyExplorerControl";
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
