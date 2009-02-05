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
    partial class MergeSourceRangeOfRevisionsPageControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeSourceRangeOfRevisionsPageControl));
            this.revisionsGroupBox = new System.Windows.Forms.GroupBox();
            this.selectRevisionsRadioButton = new System.Windows.Forms.RadioButton();
            this.allRevisionsRadioButton = new System.Windows.Forms.RadioButton();
            this.revisionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // revisionsGroupBox
            // 
            resources.ApplyResources(this.revisionsGroupBox, "revisionsGroupBox");
            this.revisionsGroupBox.Controls.Add(this.selectRevisionsRadioButton);
            this.revisionsGroupBox.Controls.Add(this.allRevisionsRadioButton);
            this.revisionsGroupBox.Name = "revisionsGroupBox";
            this.revisionsGroupBox.TabStop = false;
            // 
            // selectRevisionsRadioButton
            // 
            resources.ApplyResources(this.selectRevisionsRadioButton, "selectRevisionsRadioButton");
            this.selectRevisionsRadioButton.Checked = true;
            this.selectRevisionsRadioButton.Name = "selectRevisionsRadioButton";
            this.selectRevisionsRadioButton.TabStop = true;
            this.selectRevisionsRadioButton.UseVisualStyleBackColor = true;
            this.selectRevisionsRadioButton.CheckedChanged += new System.EventHandler(this.selectRevisionsRadioButton_CheckedChanged);
            // 
            // allRevisionsRadioButton
            // 
            resources.ApplyResources(this.allRevisionsRadioButton, "allRevisionsRadioButton");
            this.allRevisionsRadioButton.Name = "allRevisionsRadioButton";
            this.allRevisionsRadioButton.UseVisualStyleBackColor = true;
            this.allRevisionsRadioButton.CheckedChanged += new System.EventHandler(this.allRevisionsRadioButton_CheckedChanged);
            // 
            // MergeSourceRangeOfRevisionsPageControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.revisionsGroupBox);
            this.Name = "MergeSourceRangeOfRevisionsPageControl";
            this.Controls.SetChildIndex(this.mergeFromComboBox, 0);
            this.Controls.SetChildIndex(this.revisionsGroupBox, 0);
            this.revisionsGroupBox.ResumeLayout(false);
            this.revisionsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox revisionsGroupBox;
        private System.Windows.Forms.RadioButton selectRevisionsRadioButton;
        private System.Windows.Forms.RadioButton allRevisionsRadioButton;

    }
}
