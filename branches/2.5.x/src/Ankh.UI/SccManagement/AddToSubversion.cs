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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using Ankh.Configuration;
using Ankh.UI.RepositoryExplorer;
using Ankh.UI.RepositoryExplorer.RepositoryWizard;
using Ankh.VS;
using SharpSvn;

namespace Ankh.UI.SccManagement
{
    public partial class AddToSubversion : VSDialogForm
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

            if (DesignMode)
                return;

            string directory = File.Exists(PathToAdd) ? Path.GetDirectoryName(PathToAdd) : PathToAdd;
            string root = Path.GetPathRoot(directory);

            projectNameBox.Text = Path.GetFileNameWithoutExtension(PathToAdd);

            localFolder.Items.Add(directory);
            while (!root.Equals(directory, StringComparison.OrdinalIgnoreCase))
            {
                directory = SvnTools.GetNormalizedDirectoryName(directory);
                localFolder.Items.Add(directory);
            }
            if (localFolder.Items.Count > 0)
                localFolder.SelectedIndex = 0;
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

        private void selectRepositoryButton_Click(object sender, EventArgs e)
        {
            Uri dirUri;
            using (RepositorySelectionWizard dialog = new RepositorySelectionWizard(Context))
            {
                DialogResult result = dialog.ShowDialog(Context);
                if (result != DialogResult.OK)
                {
                    return;
                }
                dirUri = dialog.GetSelectedRepositoryUri();
            }
            if (dirUri != null)
            {
                this.repositoryUrl.Text = dirUri.AbsoluteUri;
                repositoryTree.AddRoot(dirUri);
            }
        }

        public Uri RepositoryAddUrl
        {
            get
            {
                Uri result = RepositoryUri;
                if (result != null)
                {
                    if (!string.IsNullOrEmpty(projectNameBox.Text))
                        result = new Uri(result, projectNameBox.Text + "/");
                    if (addTrunk.Checked)
                        result = new Uri(result, "trunk/");
                }
                return result;
            }
            set
            {
                repositoryTree.BrowseItem(value);
            }
        }

        private Uri RepositoryUri
        {
            get
            {
                if (repositoryTree.SelectedNode == null)
                {
                    return null;
                }
                return repositoryTree.SelectedNode.RawUri;
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

            Uri repoUri = RepositoryUri;
            if (repoUri != null)
            {
                repositoryUrl.Text = repoUri.AbsoluteUri;
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

        private void localFolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            errorProvider1.SetError(localFolder, null);
        }

        protected virtual void ValidateAdd(object sender, CancelEventArgs e)
        {
            ISvnClientPool clientPool = Context.GetService<ISvnClientPool>();
            if (localFolder.SelectedItem == null)
            {
                errorProvider1.SetError(localFolder, "Please select a working copy path");
                e.Cancel = true;
                return;
            }

            if (RepositoryAddUrl == null)
            {
                errorProvider1.SetError(repositoryTree, "Please select a location in the repository to add to");
                e.Cancel = true;
                return;
            }

            using (SvnClient sc = clientPool.GetClient())
            {
                SvnInfoArgs ia = new SvnInfoArgs();
                ia.ThrowOnError = false;
                Collection<SvnInfoEventArgs> info;
                bool result = sc.GetInfo(RepositoryUri, ia, out info);
                if (!result)
                {
                    errorProvider1.SetError(repositoryTree, "Please select a valid location in the repository to add to");
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            CancelEventArgs cea = new CancelEventArgs();
            ValidateAdd(this, cea);

            if (cea.Cancel)
                DialogResult = DialogResult.None;
            else if (Context != null)
            {
                RepositoryTreeNode rtn = repositoryTree.SelectedNode;

                if (rtn != null && rtn.RepositoryRoot != null)
                {
                    IAnkhConfigurationService config = Context.GetService<IAnkhConfigurationService>();

                    RegistryLifoList lifo = config.GetRecentReposUrls();

                    string url = rtn.RepositoryRoot.ToString();
                    if (!lifo.Contains(url))
                        lifo.Add(url);
                }
            }
        }

        public bool CommitAllVisible
        {
            get { return commitAll.Visible; }
            set { commitAll.Enabled = commitAll.Visible = value; }
        }

        public bool CommitAllFiles
        {
            get { return commitAll.Checked; }
            set { commitAll.Checked = value; }
        }
    }
}
