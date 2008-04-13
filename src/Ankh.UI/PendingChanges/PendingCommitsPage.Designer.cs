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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.topLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.logMessageEditor = new Ankh.UI.PendingChanges.LogMessageEditor();
            this.pendingCommits = new Ankh.UI.PendingChanges.PendingCommitsView(this.components);
            this.pathColumn = new System.Windows.Forms.ColumnHeader();
            this.projectColumn = new System.Windows.Forms.ColumnHeader();
            this.changeColumn = new System.Windows.Forms.ColumnHeader();
            this.fullPathColumn = new System.Windows.Forms.ColumnHeader();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.panel1.SuspendLayout();
            this.topLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.panel1);
            this.splitContainer.Panel1MinSize = 10;
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.pendingCommits);
            this.splitContainer.Panel2MinSize = 10;
            this.splitContainer.Size = new System.Drawing.Size(768, 300);
            this.splitContainer.SplitterDistance = 83;
            this.splitContainer.SplitterWidth = 3;
            this.splitContainer.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.topLayoutPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(768, 83);
            this.panel1.TabIndex = 1;
            // 
            // topLayoutPanel
            // 
            this.topLayoutPanel.ColumnCount = 3;
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 77F));
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 144F));
            this.topLayoutPanel.Controls.Add(this.textBox1, 2, 0);
            this.topLayoutPanel.Controls.Add(this.label2, 1, 0);
            this.topLayoutPanel.Controls.Add(this.label1, 0, 0);
            this.topLayoutPanel.Controls.Add(this.logMessageEditor, 0, 1);
            this.topLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.topLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.topLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            this.topLayoutPanel.Name = "topLayoutPanel";
            this.topLayoutPanel.RowCount = 2;
            this.topLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.topLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.topLayoutPanel.Size = new System.Drawing.Size(766, 81);
            this.topLayoutPanel.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(622, 0);
            this.textBox1.Margin = new System.Windows.Forms.Padding(0);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(144, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.Visible = false;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(584, 3);
            this.label2.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "&Issue:";
            this.label2.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Message:";
            // 
            // logMessageEditor
            // 
            this.topLayoutPanel.SetColumnSpan(this.logMessageEditor, 3);
            this.logMessageEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logMessageEditor.Location = new System.Drawing.Point(0, 20);
            this.logMessageEditor.Margin = new System.Windows.Forms.Padding(0);
            this.logMessageEditor.Name = "logMessageEditor";
            this.logMessageEditor.Size = new System.Drawing.Size(766, 61);
            this.logMessageEditor.TabIndex = 4;
            // 
            // pendingCommits
            // 
            this.pendingCommits.AllowColumnReorder = true;
            this.pendingCommits.CheckBoxes = true;
            this.pendingCommits.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pathColumn,
            this.projectColumn,
            this.changeColumn,
            this.fullPathColumn});
            this.pendingCommits.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pendingCommits.FullRowSelect = true;
            this.pendingCommits.HideSelection = false;
            this.pendingCommits.Location = new System.Drawing.Point(0, 0);
            this.pendingCommits.Name = "pendingCommits";
            this.pendingCommits.ProvideWholeListForSelection = false;
            this.pendingCommits.ServiceProvider = null;
            this.pendingCommits.ShowItemToolTips = true;
            this.pendingCommits.Size = new System.Drawing.Size(768, 214);
            this.pendingCommits.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.pendingCommits.StrictCheckboxesClick = true;
            this.pendingCommits.TabIndex = 0;
            this.pendingCommits.UseCompatibleStateImageBehavior = false;
            this.pendingCommits.View = System.Windows.Forms.View.Details;
            this.pendingCommits.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pendingCommits_MouseDoubleClick);
            this.pendingCommits.ShowContextMenu += new System.EventHandler(this.pendingCommits_ShowContextMenu);
            this.pendingCommits.RetrieveSelection += new System.EventHandler<Ankh.UI.PendingChanges.ListViewWithSelection<Ankh.UI.PendingChanges.PendingCommitItem>.RetrieveSelectionEventArgs>(this.pendingCommits_RetrieveSelection);
            this.pendingCommits.ResolveItem += new System.EventHandler<Ankh.UI.PendingChanges.ListViewWithSelection<Ankh.UI.PendingChanges.PendingCommitItem>.ResolveItemEventArgs>(this.pendingCommits_ResolveItem);
            this.pendingCommits.ColumnReordered += new System.Windows.Forms.ColumnReorderedEventHandler(this.pendingCommits_ColumnReordered);
            // 
            // pathColumn
            // 
            this.pathColumn.Text = "Path";
            this.pathColumn.Width = 290;
            // 
            // projectColumn
            // 
            this.projectColumn.Text = "Project";
            this.projectColumn.Width = 75;
            // 
            // changeColumn
            // 
            this.changeColumn.Text = "Change";
            this.changeColumn.Width = 73;
            // 
            // fullPathColumn
            // 
            this.fullPathColumn.Text = "Full Path";
            this.fullPathColumn.Width = 327;
            // 
            // PendingCommitsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "PendingCommitsPage";
            this.Text = "Source Code";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
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
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Ankh.UI.PendingChanges.LogMessageEditor logMessageEditor;
        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel panel1;
        private PendingCommitsView pendingCommits;
    }
}
