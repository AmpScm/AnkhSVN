using System;
using System.Text;
using System.Windows.Forms;
using Utils.Win32;
using System.IO;

namespace Ankh.UI
{
    public partial class AddWorkingCopyExplorerRootDialog : Form
    {


        public AddWorkingCopyExplorerRootDialog()
        {
            this.InitializeComponent();

            Win32.SHAutoComplete(this.workingCopyRootTextBox.Handle,
                Shacf.Filesystem);
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
            if (folderBrowser.ShowDialog(this) == DialogResult.OK)
            {
                this.workingCopyRootTextBox.Text = folderBrowser.DirectoryPath;
            }
        }
    }
}
