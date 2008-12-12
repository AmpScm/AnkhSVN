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

namespace Ankh.UI
{
    partial class MultiLineStringEditorWithTemplates
    {
        private void InitializeComponent()
        {
            System.Windows.Forms.Label label1;
            label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.Size = new System.Drawing.Size(524, 157);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(label1);
            this.panel1.Location = new System.Drawing.Point(0, 157);
            this.panel1.Size = new System.Drawing.Size(524, 29);
            this.panel1.Controls.SetChildIndex(label1, 0);
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(12, 8);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(178, 13);
            label1.TabIndex = 2;
            label1.Text = "Insert a template by pressing Ctrl-T";
            // 
            // MultiLineStringEditorWithTemplates
            // 
            this.ClientSize = new System.Drawing.Size(524, 186);
            this.KeyPreview = true;
            this.Name = "MultiLineStringEditorWithTemplates";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MultiLineStringEditorWithTemplates_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
