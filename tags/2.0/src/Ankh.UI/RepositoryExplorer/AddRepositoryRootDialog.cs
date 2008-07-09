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
        public Uri Uri
        {
            get
            {
                Uri uri;
                if (string.IsNullOrEmpty(urlTextBox.Text) || !Uri.TryCreate(urlTextBox.Text, UriKind.Absolute, out uri))
                    return null;

                return uri;
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

        private void ValidateAdd()
        {
            this.okButton.Enabled = this.revisionPicker.Valid &&
                Uri != null;
        }
    }
}
