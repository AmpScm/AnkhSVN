using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.UI.RepositoryExplorer;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.UI
{
    public partial class SwitchDialog : VSContainerForm
    {
        public SwitchDialog()
        {
            InitializeComponent();
            versionSelector.Revision = SvnRevision.Head;
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

        public SvnRevision Revision
        {
            get { return versionSelector.Revision; }
            set { versionSelector.Revision = value; }
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);
            if (Context != null)
                versionSelector.Context = Context;
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

        Uri _reposRoot;
        public Uri RepositoryRoot
        {
            get { return _reposRoot; }
            set
            {
                _reposRoot = value;
                UpdateRoot();
            }
        }

        private void UpdateRoot()
        {
            versionSelector.Context = Context;
            if (_reposRoot == null)
                versionSelector.SvnOrigin = null;
            else
            {
                Uri switchUri = SwitchToUri;

                if (switchUri != null)
                {
                    if (!switchUri.AbsoluteUri.StartsWith(_reposRoot.AbsoluteUri))
                        versionSelector.SvnOrigin = null;
                    else
                        versionSelector.SvnOrigin = new SvnOrigin(switchUri, _reposRoot);
                }
            }
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
                {
                    toUrlBox.Text = value.AbsoluteUri;

                    UpdateRoot();
                }
            }
        }

        private void toUrlBox_Validating(object sender, CancelEventArgs e)
        {
            bool invalid = SwitchToUri == null;
            e.Cancel = invalid;
            if (invalid)
                errorProvider1.SetError(toUrlBox, @"Enter a valid url (like http://domain.tld/svn/repos/path)
or use the browse button.");
            else
                errorProvider1.SetError(toUrlBox, null);
        }

        private void toUrlBox_TextChanged(object sender, EventArgs e)
        {
            UpdateRoot();
        }
    }
}
