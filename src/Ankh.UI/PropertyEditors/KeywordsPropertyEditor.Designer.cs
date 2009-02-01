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
    partial class KeywordsPropertyEditor
    {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.keywordList = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // keywordList
            // 
            this.keywordList.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.keywordList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keywordList.FormattingEnabled = true;
            this.keywordList.Items.AddRange(new object[] {
            "Id"});
            this.keywordList.Location = new System.Drawing.Point(0, 0);
            this.keywordList.Name = "keywordList";
            this.keywordList.Size = new System.Drawing.Size(348, 182);
            this.keywordList.Sorted = true;
            this.keywordList.TabIndex = 1;
            this.keywordList.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck);
            // 
            // KeywordsPropertyEditor
            // 
            this.Controls.Add(this.keywordList);
            this.Name = "KeywordsPropertyEditor";
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.CheckedListBox keywordList;
    }
}
