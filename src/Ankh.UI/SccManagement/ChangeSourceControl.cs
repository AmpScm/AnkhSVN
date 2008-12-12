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
using System.IO;
using System.Text;
using System.Windows.Forms;
using Ankh.VS;
using Ankh.Scc;
using SharpSvn;
using Ankh.Selection;

namespace Ankh.UI.SccManagement
{
    public partial class ChangeSourceControl : Form
    {
        IAnkhServiceProvider _context;
        public ChangeSourceControl()
        {
            InitializeComponent();
        }

        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set { _context = value; }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (bindingGrid != null)
                RefreshGrid(true);
        }

        private void RefreshGrid(bool refreshCombo)
        {
            bindingGrid.Rows.Clear();

            if (Context == null)
                return;

            IAnkhSolutionSettings settings = Context.GetService<IAnkhSolutionSettings>();
            IProjectFileMapper mapper = Context.GetService<IProjectFileMapper>();
            IFileStatusCache cache = Context.GetService<IFileStatusCache>();
            IAnkhSccService scc = Context.GetService<IAnkhSccService>();

            if (settings == null || mapper == null || cache == null || scc == null ||
                string.IsNullOrEmpty(settings.SolutionFilename))
            {
                return;
            }

            SvnItem info = cache[settings.SolutionFilename];

            Uri dirUri = info.Status.Uri;
            if(dirUri != null)
                dirUri = new Uri(dirUri, "./");

            bool solutionControlled = scc.IsSolutionManaged;
            int n = bindingGrid.Rows.Add(
                "Solution: " + Path.GetFileNameWithoutExtension(settings.SolutionFilename),
                (dirUri != null) ? dirUri.ToString() : "",
                solutionControlled,
                scc.IsSolutionManaged ? "Ok" : "Not Controlled",
                info.Parent.FullPath);

            foreach (SvnProject project in mapper.GetAllProjects())
            {
                ISvnProjectInfo projectInfo = mapper.GetProjectInfo(project);

                if (projectInfo == null || string.IsNullOrEmpty(projectInfo.ProjectDirectory))
                    continue;

                info = cache[projectInfo.ProjectDirectory];

                string uri = "";

                if (info.Status.Uri != null)
                    uri = info.Status.Uri.ToString();

                n = bindingGrid.Rows.Add(
                    projectInfo.UniqueProjectName,
                    uri,
                    scc.IsProjectManaged(project),                    
                    (scc.IsSolutionManaged || solutionControlled) ? "Ok" : "Not Controlled",
                    projectInfo.ProjectDirectory);

                bindingGrid.Rows[n].Tag = project;
            }

            if (!refreshCombo)
                return;
            SvnInfoEventArgs slnInfo = GetInfo(settings.SolutionFilename);

            if (slnInfo != null)
            {
                this.solutionRootBox.Items.Clear();

                Uri myUri = new Uri(slnInfo.Uri, "./");
                UriMap value = new UriMap(myUri, "./");
                solutionRootBox.Items.Add(value);

                if (myUri == value.Uri)
                    solutionRootBox.SelectedItem = value;

                Uri setUri = settings.ProjectRootUri;

                string dir = Path.GetDirectoryName(settings.SolutionFilename);
                string path = "";
                while(myUri != slnInfo.RepositoryRoot)
                {
                    path += "../";
                    myUri = new Uri(myUri, "../");
                    SvnInfoEventArgs dirInfo = GetInfo(Path.Combine(dir, "./" + path));

                    if (dirInfo == null || dirInfo.Uri != myUri)
                        break;

                    value = new UriMap(myUri, path);
                    solutionRootBox.Items.Add(value);
                    if (myUri == setUri)
                        solutionRootBox.SelectedItem = value;
                }
            }
                

            //bindingGrid.Rows.Add(Path.GetFileNameWithoutExtension(settings.SolutionFilename)
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

        private SvnInfoEventArgs GetInfo(string filename)
        {
            SvnInfoEventArgs solutionInfo = null;
            using (SvnClient client = Context.GetService<ISvnClientPool>().GetNoUIClient())
            {
                SvnInfoArgs a = new SvnInfoArgs();
                a.Depth = SvnDepth.Empty;
                a.ThrowOnError = false;
                client.Info(new SvnPathTarget(filename), a,
                    delegate(object sender, SvnInfoEventArgs e)
                    {
                        e.Detach();
                        solutionInfo = e;
                    });
            }

            return solutionInfo;
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshGrid(true);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (Context == null)
                return;

            IAnkhSolutionSettings settings = Context.GetService<IAnkhSolutionSettings>();
            UriMap map = solutionRootBox.SelectedItem as UriMap;

            if (map == null)
                return;

            if (settings.ProjectRootUri != null && map.Uri != settings.ProjectRootUri)
            {
                settings.ProjectRoot = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(settings.SolutionFilename), map.Value));
            }
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (Context == null)
                return;

            IAnkhSccService scc = Context.GetService<IAnkhSccService>();

            foreach (DataGridViewRow row in bindingGrid.SelectedRows)
            {
                SvnProject project = row.Tag as SvnProject;
                scc.SetProjectManaged(project, true);
            }

            RefreshGrid(false);
        }
        private void disconnectButton_Click(object sender, EventArgs e)
        {
            if (Context == null)
                return;

            IAnkhSccService scc = Context.GetService<IAnkhSccService>();

            foreach (DataGridViewCell cell in bindingGrid.SelectedCells)
            {
                DataGridViewRow row = bindingGrid.Rows[cell.RowIndex];
                SvnProject project = row.Tag as SvnProject;
                scc.SetProjectManaged(project, false);
            }

            RefreshGrid(false);
        }

        private void bindingGrid_SelectionChanged(object sender, EventArgs e)
        {
            if(DesignMode)
                return;

            bool enableConnect = false;
            bool enableDisconnect = false;

            IAnkhSccService scc = Context.GetService<IAnkhSccService>();
            IFileStatusCache cache = Context.GetService<IFileStatusCache>();
            IProjectFileMapper mapper = Context.GetService<IProjectFileMapper>();
            IAnkhSolutionSettings solset = Context.GetService<IAnkhSolutionSettings>();

            foreach (DataGridViewCell cell in bindingGrid.SelectedCells)
            {
                DataGridViewRow row = bindingGrid.Rows[cell.RowIndex];
                SvnProject project = row.Tag as SvnProject;

                if (scc.IsProjectManaged(project))
                    enableDisconnect = true;
                else if(!enableConnect)
                {
                    string dir;
                    if (project != null)
                    {
                        ISvnProjectInfo projectInfo = mapper.GetProjectInfo(project);

                        if (projectInfo == null || string.IsNullOrEmpty(projectInfo.ProjectDirectory))
                            continue;

                        dir = projectInfo.ProjectDirectory;
                    }
                    else if (string.IsNullOrEmpty(solset.SolutionFilename))
                        continue;
                    else
                        dir = Path.GetDirectoryName(solset.SolutionFilename);

                    SvnItem item = cache[dir];

                    if (item.Status.Uri != null)
                        enableConnect = true;
                }

                if (enableConnect && enableDisconnect)
                    break;
            }

            connectButton.Enabled = enableConnect;
            disconnectButton.Enabled = enableDisconnect;
        }
    }
}
