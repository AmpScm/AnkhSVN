// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Utils;
using SharpSvn;

namespace Ankh.UI
{
    /// <summary>
    /// Summary description for AddRepositoryDialog.
    /// </summary>
    public partial class AddRepositoryRootDialog : System.Windows.Forms.Form
    {
        public AddRepositoryRootDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //Set revision choices in combobox
            this.revisionPicker.WorkingEnabled = false;
            this.revisionPicker.BaseEnabled = false;
            this.revisionPicker.CommittedEnabled = false;
            this.revisionPicker.PreviousEnabled = false;

            this.ValidateAdd();
        }

        /// <summary>
        /// The revision selected by the user.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SvnRevision Revision
        {
            get{ return this.revisionPicker.Revision; }
        }

        /// <summary>
        /// The URL entered in the text box.
        /// </summary>
        public string Url
        {
            get
            {
                return this.urlTextBox.Text;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        private void revisionPicker_Changed(object sender, System.EventArgs e)
        {
            this.ValidateAdd();            
        }

        private void urlTextBox_TextChanged(object sender, System.EventArgs e)
        {
            this.ValidateAdd();
        }

        private void browseButton_Click(object sender, System.EventArgs e)
        {
            // Browse to a local repository
			using (FolderBrowserDialog browser = new FolderBrowserDialog())
			{
				browser.ShowNewFolderButton = false;

				//convert the returned directory path to a URL - for a local path URL no need for encoding
				if (browser.ShowDialog() == DialogResult.OK)
					urlTextBox.Text = "file:///" + browser.SelectedPath.Replace('\\', '/');
			}

        }

        private void ValidateAdd()
        {
            this.okButton.Enabled = this.revisionPicker.Valid &&
                UriUtils.IsValidUrl(this.urlTextBox.Text);
        }
    }
}
