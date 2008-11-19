namespace Ankh.UI.RepositoryOpen
{
    partial class CheckoutProject
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkOutFrom = new System.Windows.Forms.ComboBox();
            this.projectRootLabel = new System.Windows.Forms.Label();
            this.projectLabel = new System.Windows.Forms.Label();
            this.projectUrl = new System.Windows.Forms.TextBox();
            this.projectIcon = new System.Windows.Forms.PictureBox();
            this.locationIcon = new System.Windows.Forms.GroupBox();
            this.directory = new System.Windows.Forms.TextBox();
            this.browseDirectoryButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.dirIco = new System.Windows.Forms.PictureBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.version = new Ankh.UI.PathSelector.VersionSelector();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.projectIcon)).BeginInit();
            this.locationIcon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dirIco)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.checkOutFrom);
            this.groupBox1.Controls.Add(this.projectRootLabel);
            this.groupBox1.Controls.Add(this.projectLabel);
            this.groupBox1.Controls.Add(this.projectUrl);
            this.groupBox1.Controls.Add(this.projectIcon);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(525, 69);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Project:";
            // 
            // checkOutFrom
            // 
            this.checkOutFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.checkOutFrom.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.checkOutFrom.FormattingEnabled = true;
            this.checkOutFrom.Location = new System.Drawing.Point(93, 36);
            this.checkOutFrom.Name = "checkOutFrom";
            this.checkOutFrom.Size = new System.Drawing.Size(426, 21);
            this.checkOutFrom.TabIndex = 2;
            // 
            // projectRootLabel
            // 
            this.projectRootLabel.AutoSize = true;
            this.projectRootLabel.Location = new System.Drawing.Point(6, 39);
            this.projectRootLabel.Name = "projectRootLabel";
            this.projectRootLabel.Size = new System.Drawing.Size(84, 13);
            this.projectRootLabel.TabIndex = 1;
            this.projectRootLabel.Text = "Check Out from:";
            // 
            // projectLabel
            // 
            this.projectLabel.AutoSize = true;
            this.projectLabel.Location = new System.Drawing.Point(6, 18);
            this.projectLabel.Name = "projectLabel";
            this.projectLabel.Size = new System.Drawing.Size(43, 13);
            this.projectLabel.TabIndex = 0;
            this.projectLabel.Text = "Project:";
            // 
            // projectUrl
            // 
            this.projectUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.projectUrl.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.projectUrl.Location = new System.Drawing.Point(93, 16);
            this.projectUrl.Name = "projectUrl";
            this.projectUrl.ReadOnly = true;
            this.projectUrl.Size = new System.Drawing.Size(426, 13);
            this.projectUrl.TabIndex = 0;
            this.projectUrl.Text = "http://ankhsvn.open.collab.net/svn/ankhsvn/trunk/AnkhSvn.2008.sln";
            // 
            // projectIcon
            // 
            this.projectIcon.Location = new System.Drawing.Point(71, 15);
            this.projectIcon.Name = "projectIcon";
            this.projectIcon.Size = new System.Drawing.Size(16, 16);
            this.projectIcon.TabIndex = 0;
            this.projectIcon.TabStop = false;
            // 
            // locationIcon
            // 
            this.locationIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.locationIcon.Controls.Add(this.directory);
            this.locationIcon.Controls.Add(this.browseDirectoryButton);
            this.locationIcon.Controls.Add(this.label2);
            this.locationIcon.Controls.Add(this.dirIco);
            this.locationIcon.Location = new System.Drawing.Point(12, 87);
            this.locationIcon.Name = "locationIcon";
            this.locationIcon.Size = new System.Drawing.Size(525, 43);
            this.locationIcon.TabIndex = 1;
            this.locationIcon.TabStop = false;
            this.locationIcon.Text = "&Local Directory:";
            // 
            // directory
            // 
            this.directory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.directory.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.directory.Location = new System.Drawing.Point(93, 19);
            this.directory.Name = "directory";
            this.directory.ReadOnly = true;
            this.directory.Size = new System.Drawing.Size(397, 13);
            this.directory.TabIndex = 1;
            this.directory.Text = "C:\\Users\\SomeOne\\My Visual Studio Projects\\AnkhSvn";
            // 
            // browseDirectoryButton
            // 
            this.browseDirectoryButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseDirectoryButton.Location = new System.Drawing.Point(495, 12);
            this.browseDirectoryButton.Name = "browseDirectoryButton";
            this.browseDirectoryButton.Size = new System.Drawing.Size(24, 25);
            this.browseDirectoryButton.TabIndex = 2;
            this.browseDirectoryButton.Text = "...";
            this.browseDirectoryButton.UseVisualStyleBackColor = true;
            this.browseDirectoryButton.Click += new System.EventHandler(this.browseDirectory_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Directory:";
            // 
            // dirIco
            // 
            this.dirIco.Location = new System.Drawing.Point(71, 19);
            this.dirIco.Name = "dirIco";
            this.dirIco.Size = new System.Drawing.Size(16, 16);
            this.dirIco.TabIndex = 0;
            this.dirIco.TabStop = false;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.version);
            this.groupBox3.Location = new System.Drawing.Point(12, 136);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(525, 48);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "&Version:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(462, 204);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(381, 204);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // version
            // 
            this.version.Location = new System.Drawing.Point(93, 13);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(427, 29);
            this.version.SvnOrigin = null;
            this.version.TabIndex = 1;
            // 
            // CheckoutProject
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(549, 239);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.locationIcon);
            this.Controls.Add(this.groupBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CheckoutProject";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Open Project from Subversion";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.projectIcon)).EndInit();
            this.locationIcon.ResumeLayout(false);
            this.locationIcon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dirIco)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox projectUrl;
        private System.Windows.Forms.PictureBox projectIcon;
        private System.Windows.Forms.GroupBox locationIcon;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox dirIco;
        private System.Windows.Forms.Label projectRootLabel;
        private System.Windows.Forms.TextBox directory;
        private System.Windows.Forms.Button browseDirectoryButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ComboBox checkOutFrom;
        private System.Windows.Forms.Label projectLabel;
        private Ankh.UI.PathSelector.VersionSelector version;
    }
}