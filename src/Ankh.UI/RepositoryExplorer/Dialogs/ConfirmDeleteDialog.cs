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
using SharpSvn;
using Ankh.Scc;

namespace Ankh.UI.RepositoryExplorer.Dialogs
{
    public partial class ConfirmDeleteDialog : VSContainerForm
    {
        public ConfirmDeleteDialog()
        {
            InitializeComponent();
        }

        Uri[] _uris;
        public void SetUris(IEnumerable<SvnOrigin> uris)
        {
            deleteList.ClearSelected();

            SortedDictionary<Uri,SvnOrigin> d = new SortedDictionary<Uri, SvnOrigin>();
            foreach (SvnOrigin o in uris)
            {
                SvnUriTarget ut = o.Target as SvnUriTarget;
                if (ut != null)
                    d[ut.Uri] = o;
                else
                    d[o.Uri] = o;
            }

            _uris = new Uri[d.Count];
            List<Uri> newUris = new List<Uri>();
            foreach (SvnOrigin o in d.Values)
            {
                deleteList.Items.Add(o.Uri);
                newUris.Add(SvnTools.GetNormalizedUri(o.Uri));                
            }
            _uris = newUris.ToArray();
        }

        public string LogMessage
        {
            get { return logMessage.Text; }
        }

        public Uri[] Uris
        {
            get { return _uris; }
        }
    }
}
