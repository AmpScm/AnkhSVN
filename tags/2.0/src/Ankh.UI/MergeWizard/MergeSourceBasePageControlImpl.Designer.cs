﻿namespace Ankh.UI.MergeWizard
{
    partial class MergeSourceBasePageControlImpl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeSourceBasePageControlImpl));
            this.mergeFromComboBox = new System.Windows.Forms.ComboBox();
            this.selectButton = new System.Windows.Forms.Button();
            this.mergeFromLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // mergeFromComboBox
            // 
            resources.ApplyResources(this.mergeFromComboBox, "mergeFromComboBox");
            this.mergeFromComboBox.FormattingEnabled = true;
            this.mergeFromComboBox.Name = "mergeFromComboBox";
            // 
            // selectButton
            // 
            resources.ApplyResources(this.selectButton, "selectButton");
            this.selectButton.Name = "selectButton";
            this.selectButton.UseVisualStyleBackColor = true;
            this.selectButton.Click += new System.EventHandler(this.selectButton_Click);
            // 
            // mergeFromLabel
            // 
            resources.ApplyResources(this.mergeFromLabel, "mergeFromLabel");
            this.mergeFromLabel.Name = "mergeFromLabel";
            // 
            // MergeSourceBasePageControlImpl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mergeFromComboBox);
            this.Controls.Add(this.selectButton);
            this.Controls.Add(this.mergeFromLabel);
            this.Name = "MergeSourceBasePageControlImpl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ComboBox mergeFromComboBox;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Label mergeFromLabel;
    }
}
