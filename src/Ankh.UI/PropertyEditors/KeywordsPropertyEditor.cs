// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using SharpSvn;
using System.Text;

namespace Ankh.UI.PropertyEditors
{
    /// <summary>
    /// Property editor for keywords.
    /// </summary>
    partial class KeywordsPropertyEditor : PropertyEditControl
    {
        public KeywordsPropertyEditor()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();

            keywordList.Items.Clear();
            foreach (string word in SvnKeywords.PredefinedKeywords)
            {
                keywordList.Items.Add(word);
            }
        }

        /// <summary>
        /// Indicates whether the selection is valid.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),Browsable(false)]
        public override bool Valid
        {
            get
            {
                if (!this.dirty)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
        /// <summary>
        /// Sets and gets the property item.
        /// </summary>
        public override SvnPropertyValue PropertyItem
        {
            get
            {
                string selectedText = "";
                StringBuilder sb = new StringBuilder();

                for(int i = 0; i <keywordList.Items.Count; i++)
                {
                    if(keywordList.GetItemChecked(i))
                    {
                        if(sb.Length > 0)
                            sb.Append(" ");
                        sb.Append(keywordList.Items[i].ToString());
                    }
                }

                selectedText = sb.ToString();

                return new SvnPropertyValue(SvnPropertyNames.SvnKeywords, selectedText);
            }

            set
            {
                string text = "";

                if (value != null)
                    text = value.StringValue;

                for (int i = 0; i < keywordList.Items.Count; i++)
                    keywordList.SetItemChecked(i, false);

                for (int i = 0; i < text.Length; i++)
                {
                    if (char.IsWhiteSpace(text, i) && text[i] != ' ')
                        text = text.Replace(text[i], ' '); // Replace all possible whitespace to spaces
                }

                foreach (string s in text.Split(' '))
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        int n = keywordList.Items.IndexOf(s);

                        if (n >= 0)
                            keywordList.SetItemChecked(n, true);
                    }
                }

                this.dirty = false;
            }
        }

        /// <summary>
        /// The type of property for this editor.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SvnPropertyNames.SvnKeywords;
        }

        public override bool AllowNodeKind(SvnNodeKind kind)
        {
            return kind == SvnNodeKind.File;
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }


        /// <summary>
        /// Dispatches the Changed event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            this.dirty = true;

            OnChanged(EventArgs.Empty);
        }

        /// <summary>
        /// Flag for enabling/disabling save button
        /// </summary>
        private bool dirty;       
    }
}

