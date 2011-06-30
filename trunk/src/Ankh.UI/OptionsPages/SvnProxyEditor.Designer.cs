// $Id$
//
// Copyright 2009 The AnkhSVN Project
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

namespace Ankh.UI.OptionsPages
{
    partial class SvnProxyEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SvnProxyEditor));
            this.proxyEnabled = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.proxyGroup = new System.Windows.Forms.GroupBox();
            this.exceptionsBox = new System.Windows.Forms.TextBox();
            this.passwordBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.usernameBox = new System.Windows.Forms.TextBox();
            this.portBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.hostBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.proxyGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // proxyEnabled
            // 
            resources.ApplyResources(this.proxyEnabled, "proxyEnabled");
            this.proxyEnabled.Name = "proxyEnabled";
            this.proxyEnabled.UseVisualStyleBackColor = true;
            this.proxyEnabled.CheckedChanged += new System.EventHandler(this.proxyEnabled_CheckedChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // proxyGroup
            // 
            resources.ApplyResources(this.proxyGroup, "proxyGroup");
            this.proxyGroup.Controls.Add(this.exceptionsBox);
            this.proxyGroup.Controls.Add(this.passwordBox);
            this.proxyGroup.Controls.Add(this.label6);
            this.proxyGroup.Controls.Add(this.label5);
            this.proxyGroup.Controls.Add(this.label4);
            this.proxyGroup.Controls.Add(this.usernameBox);
            this.proxyGroup.Controls.Add(this.portBox);
            this.proxyGroup.Controls.Add(this.label3);
            this.proxyGroup.Controls.Add(this.hostBox);
            this.proxyGroup.Controls.Add(this.label2);
            this.proxyGroup.Name = "proxyGroup";
            this.proxyGroup.TabStop = false;
            // 
            // exceptionsBox
            // 
            this.exceptionsBox.AcceptsReturn = true;
            resources.ApplyResources(this.exceptionsBox, "exceptionsBox");
            this.exceptionsBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.exceptionsBox.Name = "exceptionsBox";
            // 
            // passwordBox
            // 
            resources.ApplyResources(this.passwordBox, "passwordBox");
            this.passwordBox.Name = "passwordBox";
            this.passwordBox.UseSystemPasswordChar = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // usernameBox
            // 
            resources.ApplyResources(this.usernameBox, "usernameBox");
            this.usernameBox.Name = "usernameBox";
            // 
            // portBox
            // 
            resources.ApplyResources(this.portBox, "portBox");
            this.portBox.Name = "portBox";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // hostBox
            // 
            resources.ApplyResources(this.hostBox, "hostBox");
            this.hostBox.CharacterCasing = System.Windows.Forms.CharacterCasing.Lower;
            this.hostBox.Name = "hostBox";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Name = "okButton";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // SvnProxyEditor
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.cancelButton;
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.proxyGroup);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.proxyEnabled);
            this.Name = "SvnProxyEditor";
            this.proxyGroup.ResumeLayout(false);
            this.proxyGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox proxyEnabled;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox proxyGroup;
        private System.Windows.Forms.TextBox exceptionsBox;
        private System.Windows.Forms.TextBox passwordBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox usernameBox;
        private System.Windows.Forms.TextBox portBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox hostBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}