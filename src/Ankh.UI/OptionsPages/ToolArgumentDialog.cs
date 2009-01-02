// $Id$
//
// Copyright 2006-2008 The AnkhSVN Project
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
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using Ankh.Scc.UI;

namespace Ankh.UI.OptionsPages
{
    /// <summary>
    /// A dialog for use in a type editor for a string. Presents a dialog for editing the string.
    /// </summary>
    public partial class ToolArgumentDialog : VSDialogForm
    {
        public ToolArgumentDialog()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The value entered in the dialog.
        /// </summary>
        public string Value
        {
            get
            {
                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
            }
        }

        public void SetTemplates(IList<AnkhDiffArgumentDefinition> templates)
        {
            macroView.Items.Clear();
            foreach (AnkhDiffArgumentDefinition d in templates)
            {
                ListViewItem li = new ListViewItem(
                    new string[]
                    {
                        d.Key,
                        d.Description,
                        string.Join(", ", d.Aliases)
                    });

                macroView.Items.Add(li);
            }            
        }

        private void macroView_DoubleClick(object sender, EventArgs e)
        {
            ListViewHitTestInfo hti = macroView.HitTest(macroView.PointToClient(MousePosition));

            if (hti.Location != ListViewHitTestLocations.None && hti.Item != null)
            {
                PasteItem(hti.Item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (macroView.SelectedItems.Count == 1)
            {
                PasteItem(macroView.SelectedItems[0]);
            }
        }

        private void PasteItem(ListViewItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            textBox.SelectionLength = 0;
            textBox.SelectedText = string.Format("$({0})", item.Text);
            textBox.SelectionStart += textBox.SelectionLength;
            textBox.SelectionLength = 0;
        }        
    }
}
