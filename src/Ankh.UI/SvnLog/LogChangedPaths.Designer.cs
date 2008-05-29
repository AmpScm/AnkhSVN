namespace Ankh.UI
{
    partial class LogChangedPaths
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogChangedPaths));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Action = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Path = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CopyFromPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CopyFromRevision = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.blameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.compareToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            resources.ApplyResources(this.dataGridView1, "dataGridView1");
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Action,
            this.Path,
            this.CopyFromPath,
            this.CopyFromRevision});
            this.dataGridView1.ContextMenuStrip = this.contextMenuStrip1;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // Action
            // 
            this.Action.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Action.DataPropertyName = "Action";
            resources.ApplyResources(this.Action, "Action");
            this.Action.Name = "Action";
            this.Action.ReadOnly = true;
            // 
            // Path
            // 
            this.Path.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Path.DataPropertyName = "Path";
            resources.ApplyResources(this.Path, "Path");
            this.Path.Name = "Path";
            this.Path.ReadOnly = true;
            // 
            // CopyFromPath
            // 
            this.CopyFromPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CopyFromPath.DataPropertyName = "CopyFromPath";
            resources.ApplyResources(this.CopyFromPath, "CopyFromPath");
            this.CopyFromPath.Name = "CopyFromPath";
            this.CopyFromPath.ReadOnly = true;
            // 
            // CopyFromRevision
            // 
            this.CopyFromRevision.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.CopyFromRevision.DataPropertyName = "CopyFromRevision";
            resources.ApplyResources(this.CopyFromRevision, "CopyFromRevision");
            this.CopyFromRevision.Name = "CopyFromRevision";
            this.CopyFromRevision.ReadOnly = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.blameToolStripMenuItem,
            this.compareToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // blameToolStripMenuItem
            // 
            this.blameToolStripMenuItem.Name = "blameToolStripMenuItem";
            resources.ApplyResources(this.blameToolStripMenuItem, "blameToolStripMenuItem");
            // 
            // compareToolStripMenuItem
            // 
            resources.ApplyResources(this.compareToolStripMenuItem, "compareToolStripMenuItem");
            this.compareToolStripMenuItem.Name = "compareToolStripMenuItem";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // LogChangedPaths
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Name = "LogChangedPaths";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem blameToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem compareToolStripMenuItem;
		private System.Windows.Forms.DataGridViewTextBoxColumn Action;
		private System.Windows.Forms.DataGridViewTextBoxColumn Path;
		private System.Windows.Forms.DataGridViewTextBoxColumn CopyFromPath;
		private System.Windows.Forms.DataGridViewTextBoxColumn CopyFromRevision;
    }
}
