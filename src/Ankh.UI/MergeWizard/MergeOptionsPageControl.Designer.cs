// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

namespace Ankh.UI.MergeWizard
{
    partial class MergeOptionsPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeOptionsPage));
            this.conflictsGroupBox = new System.Windows.Forms.GroupBox();
            this.binaryConflictsGroupBox = new System.Windows.Forms.GroupBox();
            this.binaryConflictsBaseRadioButton = new System.Windows.Forms.RadioButton();
            this.binaryConflictsRepositoryRadioButton = new System.Windows.Forms.RadioButton();
            this.binaryConflictsMyRadioButton = new System.Windows.Forms.RadioButton();
            this.binaryConflictsMarkRadioButton = new System.Windows.Forms.RadioButton();
            this.binaryConflictsPromptRadioButton = new System.Windows.Forms.RadioButton();
            this.textConflictsGroupBox = new System.Windows.Forms.GroupBox();
            this.textConflictsBaseRadioButton = new System.Windows.Forms.RadioButton();
            this.textConflictsRepositoryRadioButton = new System.Windows.Forms.RadioButton();
            this.textConflictsMyRadioButton = new System.Windows.Forms.RadioButton();
            this.textConflictsMarkRadioButton = new System.Windows.Forms.RadioButton();
            this.textConflictsPromptRadioButton = new System.Windows.Forms.RadioButton();
            this.mergeOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.depthLabel = new System.Windows.Forms.Label();
            this.depthComboBox = new System.Windows.Forms.ComboBox();
            this.unversionedCheckBox = new System.Windows.Forms.CheckBox();
            this.ignoreAncestryCheckBox = new System.Windows.Forms.CheckBox();
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
            this.binaryConflictsGroupBox.Controls.Add(this.binaryConflictsBaseRadioButton);
            this.binaryConflictsGroupBox.Controls.Add(this.binaryConflictsRepositoryRadioButton);
            this.binaryConflictsGroupBox.Controls.Add(this.binaryConflictsMyRadioButton);
            this.binaryConflictsGroupBox.Controls.Add(this.binaryConflictsMarkRadioButton);
            this.binaryConflictsGroupBox.Controls.Add(this.binaryConflictsPromptRadioButton);
            this.binaryConflictsGroupBox.Name = "binaryConflictsGroupBox";
            this.binaryConflictsGroupBox.TabStop = false;
            // 
            // binaryConflictsBaseRadioButton
            // 
            resources.ApplyResources(this.binaryConflictsBaseRadioButton, "binaryConflictsBaseRadioButton");
            this.binaryConflictsBaseRadioButton.Name = "binaryConflictsBaseRadioButton";
            this.binaryConflictsBaseRadioButton.UseVisualStyleBackColor = true;
            this.binaryConflictsBaseRadioButton.CheckedChanged += new System.EventHandler(this.binaryConflictsBaseRadioButton_CheckedChanged);
            // 
            // binaryConflictsRepositoryRadioButton
            // 
            resources.ApplyResources(this.binaryConflictsRepositoryRadioButton, "binaryConflictsRepositoryRadioButton");
            this.binaryConflictsRepositoryRadioButton.Name = "binaryConflictsRepositoryRadioButton";
            this.binaryConflictsRepositoryRadioButton.UseVisualStyleBackColor = true;
            this.binaryConflictsRepositoryRadioButton.CheckedChanged += new System.EventHandler(this.binaryConflictsRepositoryRadioButton_CheckedChanged);
            // 
            // binaryConflictsMyRadioButton
            // 
            resources.ApplyResources(this.binaryConflictsMyRadioButton, "binaryConflictsMyRadioButton");
            this.binaryConflictsMyRadioButton.Name = "binaryConflictsMyRadioButton";
            this.binaryConflictsMyRadioButton.UseVisualStyleBackColor = true;
            this.binaryConflictsMyRadioButton.CheckedChanged += new System.EventHandler(this.binaryConflictsMyRadioButton_CheckedChanged);
            // 
            // binaryConflictsMarkRadioButton
            // 
            resources.ApplyResources(this.binaryConflictsMarkRadioButton, "binaryConflictsMarkRadioButton");
            this.binaryConflictsMarkRadioButton.Name = "binaryConflictsMarkRadioButton";
            this.binaryConflictsMarkRadioButton.UseVisualStyleBackColor = true;
            this.binaryConflictsMarkRadioButton.CheckedChanged += new System.EventHandler(this.binaryConflictsMarkRadioButton_CheckedChanged);
            // 
            // binaryConflictsPromptRadioButton
            // 
            resources.ApplyResources(this.binaryConflictsPromptRadioButton, "binaryConflictsPromptRadioButton");
            this.binaryConflictsPromptRadioButton.Checked = true;
            this.binaryConflictsPromptRadioButton.Name = "binaryConflictsPromptRadioButton";
            this.binaryConflictsPromptRadioButton.TabStop = true;
            this.binaryConflictsPromptRadioButton.UseVisualStyleBackColor = true;
            this.binaryConflictsPromptRadioButton.CheckedChanged += new System.EventHandler(this.binaryConflictsPromptRadioButton_CheckedChanged);
            // 
            // textConflictsGroupBox
            // 
            resources.ApplyResources(this.textConflictsGroupBox, "textConflictsGroupBox");
            this.textConflictsGroupBox.Controls.Add(this.textConflictsBaseRadioButton);
            this.textConflictsGroupBox.Controls.Add(this.textConflictsRepositoryRadioButton);
            this.textConflictsGroupBox.Controls.Add(this.textConflictsMyRadioButton);
            this.textConflictsGroupBox.Controls.Add(this.textConflictsMarkRadioButton);
            this.textConflictsGroupBox.Controls.Add(this.textConflictsPromptRadioButton);
            this.textConflictsGroupBox.Name = "textConflictsGroupBox";
            this.textConflictsGroupBox.TabStop = false;
            // 
            // textConflictsBaseRadioButton
            // 
            resources.ApplyResources(this.textConflictsBaseRadioButton, "textConflictsBaseRadioButton");
            this.textConflictsBaseRadioButton.Name = "textConflictsBaseRadioButton";
            this.textConflictsBaseRadioButton.UseVisualStyleBackColor = true;
            this.textConflictsBaseRadioButton.CheckedChanged += new System.EventHandler(this.textConflictsBaseRadioButton_CheckedChanged);
            // 
            // textConflictsRepositoryRadioButton
            // 
            resources.ApplyResources(this.textConflictsRepositoryRadioButton, "textConflictsRepositoryRadioButton");
            this.textConflictsRepositoryRadioButton.Name = "textConflictsRepositoryRadioButton";
            this.textConflictsRepositoryRadioButton.UseVisualStyleBackColor = true;
            this.textConflictsRepositoryRadioButton.CheckedChanged += new System.EventHandler(this.textConflictsRepositoryRadioButton_CheckedChanged);
            // 
            // textConflictsMyRadioButton
            // 
            resources.ApplyResources(this.textConflictsMyRadioButton, "textConflictsMyRadioButton");
            this.textConflictsMyRadioButton.Name = "textConflictsMyRadioButton";
            this.textConflictsMyRadioButton.UseVisualStyleBackColor = true;
            this.textConflictsMyRadioButton.CheckedChanged += new System.EventHandler(this.textConflictsMyRadioButton_CheckedChanged);
            // 
            // textConflictsMarkRadioButton
            // 
            resources.ApplyResources(this.textConflictsMarkRadioButton, "textConflictsMarkRadioButton");
            this.textConflictsMarkRadioButton.Name = "textConflictsMarkRadioButton";
            this.textConflictsMarkRadioButton.UseVisualStyleBackColor = true;
            this.textConflictsMarkRadioButton.CheckedChanged += new System.EventHandler(this.textConflictsMarkRadioButton_CheckedChanged);
            // 
            // textConflictsPromptRadioButton
            // 
            resources.ApplyResources(this.textConflictsPromptRadioButton, "textConflictsPromptRadioButton");
            this.textConflictsPromptRadioButton.Checked = true;
            this.textConflictsPromptRadioButton.Name = "textConflictsPromptRadioButton";
            this.textConflictsPromptRadioButton.TabStop = true;
            this.textConflictsPromptRadioButton.UseVisualStyleBackColor = true;
            this.textConflictsPromptRadioButton.CheckedChanged += new System.EventHandler(this.textConflictsPromptRadioButton_CheckedChanged);
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
            this.depthComboBox.SelectedIndexChanged += new System.EventHandler(this.depthComboBox_SelectedIndexChanged);
            // 
            // unversionedCheckBox
            // 
            resources.ApplyResources(this.unversionedCheckBox, "unversionedCheckBox");
            this.unversionedCheckBox.Name = "unversionedCheckBox";
            this.unversionedCheckBox.UseVisualStyleBackColor = true;
            this.unversionedCheckBox.CheckedChanged += new System.EventHandler(this.unversionedCheckBox_CheckedChanged);
            // 
            // ignoreAncestryCheckBox
            // 
            resources.ApplyResources(this.ignoreAncestryCheckBox, "ignoreAncestryCheckBox");
            this.ignoreAncestryCheckBox.Name = "ignoreAncestryCheckBox";
            this.ignoreAncestryCheckBox.UseVisualStyleBackColor = true;
            this.ignoreAncestryCheckBox.CheckedChanged += new System.EventHandler(this.ignoreAncestryCheckBox_CheckedChanged);
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
        private System.Windows.Forms.RadioButton textConflictsBaseRadioButton;
        private System.Windows.Forms.RadioButton binaryConflictsBaseRadioButton;
    }
}
