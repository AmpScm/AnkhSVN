using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.RepositoryExplorer;

namespace Ankh.UI
{
    public partial class SwitchDialog : VSContainerForm
    {
        public SwitchDialog()
        {
            InitializeComponent();
        }

        private void browseUrl_Click(object sender, EventArgs e)
        {
            using (RepositoryFolderBrowserDialog dlg = new RepositoryFolderBrowserDialog())
            {
                dlg.SelectedUri = SwitchToUri;
                if (dlg.ShowDialog(Context) == DialogResult.OK)
                {
                    if (dlg.SelectedUri != null)
                        SwitchToUri = dlg.SelectedUri;
                }
            }
        }

        /// <summary>
        /// Gets or sets the local path.
        /// </summary>
        /// <value>The local path.</value>
        public string LocalPath
        {
            get { return pathBox.Text; }
            set { pathBox.Text = value; }
        }

        /// <summary>
        /// Gets or sets the switch to URI.
        /// </summary>
        /// <value>The switch to URI.</value>
        public Uri SwitchToUri
        {
            get
            {
                Uri uri;
                if (!string.IsNullOrEmpty(toUrlBox.Text) && Uri.TryCreate(toUrlBox.Text, UriKind.Absolute, out uri))
                    return uri;

                return null;
            }
            set
            {
                if (value == null)
                    toUrlBox.Text = "";
                else
                    toUrlBox.Text = value.AbsoluteUri;
            }
        }

        private void toUrlBox_Validating(object sender, CancelEventArgs e)
        {
            bool invalid = SwitchToUri == null;
            e.Cancel = invalid;
            if (invalid)
                errorProvider1.SetError(toUrlBox, @"Enter a valid url (like scheme://domain.tld/repos/svn/path), 
where scheme is http://, file:///, svn://, svn+ssh://
or use the browse button.");
            else
                errorProvider1.SetError(toUrlBox, null);
        }
    }
}
