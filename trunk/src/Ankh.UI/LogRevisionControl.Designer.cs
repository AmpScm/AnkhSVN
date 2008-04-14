namespace Ankh.UI
{
    partial class LogRevisionControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogRevisionControl));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Revision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Author = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LogMessage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.singleItemContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createBranchTagToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.revertSingleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.switchToRevisionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.miIssueContainer = new System.Windows.Forms.ToolStripMenuItem();
            this.miNoIssues = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.singleRefreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.multipleItemContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.revertChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.compareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.singleItemContextMenu.SuspendLayout();
            this.multipleItemContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Revision,
            this.Author,
            this.Date,
            this.LogMessage});
            this.dataGridView1.ContextMenuStrip = this.singleItemContextMenu;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 16;
            this.dataGridView1.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.VirtualMode = true;
            this.dataGridView1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.dataGridView1_Scroll);
            this.dataGridView1.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dataGridView1_CellValueNeeded);
            this.dataGridView1.CurrentCellChanged += new System.EventHandler(this.dataGridView1_CurrentCellChanged);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged);
            // 
            // Revision
            // 
            this.Revision.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.Revision, "Revision");
            this.Revision.Name = "Revision";
            this.Revision.ReadOnly = true;
            // 
            // Author
            // 
            this.Author.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.Author, "Author");
            this.Author.Name = "Author";
            this.Author.ReadOnly = true;
            // 
            // Date
            // 
            this.Date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            resources.ApplyResources(this.Date, "Date");
            this.Date.Name = "Date";
            this.Date.ReadOnly = true;
            // 
            // LogMessage
            // 
            this.LogMessage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(this.LogMessage, "LogMessage");
            this.LogMessage.Name = "LogMessage";
            this.LogMessage.ReadOnly = true;
            // 
            // singleItemContextMenu
            // 
            this.singleItemContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createBranchTagToolStripMenuItem,
            this.toolStripMenuItem4,
            this.revertSingleToolStripMenuItem,
            this.switchToRevisionToolStripMenuItem,
            this.toolStripSeparator3,
            this.miIssueContainer,
            this.toolStripSeparator1,
            this.singleRefreshToolStripMenuItem});
            this.singleItemContextMenu.Name = "singleItemContextMenu";
            resources.ApplyResources(this.singleItemContextMenu, "singleItemContextMenu");
            // 
            // createBranchTagToolStripMenuItem
            // 
            this.createBranchTagToolStripMenuItem.Name = "createBranchTagToolStripMenuItem";
            resources.ApplyResources(this.createBranchTagToolStripMenuItem, "createBranchTagToolStripMenuItem");
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            // 
            // revertSingleToolStripMenuItem
            // 
            this.revertSingleToolStripMenuItem.Name = "revertSingleToolStripMenuItem";
            resources.ApplyResources(this.revertSingleToolStripMenuItem, "revertSingleToolStripMenuItem");
            // 
            // switchToRevisionToolStripMenuItem
            // 
            this.switchToRevisionToolStripMenuItem.Name = "switchToRevisionToolStripMenuItem";
            resources.ApplyResources(this.switchToRevisionToolStripMenuItem, "switchToRevisionToolStripMenuItem");
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // miIssueContainer
            // 
            this.miIssueContainer.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miNoIssues});
            this.miIssueContainer.Name = "miIssueContainer";
            resources.ApplyResources(this.miIssueContainer, "miIssueContainer");
            // 
            // miNoIssues
            // 
            resources.ApplyResources(this.miNoIssues, "miNoIssues");
            this.miNoIssues.Name = "miNoIssues";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // singleRefreshToolStripMenuItem
            // 
            this.singleRefreshToolStripMenuItem.Name = "singleRefreshToolStripMenuItem";
            resources.ApplyResources(this.singleRefreshToolStripMenuItem, "singleRefreshToolStripMenuItem");
            this.singleRefreshToolStripMenuItem.Click += new System.EventHandler(this.singleRefreshToolStripMenuItem_Click);
            // 
            // multipleItemContextMenu
            // 
            this.multipleItemContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.revertChangesToolStripMenuItem,
            this.toolStripSeparator2,
            this.compareToolStripMenuItem,
            this.toolStripSeparator4,
            this.refreshToolStripMenuItem});
            this.multipleItemContextMenu.Name = "multipleItemContextMenu";
            resources.ApplyResources(this.multipleItemContextMenu, "multipleItemContextMenu");
            // 
            // revertChangesToolStripMenuItem
            // 
            this.revertChangesToolStripMenuItem.Name = "revertChangesToolStripMenuItem";
            resources.ApplyResources(this.revertChangesToolStripMenuItem, "revertChangesToolStripMenuItem");
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // compareToolStripMenuItem
            // 
            this.compareToolStripMenuItem.Name = "compareToolStripMenuItem";
            resources.ApplyResources(this.compareToolStripMenuItem, "compareToolStripMenuItem");
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            resources.ApplyResources(this.refreshToolStripMenuItem, "refreshToolStripMenuItem");
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // LogRevisionControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.DoubleBuffered = true;
            this.Name = "LogRevisionControl";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.singleItemContextMenu.ResumeLayout(false);
            this.multipleItemContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Revision;
        private System.Windows.Forms.DataGridViewTextBoxColumn Author;
        private System.Windows.Forms.DataGridViewTextBoxColumn Date;
        private System.Windows.Forms.DataGridViewTextBoxColumn LogMessage;
        private System.Windows.Forms.ContextMenuStrip singleItemContextMenu;
        private System.Windows.Forms.ToolStripMenuItem switchToRevisionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createBranchTagToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem revertSingleToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem singleRefreshToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip multipleItemContextMenu;
        private System.Windows.Forms.ToolStripMenuItem revertChangesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem compareToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem miIssueContainer;
        private System.Windows.Forms.ToolStripMenuItem miNoIssues;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
    }
}
