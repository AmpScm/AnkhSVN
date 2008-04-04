using System;
using System.ComponentModel;
using System.Windows.Forms;

using Utils;
using SharpSvn;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for performing checkouts.
    /// </summary>
    public partial class CheckoutDialog : System.Windows.Forms.Form
    {
        public CheckoutDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.ControlsChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// The URL of the repository.
        /// </summary>
        public string Url
        {
            get { return this.urlTextBox.Text; }
            set { this.urlTextBox.Text = value; }
        }

        /// <summary>
        /// The local path to check out to.
        /// </summary>
        public string LocalPath
        {
            get { return this.localDirTextBox.Text; }
            set { this.localDirTextBox.Text = value; }
        }

        /// <summary>
        /// The revision to check out.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnRevision Revision
        {
            get { return this.revisionPicker.Revision; }
        }

        /// <summary>
        /// Whether to perform a non-recursive checkout.
        /// </summary>
        public bool Recursive
        {
            get { return !this.nonRecursiveCheckBox.Checked; }
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


        /// <summary>
        /// Validate the input here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlsChanged(object sender, System.EventArgs e)
        {
            this.okButton.Enabled = this.revisionPicker.Valid &&
                UriUtils.IsValidUrl(this.urlTextBox.Text) &&
                this.localDirTextBox.Text.Length > 0;
        }

        /// <summary>
        /// Let the user browse for a directory.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseClicked(object sender, System.EventArgs e)
        {
			using (FolderBrowserDialog browser = new FolderBrowserDialog())
            {
                if (browser.ShowDialog(this) == DialogResult.OK)
                {
                    this.localDirTextBox.Text = browser.SelectedPath;
                }
            }
        }
    }
}
