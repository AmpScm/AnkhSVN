// $Id$
//
// Copyright 2004-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.ComponentModel;
using System.Windows.Forms;

using SharpSvn;
using Ankh.UI.RepositoryExplorer;

namespace Ankh.UI.Commands
{
    /// <summary>
    /// A dialog for performing checkouts.
    /// </summary>
    public partial class CheckoutDialog : VSDialogForm
    {
        public CheckoutDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.ControlsChanged(this, EventArgs.Empty);
            revisionPicker.Revision = SvnRevision.Head;
        }

        /// <summary>
        /// The URL of the repository.
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
            set 
            {
                if (value == null)
                    urlTextBox.Text = "";
                else
                    urlTextBox.Text = value.AbsoluteUri;
            }
        }

        Uri _reposRoot;

        public Uri RepositoryRoot
        {
            get { return _reposRoot; }
            set { _reposRoot = value; }
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);

            revisionPicker.Context = Context; 
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
        /// Gets a value indicating whether [ignore externals].
        /// </summary>
        /// <value><c>true</c> if [ignore externals]; otherwise, <c>false</c>.</value>
        public bool IgnoreExternals
        {
            get { return omitExternalsCheckBox.Checked; }
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
            this.okButton.Enabled = this.revisionPicker.Valid && Uri != null && !string.IsNullOrEmpty(localDirTextBox.Text);
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

        private void urlTextBox_TextChanged(object sender, EventArgs e)
        {
            if (Uri != null )
                revisionPicker.SvnOrigin = new Ankh.Scc.SvnOrigin(Context, Uri, RepositoryRoot);
        }

        private void urlBrowse_Click(object sender, EventArgs e)
        {
            using (RepositoryFolderBrowserDialog dlg = new RepositoryFolderBrowserDialog())
            {
                dlg.Context = Context;
                dlg.SelectedUri = Uri;

                if (dlg.ShowDialog(Context) == DialogResult.OK)
                {
                    Uri = dlg.SelectedUri;
                }
            }
        }

        private void localDirTextBox_Validating(object sender, CancelEventArgs e)
        {
            bool invalid = string.IsNullOrEmpty(LocalPath);
            e.Cancel = invalid;
            if (invalid)
                errorProvider.SetError(localDirTextBox, CommandStrings.EnterValidPath);
            else
                errorProvider.SetError(localDirTextBox, null);
        }

        private void urlTextBox_Validating(object sender, CancelEventArgs e)
        {
            bool invalid = Uri == null;
            e.Cancel = invalid;
            if (invalid)
                errorProvider.SetError(urlTextBox, CommandStrings.EnterValidUrl);
            else
                errorProvider.SetError(urlTextBox, null);
        }
    }
}
