using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Ankh.UI
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
            column.Width = logMessageList.Width - SystemInformation.VerticalScrollBarWidth - 1;
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

                    item.Text = i;
                    item.Tag = i;
                    logMessageList.Items.Add(i);
                }                
            }
        }

        string _selectedText;
        private void logMessageList_SelectedIndexChanged(object sender, EventArgs e)
        {
            string text = "";

            if(logMessageList.SelectedItems.Count == 1)
            {
                ListViewItem item = logMessageList.SelectedItems[0];
                text = item.Text;
            }

            previewBox.Text = _selectedText = text;
        }
    }
}
