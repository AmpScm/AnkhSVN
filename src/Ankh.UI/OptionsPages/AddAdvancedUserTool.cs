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
    public partial class AddAdvancedUserTool : VSDialogForm
    {
        private ListView myListView;
        private ListViewItem myItem;

        public AddAdvancedUserTool(ListView listview)
        {
            InitializeComponent();
            myListView = listview;
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
                myNewItem.SubItems.Add(commandTextBox.Text);
                myListView.Items.Add(myNewItem);
            }
            else
            {
                myItem.Text = extensionTextBox.Text;
                myItem.SubItems[1].Text = commandTextBox.Text;
            }

            this.Close();
        }    

        public void setItem(ListViewItem myInputItem)
        {
            myItem = myInputItem;
            extensionTextBox.Text = myItem.Text;
            commandTextBox.Text = myItem.SubItems[1].Text;
        }
    }
}
