// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
    partial class MergeSourceBasePage
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeSourceBasePage));
			this.mergeFromComboBox = new System.Windows.Forms.ComboBox();
			this.selectButton = new System.Windows.Forms.Button();
			this.mergeFromLabel = new System.Windows.Forms.Label();
			this.wcBox = new System.Windows.Forms.GroupBox();
			this.wcUri = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.wcHistoryBtn = new System.Windows.Forms.Button();
			this.wcPath = new System.Windows.Forms.TextBox();
			this.dirLabel = new System.Windows.Forms.Label();
			this.wcBox.SuspendLayout();
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
			// wcBox
			// 
			resources.ApplyResources(this.wcBox, "wcBox");
			this.wcBox.Controls.Add(this.wcUri);
			this.wcBox.Controls.Add(this.label1);
			this.wcBox.Controls.Add(this.wcHistoryBtn);
			this.wcBox.Controls.Add(this.wcPath);
			this.wcBox.Controls.Add(this.dirLabel);
			this.wcBox.Name = "wcBox";
			this.wcBox.TabStop = false;
			// 
			// wcUri
			// 
			resources.ApplyResources(this.wcUri, "wcUri");
			this.wcUri.Name = "wcUri";
			this.wcUri.ReadOnly = true;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// wcHistoryBtn
			// 
			resources.ApplyResources(this.wcHistoryBtn, "wcHistoryBtn");
			this.wcHistoryBtn.Name = "wcHistoryBtn";
			this.wcHistoryBtn.UseVisualStyleBackColor = true;
			this.wcHistoryBtn.Click += new System.EventHandler(this.wcHistoryBtn_Click);
			// 
			// wcPath
			// 
			resources.ApplyResources(this.wcPath, "wcPath");
			this.wcPath.Name = "wcPath";
			this.wcPath.ReadOnly = true;
			// 
			// dirLabel
			// 
			resources.ApplyResources(this.dirLabel, "dirLabel");
			this.dirLabel.Name = "dirLabel";
			// 
			// MergeSourceBasePage
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.wcBox);
			this.Controls.Add(this.mergeFromComboBox);
			this.Controls.Add(this.selectButton);
			this.Controls.Add(this.mergeFromLabel);
			this.Name = "MergeSourceBasePage";
			this.wcBox.ResumeLayout(false);
			this.wcBox.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.ComboBox mergeFromComboBox;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.Label mergeFromLabel;
		private System.Windows.Forms.GroupBox wcBox;
		private System.Windows.Forms.Button wcHistoryBtn;
		private System.Windows.Forms.TextBox wcPath;
		private System.Windows.Forms.Label dirLabel;
		private System.Windows.Forms.TextBox wcUri;
		private System.Windows.Forms.Label label1;
    }
}
