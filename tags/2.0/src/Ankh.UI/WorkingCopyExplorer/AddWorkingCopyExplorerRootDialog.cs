using System;
using System.Windows.Forms;
using System.IO;

namespace Ankh.UI
{
    public partial class AddWorkingCopyExplorerRootDialog : Form
    {


        public AddWorkingCopyExplorerRootDialog()
        {
            this.InitializeComponent();
        }

        public string NewRoot
        {
            get { return this.workingCopyRootTextBox.Text; }
        }


        private void workingCopyRootTextBox_TextChanged(object sender, EventArgs e)
        {
            this.okButton.Enabled = Directory.Exists(this.workingCopyRootTextBox.Text);
        }

        private void browseFolderButton_Click(object sender, EventArgs e)
        {
			using (FolderBrowserDialog folderBrowser = new FolderBrowserDialog())
			{
				folderBrowser.ShowNewFolderButton = false;

				if (folderBrowser.ShowDialog(this) == DialogResult.OK)
				{
					this.workingCopyRootTextBox.Text = folderBrowser.SelectedPath;
				}
			}
        }
    }
}
