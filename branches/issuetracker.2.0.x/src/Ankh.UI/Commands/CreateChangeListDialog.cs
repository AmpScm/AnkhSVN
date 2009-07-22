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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI.Commands
{
    public partial class CreateChangeListDialog : VSDialogForm
    {
        public CreateChangeListDialog()
        {
            InitializeComponent();
        }

        private void nameBox_TextChanged(object sender, EventArgs e)
        {
            okButton.Enabled = !string.IsNullOrEmpty(nameBox.Text);
        }

        RegistryLifoList _ll;

        RegistryLifoList RecentList
        {
            get { return _ll ?? ((Context != null) ? (_ll = new RegistryLifoList(Context, "RecentChangeListNames", 32)) : null); }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode && RecentList != null)
            {
                foreach (string txt in RecentList)
                {
                    if(!nameBox.Items.Contains(txt))
                        nameBox.Items.Add(txt);
                }

                if (!nameBox.Items.Contains("ignore-on-commit"))
                    nameBox.Items.Add("ignore-on-commit");
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (RecentList != null && !string.IsNullOrEmpty(nameBox.Text))
            {
                RecentList.Add(nameBox.Text);
            }            
        }

        public string ChangeListName
        {
            get { return nameBox.Text; }
        }
    }
}
