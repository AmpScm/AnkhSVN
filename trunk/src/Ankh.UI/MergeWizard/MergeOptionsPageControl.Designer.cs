﻿namespace Ankh.UI.MergeWizard
{
    partial class MergeOptionsPageControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeOptionsPageControl));
            this.conflictsGroupBox = new System.Windows.Forms.GroupBox();
            this.binaryConflictsGroupBox = new System.Windows.Forms.GroupBox();
            this.binaryConflictsRepositoryRadioButton = new System.Windows.Forms.RadioButton();
            this.binaryConflictsMyRadioButton = new System.Windows.Forms.RadioButton();
            this.binaryConflictsMarkRadioButton = new System.Windows.Forms.RadioButton();
            this.binaryConflictsPromptRadioButton = new System.Windows.Forms.RadioButton();
            this.textConflictsGroupBox = new System.Windows.Forms.GroupBox();
            this.textConflictsMarkRadioButton = new System.Windows.Forms.RadioButton();
            this.textConflictsPromptRadioButton = new System.Windows.Forms.RadioButton();
            this.mergeOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.depthLabel = new System.Windows.Forms.Label();
            this.depthComboBox = new System.Windows.Forms.ComboBox();
            this.unversionedCheckBox = new System.Windows.Forms.CheckBox();
            this.ignoreAncestryCheckBox = new System.Windows.Forms.CheckBox();
            this.textConflictsRepositoryRadioButton = new System.Windows.Forms.RadioButton();
            this.textConflictsMyRadioButton = new System.Windows.Forms.RadioButton();
            this.conflictsGroupBox.SuspendLayout();
            this.binaryConflictsGroupBox.SuspendLayout();
            this.textConflictsGroupBox.SuspendLayout();
            this.mergeOptionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // conflictsGroupBox
            // 
            resources.ApplyResources(this.conflictsGroupBox, "conflictsGroupBox");
            this.conflictsGroupBox.Controls.Add(this.binaryConflictsGroupBox);
            this.conflictsGroupBox.Controls.Add(this.textConflictsGroupBox);
            this.conflictsGroupBox.Name = "conflictsGroupBox";
            this.conflictsGroupBox.TabStop = false;
            // 
            // binaryConflictsGroupBox
            // 
            resources.ApplyResources(this.binaryConflictsGroupBox, "binaryConflictsGroupBox");
            this.binaryConflictsGroupBox.Controls.Add(this.binaryConflictsRepositoryRadioButton);
            this.binaryConflictsGroupBox.Controls.Add(this.binaryConflictsMyRadioButton);
            this.binaryConflictsGroupBox.Controls.Add(this.binaryConflictsMarkRadioButton);
            this.binaryConflictsGroupBox.Controls.Add(this.binaryConflictsPromptRadioButton);
            this.binaryConflictsGroupBox.Name = "binaryConflictsGroupBox";
            this.binaryConflictsGroupBox.TabStop = false;
            // 
            // binaryConflictsRepositoryRadioButton
            // 
            resources.ApplyResources(this.binaryConflictsRepositoryRadioButton, "binaryConflictsRepositoryRadioButton");
            this.binaryConflictsRepositoryRadioButton.Name = "binaryConflictsRepositoryRadioButton";
            this.binaryConflictsRepositoryRadioButton.UseVisualStyleBackColor = true;
            // 
            // binaryConflictsMyRadioButton
            // 
            resources.ApplyResources(this.binaryConflictsMyRadioButton, "binaryConflictsMyRadioButton");
            this.binaryConflictsMyRadioButton.Name = "binaryConflictsMyRadioButton";
            this.binaryConflictsMyRadioButton.UseVisualStyleBackColor = true;
            // 
            // binaryConflictsMarkRadioButton
            // 
            resources.ApplyResources(this.binaryConflictsMarkRadioButton, "binaryConflictsMarkRadioButton");
            this.binaryConflictsMarkRadioButton.Name = "binaryConflictsMarkRadioButton";
            this.binaryConflictsMarkRadioButton.UseVisualStyleBackColor = true;
            // 
            // binaryConflictsPromptRadioButton
            // 
            resources.ApplyResources(this.binaryConflictsPromptRadioButton, "binaryConflictsPromptRadioButton");
            this.binaryConflictsPromptRadioButton.Checked = true;
            this.binaryConflictsPromptRadioButton.Name = "binaryConflictsPromptRadioButton";
            this.binaryConflictsPromptRadioButton.TabStop = true;
            this.binaryConflictsPromptRadioButton.UseVisualStyleBackColor = true;
            // 
            // textConflictsGroupBox
            // 
            resources.ApplyResources(this.textConflictsGroupBox, "textConflictsGroupBox");
            this.textConflictsGroupBox.Controls.Add(this.textConflictsRepositoryRadioButton);
            this.textConflictsGroupBox.Controls.Add(this.textConflictsMyRadioButton);
            this.textConflictsGroupBox.Controls.Add(this.textConflictsMarkRadioButton);
            this.textConflictsGroupBox.Controls.Add(this.textConflictsPromptRadioButton);
            this.textConflictsGroupBox.Name = "textConflictsGroupBox";
            this.textConflictsGroupBox.TabStop = false;
            // 
            // textConflictsMarkRadioButton
            // 
            resources.ApplyResources(this.textConflictsMarkRadioButton, "textConflictsMarkRadioButton");
            this.textConflictsMarkRadioButton.Name = "textConflictsMarkRadioButton";
            this.textConflictsMarkRadioButton.UseVisualStyleBackColor = true;
            // 
            // textConflictsPromptRadioButton
            // 
            resources.ApplyResources(this.textConflictsPromptRadioButton, "textConflictsPromptRadioButton");
            this.textConflictsPromptRadioButton.Checked = true;
            this.textConflictsPromptRadioButton.Name = "textConflictsPromptRadioButton";
            this.textConflictsPromptRadioButton.TabStop = true;
            this.textConflictsPromptRadioButton.UseVisualStyleBackColor = true;
            // 
            // mergeOptionsGroupBox
            // 
            resources.ApplyResources(this.mergeOptionsGroupBox, "mergeOptionsGroupBox");
            this.mergeOptionsGroupBox.Controls.Add(this.depthLabel);
            this.mergeOptionsGroupBox.Controls.Add(this.depthComboBox);
            this.mergeOptionsGroupBox.Controls.Add(this.unversionedCheckBox);
            this.mergeOptionsGroupBox.Controls.Add(this.ignoreAncestryCheckBox);
            this.mergeOptionsGroupBox.Name = "mergeOptionsGroupBox";
            this.mergeOptionsGroupBox.TabStop = false;
            // 
            // depthLabel
            // 
            resources.ApplyResources(this.depthLabel, "depthLabel");
            this.depthLabel.Name = "depthLabel";
            // 
            // depthComboBox
            // 
            this.depthComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.depthComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.depthComboBox, "depthComboBox");
            this.depthComboBox.Name = "depthComboBox";
            // 
            // unversionedCheckBox
            // 
            resources.ApplyResources(this.unversionedCheckBox, "unversionedCheckBox");
            this.unversionedCheckBox.Name = "unversionedCheckBox";
            this.unversionedCheckBox.UseVisualStyleBackColor = true;
            // 
            // ignoreAncestryCheckBox
            // 
            resources.ApplyResources(this.ignoreAncestryCheckBox, "ignoreAncestryCheckBox");
            this.ignoreAncestryCheckBox.Name = "ignoreAncestryCheckBox";
            this.ignoreAncestryCheckBox.UseVisualStyleBackColor = true;
            // 
            // textConflictsRepositoryRadioButton
            // 
            resources.ApplyResources(this.textConflictsRepositoryRadioButton, "textConflictsRepositoryRadioButton");
            this.textConflictsRepositoryRadioButton.Name = "textConflictsRepositoryRadioButton";
            this.textConflictsRepositoryRadioButton.UseVisualStyleBackColor = true;
            // 
            // textConflictsMyRadioButton
            // 
            resources.ApplyResources(this.textConflictsMyRadioButton, "textConflictsMyRadioButton");
            this.textConflictsMyRadioButton.Name = "textConflictsMyRadioButton";
            this.textConflictsMyRadioButton.UseVisualStyleBackColor = true;
            // 
            // MergeOptionsPageControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mergeOptionsGroupBox);
            this.Controls.Add(this.conflictsGroupBox);
            this.Name = "MergeOptionsPageControl";
            this.Load += new System.EventHandler(this.MergeOptionsPageControl_Load);
            this.conflictsGroupBox.ResumeLayout(false);
            this.binaryConflictsGroupBox.ResumeLayout(false);
            this.binaryConflictsGroupBox.PerformLayout();
            this.textConflictsGroupBox.ResumeLayout(false);
            this.textConflictsGroupBox.PerformLayout();
            this.mergeOptionsGroupBox.ResumeLayout(false);
            this.mergeOptionsGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox conflictsGroupBox;
        private System.Windows.Forms.GroupBox textConflictsGroupBox;
        private System.Windows.Forms.RadioButton textConflictsPromptRadioButton;
        private System.Windows.Forms.RadioButton textConflictsMarkRadioButton;
        private System.Windows.Forms.GroupBox binaryConflictsGroupBox;
        private System.Windows.Forms.RadioButton binaryConflictsMarkRadioButton;
        private System.Windows.Forms.RadioButton binaryConflictsPromptRadioButton;
        private System.Windows.Forms.RadioButton binaryConflictsMyRadioButton;
        private System.Windows.Forms.RadioButton binaryConflictsRepositoryRadioButton;
        private System.Windows.Forms.GroupBox mergeOptionsGroupBox;
        private System.Windows.Forms.CheckBox unversionedCheckBox;
        private System.Windows.Forms.CheckBox ignoreAncestryCheckBox;
        private System.Windows.Forms.ComboBox depthComboBox;
        private System.Windows.Forms.Label depthLabel;
        private System.Windows.Forms.RadioButton textConflictsRepositoryRadioButton;
        private System.Windows.Forms.RadioButton textConflictsMyRadioButton;
    }
}
