using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Utils;
using Utils.Win32;
using SharpSvn;
using Utils.Services;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for performing exports.
    /// </summary>
    public partial class ExportDialog : System.Windows.Forms.Form
    {
        public ExportDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            Win32.SHAutoComplete(this.urlTextBox.Handle,
                Shacf.UrlAll);
            Win32.SHAutoComplete(this.localDirTextBox.Handle,
                Shacf.Filesystem);
            Win32.SHAutoComplete(this.exportFromDirTextBox.Handle,
                Shacf.Filesystem);

            this.ControlsChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// The URL of the repository.
        /// </summary>
        public string Source
        {
            get
            {
                if (this.radioButtonFromURL.Checked)
                    return this.urlTextBox.Text;
                else
                    return this.exportFromDirTextBox.Text;
            }
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
        /// Whether to perform a non-recursive export.
        /// </summary>
        public bool NonRecursive
        {
            get { return this.nonRecursiveCheckBox.Checked; }
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
            IWorkingCopyOperations wcOps = AnkhServices.GetService<IWorkingCopyOperations>();
            if (this.revisionPicker.Valid && this.localDirTextBox.Text.Length > 0)
            {
                if (this.radioButtonFromURL.Checked)
                    this.okButton.Enabled = UriUtils.ValidUrl.IsMatch(this.urlTextBox.Text);
                else
                    this.okButton.Enabled = wcOps.IsWorkingCopyPath(this.exportFromDirTextBox.Text);
            }
        }

        /// <summary>
        ///   User clicked radio button to export from a dir
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonFromDir_CheckedChanged(object sender, System.EventArgs e)
        {
            this.urlGroupBox.Visible = false;
            this.exportFromDirGroupbox.Visible = true;

        }// <summary>
        ///   User clicked radio button to export from a URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonFromURL_CheckedChanged(object sender, System.EventArgs e)
        {

            this.urlGroupBox.Visible = true;
            this.exportFromDirGroupbox.Visible = false;
        }

        /// <summary>
        /// Let the user browse for a directory to export from
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportFromDirButton_Click(object sender, System.EventArgs e)
        {
            using (FolderBrowser browser = new FolderBrowser())
            {
                if (browser.ShowDialog(this) == DialogResult.OK)
                {
                    this.exportFromDirTextBox.Text = browser.DirectoryPath;
                }
            }

        }

        /// <summary>
        /// Let the user browse for a directory to export To.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BrowseClicked(object sender, System.EventArgs e)
        {
            using (FolderBrowser browser = new FolderBrowser())
            {
                if (browser.ShowDialog(this) == DialogResult.OK)
                {
                    this.localDirTextBox.Text = browser.DirectoryPath;
                }
            }
        }
    }
}
