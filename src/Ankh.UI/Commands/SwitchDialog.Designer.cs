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

namespace Ankh.UI
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
            this.switchBox = new System.Windows.Forms.GroupBox();
            this.pathLabel = new System.Windows.Forms.Label();
            this.pathBox = new System.Windows.Forms.TextBox();
            this.toBox = new System.Windows.Forms.GroupBox();
            this.browseUrl = new System.Windows.Forms.Button();
            this.toUrlBox = new System.Windows.Forms.ComboBox();
            this.urlLabel = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.versionSelector = new Ankh.UI.PathSelector.VersionSelector();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.allowObstructions = new System.Windows.Forms.CheckBox();
            this.switchBox.SuspendLayout();
            this.toBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // switchBox
            // 
            this.switchBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.switchBox.Controls.Add(this.pathLabel);
            this.switchBox.Controls.Add(this.pathBox);
            this.switchBox.Location = new System.Drawing.Point(12, 12);
            this.switchBox.Name = "switchBox";
            this.switchBox.Size = new System.Drawing.Size(515, 52);
            this.switchBox.TabIndex = 4;
            this.switchBox.TabStop = false;
            this.switchBox.Text = "&Switch:";
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Location = new System.Drawing.Point(6, 22);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(32, 13);
            this.pathLabel.TabIndex = 0;
            this.pathLabel.Text = "&Path:";
            // 
            // pathBox
            // 
            this.pathBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pathBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pathBox.Location = new System.Drawing.Point(52, 22);
            this.pathBox.Name = "pathBox";
            this.pathBox.ReadOnly = true;
            this.pathBox.Size = new System.Drawing.Size(457, 13);
            this.pathBox.TabIndex = 1;
            // 
            // toBox
            // 
            this.toBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toBox.Controls.Add(this.versionSelector);
            this.toBox.Controls.Add(this.browseUrl);
            this.toBox.Controls.Add(this.toUrlBox);
            this.toBox.Controls.Add(this.urlLabel);
            this.toBox.Location = new System.Drawing.Point(12, 70);
            this.toBox.Name = "toBox";
            this.toBox.Size = new System.Drawing.Size(515, 82);
            this.toBox.TabIndex = 0;
            this.toBox.TabStop = false;
            this.toBox.Text = "&To:";
            // 
            // browseUrl
            // 
            this.browseUrl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseUrl.Location = new System.Drawing.Point(469, 19);
            this.browseUrl.Name = "browseUrl";
            this.browseUrl.Size = new System.Drawing.Size(40, 23);
            this.browseUrl.TabIndex = 2;
            this.browseUrl.Text = "...";
            this.browseUrl.UseVisualStyleBackColor = true;
            this.browseUrl.Click += new System.EventHandler(this.browseUrl_Click);
            // 
            // toUrlBox
            // 
            this.toUrlBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.toUrlBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.toUrlBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.AllUrl;
            this.toUrlBox.FormattingEnabled = true;
            this.toUrlBox.Location = new System.Drawing.Point(52, 19);
            this.toUrlBox.Name = "toUrlBox";
            this.toUrlBox.Size = new System.Drawing.Size(411, 21);
            this.toUrlBox.TabIndex = 1;
            this.toUrlBox.Validating += new System.ComponentModel.CancelEventHandler(this.toUrlBox_Validating);
            this.toUrlBox.TextChanged += new System.EventHandler(this.toUrlBox_TextChanged);
            // 
            // urlLabel
            // 
            this.urlLabel.AutoSize = true;
            this.urlLabel.Location = new System.Drawing.Point(6, 22);
            this.urlLabel.Name = "urlLabel";
            this.urlLabel.Size = new System.Drawing.Size(23, 13);
            this.urlLabel.TabIndex = 0;
            this.urlLabel.Text = "&Url:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.CausesValidation = false;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(452, 186);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(371, 186);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // versionSelector
            // 
            this.versionSelector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.versionSelector.Location = new System.Drawing.Point(52, 46);
            this.versionSelector.Name = "versionSelector";
            this.versionSelector.Size = new System.Drawing.Size(457, 26);
            this.versionSelector.SvnOrigin = null;
            this.versionSelector.TabIndex = 3;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // allowObstructions
            // 
            this.allowObstructions.AutoSize = true;
            this.allowObstructions.Location = new System.Drawing.Point(21, 158);
            this.allowObstructions.Name = "allowObstructions";
            this.allowObstructions.Size = new System.Drawing.Size(176, 17);
            this.allowObstructions.TabIndex = 1;
            this.allowObstructions.Text = "Allow Unversioned O&bstructions";
            this.allowObstructions.UseVisualStyleBackColor = true;
            // 
            // SwitchDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(539, 215);
            this.Controls.Add(this.allowObstructions);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.toBox);
            this.Controls.Add(this.switchBox);
            this.Name = "SwitchDialog";
            this.Text = "Switch To";
            this.switchBox.ResumeLayout(false);
            this.switchBox.PerformLayout();
            this.toBox.ResumeLayout(false);
            this.toBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
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
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private Ankh.UI.PathSelector.VersionSelector versionSelector;
        private System.Windows.Forms.CheckBox allowObstructions;
    }
}
