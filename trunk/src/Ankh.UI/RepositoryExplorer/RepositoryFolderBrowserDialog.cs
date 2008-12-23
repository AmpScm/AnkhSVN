// $Id$
//
// Copyright 2008 The AnkhSVN Project
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Ankh.VS;
using Ankh.Commands;
using Ankh.Scc;

namespace Ankh.UI.RepositoryExplorer
{
    public partial class RepositoryFolderBrowserDialog : VSContainerForm
    {
        public RepositoryFolderBrowserDialog()
        {
            InitializeComponent();
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);
            if(!IsDisposed && !reposBrowser.IsDisposed)
                reposBrowser.Context = Context;
        }

        public Uri SelectedUri
        {
            get
            {
                Uri uri;
                if (!string.IsNullOrEmpty(urlBox.Text) && Uri.TryCreate(urlBox.Text, UriKind.Absolute, out uri))
                    return uri;

                return null;
            }

            set
            {
                if (value != null && value.IsAbsoluteUri)
                    urlBox.Text = value.AbsoluteUri;
                else
                    urlBox.Text = "";
            }
        }

        bool _initialized;

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (DesignMode)
                return;
            if (!_initialized && IsHandleCreated && Visible)
            {
                _initialized = true;
                if (SelectedUri != null)
                {
                    BrowseText();
                }

                if (Context != null)
                {
                    IAnkhSolutionSettings settings = Context.GetService<IAnkhSolutionSettings>();
                    if(settings != null)
                        foreach (Uri uri in settings.GetRepositoryUris(true))
                        {
                            if (!urlBox.Items.Contains(uri))
                                urlBox.Items.Add(uri);
                        }
                }
            }
        }

        Uri _rootUri;
        /// <summary>
        /// Gets or sets the top level uri to browse below; Currently only used when initializing the dialog
        /// </summary>
        [DefaultValue(null)]
        public Uri RootUri
        {
            get { return _rootUri; }
            set { _rootUri = value; }
        }

        [DefaultValue(false)]
        public bool EnableNewFolderButton
        {
            get { return newFolderButton.Enabled; }
            set { newFolderButton.Visible = newFolderButton.Enabled = value; }
        }

        private void urlBox_Leave(object sender, EventArgs e)
        {
            BrowseText();
        }

        private void newFolderButton_Click(object sender, EventArgs e)
        {
            reposBrowser.DoCreateDirectory();
        }

        private void urlBox_TextChanged(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            BrowseText();
        }

        private void urlBox_TextUpdate(object sender, EventArgs e)
        {
            BrowseText();
        }

        void BrowseText()
        {
            timer.Stop();

            Uri uri = SelectedUri;

            if(uri != null)
                reposBrowser.BrowseTo(uri);
        }

        private void reposBrowser_AfterSelect(object sender, TreeViewEventArgs e)
        {
            RepositoryTreeNode tn = reposBrowser.SelectedNode;

            if(tn != null && tn.RawUri != null)
            {
                urlBox.Text = tn.RawUri.AbsoluteUri;
                timer.Stop();
            }
        }

        BusyOverlay _overlay;
        bool _busy;

        private void reposBrowser_RetrievingChanged(object sender, EventArgs e)
        {
            if (reposBrowser.Retrieving != _busy)
            {
                if (!_busy)
                {
                    _busy = true;
                    if (_overlay == null)
                        _overlay = new BusyOverlay(reposBrowser, AnchorStyles.Right | AnchorStyles.Top);

                    _overlay.Show();
                }
                else
                {
                    _busy = false;
                    if (_overlay != null)
                        _overlay.Hide();
                }
            }
        }
    }
}
