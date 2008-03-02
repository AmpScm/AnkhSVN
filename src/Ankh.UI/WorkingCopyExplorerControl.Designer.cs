using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
{
    partial class WorkingCopyExplorerControl
    {
        #region InitializeComponent
        private void InitializeComponent()
        {
            this.explorerPanel = new System.Windows.Forms.Panel();
            this.listView = new Ankh.UI.FileSystemDetailsView();
            this.treeView = new Ankh.UI.FileSystemTreeView();
            this.topPanel = new System.Windows.Forms.Panel();
            this.addWorkingCopyLabel = new System.Windows.Forms.Label();
            this.addButton = new System.Windows.Forms.Button();
            this.newRootTextBox = new System.Windows.Forms.TextBox();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.explorerPanel.SuspendLayout();
            this.topPanel.SuspendLayout();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
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
            // listView
            // 
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 0);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(598, 604);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.HideSelection = false;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.SelectedItem = null;
            this.treeView.Size = new System.Drawing.Size(300, 604);
            this.treeView.TabIndex = 0;
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
            this.addButton.Location = new System.Drawing.Point(351, 2);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(65, 23);
            this.addButton.TabIndex = 1;
            this.addButton.Text = "Add";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // newRootTextBox
            // 
            this.newRootTextBox.Location = new System.Drawing.Point(105, 5);
            this.newRootTextBox.Name = "newRootTextBox";
            this.newRootTextBox.Size = new System.Drawing.Size(240, 20);
            this.newRootTextBox.TabIndex = 0;
            this.newRootTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.newRootTextBox_KeyDown);
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
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.listView);
            this.splitContainer.Size = new System.Drawing.Size(902, 604);
            this.splitContainer.SplitterDistance = 300;
            this.splitContainer.TabIndex = 0;
            // 
            // WorkingCopyExplorerControl
            // 
            this.Controls.Add(this.explorerPanel);
            this.Controls.Add(this.topPanel);
            this.Name = "WorkingCopyExplorerControl";
            this.Size = new System.Drawing.Size(902, 633);
            this.explorerPanel.ResumeLayout(false);
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private Panel explorerPanel;
        private FileSystemTreeView treeView;
        private FileSystemDetailsView listView;

        private IFileSystemItem[] selection = new IFileSystemItem[] { };
        private Panel topPanel;
        private Label addWorkingCopyLabel;
        private Button addButton;
        private TextBox newRootTextBox;
        private SplitContainer splitContainer;
    }
}
