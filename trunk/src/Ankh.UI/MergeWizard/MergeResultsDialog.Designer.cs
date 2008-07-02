namespace Ankh.UI.MergeWizard
{
    partial class MergeResultsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeResultsDialog));
            this.horizontalDividerLabel = new System.Windows.Forms.Label();
            this.headerDescription = new System.Windows.Forms.Label();
            this.headerTitle = new System.Windows.Forms.Label();
            this.controlPanel = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.headerImage = new System.Windows.Forms.PictureBox();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.controlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerImage)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // horizontalDividerLabel
            // 
            resources.ApplyResources(this.horizontalDividerLabel, "horizontalDividerLabel");
            this.horizontalDividerLabel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.horizontalDividerLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.horizontalDividerLabel.Name = "horizontalDividerLabel";
            // 
            // headerDescription
            // 
            resources.ApplyResources(this.headerDescription, "headerDescription");
            this.headerDescription.Name = "headerDescription";
            // 
            // headerTitle
            // 
            resources.ApplyResources(this.headerTitle, "headerTitle");
            this.headerTitle.Name = "headerTitle";
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.okButton);
            this.controlPanel.Controls.Add(this.horizontalDividerLabel);
            this.controlPanel.Controls.Add(this.cancelButton);
            resources.ApplyResources(this.controlPanel, "controlPanel");
            this.controlPanel.Name = "controlPanel";
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // headerImage
            // 
            resources.ApplyResources(this.headerImage, "headerImage");
            this.headerImage.ErrorImage = null;
            this.headerImage.MaximumSize = new System.Drawing.Size(64, 64);
            this.headerImage.MinimumSize = new System.Drawing.Size(64, 64);
            this.headerImage.Name = "headerImage";
            this.headerImage.TabStop = false;
            // 
            // headerPanel
            // 
            resources.ApplyResources(this.headerPanel, "headerPanel");
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.Controls.Add(this.headerDescription);
            this.headerPanel.Controls.Add(this.headerTitle);
            this.headerPanel.Controls.Add(this.headerImage);
            this.headerPanel.Name = "headerPanel";
            // 
            // MergeResultsDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.headerPanel);
            this.MinimizeBox = false;
            this.Name = "MergeResultsDialog";
            this.ShowInTaskbar = false;
            this.controlPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerImage)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label horizontalDividerLabel;
        private System.Windows.Forms.Label headerDescription;
        private System.Windows.Forms.Label headerTitle;
        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PictureBox headerImage;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.Button okButton;
    }
}