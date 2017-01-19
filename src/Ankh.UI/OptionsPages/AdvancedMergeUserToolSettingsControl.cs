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
    public partial class AdvancedMergeUserToolSettingsControl : AnkhOptionsPageControl
    {
        private List<ExtToolDefinition> mergeExePaths;

        public AdvancedMergeUserToolSettingsControl()
        {
            InitializeComponent();
        }

        protected override void LoadSettingsCore()
        {
            mergeExePaths = Config._mergeExePaths;
            listView.Items.Clear();
            
            foreach (ExtToolDefinition extToolDef in mergeExePaths)
            {
                ListViewItem myItem = new ListViewItem(extToolDef.extension);
                myItem.SubItems.Add(extToolDef.exePath);
                listView.Items.Add(myItem);
            }
        }

        protected override void SaveSettingsCore()
        {
            mergeExePaths.Clear();

            foreach (ListViewItem item in listView.Items)
            {
                ExtToolDefinition extToolDef = new ExtToolDefinition();
                extToolDef.extension = item.Text;
                extToolDef.exePath = item.SubItems[1].Text;

                mergeExePaths.Add(extToolDef);
            }
            Config._mergeExePaths = mergeExePaths;

        }

        private void addButton_Click(object sender, EventArgs e)
        {
            IAnkhDiffHandler diffHandler = Context.GetService<IAnkhDiffHandler>();

            using (AddAdvancedMergeUserTool myAddAdvancedMergeUserTool = new AddAdvancedMergeUserTool(listView, diffHandler))
            {
                myAddAdvancedMergeUserTool.ShowDialog(Context);
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            mergeExePaths.Remove(mergeExePaths.Find(extTool => extTool.extension.ToLower().Equals(listView.SelectedItems[0].Text.ToLower())));
            listView.SelectedItems[0].Remove();
        }

        private void modifyButton_Click(object sender, EventArgs e)
        {
            IAnkhDiffHandler diffHandler = Context.GetService<IAnkhDiffHandler>();

            using (AddAdvancedMergeUserTool myAddAdvancedUserTool = new AddAdvancedMergeUserTool(listView, diffHandler))
            {
                myAddAdvancedUserTool.setItem(listView.SelectedItems[0]);
                myAddAdvancedUserTool.ShowDialog(Context);
            }
        }
    }
}
