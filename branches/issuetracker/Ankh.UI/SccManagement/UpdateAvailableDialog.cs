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

namespace Ankh.UI.SccManagement
{
    public partial class UpdateAvailableDialog : VSDialogForm
    {
        public UpdateAvailableDialog()
        {
            InitializeComponent();
        }

        private void linkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Uri uri; // Just some minor precautions
            if (Uri.TryCreate((string)e.Link.LinkData, UriKind.Absolute, out uri) && !uri.IsFile && !uri.IsUnc)
            {
                try
                {
                    System.Diagnostics.Process.Start((string)e.Link.LinkData);
                }
                catch
                { }
            }
        }
    }
}
