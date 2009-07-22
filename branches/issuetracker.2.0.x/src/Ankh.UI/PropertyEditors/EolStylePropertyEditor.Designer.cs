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

using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.PropertyEditors
{
    partial class EolStylePropertyEditor
    {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.nativeRadioButton = new System.Windows.Forms.RadioButton();
            this.lfRadioButton = new System.Windows.Forms.RadioButton();
            this.crRadioButton = new System.Windows.Forms.RadioButton();
            this.crlfRdioButton = new System.Windows.Forms.RadioButton();
            this.eolStyleGroupBox = new System.Windows.Forms.GroupBox();
            this.conflictToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.eolStyleGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // nativeRadioButton
            // 
            this.nativeRadioButton.Checked = true;
            this.nativeRadioButton.Location = new System.Drawing.Point(6, 19);
            this.nativeRadioButton.Name = "nativeRadioButton";
            this.nativeRadioButton.Size = new System.Drawing.Size(241, 24);
            this.nativeRadioButton.TabIndex = 1;
            this.nativeRadioButton.TabStop = true;
            this.nativeRadioButton.Tag = "native";
            this.nativeRadioButton.Text = "&Native";
            this.nativeRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // lfRadioButton
            // 
            this.lfRadioButton.Location = new System.Drawing.Point(6, 49);
            this.lfRadioButton.Name = "lfRadioButton";
            this.lfRadioButton.Size = new System.Drawing.Size(241, 24);
            this.lfRadioButton.TabIndex = 3;
            this.lfRadioButton.Tag = "LF";
            this.lfRadioButton.Text = "&LF (Unix)";
            this.lfRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // crRadioButton
            // 
            this.crRadioButton.Location = new System.Drawing.Point(6, 79);
            this.crRadioButton.Name = "crRadioButton";
            this.crRadioButton.Size = new System.Drawing.Size(241, 24);
            this.crRadioButton.TabIndex = 4;
            this.crRadioButton.Tag = "CR";
            this.crRadioButton.Text = "&CR (MacOS)";
            this.crRadioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // crlfRdioButton
            // 
            this.crlfRdioButton.Location = new System.Drawing.Point(6, 109);
            this.crlfRdioButton.Name = "crlfRdioButton";
            this.crlfRdioButton.Size = new System.Drawing.Size(241, 24);
            this.crlfRdioButton.TabIndex = 5;
            this.crlfRdioButton.Tag = "CRLF";
            this.crlfRdioButton.Text = "CRLF (Windows)";
            this.crlfRdioButton.CheckedChanged += new System.EventHandler(this.RadioButton_CheckedChanged);
            // 
            // eolStyleGroupBox
            // 
            this.eolStyleGroupBox.Controls.Add(this.nativeRadioButton);
            this.eolStyleGroupBox.Controls.Add(this.crlfRdioButton);
            this.eolStyleGroupBox.Controls.Add(this.lfRadioButton);
            this.eolStyleGroupBox.Controls.Add(this.crRadioButton);
            this.eolStyleGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.eolStyleGroupBox.Location = new System.Drawing.Point(0, 0);
            this.eolStyleGroupBox.Name = "eolStyleGroupBox";
            this.eolStyleGroupBox.Size = new System.Drawing.Size(348, 196);
            this.eolStyleGroupBox.TabIndex = 6;
            this.eolStyleGroupBox.TabStop = false;
            this.eolStyleGroupBox.Text = "Select eol-style";
            // 
            // EolStylePropertyEditor
            // 
            this.Controls.Add(this.eolStyleGroupBox);
            this.Name = "EolStylePropertyEditor";
            this.eolStyleGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.RadioButton nativeRadioButton;
        private System.Windows.Forms.RadioButton lfRadioButton;
        private System.Windows.Forms.RadioButton crRadioButton;
        private System.Windows.Forms.RadioButton crlfRdioButton;
        private System.Windows.Forms.GroupBox eolStyleGroupBox;
        private System.Windows.Forms.ToolTip conflictToolTip;
        private System.ComponentModel.IContainer components;
    }
}
