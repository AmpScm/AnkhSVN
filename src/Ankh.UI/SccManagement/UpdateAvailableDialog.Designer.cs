namespace Ankh.UI.SccManagement
{
    partial class UpdateAvailableDialog
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.bodyLabel = new System.Windows.Forms.Label();
            this.linkLabel = new System.Windows.Forms.LinkLabel();
            this.versionPanel = new System.Windows.Forms.Panel();
            this.newVerLabel = new System.Windows.Forms.Label();
            this.curVerLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.headLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.updateInterval = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.versionPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Window;
            this.panel1.Controls.Add(this.bodyLabel);
            this.panel1.Controls.Add(this.linkLabel);
            this.panel1.Controls.Add(this.versionPanel);
            this.panel1.Controls.Add(this.headLabel);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(411, 199);
            this.panel1.TabIndex = 0;
            // 
            // bodyLabel
            // 
            this.bodyLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.bodyLabel.Location = new System.Drawing.Point(12, 39);
            this.bodyLabel.Name = "bodyLabel";
            this.bodyLabel.Size = new System.Drawing.Size(396, 76);
            this.bodyLabel.TabIndex = 1;
            this.bodyLabel.Text = "bodyLabel";
            // 
            // linkLabel
            // 
            this.linkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.linkLabel.Location = new System.Drawing.Point(12, 166);
            this.linkLabel.Name = "linkLabel";
            this.linkLabel.Size = new System.Drawing.Size(387, 23);
            this.linkLabel.TabIndex = 2;
            this.linkLabel.TabStop = true;
            this.linkLabel.Text = "linkLabel";
            this.linkLabel.UseMnemonic = false;
            this.linkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel_LinkClicked);
            // 
            // versionPanel
            // 
            this.versionPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.versionPanel.Controls.Add(this.newVerLabel);
            this.versionPanel.Controls.Add(this.curVerLabel);
            this.versionPanel.Controls.Add(this.label2);
            this.versionPanel.Controls.Add(this.label1);
            this.versionPanel.Enabled = false;
            this.versionPanel.Location = new System.Drawing.Point(15, 118);
            this.versionPanel.Name = "versionPanel";
            this.versionPanel.Size = new System.Drawing.Size(384, 45);
            this.versionPanel.TabIndex = 2;
            this.versionPanel.Visible = false;
            // 
            // newVerLabel
            // 
            this.newVerLabel.Location = new System.Drawing.Point(109, 23);
            this.newVerLabel.Name = "newVerLabel";
            this.newVerLabel.Size = new System.Drawing.Size(275, 23);
            this.newVerLabel.TabIndex = 3;
            this.newVerLabel.Text = "newVerLabel";
            this.newVerLabel.UseMnemonic = false;
            // 
            // curVerLabel
            // 
            this.curVerLabel.Location = new System.Drawing.Point(109, 0);
            this.curVerLabel.Name = "curVerLabel";
            this.curVerLabel.Size = new System.Drawing.Size(275, 23);
            this.curVerLabel.TabIndex = 1;
            this.curVerLabel.Text = "curVerLabel";
            this.curVerLabel.UseMnemonic = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Updated version:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current version:";
            // 
            // headLabel
            // 
            this.headLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.headLabel.Location = new System.Drawing.Point(12, 9);
            this.headLabel.Name = "headLabel";
            this.headLabel.Size = new System.Drawing.Size(387, 30);
            this.headLabel.TabIndex = 0;
            this.headLabel.Text = "headLabel";
            this.headLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.headLabel.UseMnemonic = false;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(324, 210);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Enabled = false;
            this.label3.Location = new System.Drawing.Point(12, 215);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "&Next check:";
            this.label3.Visible = false;
            // 
            // updateInterval
            // 
            this.updateInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.updateInterval.Enabled = false;
            this.updateInterval.FormattingEnabled = true;
            this.updateInterval.Items.AddRange(new object[] {
            "Tomorrow",
            "In one week",
            "In two weeks",
            "In four weeks",
            "Never"});
            this.updateInterval.Location = new System.Drawing.Point(83, 212);
            this.updateInterval.Name = "updateInterval";
            this.updateInterval.Size = new System.Drawing.Size(140, 21);
            this.updateInterval.TabIndex = 2;
            this.updateInterval.Visible = false;
            // 
            // UpdateAvailableDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.okButton;
            this.ClientSize = new System.Drawing.Size(411, 245);
            this.Controls.Add(this.updateInterval);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateAvailableDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "AnkhSVN - {0}";
            this.panel1.ResumeLayout(false);
            this.versionPanel.ResumeLayout(false);
            this.versionPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button okButton;
        public System.Windows.Forms.Label headLabel;
        public System.Windows.Forms.LinkLabel linkLabel;
        public System.Windows.Forms.Panel versionPanel;
        public System.Windows.Forms.Label newVerLabel;
        public System.Windows.Forms.Label curVerLabel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox updateInterval;
        public System.Windows.Forms.Label bodyLabel;
    }
}