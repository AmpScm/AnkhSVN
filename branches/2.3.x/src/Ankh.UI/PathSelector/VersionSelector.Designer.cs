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

using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.PathSelector
{
    partial class VersionSelector
    {

        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.typeCombo = new System.Windows.Forms.ComboBox();
            this.typeLabel = new System.Windows.Forms.Label();
            this.versionTypePanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // typeCombo
            // 
            this.typeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.typeCombo.FormattingEnabled = true;
            this.typeCombo.Location = new System.Drawing.Point(40, 2);
            this.typeCombo.Name = "typeCombo";
            this.typeCombo.Size = new System.Drawing.Size(111, 21);
            this.typeCombo.TabIndex = 1;
            this.typeCombo.SelectedValueChanged += new System.EventHandler(this.typeCombo_SelectedValueChanged);
            // 
            // typeLabel
            // 
            this.typeLabel.AutoSize = true;
            this.typeLabel.Location = new System.Drawing.Point(0, 5);
            this.typeLabel.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size(34, 13);
            this.typeLabel.TabIndex = 0;
            this.typeLabel.Text = "&Type:";
            // 
            // versionTypePanel
            // 
            this.versionTypePanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.versionTypePanel.Location = new System.Drawing.Point(157, 0);
            this.versionTypePanel.Name = "versionTypePanel";
            this.versionTypePanel.Size = new System.Drawing.Size(263, 25);
            this.versionTypePanel.TabIndex = 2;
            // 
            // VersionSelector
            // 
            this.Controls.Add(this.versionTypePanel);
            this.Controls.Add(this.typeLabel);
            this.Controls.Add(this.typeCombo);
            this.Name = "VersionSelector";
            this.Size = new System.Drawing.Size(420, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private System.Windows.Forms.ComboBox typeCombo;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.Panel versionTypePanel;
    }
}
