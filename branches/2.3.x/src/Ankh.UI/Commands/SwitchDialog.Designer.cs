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

namespace Ankh.UI.Commands
{
    partial class SwitchDialog
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SwitchDialog));
			this.switchBox = new System.Windows.Forms.GroupBox();
			this.pathLabel = new System.Windows.Forms.Label();
			this.pathBox = new System.Windows.Forms.TextBox();
			this.toBox = new System.Windows.Forms.GroupBox();
			this.versionSelector = new Ankh.UI.PathSelector.VersionSelector();
			this.browseUrl = new System.Windows.Forms.Button();
			this.toUrlBox = new System.Windows.Forms.ComboBox();
			this.urlLabel = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.allowObstructions = new System.Windows.Forms.CheckBox();
			this.switchBox.SuspendLayout();
			this.toBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// switchBox
			// 
			resources.ApplyResources(this.switchBox, "switchBox");
			this.switchBox.Controls.Add(this.pathLabel);
			this.switchBox.Controls.Add(this.pathBox);
			this.switchBox.Name = "switchBox";
			this.switchBox.TabStop = false;
			// 
			// pathLabel
			// 
			resources.ApplyResources(this.pathLabel, "pathLabel");
			this.pathLabel.Name = "pathLabel";
			// 
			// pathBox
			// 
			resources.ApplyResources(this.pathBox, "pathBox");
			this.pathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.pathBox.Name = "pathBox";
			this.pathBox.ReadOnly = true;
			// 
			// toBox
			// 
			resources.ApplyResources(this.toBox, "toBox");
			this.toBox.Controls.Add(this.versionSelector);
			this.toBox.Controls.Add(this.browseUrl);
			this.toBox.Controls.Add(this.toUrlBox);
			this.toBox.Controls.Add(this.urlLabel);
			this.toBox.Name = "toBox";
			this.toBox.TabStop = false;
			// 
			// versionSelector
			// 
			resources.ApplyResources(this.versionSelector, "versionSelector");
			this.versionSelector.Name = "versionSelector";
			this.versionSelector.SvnOrigin = null;
			// 
			// browseUrl
			// 
			resources.ApplyResources(this.browseUrl, "browseUrl");
			this.browseUrl.CausesValidation = false;
			this.browseUrl.Name = "browseUrl";
			this.browseUrl.UseVisualStyleBackColor = true;
			this.browseUrl.Click += new System.EventHandler(this.browseUrl_Click);
			// 
			// toUrlBox
			// 
			resources.ApplyResources(this.toUrlBox, "toUrlBox");
			this.toUrlBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
			this.toUrlBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
			this.toUrlBox.FormattingEnabled = true;
			this.toUrlBox.Name = "toUrlBox";
			this.toUrlBox.Validating += new System.ComponentModel.CancelEventHandler(this.toUrlBox_Validating);
			this.toUrlBox.TextChanged += new System.EventHandler(this.toUrlBox_TextChanged);
			// 
			// urlLabel
			// 
			resources.ApplyResources(this.urlLabel, "urlLabel");
			this.urlLabel.Name = "urlLabel";
			// 
			// cancelButton
			// 
			resources.ApplyResources(this.cancelButton, "cancelButton");
			this.cancelButton.CausesValidation = false;
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
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// allowObstructions
			// 
			resources.ApplyResources(this.allowObstructions, "allowObstructions");
			this.allowObstructions.Name = "allowObstructions";
			this.allowObstructions.UseVisualStyleBackColor = true;
			// 
			// SwitchDialog
			// 
			this.AcceptButton = this.okButton;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.Controls.Add(this.allowObstructions);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.toBox);
			this.Controls.Add(this.switchBox);
			this.Name = "SwitchDialog";
			this.switchBox.ResumeLayout(false);
			this.switchBox.PerformLayout();
			this.toBox.ResumeLayout(false);
			this.toBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox switchBox;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.TextBox pathBox;
        private System.Windows.Forms.GroupBox toBox;
        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.Button browseUrl;
        private System.Windows.Forms.ComboBox toUrlBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private Ankh.UI.PathSelector.VersionSelector versionSelector;
        private System.Windows.Forms.CheckBox allowObstructions;
    }
}
