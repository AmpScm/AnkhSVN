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

namespace Ankh.UI.Commands
{
    partial class UpdateDialog
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UpdateDialog));
			this.projectRootLabel = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.projectRootBox = new System.Windows.Forms.TextBox();
			this.urlBox = new System.Windows.Forms.TextBox();
			this.versionBox = new Ankh.UI.PathSelector.VersionSelector();
			this.ignoreExternals = new System.Windows.Forms.CheckBox();
			this.allowObstructions = new System.Windows.Forms.CheckBox();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.makeDepthInfinity = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// projectRootLabel
			// 
			resources.ApplyResources(this.projectRootLabel, "projectRootLabel");
			this.projectRootLabel.Name = "projectRootLabel";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// projectRootBox
			// 
			resources.ApplyResources(this.projectRootBox, "projectRootBox");
			this.projectRootBox.Name = "projectRootBox";
			this.projectRootBox.ReadOnly = true;
			// 
			// urlBox
			// 
			resources.ApplyResources(this.urlBox, "urlBox");
			this.urlBox.Name = "urlBox";
			this.urlBox.ReadOnly = true;
			// 
			// versionBox
			// 
			resources.ApplyResources(this.versionBox, "versionBox");
			this.versionBox.Name = "versionBox";
			this.versionBox.SvnOrigin = null;
			// 
			// ignoreExternals
			// 
			resources.ApplyResources(this.ignoreExternals, "ignoreExternals");
			this.ignoreExternals.Name = "ignoreExternals";
			this.ignoreExternals.UseVisualStyleBackColor = true;
			// 
			// allowObstructions
			// 
			resources.ApplyResources(this.allowObstructions, "allowObstructions");
			this.allowObstructions.Name = "allowObstructions";
			this.allowObstructions.UseVisualStyleBackColor = true;
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
			// makeDepthInfinity
			// 
			resources.ApplyResources(this.makeDepthInfinity, "makeDepthInfinity");
			this.makeDepthInfinity.Name = "makeDepthInfinity";
			this.makeDepthInfinity.UseVisualStyleBackColor = true;
			// 
			// UpdateDialog
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.makeDepthInfinity);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.allowObstructions);
			this.Controls.Add(this.ignoreExternals);
			this.Controls.Add(this.versionBox);
			this.Controls.Add(this.urlBox);
			this.Controls.Add(this.projectRootBox);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.projectRootLabel);
			this.Name = "UpdateDialog";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label projectRootLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox projectRootBox;
        private System.Windows.Forms.TextBox urlBox;
        private Ankh.UI.PathSelector.VersionSelector versionBox;
        private System.Windows.Forms.CheckBox ignoreExternals;
        private System.Windows.Forms.CheckBox allowObstructions;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox makeDepthInfinity;
    }
}