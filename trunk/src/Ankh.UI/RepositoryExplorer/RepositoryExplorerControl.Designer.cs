using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepositoryExplorerControl));
            this.toolbarImageList = new System.Windows.Forms.ImageList(this.components);
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.toolFolders = new System.Windows.Forms.ToolStrip();
            this.foldersLabel = new System.Windows.Forms.ToolStripLabel();
            this.folderCloseButton = new System.Windows.Forms.ToolStripButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.toolStripFiles = new System.Windows.Forms.ToolStrip();
            this.filesLabel = new System.Windows.Forms.ToolStripLabel();
            this.busyProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.treeView = new Ankh.UI.RepositoryExplorer.RepositoryTreeView();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.toolFolders.SuspendLayout();
            this.toolStripFiles.SuspendLayout();
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
            this.splitContainer.Panel2.Controls.Add(this.listView1);
            this.splitContainer.Panel2.Controls.Add(this.toolStripFiles);
            this.splitContainer.Size = new System.Drawing.Size(492, 425);
            this.splitContainer.SplitterDistance = 199;
            this.splitContainer.TabIndex = 0;
            // 
            // toolFolders
            // 
            this.toolFolders.CanOverflow = false;
            this.toolFolders.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolFolders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.foldersLabel,
            this.folderCloseButton});
            this.toolFolders.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolFolders.Location = new System.Drawing.Point(0, 0);
            this.toolFolders.Name = "toolFolders";
            this.toolFolders.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolFolders.Size = new System.Drawing.Size(199, 25);
            this.toolFolders.Stretch = true;
            this.toolFolders.TabIndex = 0;
            this.toolFolders.Text = "toolStrip1";
            // 
            // foldersLabel
            // 
            this.foldersLabel.Name = "foldersLabel";
            this.foldersLabel.Size = new System.Drawing.Size(45, 22);
            this.foldersLabel.Text = "Folders";
            // 
            // folderCloseButton
            // 
            this.folderCloseButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.folderCloseButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.folderCloseButton.Image = ((System.Drawing.Image)(resources.GetObject("folderCloseButton.Image")));
            this.folderCloseButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.folderCloseButton.Name = "folderCloseButton";
            this.folderCloseButton.Size = new System.Drawing.Size(23, 22);
            this.folderCloseButton.Text = "Close";
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Location = new System.Drawing.Point(0, 25);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(289, 400);
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // toolStripFiles
            // 
            this.toolStripFiles.CanOverflow = false;
            this.toolStripFiles.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripFiles.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesLabel,
            this.busyProgress});
            this.toolStripFiles.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripFiles.Location = new System.Drawing.Point(0, 0);
            this.toolStripFiles.Name = "toolStripFiles";
            this.toolStripFiles.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStripFiles.Size = new System.Drawing.Size(289, 25);
            this.toolStripFiles.Stretch = true;
            this.toolStripFiles.TabIndex = 0;
            // 
            // filesLabel
            // 
            this.filesLabel.Name = "filesLabel";
            this.filesLabel.Size = new System.Drawing.Size(30, 22);
            this.filesLabel.Text = "Files";
            // 
            // busyProgress
            // 
            this.busyProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.busyProgress.Enabled = false;
            this.busyProgress.Name = "busyProgress";
            this.busyProgress.Size = new System.Drawing.Size(100, 22);
            this.busyProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.busyProgress.Visible = false;
            // 
            // treeView
            // 
            this.treeView.Context = null;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 25);
            this.treeView.Name = "treeView";
            this.treeView.RetrieveItems = ((SharpSvn.SvnDirEntryItems)(((SharpSvn.SvnDirEntryItems.Kind | SharpSvn.SvnDirEntryItems.Revision)
                        | SharpSvn.SvnDirEntryItems.Time)));
            this.treeView.ShowRootLines = false;
            this.treeView.Size = new System.Drawing.Size(199, 400);
            this.treeView.TabIndex = 1;
            this.treeView.RetrievingChanged += new System.EventHandler(this.treeView_RetrievingChanged);
            this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeViewMouseDown);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // RepositoryExplorerControl
            // 
            this.Controls.Add(this.splitContainer);
            this.Name = "RepositoryExplorerControl";
            this.Size = new System.Drawing.Size(492, 425);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            this.splitContainer.ResumeLayout(false);
            this.toolFolders.ResumeLayout(false);
            this.toolFolders.PerformLayout();
            this.toolStripFiles.ResumeLayout(false);
            this.toolStripFiles.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private Ankh.UI.RepositoryExplorer.RepositoryTreeView treeView;
        private System.ComponentModel.IContainer components;
        private System.Windows.Forms.ImageList toolbarImageList;
        private SplitContainer splitContainer;
        private ToolStrip toolFolders;
        private ToolStripLabel foldersLabel;
        private ToolStripButton folderCloseButton;
        private ToolStrip toolStripFiles;
        private ToolStripLabel filesLabel;
        private ListView listView1;
        private ToolStripProgressBar busyProgress;
    }
}
