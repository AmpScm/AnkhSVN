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
using System.Text;
using System.Windows.Forms;
using Ankh.Configuration;

namespace Ankh.UI.Commands
{
    public partial class RecentMessageDialog : VSContainerForm
    {
        ColumnHeader column;
        public RecentMessageDialog()
        {
            InitializeComponent();

            logMessageList.Columns.Clear();
            column = new ColumnHeader();
            logMessageList.Columns.Add(column);
            logMessageList.SizeChanged += new EventHandler(logMessageList_SizeChanged);
            SizeColumn();
        }

        void logMessageList_SizeChanged(object sender, EventArgs e)
        {
            SizeColumn();
        }

        private void SizeColumn()
        {
            column.Width = logMessageList.Width - SystemInformation.VerticalScrollBarWidth - 4;
        }

        public string SelectedText
        {
            get { return _selectedText; }
        }

        RegistryLifoList _items;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Context != null)
            {
                IAnkhConfigurationService config = Context.GetService<IAnkhConfigurationService>();

                _items = config.GetRecentLogMessages();

                logMessageList.Items.Clear();

                foreach (string i in _items)
                {
                    if (string.IsNullOrEmpty(i))
                        continue;

                    ListViewItem item = new ListViewItem();

                    item.Text = i.Trim().Replace("\r", "").Replace("\n", "\x23CE");
                    item.Tag = i;
                    logMessageList.Items.Add(i);
                }                
            }
        }

        string _selectedText;
        private void logMessageList_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            string text = "";

            if(logMessageList.SelectedItems.Count >= 1)
            {
                StringBuilder msg = new StringBuilder();
                bool next = false;
                foreach (ListViewItem item in logMessageList.SelectedItems)
                {
                    if(next)
                        msg.AppendLine();
                    else
                        next = true;
                    msg.Append(item.Text);
                }
                text = msg.ToString();
            }

            previewBox.Text = _selectedText = text;
        }
    }
}
