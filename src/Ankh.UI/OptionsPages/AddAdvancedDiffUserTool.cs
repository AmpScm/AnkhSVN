using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ankh.Scc.UI;

namespace Ankh.UI.OptionsPages
{
    public partial class AddAdvancedDiffUserTool : VSDialogForm
    {
        private ListView myListView;
        private ListViewItem myItem;

        public AddAdvancedDiffUserTool(ListView listview, IAnkhDiffHandler diff)
        {
            InitializeComponent();
            myListView = listview;
            LoadBox(diffExeBox, "", diff.DiffToolTemplates);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
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

        public void setItem(ListViewItem myInputItem)
        {
            myItem = myInputItem;
            extensionTextBox.Text = myItem.Text;
            diffExeBox.Text = myItem.SubItems[1].Text;
        }




        //protected override void LoadSettingsCore()
        //{
        //    IAnkhDiffHandler diff = Context.GetService<IAnkhDiffHandler>();

        //    LoadBox(diffExeBox, Config.DiffExePath, diff.DiffToolTemplates);
        //}

        sealed class OtherTool
        {
            string _title;
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


        //protected override void SaveSettingsCore()
        //{
        //    Config.DiffExePath = SaveBox(diffExeBox);
        //}

        static string SaveBox(ComboBox box)
        {
            if (box == null)
                throw new ArgumentNullException("box");

            AnkhDiffTool tool = box.SelectedItem as AnkhDiffTool;

            if (tool != null)
                return tool.ToolTemplate;

            return box.Text;
        }

        void BrowseCombo(ComboBox box)
        {
            AnkhDiffTool tool = box.SelectedItem as AnkhDiffTool;

            string line;
            if (tool != null)
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

        private void diffExeBox_TextChanged(object sender, EventArgs e)
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

        private void tool_selectionCommitted(object sender, EventArgs e)
        {
            ComboBox box = (ComboBox)sender;

            AnkhDiffTool tool = box.SelectedItem as AnkhDiffTool;

            if (tool != null)
            {
                box.DropDownStyle = ComboBoxStyle.DropDownList;
                //box.Tag = tool.ToolTemplate;
                box.Tag = string.Format("\"{0}\" {1}", tool.Program, tool.Arguments);
                box.Text = (string)box.Tag;

            }
            else
            {
                box.DropDownStyle = ComboBoxStyle.DropDown;
                if (box.SelectedItem != null)
                    box.SelectedItem = null;
                if (box.Tag is string)
                    box.Text = (string)box.Tag;
                else if (box.Tag is AnkhDiffTool)
                    box.Text = ((AnkhDiffTool)box.Tag).ToolTemplate;

                if (!string.IsNullOrEmpty(box.Text))
                {
                    box.SelectionStart = 0;
                    box.SelectionLength = box.Text.Length;
                }
            }
        }

        private void diffBrowseBtn_Click(object sender, EventArgs e)
        {
            BrowseCombo(diffExeBox);
        }

    }
}
