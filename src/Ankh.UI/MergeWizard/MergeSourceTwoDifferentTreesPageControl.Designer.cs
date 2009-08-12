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
    partial class MergeSourceTwoDifferentTreesPage
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MergeSourceTwoDifferentTreesPage));
            this.fromGroupBox = new System.Windows.Forms.GroupBox();
            this.fromRevisionSelectButton = new System.Windows.Forms.Button();
            this.fromRevisionTextBox = new System.Windows.Forms.TextBox();
            this.fromRevisionRadioButton = new System.Windows.Forms.RadioButton();
            this.fromHEADRevisionRadioButton = new System.Windows.Forms.RadioButton();
            this.fromURLTextBox = new System.Windows.Forms.TextBox();
            this.fromURLSelectButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.useFromURLCheckBox = new System.Windows.Forms.CheckBox();
            this.toRevisionSelectButton = new System.Windows.Forms.Button();
            this.toRevisionTextBox = new System.Windows.Forms.TextBox();
            this.toRevisionRadioButton = new System.Windows.Forms.RadioButton();
            this.toHEADRevisionRadioButton = new System.Windows.Forms.RadioButton();
            this.toURLTextBox = new System.Windows.Forms.TextBox();
            this.toURLSelectButton = new System.Windows.Forms.Button();
            this.fromGroupBox.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // fromGroupBox
            // 
            resources.ApplyResources(this.fromGroupBox, "fromGroupBox");
            this.fromGroupBox.Controls.Add(this.fromRevisionSelectButton);
            this.fromGroupBox.Controls.Add(this.fromRevisionTextBox);
            this.fromGroupBox.Controls.Add(this.fromRevisionRadioButton);
            this.fromGroupBox.Controls.Add(this.fromHEADRevisionRadioButton);
            this.fromGroupBox.Controls.Add(this.fromURLTextBox);
            this.fromGroupBox.Controls.Add(this.fromURLSelectButton);
            this.fromGroupBox.Name = "fromGroupBox";
            this.fromGroupBox.TabStop = false;
            // 
            // fromRevisionSelectButton
            // 
            resources.ApplyResources(this.fromRevisionSelectButton, "fromRevisionSelectButton");
            this.fromRevisionSelectButton.Name = "fromRevisionSelectButton";
            this.fromRevisionSelectButton.UseVisualStyleBackColor = true;
            this.fromRevisionSelectButton.Click += new System.EventHandler(this.fromRevisionSelectButton_Click);
            // 
            // fromRevisionTextBox
            // 
            resources.ApplyResources(this.fromRevisionTextBox, "fromRevisionTextBox");
            this.fromRevisionTextBox.Name = "fromRevisionTextBox";
            this.fromRevisionTextBox.TextChanged += new System.EventHandler(this.fromRevisionTextBox_TextChanged);
            // 
            // fromRevisionRadioButton
            // 
            resources.ApplyResources(this.fromRevisionRadioButton, "fromRevisionRadioButton");
            this.fromRevisionRadioButton.Checked = true;
            this.fromRevisionRadioButton.Name = "fromRevisionRadioButton";
            this.fromRevisionRadioButton.TabStop = true;
            this.fromRevisionRadioButton.UseVisualStyleBackColor = true;
            this.fromRevisionRadioButton.CheckedChanged += new System.EventHandler(this.fromRevisionRadioButton_CheckedChanged);
            // 
            // fromHEADRevisionRadioButton
            // 
            resources.ApplyResources(this.fromHEADRevisionRadioButton, "fromHEADRevisionRadioButton");
            this.fromHEADRevisionRadioButton.Name = "fromHEADRevisionRadioButton";
            this.fromHEADRevisionRadioButton.UseVisualStyleBackColor = true;
            this.fromHEADRevisionRadioButton.CheckedChanged += new System.EventHandler(this.fromHeadRevisionRadioButton_CheckedChanged);
            // 
            // fromURLTextBox
            // 
            resources.ApplyResources(this.fromURLTextBox, "fromURLTextBox");
            this.fromURLTextBox.Name = "fromURLTextBox";
            this.fromURLTextBox.TextChanged += new System.EventHandler(this.fromURLTextBox_TextChanged);
            // 
            // fromURLSelectButton
            // 
            resources.ApplyResources(this.fromURLSelectButton, "fromURLSelectButton");
            this.fromURLSelectButton.Name = "fromURLSelectButton";
            this.fromURLSelectButton.UseVisualStyleBackColor = true;
            this.fromURLSelectButton.Click += new System.EventHandler(this.fromURLSelectButton_Click);
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.useFromURLCheckBox);
            this.groupBox1.Controls.Add(this.toRevisionSelectButton);
            this.groupBox1.Controls.Add(this.toRevisionTextBox);
            this.groupBox1.Controls.Add(this.toRevisionRadioButton);
            this.groupBox1.Controls.Add(this.toHEADRevisionRadioButton);
            this.groupBox1.Controls.Add(this.toURLTextBox);
            this.groupBox1.Controls.Add(this.toURLSelectButton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // useFromURLCheckBox
            // 
            resources.ApplyResources(this.useFromURLCheckBox, "useFromURLCheckBox");
            this.useFromURLCheckBox.Checked = true;
            this.useFromURLCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.useFromURLCheckBox.Name = "useFromURLCheckBox";
            this.useFromURLCheckBox.UseVisualStyleBackColor = true;
            this.useFromURLCheckBox.CheckedChanged += new System.EventHandler(this.useFromURLCheckBox_CheckedChanged);
            // 
            // toRevisionSelectButton
            // 
            resources.ApplyResources(this.toRevisionSelectButton, "toRevisionSelectButton");
            this.toRevisionSelectButton.Name = "toRevisionSelectButton";
            this.toRevisionSelectButton.UseVisualStyleBackColor = true;
            this.toRevisionSelectButton.Click += new System.EventHandler(this.toRevisionSelectButton_Click);
            // 
            // toRevisionTextBox
            // 
            resources.ApplyResources(this.toRevisionTextBox, "toRevisionTextBox");
            this.toRevisionTextBox.Name = "toRevisionTextBox";
            this.toRevisionTextBox.TextChanged += new System.EventHandler(this.toRevisionTextBox_TextChanged);
            // 
            // toRevisionRadioButton
            // 
            resources.ApplyResources(this.toRevisionRadioButton, "toRevisionRadioButton");
            this.toRevisionRadioButton.Checked = true;
            this.toRevisionRadioButton.Name = "toRevisionRadioButton";
            this.toRevisionRadioButton.TabStop = true;
            this.toRevisionRadioButton.UseVisualStyleBackColor = true;
            this.toRevisionRadioButton.CheckedChanged += new System.EventHandler(this.toRevisionRadioButton_CheckedChanged);
            // 
            // toHEADRevisionRadioButton
            // 
            resources.ApplyResources(this.toHEADRevisionRadioButton, "toHEADRevisionRadioButton");
            this.toHEADRevisionRadioButton.Name = "toHEADRevisionRadioButton";
            this.toHEADRevisionRadioButton.UseVisualStyleBackColor = true;
            this.toHEADRevisionRadioButton.CheckedChanged += new System.EventHandler(this.toHEADRevisionRadioButton_CheckedChanged);
            // 
            // toURLTextBox
            // 
            resources.ApplyResources(this.toURLTextBox, "toURLTextBox");
            this.toURLTextBox.Name = "toURLTextBox";
            this.toURLTextBox.TextChanged += new System.EventHandler(this.toURLTextBox_TextChanged);
            // 
            // toURLSelectButton
            // 
            resources.ApplyResources(this.toURLSelectButton, "toURLSelectButton");
            this.toURLSelectButton.Name = "toURLSelectButton";
            this.toURLSelectButton.UseVisualStyleBackColor = true;
            this.toURLSelectButton.Click += new System.EventHandler(this.toURLSelectButton_Click);
            // 
            // MergeSourceTwoDifferentTreesPageControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.fromGroupBox);
            this.Name = "MergeSourceTwoDifferentTreesPageControl";
            this.Load += new System.EventHandler(this.MergeSourceTwoDifferentTreesPageControl_Load);
            this.fromGroupBox.ResumeLayout(false);
            this.fromGroupBox.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox fromGroupBox;
        private System.Windows.Forms.Button fromURLSelectButton;
        private System.Windows.Forms.TextBox fromURLTextBox;
        private System.Windows.Forms.RadioButton fromHEADRevisionRadioButton;
        private System.Windows.Forms.RadioButton fromRevisionRadioButton;
        private System.Windows.Forms.TextBox fromRevisionTextBox;
        private System.Windows.Forms.Button fromRevisionSelectButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox useFromURLCheckBox;
        private System.Windows.Forms.Button toRevisionSelectButton;
        private System.Windows.Forms.TextBox toRevisionTextBox;
        private System.Windows.Forms.RadioButton toRevisionRadioButton;
        private System.Windows.Forms.RadioButton toHEADRevisionRadioButton;
        private System.Windows.Forms.TextBox toURLTextBox;
        private System.Windows.Forms.Button toURLSelectButton;
    }
}
