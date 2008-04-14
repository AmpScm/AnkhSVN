namespace Ankh.UI.RepositoryOpen
{
    partial class RepositoryOpenDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RepositoryOpenDialog));
            this.openButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.urlBox = new System.Windows.Forms.ToolStripComboBox();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.dirUpButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.versionButton = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dirView = new System.Windows.Forms.ListView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.fileNameBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.fileTypeBox = new System.Windows.Forms.ComboBox();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openButton
            // 
            this.openButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.openButton.Location = new System.Drawing.Point(489, 255);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(75, 23);
            this.openButton.TabIndex = 1;
            this.openButton.Text = "&Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.OnOkClicked);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(489, 284);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.urlBox,
            this.refreshButton,
            this.dirUpButton,
            this.toolStripSeparator,
            this.versionButton});
            this.toolStrip1.Location = new System.Drawing.Point(77, 12);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(484, 25);
            this.toolStrip1.TabIndex = 4;
            this.toolStrip1.Text = "Url:";
            // 
            // urlBox
            // 
            this.urlBox.AutoToolTip = true;
            this.urlBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.urlBox.Name = "urlBox";
            this.urlBox.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.urlBox.Size = new System.Drawing.Size(380, 25);
            this.urlBox.SelectedIndexChanged += new System.EventHandler(this.urlBox_SelectedIndexChanged);
            // 
            // refreshButton
            // 
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.Image = ((System.Drawing.Image)(resources.GetObject("refreshButton.Image")));
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 22);
            this.refreshButton.Text = "&Open";
            this.refreshButton.Click += new System.EventHandler(this.refreshButton_Click);
            // 
            // dirUpButton
            // 
            this.dirUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.dirUpButton.Image = ((System.Drawing.Image)(resources.GetObject("dirUpButton.Image")));
            this.dirUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.dirUpButton.Name = "dirUpButton";
            this.dirUpButton.Size = new System.Drawing.Size(23, 22);
            this.dirUpButton.Text = "&Save";
            this.dirUpButton.Click += new System.EventHandler(this.dirUpButton_Click);
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // versionButton
            // 
            this.versionButton.Image = ((System.Drawing.Image)(resources.GetObject("versionButton.Image")));
            this.versionButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.versionButton.Name = "versionButton";
            this.versionButton.Size = new System.Drawing.Size(55, 20);
            this.versionButton.Text = "Head";
            this.versionButton.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.panel1.Location = new System.Drawing.Point(13, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(57, 266);
            this.panel1.TabIndex = 5;
            // 
            // dirView
            // 
            this.dirView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dirView.Location = new System.Drawing.Point(77, 41);
            this.dirView.MultiSelect = false;
            this.dirView.Name = "dirView";
            this.dirView.Size = new System.Drawing.Size(484, 208);
            this.dirView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.dirView.TabIndex = 6;
            this.dirView.UseCompatibleStateImageBehavior = false;
            this.dirView.View = System.Windows.Forms.View.List;
            this.dirView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dirView_MouseDoubleClick);
            this.dirView.SelectedIndexChanged += new System.EventHandler(this.dirView_SelectedIndexChanged);
            this.dirView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dirView_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(38, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(23, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Url:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(76, 260);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "File &name:";
            // 
            // fileNameBox
            // 
            this.fileNameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileNameBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.fileNameBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.fileNameBox.Location = new System.Drawing.Point(151, 257);
            this.fileNameBox.Name = "fileNameBox";
            this.fileNameBox.Size = new System.Drawing.Size(332, 20);
            this.fileNameBox.TabIndex = 9;
            this.fileNameBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fileNameBox_KeyDown);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(76, 284);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Files of &type:";
            // 
            // fileTypeBox
            // 
            this.fileTypeBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fileTypeBox.FormattingEnabled = true;
            this.fileTypeBox.Items.AddRange(new object[] {
            "All Files (*.*)"});
            this.fileTypeBox.Location = new System.Drawing.Point(151, 284);
            this.fileTypeBox.Name = "fileTypeBox";
            this.fileTypeBox.Size = new System.Drawing.Size(332, 21);
            this.fileTypeBox.TabIndex = 11;
            this.fileTypeBox.SelectedIndexChanged += new System.EventHandler(this.fileTypeBox_SelectedIndexChanged);
            // 
            // RepositoryOpenDialog
            // 
            this.AcceptButton = this.openButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(573, 319);
            this.Controls.Add(this.fileTypeBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fileNameBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dirView);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.openButton);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RepositoryOpenDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open from Subversion";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.ToolStripButton dirUpButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripComboBox urlBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListView dirView;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox fileNameBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox fileTypeBox;
        private System.Windows.Forms.ToolStripButton versionButton;
    }
}