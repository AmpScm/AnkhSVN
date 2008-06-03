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
            this.busyProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.foldersLabel = new System.Windows.Forms.ToolStripLabel();
            this.fileView = new Ankh.UI.RepositoryExplorer.RepositoryListView();
            this.fileColumn = new System.Windows.Forms.ColumnHeader();
            this.extensionColumn = new System.Windows.Forms.ColumnHeader();
            this.revisionColumn = new System.Windows.Forms.ColumnHeader();
            this.authorColumn = new System.Windows.Forms.ColumnHeader();
            this.sizeColumn = new System.Windows.Forms.ColumnHeader();
            this.dateColumn = new System.Windows.Forms.ColumnHeader();
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
            this.treeView.Context = null;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 25);
            this.treeView.Name = "treeView";
            this.treeView.RetrieveItems = ((SharpSvn.SvnDirEntryItems)(((SharpSvn.SvnDirEntryItems.Kind | SharpSvn.SvnDirEntryItems.Revision)
                        | SharpSvn.SvnDirEntryItems.Time)));
            this.treeView.ShowRootLines = false;
            this.treeView.Size = new System.Drawing.Size(233, 400);
            this.treeView.TabIndex = 1;
            this.treeView.RetrievingChanged += new System.EventHandler(this.treeView_RetrievingChanged);
            this.treeView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TreeViewMouseDown);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // toolFolders
            // 
            this.toolFolders.CanOverflow = false;
            this.toolFolders.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolFolders.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.busyProgress,
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
            // busyProgress
            // 
            this.busyProgress.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.busyProgress.Enabled = false;
            this.busyProgress.Name = "busyProgress";
            this.busyProgress.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.busyProgress.Size = new System.Drawing.Size(160, 22);
            this.busyProgress.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.busyProgress.Visible = false;
            // 
            // foldersLabel
            // 
            this.foldersLabel.Name = "foldersLabel";
            this.foldersLabel.Size = new System.Drawing.Size(45, 22);
            this.foldersLabel.Text = "Folders";
            // 
            // fileView
            // 
            this.fileView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileColumn,
            this.extensionColumn,
            this.revisionColumn,
            this.authorColumn,
            this.sizeColumn,
            this.dateColumn});
            this.fileView.Context = null;
            this.fileView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileView.FullRowSelect = true;
            this.fileView.Location = new System.Drawing.Point(0, 0);
            this.fileView.Name = "fileView";
            this.fileView.ProvideWholeListForSelection = false;
            this.fileView.Size = new System.Drawing.Size(534, 425);
            this.fileView.TabIndex = 1;
            this.fileView.UseCompatibleStateImageBehavior = false;
            this.fileView.View = System.Windows.Forms.View.Details;
            // 
            // fileColumn
            // 
            this.fileColumn.Text = "File";
            this.fileColumn.Width = 100;
            // 
            // extensionColumn
            // 
            this.extensionColumn.Text = "Extension";
            this.extensionColumn.Width = 70;
            // 
            // revisionColumn
            // 
            this.revisionColumn.Text = "Revision";
            this.revisionColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // authorColumn
            // 
            this.authorColumn.Text = "Author";
            // 
            // sizeColumn
            // 
            this.sizeColumn.Text = "Size";
            this.sizeColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // dateColumn
            // 
            this.dateColumn.Text = "Date";
            this.dateColumn.Width = 100;
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
        private ColumnHeader fileColumn;
        private ColumnHeader extensionColumn;
        private ColumnHeader revisionColumn;
        private ColumnHeader authorColumn;
        private ColumnHeader sizeColumn;
        private ColumnHeader dateColumn;
        private ToolStripProgressBar busyProgress;
    }
}
