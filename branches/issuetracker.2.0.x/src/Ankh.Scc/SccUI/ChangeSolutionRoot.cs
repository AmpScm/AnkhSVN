// $Id$
//
// Copyright 2009 The AnkhSVN Project
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
using Ankh.UI;
using Ankh.VS;
using System.IO;
using SharpSvn;

namespace Ankh.Scc.SccUI
{
    public partial class ChangeSolutionRoot : VSContainerForm
    {
        public ChangeSolutionRoot()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            IAnkhSolutionSettings settings = Context.GetService<IAnkhSolutionSettings>();
            IFileStatusCache cache = Context.GetService<IFileStatusCache>();

            SvnItem slnDirItem = cache[settings.SolutionFilename].Parent;
            SvnWorkingCopy wc = slnDirItem.WorkingCopy;

            if (wc != null && slnDirItem.Status.Uri != null)
            {
                SvnItem dirItem = slnDirItem;
                Uri cur = dirItem.Status.Uri;
                Uri setUri = settings.ProjectRootUri;

                while (dirItem != null && dirItem.IsBelowPath(wc.FullPath))
                {
                    UriMap value = new UriMap(cur, dirItem.FullPath);
                    slnBindPath.Items.Add(value);

                    if (setUri == value.Uri)
                        slnBindPath.SelectedItem = value;
                    dirItem = dirItem.Parent;
                    cur = new Uri(cur, "../");
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        class UriMap
        {
            Uri _uri;
            string _value;
            public UriMap(Uri uri, string value)
            {
                _uri = uri;
                _value = value;
            }

            public override string ToString()
            {
                return _uri.ToString();
            }

            public string Value
            {
                get { return _value; }
            }

            public Uri Uri
            {
                get { return _uri; }
            }

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            IAnkhSolutionSettings settings = Context.GetService<IAnkhSolutionSettings>();
            UriMap map = slnBindPath.SelectedItem as UriMap;

            if (map == null)
                return;

            if (settings.ProjectRootUri != null && map.Uri != settings.ProjectRootUri)
            {
                string dir = SvnTools.GetNormalizedDirectoryName(settings.SolutionFilename);
                settings.ProjectRoot = SvnTools.GetNormalizedFullPath(Path.Combine(dir, map.Value));
            }
        }
    }
}
