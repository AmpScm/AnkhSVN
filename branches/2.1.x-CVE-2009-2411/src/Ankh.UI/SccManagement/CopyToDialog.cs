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
using Ankh.UI.RepositoryExplorer;
using SharpSvn;
using Ankh.Scc;

namespace Ankh.UI.SccManagement
{
    public partial class CopyToDialog : VSContainerForm
    {
        public CopyToDialog()
        {
            InitializeComponent();
        }

        Uri _rootUri;
        public Uri RootUri
        {
            get { return _rootUri; }
            set { _rootUri = value; }
        }

        public string LogMessage
        {
            get { return logMessage.Text; }
        }

        private void toUrlBrowse_Click(object sender, EventArgs e)
        {
            using (RepositoryFolderBrowserDialog dlg = new RepositoryFolderBrowserDialog())
            {
                dlg.RootUri = RootUri;
                dlg.EnableNewFolderButton = true;
                Uri r;

                if (Uri.TryCreate(toUrlBox.Text, UriKind.Absolute, out r))
                    dlg.SelectedUri = r;

                if (dlg.ShowDialog(Context) == DialogResult.OK)
                {
                    if (dlg.SelectedUri != null)
                        toUrlBox.Text = dlg.SelectedUri.AbsoluteUri;
                }
            }
        }

        private void toUrlBox_TextChanged(object sender, EventArgs e)
        {
            btnOk.Enabled = (SelectedUri != null);
        }

        public Uri SelectedUri
        {
            get
            {
                Uri uri;

                if (!string.IsNullOrEmpty(toUrlBox.Text) &&
                    Uri.TryCreate(toUrlBox.Text, UriKind.Absolute, out uri))
                {
                    return uri;
                }

                return null;
            }
            set
            {
                if (value != null)
                    toUrlBox.Text = value.ToString();
                else
                    toUrlBox.Text = "";
            }
        }        
    }
}
