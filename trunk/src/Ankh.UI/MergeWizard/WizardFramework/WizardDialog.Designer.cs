/* 
 * WizardDialog.Designer.cs
 * 
 * Copyright (c) 2008 CollabNet, Inc. ("CollabNet"), http://www.collab.net,
 * Licensed under the Apache License, Version 2.0 (the "License"); 
 * you may not use this file except in compliance with the License. 
 * You may obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software 
 * distributed under the License is distributed on an "AS IS" BASIS, 
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
 * See the License for the specific language governing permissions and 
 * limitations under the License.
 * 
 **/
namespace WizardFramework
{
    partial class WizardDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardDialog));
            this.controlPanel = new System.Windows.Forms.Panel();
            this.horizontalDividerLabel = new System.Windows.Forms.Label();
            this.backButton = new System.Windows.Forms.Button();
            this.nextButton = new System.Windows.Forms.Button();
            this.finishButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.wizardPagePanel = new System.Windows.Forms.Panel();
            this.headerPanel = new System.Windows.Forms.Panel();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.statusIcon = new System.Windows.Forms.PictureBox();
            this.statusMessage = new System.Windows.Forms.Label();
            this.headerDescription = new System.Windows.Forms.Label();
            this.headerTitle = new System.Windows.Forms.Label();
            this.headerImage = new System.Windows.Forms.PictureBox();
            this.controlPanel.SuspendLayout();
            this.headerPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerImage)).BeginInit();
            this.SuspendLayout();
            // 
            // controlPanel
            // 
            this.controlPanel.Controls.Add(this.horizontalDividerLabel);
            this.controlPanel.Controls.Add(this.backButton);
            this.controlPanel.Controls.Add(this.nextButton);
            this.controlPanel.Controls.Add(this.finishButton);
            this.controlPanel.Controls.Add(this.cancelButton);
            resources.ApplyResources(this.controlPanel, "controlPanel");
            this.controlPanel.Name = "controlPanel";
            // 
            // horizontalDividerLabel
            // 
            resources.ApplyResources(this.horizontalDividerLabel, "horizontalDividerLabel");
            this.horizontalDividerLabel.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.horizontalDividerLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.horizontalDividerLabel.Name = "horizontalDividerLabel";
            // 
            // backButton
            // 
            resources.ApplyResources(this.backButton, "backButton");
            this.backButton.Name = "backButton";
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click);
            // 
            // nextButton
            // 
            resources.ApplyResources(this.nextButton, "nextButton");
            this.nextButton.Name = "nextButton";
            this.nextButton.UseVisualStyleBackColor = true;
            this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
            // 
            // finishButton
            // 
            resources.ApplyResources(this.finishButton, "finishButton");
            this.finishButton.Name = "finishButton";
            this.finishButton.UseVisualStyleBackColor = true;
            this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // wizardPagePanel
            // 
            resources.ApplyResources(this.wizardPagePanel, "wizardPagePanel");
            this.wizardPagePanel.MinimumSize = new System.Drawing.Size(349, 200);
            this.wizardPagePanel.Name = "wizardPagePanel";
            // 
            // headerPanel
            // 
            this.headerPanel.BackColor = System.Drawing.Color.White;
            this.headerPanel.Controls.Add(this.statusPanel);
            this.headerPanel.Controls.Add(this.headerDescription);
            this.headerPanel.Controls.Add(this.headerTitle);
            this.headerPanel.Controls.Add(this.headerImage);
            resources.ApplyResources(this.headerPanel, "headerPanel");
            this.headerPanel.Name = "headerPanel";
            // 
            // statusPanel
            // 
            resources.ApplyResources(this.statusPanel, "statusPanel");
            this.statusPanel.Controls.Add(this.statusIcon);
            this.statusPanel.Controls.Add(this.statusMessage);
            this.statusPanel.Name = "statusPanel";
            // 
            // statusIcon
            // 
            resources.ApplyResources(this.statusIcon, "statusIcon");
            this.statusIcon.Name = "statusIcon";
            this.statusIcon.TabStop = false;
            // 
            // statusMessage
            // 
            resources.ApplyResources(this.statusMessage, "statusMessage");
            this.statusMessage.Name = "statusMessage";
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
            // headerImage
            // 
            resources.ApplyResources(this.headerImage, "headerImage");
            this.headerImage.ErrorImage = null;
            this.headerImage.MaximumSize = new System.Drawing.Size(64, 64);
            this.headerImage.MinimumSize = new System.Drawing.Size(64, 64);
            this.headerImage.Name = "headerImage";
            this.headerImage.TabStop = false;
            // 
            // WizardDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.headerPanel);
            this.Controls.Add(this.wizardPagePanel);
            this.Controls.Add(this.controlPanel);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WizardDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.controlPanel.ResumeLayout(false);
            this.headerPanel.ResumeLayout(false);
            this.headerPanel.PerformLayout();
            this.statusPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.statusIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.headerImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel controlPanel;
        private System.Windows.Forms.Button backButton;
        private System.Windows.Forms.Button nextButton;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label horizontalDividerLabel;
        private System.Windows.Forms.Panel wizardPagePanel;
        private System.Windows.Forms.Panel headerPanel;
        private System.Windows.Forms.PictureBox headerImage;
        private System.Windows.Forms.Label headerTitle;
        private System.Windows.Forms.Label headerDescription;
        private System.Windows.Forms.Label statusMessage;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.PictureBox statusIcon;


    }
}