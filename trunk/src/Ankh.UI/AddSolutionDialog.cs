using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for adding a solution to the repository.
    /// </summary>
    public partial class AddSolutionDialog : System.Windows.Forms.Form
    {
        public AddSolutionDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        /// <summary>
        /// The base URL.
        /// </summary>
        public string BaseUrl
        {
            get { return this.urlTextBox.Text; }
            set { this.urlTextBox.Text = value; }
        }

        /// <summary>
        /// Whether to create a subdirectory of the base URL.
        /// </summary>
        public bool CreateSubDirectory
        {
            get { return this.createSubDirCheckBox.Checked; }
        }

        /// <summary>
        /// The name of the sub directory name to optionally create.
        /// </summary>
        public string SubDirectoryName
        {
            get { return this.subDirNameTextBox.Text; }
        }

        /// <summary>
        /// The log message to be used when creating the subdirectory.
        /// </summary>
        public string LogMessage
        {
            get { return this.logMessageTextBox.Text; }
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

    }
}
