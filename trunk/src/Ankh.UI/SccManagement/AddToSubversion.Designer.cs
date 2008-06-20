namespace Ankh.UI.SccManagement
{
    partial class AddToSubversion
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
            this.locationBox = new System.Windows.Forms.GroupBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.createFolderButton = new System.Windows.Forms.Button();
            this.treeView1 = new Ankh.UI.RepositoryExplorer.RepositoryTreeView();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.projectBox = new System.Windows.Forms.GroupBox();
            this.projectNameBox = new System.Windows.Forms.TextBox();
            this.locationBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.projectBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // locationBox
            // 
            this.locationBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.locationBox.Controls.Add(this.comboBox3);
            this.locationBox.Controls.Add(this.createFolderButton);
            this.locationBox.Controls.Add(this.treeView1);
            this.locationBox.Controls.Add(this.comboBox1);
            this.locationBox.Location = new System.Drawing.Point(12, 89);
            this.locationBox.Name = "locationBox";
            this.locationBox.Size = new System.Drawing.Size(436, 259);
            this.locationBox.TabIndex = 2;
            this.locationBox.TabStop = false;
            this.locationBox.Text = "&Repository Url:";
            // 
            // comboBox3
            // 
            this.comboBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Location = new System.Drawing.Point(6, 19);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(424, 21);
            this.comboBox3.TabIndex = 1;
            // 
            // createFolderButton
            // 
            this.createFolderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.createFolderButton.Location = new System.Drawing.Point(337, 46);
            this.createFolderButton.Name = "createFolderButton";
            this.createFolderButton.Size = new System.Drawing.Size(93, 23);
            this.createFolderButton.TabIndex = 3;
            this.createFolderButton.Text = "&Create Folder...";
            this.createFolderButton.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Context = null;
            this.treeView1.Location = new System.Drawing.Point(6, 46);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(325, 207);
            this.treeView1.TabIndex = 2;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(62, -22);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.comboBox2);
            this.groupBox1.Location = new System.Drawing.Point(12, 354);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(436, 43);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Local &Folder:";
            // 
            // comboBox2
            // 
            this.comboBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(6, 16);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(424, 21);
            this.comboBox2.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(18, 403);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(155, 17);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "A&dd trunk Folder for Project";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(373, 423);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(292, 423);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(427, 28);
            this.label1.TabIndex = 0;
            this.label1.Text = "This will create a folder in the repository to hold your project and initialze yo" +
                "ur working copy.";
            // 
            // projectBox
            // 
            this.projectBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.projectBox.Controls.Add(this.projectNameBox);
            this.projectBox.Location = new System.Drawing.Point(12, 40);
            this.projectBox.Name = "projectBox";
            this.projectBox.Size = new System.Drawing.Size(436, 43);
            this.projectBox.TabIndex = 1;
            this.projectBox.TabStop = false;
            this.projectBox.Text = "Project &Name:";
            // 
            // projectNameBox
            // 
            this.projectNameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.projectNameBox.Location = new System.Drawing.Point(6, 17);
            this.projectNameBox.Name = "projectNameBox";
            this.projectNameBox.Size = new System.Drawing.Size(424, 20);
            this.projectNameBox.TabIndex = 0;
            // 
            // AddToSubversion
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(460, 458);
            this.Controls.Add(this.projectBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.locationBox);
            this.Name = "AddToSubversion";
            this.Text = "Add to Subversion";
            this.locationBox.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.projectBox.ResumeLayout(false);
            this.projectBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox locationBox;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Button createFolderButton;
        private Ankh.UI.RepositoryExplorer.RepositoryTreeView treeView1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox projectBox;
        private System.Windows.Forms.TextBox projectNameBox;
        private System.Windows.Forms.ComboBox comboBox3;
    }
}