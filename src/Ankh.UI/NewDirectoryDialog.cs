using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for entering the name of a new directory.
    /// </summary>
    public partial class NewDirectoryDialog : System.Windows.Forms.Form
    {


        public NewDirectoryDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public string DirectoryName
        {
            get { return this.newDirectoryTextBox.Text; }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void DirectoryTextChanged(object sender, System.EventArgs e)
        {
            this.okButton.Enabled = this.newDirectoryTextBox.Text != String.Empty;
        }
    }
}
