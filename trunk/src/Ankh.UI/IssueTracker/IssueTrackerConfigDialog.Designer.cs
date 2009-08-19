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
            this.connectorLabel = new System.Windows.Forms.Label();
            this.connectorComboBox = new System.Windows.Forms.ComboBox();
            this.configPagePanel = new System.Windows.Forms.Panel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connectorLabel
            // 
            this.connectorLabel.AutoSize = true;
            this.connectorLabel.Location = new System.Drawing.Point(12, 9);
            this.connectorLabel.Name = "connectorLabel";
            this.connectorLabel.Size = new System.Drawing.Size(127, 13);
            this.connectorLabel.TabIndex = 0;
            this.connectorLabel.Text = "Issue Tracker Connector:";
            // 
            // connectorComboBox
            // 
            this.connectorComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.connectorComboBox.FormattingEnabled = true;
            this.connectorComboBox.Location = new System.Drawing.Point(15, 25);
            this.connectorComboBox.Name = "connectorComboBox";
            this.connectorComboBox.Size = new System.Drawing.Size(421, 21);
            this.connectorComboBox.TabIndex = 1;
            this.connectorComboBox.SelectedIndexChanged += new System.EventHandler(this.connectorComboBox_SelectedIndexChanged);
            // 
            // configPagePanel
            // 
            this.configPagePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.configPagePanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.configPagePanel.Location = new System.Drawing.Point(15, 52);
            this.configPagePanel.Name = "configPagePanel";
            this.configPagePanel.Size = new System.Drawing.Size(421, 273);
            this.configPagePanel.TabIndex = 2;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(280, 331);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(361, 331);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // IssueTrackerConfigDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(448, 366);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.configPagePanel);
            this.Controls.Add(this.connectorComboBox);
            this.Controls.Add(this.connectorLabel);
            this.Name = "IssueTrackerConfigDialog";
            this.Text = "Issue Tracker Configuration";
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