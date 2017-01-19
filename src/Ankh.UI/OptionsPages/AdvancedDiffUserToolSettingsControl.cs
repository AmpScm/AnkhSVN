using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ankh.Configuration;
using Ankh.Scc.UI;


namespace Ankh.UI.OptionsPages
{
    public partial class AdvancedDiffUserToolSettingsControl : AnkhOptionsPageControl
    {
        private List<ExtToolDefinition> diffExePaths;

        public AdvancedDiffUserToolSettingsControl()
        {
            InitializeComponent();
        }

        protected override void LoadSettingsCore()
        {
            diffExePaths = Config._diffExePaths;
            listView.Items.Clear();

            foreach (ExtToolDefinition extToolDef in diffExePaths)
            {
                ListViewItem myItem = new ListViewItem(extToolDef.extension);
                myItem.SubItems.Add(extToolDef.exePath);
                listView.Items.Add(myItem);
            }
        }

        protected override void SaveSettingsCore()
        {
            diffExePaths.Clear();

            foreach (ListViewItem item in listView.Items)
            {
                ExtToolDefinition extToolDef = new ExtToolDefinition();
                extToolDef.extension = item.Text;
                extToolDef.exePath = item.SubItems[1].Text;

                diffExePaths.Add(extToolDef);
            }

            Config._diffExePaths = diffExePaths;
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            IAnkhDiffHandler diffHandler = Context.GetService<IAnkhDiffHandler>();

            using (AddAdvancedDiffUserTool myAddAdvancedDiffUserTool = new AddAdvancedDiffUserTool(listView, diffHandler))
            {
                myAddAdvancedDiffUserTool.ShowDialog(Context);
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            diffExePaths.Remove(diffExePaths.Find(extTool => extTool.extension.ToLower().Equals(listView.SelectedItems[0].Text.ToLower())));
            listView.SelectedItems[0].Remove();
        }

        private void modifyButton_Click(object sender, EventArgs e)
        {
            IAnkhDiffHandler diffHandler = Context.GetService<IAnkhDiffHandler>();

            using (AddAdvancedDiffUserTool myAddAdvancedUserTool = new AddAdvancedDiffUserTool(listView, diffHandler))
            {
                myAddAdvancedUserTool.setItem(listView.SelectedItems[0]);
                myAddAdvancedUserTool.ShowDialog(Context);
            }
        }
    }
}
