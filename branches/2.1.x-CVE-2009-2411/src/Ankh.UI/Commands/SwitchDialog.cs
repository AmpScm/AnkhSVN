// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
using Ankh.UI.RepositoryExplorer;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.UI.Commands
{
    public partial class SwitchDialog : VSDialogForm
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

        public bool AllowUnversionedObstructions
        {
            get { return allowObstructions.Checked; }
            set { allowObstructions.Checked = value; }
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
                errorProvider.SetError(toUrlBox, CommandStrings.EnterValidUrl);
            else
                errorProvider.SetError(toUrlBox, null);
        }

        private void toUrlBox_TextChanged(object sender, EventArgs e)
        {
            UpdateRoot();
        }
    }
}
