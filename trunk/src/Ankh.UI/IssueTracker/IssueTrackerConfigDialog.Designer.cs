namespace Ankh.UI.IssueTracker
{
    partial class IssueTrackerConfigDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IssueTrackerConfigDialog));
            this.connectorLabel = new System.Windows.Forms.Label();
            this.connectorComboBox = new System.Windows.Forms.ComboBox();
            this.configPagePanel = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connectorLabel
            // 
            resources.ApplyResources(this.connectorLabel, "connectorLabel");
            this.connectorLabel.Name = "connectorLabel";
            // 
            // connectorComboBox
            // 
            resources.ApplyResources(this.connectorComboBox, "connectorComboBox");
            this.connectorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.connectorComboBox.FormattingEnabled = true;
            this.connectorComboBox.Name = "connectorComboBox";
            this.connectorComboBox.SelectedIndexChanged += new System.EventHandler(this.connectorComboBox_SelectedIndexChanged);
            // 
            // configPagePanel
            // 
            resources.ApplyResources(this.configPagePanel, "configPagePanel");
            this.configPagePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.configPagePanel.Name = "configPagePanel";
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
            // IssueTrackerConfigDialog
            // 
            this.AcceptButton = this.cancelButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.configPagePanel);
            this.Controls.Add(this.connectorComboBox);
            this.Controls.Add(this.connectorLabel);
            this.Name = "IssueTrackerConfigDialog";
            this.Load += new System.EventHandler(this.IssueTrackerConfigDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label connectorLabel;
        private System.Windows.Forms.ComboBox connectorComboBox;
        private System.Windows.Forms.Panel configPagePanel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}