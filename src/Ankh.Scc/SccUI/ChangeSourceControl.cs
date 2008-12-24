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
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Windows.Forms.Design;

using Ankh.Selection;
using Ankh.Scc;
using Ankh.UI;
using Ankh.VS;

namespace Ankh.Scc.SccUI
{
    public partial class ChangeSourceControl : VSContainerForm
    {
        public ChangeSourceControl()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (DesignMode)
                return;

            if (Context != null)
            {
                IUIService ds = Context.GetService<IUIService>();

                if (ds != null)
                {
                    ToolStripRenderer renderer = ds.Styles["VsToolWindowRenderer"] as ToolStripRenderer;

                    if (renderer != null)
                        toolStrip1.Renderer = renderer;
                }
            }

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

            if (settings == null || mapper == null || cache == null ||
                string.IsNullOrEmpty(settings.SolutionFilename))
            {
                return;
            }

            // TODO: Optimize to one time init and then just refresh
            bindingGrid.Rows.Add(new ChangeSourceControlRow(Context, SvnProject.Solution));
            foreach (SvnProject project in mapper.GetAllProjects())
            {
                if(project.IsSolution)
                    continue;

                ISvnProjectInfo projectInfo = mapper.GetProjectInfo(project);
                
                if(projectInfo == null || string.IsNullOrEmpty(projectInfo.ProjectDirectory))
                    continue;

                bindingGrid.Rows.Add(new ChangeSourceControlRow(Context, project));
            }
            // /TODO

            foreach (ChangeSourceControlRow row in bindingGrid.Rows)
            {
                row.Refresh();
            }

            if (!refreshCombo)
                return;

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
                    solutionRootBox.Items.Add(value);

                    if (setUri == value.Uri)
                        solutionRootBox.SelectedItem = value;
                    dirItem = dirItem.Parent;
                    cur = new Uri(cur, "../");
                }
            }
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

            foreach (SvnProject project in SelectedProjects)
            {
                scc.SetProjectManaged(project, true);
            }

            RefreshGrid(false);
        }
        private void disconnectButton_Click(object sender, EventArgs e)
        {
            if (Context == null)
                return;

            IAnkhSccService scc = Context.GetService<IAnkhSccService>();

            foreach (SvnProject project in SelectedProjects)
            {
                scc.SetProjectManaged(project, false);
            }

            RefreshGrid(false);
        }

        IEnumerable<SvnProject> SelectedProjects
        {
            get
            {
                List<SvnProject> projects = new List<SvnProject>();
                foreach (ChangeSourceControlRow row in bindingGrid.SelectedRows)
                {
                    SvnProject project = row.Project;

                    if (projects.Contains(project))
                        continue;

                    projects.Add(project);
                    yield return project;
                }
            }
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

            bool isSolution = false;
            foreach (SvnProject project in SelectedProjects)
            {
                if (project.IsSolution)
                    isSolution = true;
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

                if (enableConnect && enableDisconnect && isSolution)
                    break;
            }

            connectButton.Enabled = enableConnect;
            disconnectButton.Enabled = enableDisconnect;

            bool contains = tabControl1.Controls.Contains(solutionTab);

            if (isSolution && !contains)
            {
                tabControl1.Controls.Add(solutionTab);
                tabControl1.SelectedTab = solutionTab;
            }
            else if (!isSolution && contains)
                tabControl1.Controls.Remove(solutionTab);
        }
    }
}
