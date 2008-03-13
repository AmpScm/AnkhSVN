using System;
using System.ComponentModel;
using System.Windows.Forms;

using Utils;
using SharpSvn;
using Utils.Services;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for performing exports.
    /// </summary>
    public partial class ExportDialog : System.Windows.Forms.Form
    {
        readonly IAnkhServiceProvider _context;
        protected ExportDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.ControlsChanged(this, EventArgs.Empty);
        }

        
        public ExportDialog(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
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
            IWorkingCopyOperations wcOps = _context.GetService<IWorkingCopyOperations>();

            bool enable = false;
            if (wcOps != null)
            {
                if (this.revisionPicker.Valid && this.localDirTextBox.Text.Length > 0)
                {
                    if (this.radioButtonFromURL.Checked)
                        enable = UriUtils.ValidUrl.IsMatch(this.urlTextBox.Text);
                    else
                        enable = wcOps.IsWorkingCopyPath(this.exportFromDirTextBox.Text);
                }
            }

            this.okButton.Enabled = enable;
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
			using (FolderBrowserDialog browser = new FolderBrowserDialog())
            {
				browser.ShowNewFolderButton = false;

                if (browser.ShowDialog(this) == DialogResult.OK)
                {
                    this.exportFromDirTextBox.Text = browser.SelectedPath;
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
