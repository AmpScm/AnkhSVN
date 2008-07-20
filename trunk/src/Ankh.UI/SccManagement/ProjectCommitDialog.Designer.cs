namespace Ankh.UI.SccManagement
{
    partial class ProjectCommitDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.pendingList = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.logMessage = new Ankh.UI.PendingChanges.LogMessageEditor();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.pathColumn = new System.Windows.Forms.ColumnHeader();
            this.changeColumn = new System.Windows.Forms.ColumnHeader();
            this.projectColumn = new System.Windows.Forms.ColumnHeader();
            this.fullPathColumn = new System.Windows.Forms.ColumnHeader();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.pendingList);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.textBox1);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.logMessage);
            this.splitContainer1.Panel2.Controls.Add(this.label1);
            this.splitContainer1.Size = new System.Drawing.Size(504, 326);
            this.splitContainer1.SplitterDistance = 168;
            this.splitContainer1.TabIndex = 0;
            // 
            // pendingList
            // 
            this.pendingList.CheckBoxes = true;
            this.pendingList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.pathColumn,
            this.projectColumn,
            this.changeColumn,
            this.fullPathColumn});
            this.pendingList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pendingList.FullRowSelect = true;
            this.pendingList.HideSelection = false;
            this.pendingList.Location = new System.Drawing.Point(0, 0);
            this.pendingList.Name = "pendingList";
            this.pendingList.Size = new System.Drawing.Size(504, 168);
            this.pendingList.TabIndex = 0;
            this.pendingList.UseCompatibleStateImageBehavior = false;
            this.pendingList.View = System.Windows.Forms.View.Details;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Log Message:";
            // 
            // logMessage
            // 
            this.logMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.logMessage.Location = new System.Drawing.Point(0, 23);
            this.logMessage.Name = "logMessage";
            this.logMessage.Size = new System.Drawing.Size(504, 131);
            this.logMessage.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Enabled = false;
            this.label2.Location = new System.Drawing.Point(292, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "&Bug/Issue:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label2.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(404, -3);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.Visible = false;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 344);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(83, 17);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Keep &Locks";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(12, 361);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(108, 17);
            this.checkBox2.TabIndex = 2;
            this.checkBox2.Text = "Keep &Changelists";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(441, 355);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(360, 355);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // pathColumn
            // 
            this.pathColumn.Text = "Path";
            this.pathColumn.Width = 200;
            // 
            // changeColumn
            // 
            this.changeColumn.DisplayIndex = 1;
            this.changeColumn.Text = "Change";
            this.changeColumn.Width = 70;
            // 
            // projectColumn
            // 
            this.projectColumn.DisplayIndex = 2;
            this.projectColumn.Text = "Project";
            this.projectColumn.Width = 70;
            // 
            // fullPathColumn
            // 
            this.fullPathColumn.Text = "Full Path";
            // 
            // ProjectCommitDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(528, 390);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.splitContainer1);
            this.Name = "ProjectCommitDialog";
            this.Text = "Commit to Subversion";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView pendingList;
        private Ankh.UI.PendingChanges.LogMessageEditor logMessage;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ColumnHeader pathColumn;
        private System.Windows.Forms.ColumnHeader projectColumn;
        private System.Windows.Forms.ColumnHeader changeColumn;
        private System.Windows.Forms.ColumnHeader fullPathColumn;
    }
}