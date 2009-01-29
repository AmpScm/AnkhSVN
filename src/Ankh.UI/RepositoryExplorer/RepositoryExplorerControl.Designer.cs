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

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.VSSelectionControls;

namespace Ankh.UI.RepositoryExplorer
{
    public partial class RepositoryExplorerControl
    {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.toolbarImageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.treeView = new Ankh.UI.RepositoryExplorer.RepositoryTreeView();
            this.toolFolders = new System.Windows.Forms.ToolStrip();
            this.foldersLabel = new System.Windows.Forms.ToolStripLabel();
            this.fileView = new Ankh.UI.RepositoryExplorer.RepositoryListView();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolFolders.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolbarImageList
            // 
            this.toolbarImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.toolbarImageList.ImageSize = new System.Drawing.Size(16, 16);
            this.toolbarImageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.treeView);
            this.splitContainer.Panel1.Controls.Add(this.toolFolders);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.fileView);
            this.splitContainer.Size = new System.Drawing.Size(771, 425);
            this.splitContainer.SplitterDistance = 233;
            this.splitContainer.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.AllowRenames = true;
            this.treeView.Context = null;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.LabelEdit = true;
            this.treeView.Location = new System.Drawing.Point(0, 25);
            this.treeView.Name = "treeView";
            this.treeView.RetrieveLocks = true;
            this.treeView.Size = new System.Drawing.Size(233, 400);
            this.treeView.TabIndex = 1;
            this.treeView.ShowContextMenu += new System.Windows.Forms.MouseEventHandler(this.OnTreeViewShowContextMenu);
            this.treeView.SelectedNodeRefresh += new System.EventHandler(this.treeView_SelectedNodeRefresh);
            this.treeView.RetrievingChanged += new System.EventHandler(this.treeView_RetrievingChanged);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // toolFolders
            // 
            this.toolFolders.CanOverflow = false;
            this.toolFolders.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolFolders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.foldersLabel});
            this.toolFolders.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolFolders.Location = new System.Drawing.Point(0, 0);
            this.toolFolders.Name = "toolFolders";
            this.toolFolders.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolFolders.Size = new System.Drawing.Size(233, 25);
            this.toolFolders.Stretch = true;
            this.toolFolders.TabIndex = 0;
            this.toolFolders.Text = "toolStrip1";
            // 
            // foldersLabel
            // 
            this.foldersLabel.Name = "foldersLabel";
            this.foldersLabel.Size = new System.Drawing.Size(104, 22);
            this.foldersLabel.Text = "Repository Folders";
            // 
            // fileView
            // 
            this.fileView.AllowColumnReorder = true;
            this.fileView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileView.HideSelection = false;
            this.fileView.LabelEdit = true;
            this.fileView.Location = new System.Drawing.Point(0, 0);
            this.fileView.Name = "fileView";
            this.fileView.RepositoryTreeView = this.treeView;
            this.fileView.Size = new System.Drawing.Size(534, 425);
            this.fileView.TabIndex = 1;
            this.fileView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.fileView_MouseDoubleClick);
            // 
            // RepositoryExplorerControl
            // 
            this.Controls.Add(this.splitContainer);
            this.Name = "RepositoryExplorerControl";
            this.Size = new System.Drawing.Size(771, 425);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.toolFolders.ResumeLayout(false);
            this.toolFolders.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }


        private Ankh.UI.RepositoryExplorer.RepositoryTreeView treeView;
        private Ankh.UI.RepositoryExplorer.RepositoryListView fileView;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ImageList toolbarImageList;
        private SplitContainer splitContainer;
        private ToolStrip toolFolders;
        private ToolStripLabel foldersLabel;
    }
}
