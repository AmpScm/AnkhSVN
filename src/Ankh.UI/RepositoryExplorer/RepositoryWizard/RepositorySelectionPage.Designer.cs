namespace Ankh.UI.RepositoryExplorer.RepositoryWizard
{
    partial class RepositorySelectionPage
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
            this.repoProviderControlPanel = new System.Windows.Forms.Panel();
            this.errorLabel = new System.Windows.Forms.Label();
            this.repoProviderControlPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // repoProviderControlPanel
            // 
            this.repoProviderControlPanel.Controls.Add(this.errorLabel);
            this.repoProviderControlPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.repoProviderControlPanel.Location = new System.Drawing.Point(10, 10);
            this.repoProviderControlPanel.Name = "repoProviderControlPanel";
            this.repoProviderControlPanel.Size = new System.Drawing.Size(455, 260);
            this.repoProviderControlPanel.TabIndex = 1;
            // 
            // errorLabel
            // 
            this.errorLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorLabel.Location = new System.Drawing.Point(0, 0);
            this.errorLabel.Margin = new System.Windows.Forms.Padding(3);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(455, 260);
            this.errorLabel.TabIndex = 0;
            this.errorLabel.Text = "Selected provider does not provide UI. Please contact your repository provider.";
            this.errorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RepositorySelectionPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.repoProviderControlPanel);
            this.Name = "RepositorySelectionPage";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(475, 350);
            this.repoProviderControlPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel repoProviderControlPanel;
        private System.Windows.Forms.Label errorLabel;
    }
}
