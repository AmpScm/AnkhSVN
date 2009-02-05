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
using System.IO;
using SharpSvn;
using Ankh.VS;
using System.Collections.ObjectModel;

namespace Ankh.UI.SccManagement
{
    public partial class AddToSubversion : VSContainerForm
    {
        public AddToSubversion()
        {
            InitializeComponent();
        }

        string _pathToAdd;
        public string PathToAdd
        {
            get { return _pathToAdd; }
            set { _pathToAdd = value; }
        }


        public string WorkingCopyDir
        {
            get { return (string)localFolder.SelectedItem; }
        }

        protected override void OnContextChanged(EventArgs e)
        {
            base.OnContextChanged(e);

            if (Context != null)
            {
                repositoryTree.Context = Context;
                Initialize();
            }
        }

        bool _initialized;
        private void Initialize()
        {
            if (_initialized)
                return;

            if (Context != null)
            {
                IAnkhSolutionSettings ss = Context.GetService<IAnkhSolutionSettings>();
                foreach (Uri u in ss.GetRepositoryUris(true))
                {
                    repositoryUrl.Items.Add(u);
                }
                _initialized = true;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            string directory = File.Exists(PathToAdd) ? Path.GetDirectoryName(PathToAdd) : PathToAdd;
            string root = Path.GetPathRoot(directory);

            projectNameBox.Text = Path.GetFileNameWithoutExtension(PathToAdd);

            localFolder.Items.Add(directory);
            while (!root.Equals(directory, StringComparison.OrdinalIgnoreCase))
            {
                directory = Path.GetDirectoryName(directory);
                localFolder.Items.Add(directory);
            }
            if (localFolder.Items.Count > 0)
                localFolder.SelectedIndex = 0;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            // TODO: Save repository location
        }

        SvnClient _client;
        SvnClient Client
        {
            get { return _client ?? (_client = Context.GetService<ISvnClientPool>().GetClient()); }
        }

        private void createFolderButton_Click(object sender, EventArgs e)
        {
            if (repositoryTree.SelectedNode == null)
                return;

            repositoryTree.DoCreateDirectory();
        }

        string previousUrl;
        private void timer1_Tick(object sender, EventArgs e)
        {
            Uri u;
            if (previousUrl != repositoryUrl.Text)
            {
                previousUrl = repositoryUrl.Text;
            }
            else if (!string.IsNullOrEmpty(previousUrl) && Uri.TryCreate(previousUrl, UriKind.Absolute, out u))
            {
                timer1.Enabled = false;
                repositoryTree.BrowseTo(u);
            }
        }

        private void repositoryUrl_TextUpdate(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        public Uri RepositoryAddUrl
        {
            get
            {
                if (repositoryTree.SelectedNode == null)
                    return null;

                Uri u = repositoryTree.SelectedNode.RawUri;
                if (u == null)
                    return null;

                if (!string.IsNullOrEmpty(projectNameBox.Text))
                    u = new Uri(u, projectNameBox.Text + "/");
                if (addTrunk.Checked)
                    u = new Uri(u, "trunk/");
                return u;
            }
        }

        void UpdateUrlPreview()
        {
            if (RepositoryAddUrl == null)
                resultUriBox.Text = "";
            else
                resultUriBox.Text = RepositoryAddUrl.ToString();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UpdateUrlPreview();
            errorProvider1.SetError(repositoryTree, null);

            createFolderButton.Enabled = repositoryTree.CanCreateDirectory;

            if (repositoryTree.SelectedNode != null && repositoryTree.SelectedNode.NormalizedUri != null)
            {
                repositoryUrl.Text = repositoryTree.SelectedNode.RawUri.AbsoluteUri;
            }
        }

        private void addTrunk_CheckedChanged(object sender, EventArgs e)
        {
            UpdateUrlPreview();
        }

        private void projectNameBox_TextChanged(object sender, EventArgs e)
        {
            UpdateUrlPreview();
        }

        private void repositoryUrl_SelectedIndexChanged(object sender, EventArgs e)
        {
            Uri u = (Uri)repositoryUrl.SelectedItem;
            repositoryTree.AddRoot(u);
        }

        private void AddToSubversion_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DialogResult == DialogResult.Cancel)
                return; // Always allow cancel

            if (localFolder.SelectedItem == null)
            {
                e.Cancel = true;

                errorProvider1.SetError(localFolder, "Please select a working copy path");
            }
            if (RepositoryAddUrl == null)
            {
                e.Cancel = true;

                errorProvider1.SetError(repositoryTree, "Please select a location in the repository to add to");
            }
        }

        private void localFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(localFolder, null);
        }
    }
}
