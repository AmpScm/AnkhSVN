using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.Scc.UI;

namespace Ankh.UI.OptionsPages
{
    public partial class AddAdvancedDiffUserTool : VSDialogForm
    {
        private readonly ListView myListView;
        private ListViewItem myItem;

        public AddAdvancedDiffUserTool(ListView listview, IAnkhDiffHandler diff)
        {
            InitializeComponent();
            myListView = listview;
            LoadBox(diffExeBox, "", diff.DiffToolTemplates);
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (myItem == null)
            {
                ListViewItem myNewItem = new ListViewItem(extensionTextBox.Text);
                myNewItem.SubItems.Add((string)diffExeBox.Tag);
                myListView.Items.Add(myNewItem);
            }
            else
            {
                myItem.Text = extensionTextBox.Text;
                myItem.SubItems[1].Text = diffExeBox.Text;
            }

            this.Close();
        }    

        public void SetItem(ListViewItem myInputItem)
        {
            myItem = myInputItem;
            extensionTextBox.Text = myItem.Text;
            diffExeBox.Text = myItem.SubItems[1].Text;
        }

        sealed class OtherTool
        {
            readonly string _title;
            public OtherTool(string title)
            {
                _title = string.IsNullOrEmpty(title) ? "Other..." : title;
            }
            public override string ToString()
            {
                return _title;
            }

            public string Title
            {
                get { return _title; }
            }

            public string DisplayName
            {
                get { return Title; }
            }
        }

        static void LoadBox(ComboBox combo, string value, IEnumerable<AnkhDiffTool> tools)
        {
            if (combo == null)
                throw new ArgumentNullException("combo");

            combo.DropDownStyle = ComboBoxStyle.DropDown;
            combo.Items.Clear();

            string selectedName = string.IsNullOrEmpty(value) ? null : AnkhDiffTool.GetToolNameFromTemplate(value);
            bool search = !string.IsNullOrEmpty(selectedName);
            bool found = false;
            foreach (AnkhDiffTool tool in tools)
            {
                // Items are presorted
                combo.Items.Add(tool);

                if (search && string.Equals(tool.Name, selectedName, StringComparison.OrdinalIgnoreCase))
                {
                    search = false;
                    found = true;
                    combo.DropDownStyle = ComboBoxStyle.DropDownList;
                    combo.SelectedItem = tool;
                }
            }

            combo.Items.Add(new OtherTool(null));

            if (!found)
            {
                combo.SelectedItem = null;
                combo.Text = value ?? "";
            }
        }

        void BrowseCombo(ComboBox box)
        {
            string line;
            if (box.SelectedItem is AnkhDiffTool tool)
            {
                line = string.Format("\"{0}\" {1}", tool.Program, tool.Arguments);
            }
            else
                line = box.Text;

            using (ToolArgumentDialog dlg = new ToolArgumentDialog())
            {
                dlg.Value = line;
                dlg.SetTemplates(Context.GetService<IAnkhDiffHandler>().ArgumentDefinitions);

                if (DialogResult.OK == dlg.ShowDialog(Context))
                {
                    string newValue = dlg.Value;

                    if (!string.IsNullOrEmpty(newValue) && newValue != line)
                    {
                        box.DropDownStyle = ComboBoxStyle.DropDown;
                        box.SelectedItem = null;
                        box.Text = newValue;
                    }
                }
            }
        }

        private void DiffExeBox_TextChanged(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            if (box.DropDownStyle == ComboBoxStyle.DropDown)
            {
                if (box.SelectedItem == null && !string.IsNullOrEmpty(box.Text))
                {
                    box.Tag = box.Text;
                }
            }
        }

        private void Tool_selectionCommitted(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            if (box.SelectedItem is AnkhDiffTool tool)
            {
                box.DropDownStyle = ComboBoxStyle.DropDownList;
                box.Tag = string.Format("\"{0}\" {1}", tool.Program, tool.Arguments);
                box.Text = (string)box.Tag;

            }
            else
            {
                box.DropDownStyle = ComboBoxStyle.DropDown;
                if (box.SelectedItem != null)
                    box.SelectedItem = null;
                if (box.Tag is string text)
                    box.Text = text;
                else if (box.Tag is AnkhDiffTool tool1)
                    box.Text = tool1.ToolTemplate;

                if (!string.IsNullOrEmpty(box.Text))
                {
                    box.SelectionStart = 0;
                    box.SelectionLength = box.Text.Length;
                }
            }
        }

        private void DiffBrowseBtn_Click(object sender, EventArgs e)
        {
            BrowseCombo(diffExeBox);
        }

    }
}
