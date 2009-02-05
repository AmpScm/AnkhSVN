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
using Ankh.Scc;
using System.IO;
using Ankh.UI.RepositoryExplorer;
using System.Windows.Forms.Design;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog for performing exports.
    /// </summary>
    public partial class ExportDialog : VSDialogForm
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
            : this()
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

        public Uri OriginUri
        {
            set
            {
                if (value != null && value.IsAbsoluteUri)
                    urlTextBox.Text = value.AbsoluteUri;
                else
                    urlTextBox.Text = "";
            }
        }

        public string OriginPath
        {
            set { exportFromDirTextBox.Text = value; }
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
            bool enable = false;
            if (_context != null)
            {
                IFileStatusCache cache = _context.GetService<IFileStatusCache>();


                if (cache != null)
                {
                    if (this.revisionPicker.Valid && ExportSource != null && !string.IsNullOrEmpty(localDirTextBox.Text))
                    {
                        if (!this.radioButtonFromDir.Checked)
                            enable = true;
                        else
                            try
                            {
                                enable = cache[exportFromDirTextBox.Text].IsVersioned;
                            }
                            catch
                            { }
                    }
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
            exportFromDirTextBox.Enabled = wcBrowseBtn.Enabled = radioButtonFromDir.Checked;

        }
        /// <summary>
        ///   User clicked radio button to export from a URL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButtonFromURL_CheckedChanged(object sender, System.EventArgs e)
        {
            urlTextBox.Enabled = urlBrowseBtn.Enabled = radioButtonFromURL.Checked;
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
                SvnPathTarget pt = ExportSource as SvnPathTarget;

                if (pt != null)
                    browser.SelectedPath = pt.FullPath;

				browser.ShowNewFolderButton = false;

                if (browser.ShowDialog(this) != DialogResult.OK)
                    return;
                
                this.exportFromDirTextBox.Text = browser.SelectedPath;
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
                if (!string.IsNullOrEmpty(localDirTextBox.Text))
                    browser.SelectedPath = localDirTextBox.Text;

                if (browser.ShowDialog(this) != DialogResult.OK)
                    return;
                
                this.localDirTextBox.Text = browser.SelectedPath;
            }
        }

        private void urlBrowseBtn_Click(object sender, EventArgs e)
        {
            using (RepositoryFolderBrowserDialog dlg = new RepositoryFolderBrowserDialog())
            {
                SvnUriTarget ut = ExportSource as SvnUriTarget;

                if (ut != null)
                    dlg.SelectedUri = ut.Uri;

                if (dlg.ShowDialog(_context) != DialogResult.OK)
                    return;

                if (dlg.SelectedUri != null)
                    urlTextBox.Text = dlg.SelectedUri.AbsoluteUri;
            }
        }

        public SvnTarget ExportSource
        {
            get
            {
                if (radioButtonFromURL.Checked)
                {
                    Uri r;

                    string txt = urlTextBox.Text;
                    if (!string.IsNullOrEmpty(txt) && Uri.TryCreate(txt, UriKind.Absolute, out r))
                        return new SvnUriTarget(r);
                }
                else if (radioButtonFromDir.Checked)
                {
                    string txt = this.localDirTextBox.Text;
                    if (!string.IsNullOrEmpty(txt))
                        try
                        {
                            txt = SvnTools.GetTruePath(SvnTools.GetNormalizedFullPath(Path.GetFullPath(txt)));

                            return new SvnPathTarget(txt);
                        }
                        catch
                        {
                            return null;
                        }
                }
                
                return null;
            }
        }
    }
}
