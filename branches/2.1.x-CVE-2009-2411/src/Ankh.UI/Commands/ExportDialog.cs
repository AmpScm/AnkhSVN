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

namespace Ankh.UI.Commands
{
    /// <summary>
    /// A dialog for performing exports.
    /// </summary>
    public partial class ExportDialog : VSDialogForm
    {
        protected ExportDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        
        public ExportDialog(IAnkhServiceProvider context)
            : this()
        {
            if (context == null)
                throw new ArgumentNullException("context");

            Context = context;
        }    

        public string OriginPath
        {
            set { originBox.Text = value; }
            get  { return originBox.Text; }
        }
            
        /// <summary>
        /// The local path to check out to.
        /// </summary>
        public string LocalPath
        {
            get { return this.toBox.Text; }
            set { this.toBox.Text = value; }
        }

        IFileStatusCache _fc;
        IFileStatusCache FileCache
        {
            get { return _fc ?? (_fc = Context.GetService<IFileStatusCache>()); }
        }

        SvnOrigin _baseOrigin;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (FileCache != null && !string.IsNullOrEmpty(OriginPath))
            {
                SvnItem item = FileCache[OriginPath];

                if (item.IsVersioned)
                    _baseOrigin = new SvnOrigin(item);

                revisionPicker.Context = Context;
                revisionPicker.Revision = SvnRevision.Working;
                revisionPicker.SvnOrigin = _baseOrigin;                
            }
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
        
        string _lastOrigin;
        /// <summary>
        /// Validate the input here.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlsChanged(object sender, System.EventArgs e)
        {
            bool enable = false;
            if (Context != null && FileCache != null)
            {
                if (this.revisionPicker.Valid && ExportSource != null && !string.IsNullOrEmpty(toBox.Text))
                {
                    string origin = string.IsNullOrEmpty(originBox.Text) ? null : originBox.Text;

                    if (origin != null)
                    {
                        if (origin == _lastOrigin)
                            enable = true;
                        else
                        {
                            SvnItem i = FileCache[origin];

                            if (i != null && i.Exists && i.IsVersioned)
                            {
                                revisionPicker.SvnOrigin = new SvnOrigin(i);
                                _lastOrigin = origin;
                                enable = true;
                            }
                            else
                                enable = false;
                        }
                    }
                }
            }
            
            this.okButton.Enabled = enable;
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
                
                this.originBox.Text = browser.SelectedPath;
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
                if (!string.IsNullOrEmpty(toBox.Text))
                    browser.SelectedPath = toBox.Text;

                if (browser.ShowDialog(this) != DialogResult.OK)
                    return;
                
                this.toBox.Text = browser.SelectedPath;
            }
        }

        public SvnTarget ExportSource
        {
            get
            {
                string origin = string.IsNullOrEmpty(originBox.Text) ? null : originBox.Text;

                if (origin != null)
                {
                    SvnItem i = FileCache[origin];

                    if (i != null && i.Exists && i.IsVersioned)
                    {
                        revisionPicker.SvnOrigin = new SvnOrigin(i);
                        _lastOrigin = origin;
                        return i.FullPath;
                    }
                }
                
                return null;
            }
        }
    }
}
