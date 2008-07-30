using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.WorkingCopyExplorer;

namespace Ankh.UI.WorkingCopyExplorer
{
    partial class WorkingCopyExplorerControl
    {
        #region InitializeComponent
        private void InitializeComponent()
        {
            this.explorerPanel = new System.Windows.Forms.Panel();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.folderTree = new Ankh.UI.WorkingCopyExplorer.FileSystemTreeView();
            this.foldersStrip = new System.Windows.Forms.ToolStrip();
            this.foldersLabel = new System.Windows.Forms.ToolStripLabel();
            this.fileList = new Ankh.UI.WorkingCopyExplorer.FileSystemDetailsView();
            this.fileColumn = new System.Windows.Forms.ColumnHeader();
            this.extensionColumn = new System.Windows.Forms.ColumnHeader();
            this.topPanel = new System.Windows.Forms.Panel();
            this.addWorkingCopyLabel = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.newRootTextBox = new System.Windows.Forms.TextBox();
            this.explorerPanel.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.foldersStrip.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // explorerPanel
            // 
            this.explorerPanel.Controls.Add(this.splitContainer);
            this.explorerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.explorerPanel.Location = new System.Drawing.Point(0, 29);
            this.explorerPanel.Name = "explorerPanel";
            this.explorerPanel.Size = new System.Drawing.Size(902, 604);
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
            this.splitContainer.Size = new System.Drawing.Size(902, 604);
            this.splitContainer.SplitterDistance = 300;
            this.splitContainer.TabIndex = 0;
            // 
            // folderTree
            // 
            this.folderTree.Context = null;
            this.folderTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.folderTree.HideSelection = false;
            this.folderTree.Location = new System.Drawing.Point(0, 25);
            this.folderTree.Name = "folderTree";
            this.folderTree.SelectedItem = null;
            this.folderTree.Size = new System.Drawing.Size(300, 579);
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
            this.foldersLabel.Size = new System.Drawing.Size(45, 22);
            this.foldersLabel.Text = "Folders";
            // 
            // fileList
            // 
            this.fileList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileColumn,
            this.extensionColumn});
            this.fileList.Context = null;
            this.fileList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileList.HideSelection = false;
            this.fileList.Location = new System.Drawing.Point(0, 0);
            this.fileList.Name = "fileList";
            this.fileList.ProvideWholeListForSelection = false;
            this.fileList.Size = new System.Drawing.Size(598, 604);
            this.fileList.TabIndex = 0;
            this.fileList.UseCompatibleStateImageBehavior = false;
            this.fileList.View = System.Windows.Forms.View.Details;
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
            // topPanel
            // 
            this.topPanel.Controls.Add(this.addWorkingCopyLabel);
            this.topPanel.Controls.Add(this.addButton);
            this.topPanel.Controls.Add(this.newRootTextBox);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(902, 29);
            this.topPanel.TabIndex = 0;
            // 
            // addWorkingCopyLabel
            // 
            this.addWorkingCopyLabel.AutoSize = true;
            this.addWorkingCopyLabel.Location = new System.Drawing.Point(3, 8);
            this.addWorkingCopyLabel.Name = "addWorkingCopyLabel";
            this.addWorkingCopyLabel.Size = new System.Drawing.Size(95, 13);
            this.addWorkingCopyLabel.TabIndex = 2;
            this.addWorkingCopyLabel.Text = "Add working copy:";
            // 
            // addButton
            // 
            this.addButton.Enabled = false;
            this.addButton.Location = new System.Drawing.Point(350, 3);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(65, 23);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // newRootTextBox
            // 
            this.newRootTextBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.newRootTextBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.FileSystemDirectories;
            this.newRootTextBox.Location = new System.Drawing.Point(104, 5);
            this.newRootTextBox.Name = "newRootTextBox";
            this.newRootTextBox.Size = new System.Drawing.Size(240, 20);
            this.newRootTextBox.TabIndex = 0;
            this.newRootTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newRootTextBox_KeyDown);
            // 
            // WorkingCopyExplorerControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.explorerPanel);
            this.Controls.Add(this.topPanel);
            this.Name = "WorkingCopyExplorerControl";
            this.Size = new System.Drawing.Size(902, 633);
            this.explorerPanel.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.foldersStrip.ResumeLayout(false);
            this.foldersStrip.PerformLayout();
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private Panel explorerPanel;
        private FileSystemDetailsView fileList;

        private IFileSystemItem[] selection = new IFileSystemItem[] { };
        private Panel topPanel;
        private Label addWorkingCopyLabel;
        private Button addButton;
        private TextBox newRootTextBox;
        private SplitContainer splitContainer;
        private ToolStrip foldersStrip;
        private ToolStripLabel foldersLabel;
        private FileSystemTreeView folderTree;
        private ColumnHeader fileColumn;
        private ColumnHeader extensionColumn;
    }
}
