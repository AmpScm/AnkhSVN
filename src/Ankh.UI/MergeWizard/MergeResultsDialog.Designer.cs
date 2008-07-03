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
            this.fileStatisticsGroupBox = new System.Windows.Forms.GroupBox();
            this.propertyStatisticsGroupBox = new System.Windows.Forms.GroupBox();
            this.fileUpdatedLabel = new System.Windows.Forms.Label();
            this.fileAddedLabel = new System.Windows.Forms.Label();
            this.fileExistedLabel = new System.Windows.Forms.Label();
            this.fileDeletedLabel = new System.Windows.Forms.Label();
            this.fileMergedLabel = new System.Windows.Forms.Label();
            this.fileConflictedLabel = new System.Windows.Forms.Label();
            this.fileResolvedLabel = new System.Windows.Forms.Label();
            this.filesSkipedDirectoriesLabel = new System.Windows.Forms.Label();
            this.fileSkippedFilesLabel = new System.Windows.Forms.Label();
            this.fileUpdatedValueLabel = new System.Windows.Forms.Label();
            this.fileAddedValueLabel = new System.Windows.Forms.Label();
            this.fileExistedValueLabel = new System.Windows.Forms.Label();
            this.fileDeletedValueLabel = new System.Windows.Forms.Label();
            this.fileMergedValueLabel = new System.Windows.Forms.Label();
            this.fileConflictedValueLabel = new System.Windows.Forms.Label();
            this.fileResolvedValueLabel = new System.Windows.Forms.Label();
            this.fileSkippedDirectoriesValueLabel = new System.Windows.Forms.Label();
            this.fileSkippedFilesValueLabel = new System.Windows.Forms.Label();
            this.propertyResolvedValueLabel = new System.Windows.Forms.Label();
            this.propertyConflictedValueLabel = new System.Windows.Forms.Label();
            this.propertyMergedValueLabel = new System.Windows.Forms.Label();
            this.propertyUpdatedValueLabel = new System.Windows.Forms.Label();
            this.propertyResolvedLabel = new System.Windows.Forms.Label();
            this.propertyConflictedLabel = new System.Windows.Forms.Label();
            this.propertyMergedLabel = new System.Windows.Forms.Label();
            this.propertyUpdatedLabel = new System.Windows.Forms.Label();
            this.controlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.headerImage)).BeginInit();
            this.headerPanel.SuspendLayout();
            this.fileStatisticsGroupBox.SuspendLayout();
            this.propertyStatisticsGroupBox.SuspendLayout();
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
            // fileStatisticsGroupBox
            // 
            this.fileStatisticsGroupBox.Controls.Add(this.fileSkippedFilesValueLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileSkippedDirectoriesValueLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileResolvedValueLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileConflictedValueLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileMergedValueLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileDeletedValueLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileExistedValueLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileAddedValueLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileUpdatedValueLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileSkippedFilesLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.filesSkipedDirectoriesLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileResolvedLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileConflictedLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileMergedLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileDeletedLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileExistedLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileAddedLabel);
            this.fileStatisticsGroupBox.Controls.Add(this.fileUpdatedLabel);
            resources.ApplyResources(this.fileStatisticsGroupBox, "fileStatisticsGroupBox");
            this.fileStatisticsGroupBox.Name = "fileStatisticsGroupBox";
            this.fileStatisticsGroupBox.TabStop = false;
            // 
            // propertyStatisticsGroupBox
            // 
            this.propertyStatisticsGroupBox.Controls.Add(this.propertyResolvedValueLabel);
            this.propertyStatisticsGroupBox.Controls.Add(this.propertyConflictedValueLabel);
            this.propertyStatisticsGroupBox.Controls.Add(this.propertyMergedValueLabel);
            this.propertyStatisticsGroupBox.Controls.Add(this.propertyUpdatedValueLabel);
            this.propertyStatisticsGroupBox.Controls.Add(this.propertyResolvedLabel);
            this.propertyStatisticsGroupBox.Controls.Add(this.propertyConflictedLabel);
            this.propertyStatisticsGroupBox.Controls.Add(this.propertyMergedLabel);
            this.propertyStatisticsGroupBox.Controls.Add(this.propertyUpdatedLabel);
            resources.ApplyResources(this.propertyStatisticsGroupBox, "propertyStatisticsGroupBox");
            this.propertyStatisticsGroupBox.Name = "propertyStatisticsGroupBox";
            this.propertyStatisticsGroupBox.TabStop = false;
            // 
            // fileUpdatedLabel
            // 
            resources.ApplyResources(this.fileUpdatedLabel, "fileUpdatedLabel");
            this.fileUpdatedLabel.Name = "fileUpdatedLabel";
            // 
            // fileAddedLabel
            // 
            resources.ApplyResources(this.fileAddedLabel, "fileAddedLabel");
            this.fileAddedLabel.Name = "fileAddedLabel";
            // 
            // fileExistedLabel
            // 
            resources.ApplyResources(this.fileExistedLabel, "fileExistedLabel");
            this.fileExistedLabel.Name = "fileExistedLabel";
            // 
            // fileDeletedLabel
            // 
            resources.ApplyResources(this.fileDeletedLabel, "fileDeletedLabel");
            this.fileDeletedLabel.Name = "fileDeletedLabel";
            // 
            // fileMergedLabel
            // 
            resources.ApplyResources(this.fileMergedLabel, "fileMergedLabel");
            this.fileMergedLabel.Name = "fileMergedLabel";
            // 
            // fileConflictedLabel
            // 
            resources.ApplyResources(this.fileConflictedLabel, "fileConflictedLabel");
            this.fileConflictedLabel.Name = "fileConflictedLabel";
            // 
            // fileResolvedLabel
            // 
            resources.ApplyResources(this.fileResolvedLabel, "fileResolvedLabel");
            this.fileResolvedLabel.Name = "fileResolvedLabel";
            // 
            // filesSkipedDirectoriesLabel
            // 
            resources.ApplyResources(this.filesSkipedDirectoriesLabel, "filesSkipedDirectoriesLabel");
            this.filesSkipedDirectoriesLabel.Name = "filesSkipedDirectoriesLabel";
            // 
            // fileSkippedFilesLabel
            // 
            resources.ApplyResources(this.fileSkippedFilesLabel, "fileSkippedFilesLabel");
            this.fileSkippedFilesLabel.Name = "fileSkippedFilesLabel";
            // 
            // fileUpdatedValueLabel
            // 
            resources.ApplyResources(this.fileUpdatedValueLabel, "fileUpdatedValueLabel");
            this.fileUpdatedValueLabel.Name = "fileUpdatedValueLabel";
            // 
            // fileAddedValueLabel
            // 
            resources.ApplyResources(this.fileAddedValueLabel, "fileAddedValueLabel");
            this.fileAddedValueLabel.Name = "fileAddedValueLabel";
            // 
            // fileExistedValueLabel
            // 
            resources.ApplyResources(this.fileExistedValueLabel, "fileExistedValueLabel");
            this.fileExistedValueLabel.Name = "fileExistedValueLabel";
            // 
            // fileDeletedValueLabel
            // 
            resources.ApplyResources(this.fileDeletedValueLabel, "fileDeletedValueLabel");
            this.fileDeletedValueLabel.Name = "fileDeletedValueLabel";
            // 
            // fileMergedValueLabel
            // 
            resources.ApplyResources(this.fileMergedValueLabel, "fileMergedValueLabel");
            this.fileMergedValueLabel.Name = "fileMergedValueLabel";
            // 
            // fileConflictedValueLabel
            // 
            resources.ApplyResources(this.fileConflictedValueLabel, "fileConflictedValueLabel");
            this.fileConflictedValueLabel.Name = "fileConflictedValueLabel";
            // 
            // fileResolvedValueLabel
            // 
            resources.ApplyResources(this.fileResolvedValueLabel, "fileResolvedValueLabel");
            this.fileResolvedValueLabel.Name = "fileResolvedValueLabel";
            // 
            // fileSkippedDirectoriesValueLabel
            // 
            resources.ApplyResources(this.fileSkippedDirectoriesValueLabel, "fileSkippedDirectoriesValueLabel");
            this.fileSkippedDirectoriesValueLabel.Name = "fileSkippedDirectoriesValueLabel";
            // 
            // fileSkippedFilesValueLabel
            // 
            resources.ApplyResources(this.fileSkippedFilesValueLabel, "fileSkippedFilesValueLabel");
            this.fileSkippedFilesValueLabel.Name = "fileSkippedFilesValueLabel";
            // 
            // propertyResolvedValueLabel
            // 
            resources.ApplyResources(this.propertyResolvedValueLabel, "propertyResolvedValueLabel");
            this.propertyResolvedValueLabel.Name = "propertyResolvedValueLabel";
            // 
            // propertyConflictedValueLabel
            // 
            resources.ApplyResources(this.propertyConflictedValueLabel, "propertyConflictedValueLabel");
            this.propertyConflictedValueLabel.Name = "propertyConflictedValueLabel";
            // 
            // propertyMergedValueLabel
            // 
            resources.ApplyResources(this.propertyMergedValueLabel, "propertyMergedValueLabel");
            this.propertyMergedValueLabel.Name = "propertyMergedValueLabel";
            // 
            // propertyUpdatedValueLabel
            // 
            resources.ApplyResources(this.propertyUpdatedValueLabel, "propertyUpdatedValueLabel");
            this.propertyUpdatedValueLabel.Name = "propertyUpdatedValueLabel";
            // 
            // propertyResolvedLabel
            // 
            resources.ApplyResources(this.propertyResolvedLabel, "propertyResolvedLabel");
            this.propertyResolvedLabel.Name = "propertyResolvedLabel";
            // 
            // propertyConflictedLabel
            // 
            resources.ApplyResources(this.propertyConflictedLabel, "propertyConflictedLabel");
            this.propertyConflictedLabel.Name = "propertyConflictedLabel";
            // 
            // propertyMergedLabel
            // 
            resources.ApplyResources(this.propertyMergedLabel, "propertyMergedLabel");
            this.propertyMergedLabel.Name = "propertyMergedLabel";
            // 
            // propertyUpdatedLabel
            // 
            resources.ApplyResources(this.propertyUpdatedLabel, "propertyUpdatedLabel");
            this.propertyUpdatedLabel.Name = "propertyUpdatedLabel";
            // 
            // MergeResultsDialog
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.propertyStatisticsGroupBox);
            this.Controls.Add(this.fileStatisticsGroupBox);
            this.Controls.Add(this.controlPanel);
            this.Controls.Add(this.headerPanel);
            this.MinimizeBox = false;
            this.Name = "MergeResultsDialog";
            this.ShowInTaskbar = false;
            this.controlPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.headerImage)).EndInit();
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.fileStatisticsGroupBox.ResumeLayout(false);
            this.fileStatisticsGroupBox.PerformLayout();
            this.propertyStatisticsGroupBox.ResumeLayout(false);
            this.propertyStatisticsGroupBox.PerformLayout();
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
        private System.Windows.Forms.GroupBox fileStatisticsGroupBox;
        private System.Windows.Forms.GroupBox propertyStatisticsGroupBox;
        private System.Windows.Forms.Label fileUpdatedLabel;
        private System.Windows.Forms.Label fileExistedLabel;
        private System.Windows.Forms.Label fileAddedLabel;
        private System.Windows.Forms.Label fileResolvedLabel;
        private System.Windows.Forms.Label fileConflictedLabel;
        private System.Windows.Forms.Label fileMergedLabel;
        private System.Windows.Forms.Label fileDeletedLabel;
        private System.Windows.Forms.Label fileSkippedFilesLabel;
        private System.Windows.Forms.Label filesSkipedDirectoriesLabel;
        private System.Windows.Forms.Label fileUpdatedValueLabel;
        private System.Windows.Forms.Label fileExistedValueLabel;
        private System.Windows.Forms.Label fileAddedValueLabel;
        private System.Windows.Forms.Label fileSkippedFilesValueLabel;
        private System.Windows.Forms.Label fileSkippedDirectoriesValueLabel;
        private System.Windows.Forms.Label fileResolvedValueLabel;
        private System.Windows.Forms.Label fileConflictedValueLabel;
        private System.Windows.Forms.Label fileMergedValueLabel;
        private System.Windows.Forms.Label fileDeletedValueLabel;
        private System.Windows.Forms.Label propertyResolvedValueLabel;
        private System.Windows.Forms.Label propertyConflictedValueLabel;
        private System.Windows.Forms.Label propertyMergedValueLabel;
        private System.Windows.Forms.Label propertyUpdatedValueLabel;
        private System.Windows.Forms.Label propertyResolvedLabel;
        private System.Windows.Forms.Label propertyConflictedLabel;
        private System.Windows.Forms.Label propertyMergedLabel;
        private System.Windows.Forms.Label propertyUpdatedLabel;
    }
}