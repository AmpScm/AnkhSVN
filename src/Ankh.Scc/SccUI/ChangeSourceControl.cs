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
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using Ankh.Selection;
using Ankh.Services;
using Ankh.UI;
using Ankh.VS;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc.SccUI
{
    public partial class ChangeSourceControl : VSDialogForm
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
                if (!VSVersion.VS2012OrLater)
                {
                    IUIService ds = Context.GetService<IUIService>();

                    if (ds != null)
                    {
                        ToolStripRenderer renderer = ds.Styles["VsToolWindowRenderer"] as ToolStripRenderer;

                        if (renderer != null)
                            toolStrip1.Renderer = renderer;
                    }
                }
            }

            Prepare();
        }

        bool _prepared;
        public void Prepare()
        {
            if (_prepared)
                return;

            if (Context == null)
                return;

            _prepared = true;

            IAnkhSolutionSettings settings = Context.GetService<IAnkhSolutionSettings>();

            if (settings.SolutionFilename != null)
                Text += " - " + Path.GetFileName(settings.SolutionFilename);

            InitializeGrid();
        }

        Ankh.UI.IAnkhThreadedWaitService _tws;
        Ankh.UI.IAnkhThreadedWaitService ThreadedWaitService
        {
            get { return _tws ?? (_tws = GetService<Ankh.UI.IAnkhThreadedWaitService>()); }
        }

        private void InitializeGrid()
        {
            bindingGrid.Rows.Clear();

            if (Context == null)
                return;

            if (string.IsNullOrEmpty(SolutionSettings.SolutionFilename))
            {
                return;
            }

            // TODO: Optimize to one time init and then just refresh
            if (SvnItem.IsValidPath(SolutionSettings.SolutionFilename))
            {
                bindingGrid.Rows.Add(new ChangeSourceControlRow(Context, SccProject.Solution));
            }
            foreach (SccProject project in ProjectMapper.GetAllSccProjects())
            {
                if (project.IsSolution)
                    continue;

                ISccProjectInfo projectInfo = ProjectMapper.GetProjectInfo(project);

                if (projectInfo == null || string.IsNullOrEmpty(projectInfo.ProjectDirectory))
                    continue;

                if (!projectInfo.IsSccBindable && !Scc.IsProjectManaged(project))
                    continue;

                bindingGrid.Rows.Add(new ChangeSourceControlRow(Context, project));
            }
            // /TODO

            RefreshGrid();
        }

        SvnSccProvider _scc;
        SvnSccProvider Scc
        {
            get { return _scc ?? (_scc = Context.GetService<SvnSccProvider>(typeof(IAnkhSccService))); }
        }

        void RefreshGrid()
        {
            ISupportInitialize init = bindingGrid;
            init.BeginInit();
            try
            {
                foreach (ChangeSourceControlRow row in bindingGrid.Rows)
                {
                    row.Refresh();
                }
            }
            finally
            {
                init.EndInit();
            }

            bool enableConnect = false;
            bool enableDisconnect = false;

            IAnkhSccService scc = Context.GetService<IAnkhSccService>();

            bool isSolution = false;
            foreach (SccProject project in SelectedProjects)
            {
                if (project.IsSolution)
                    isSolution = true;

                if (scc.IsProjectManaged(project))
                    enableDisconnect = true;
                else if (!enableConnect)
                {
                    SvnItem item = null;
                    if (!project.IsSolution)
                    {
                        ISccProjectInfo projectInfo = ProjectMapper.GetProjectInfo(project);

                        if (projectInfo == null || string.IsNullOrEmpty(projectInfo.ProjectDirectory))
                            continue;

                        item = StatusCache[projectInfo.SccBaseDirectory];
                    }
                    else
                        item = SolutionSettings.ProjectRootSvnItem;

                    if (item != null && item.Uri != null)
                        enableConnect = true;
                }

                if (enableConnect && enableDisconnect && isSolution)
                    break;
            }

            EnableTab(solutionSettingsTab, isSolution);
            EnableTab(sharedSettingsTab, !isSolution);
            EnableTab(userSettingsTab, false);

            UpdateSettingTabs();

            connectButton.Enabled = enableConnect;
            disconnectButton.Enabled = enableDisconnect;

            UpdateSettingTabs();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            if (Context == null)
                return;

            IAnkhSccService scc = Context.GetService<IAnkhSccService>();

            foreach (SccProject project in SelectedProjects)
            {
                scc.SetProjectManaged(project, true);
            }

            RefreshGrid();
        }
        private void disconnectButton_Click(object sender, EventArgs e)
        {
            if (Context == null)
                return;

            IAnkhSccService scc = Context.GetService<IAnkhSccService>();

            foreach (SccProject project in SelectedProjects)
            {
                scc.SetProjectManaged(project, false);
            }

            RefreshGrid();
        }

        IProjectFileMapper _projectMapper;
        IProjectFileMapper ProjectMapper
        {
            get { return _projectMapper ?? (_projectMapper = Context.GetService<IProjectFileMapper>()); }
        }

        ISvnStatusCache _fileCache;
        ISvnStatusCache StatusCache
        {
            get { return _fileCache ?? (_fileCache = Context.GetService<ISvnStatusCache>()); }
        }

        IAnkhSolutionSettings _solutionSettings;
        IAnkhSolutionSettings SolutionSettings
        {
            get { return _solutionSettings ?? (_solutionSettings = Context.GetService<IAnkhSolutionSettings>()); }
        }

        IEnumerable<SccProject> SelectedProjects
        {
            get
            {
                List<SccProject> projects = new List<SccProject>();
                foreach (ChangeSourceControlRow row in bindingGrid.SelectedRows)
                {
                    SccProject project = row.Project;

                    if (projects.Contains(project))
                        continue;

                    projects.Add(project);
                    yield return project;
                }
            }
        }

        private void bindingGrid_SelectionChanged(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            RefreshGrid();
        }

        private void EnableTab(TabPage tab, bool enable)
        {
            if (enable == settingsTabControl.Controls.Contains(tab))
                return;

            Control activeControl = ActiveControl;

            if (enable)
                settingsTabControl.Controls.Add(tab);
            else
                settingsTabControl.Controls.Remove(tab);

            if (activeControl != ActiveControl)
                ActiveControl = activeControl;
        }

        private void UpdateSettingTabs()
        {
            List<ChangeSourceControlProjectBinding> bindings =
                new List<ChangeSourceControlProjectBinding>();

            foreach (SccProject project in SelectedProjects)
            {
                ISccProjectInfo info =
                    project.IsSolution ? null : ProjectMapper.GetProjectInfo(project);
                bool usable =
                    info != null && !string.IsNullOrEmpty(info.ProjectDirectory);

                bindings.Add(
                    new ChangeSourceControlProjectBinding(
                        usable,
                        usable ? info.SccBaseDirectory : null,
                        usable ? info.ProjectDirectory : null,
                        usable ? GetProjectLocation(project) : null,
                        usable ? GetProjectUri(info) : null));
            }

            ChangeSourceControlSelectionState state =
                ChangeSourceControlLogic.BuildSelection(bindings);

            shProjectLocation.Text = state.ProjectLocationText;
            shBindPath.Text = state.ProjectBaseText;
            shRelativePath.Text = state.RelativePathText;
            shProjectUrl.Text = state.ProjectUrlText;

            usProjectLocation.Text = state.ProjectLocationText;
            usBindPath.Text = state.ProjectBaseText;
            usRelativePath.Text = state.RelativePathText;
            usProjectUrl.Text = state.ProjectUrlText;

            slnProjectLocation.Text = SolutionSettings.SolutionFilename;
            slnBindPath.Text = SolutionSettings.ProjectRoot;

            string slRelativePath = Path.GetDirectoryName(
                SvnItem.MakeRelative(
                    SolutionSettings.ProjectRoot,
                    SolutionSettings.SolutionFilename));

            slnRelativePath.Text =
                string.IsNullOrEmpty(slRelativePath) ? "." : slRelativePath;

            slnBindUrl.Text =
                (SolutionSettings.ProjectRootUri != null)
                    ? SolutionSettings.ProjectRootUri.ToString()
                    : "";

            usProjectLocationBrowse.Visible = false;
            usProjectLocationBrowse.Enabled = false;

            sharedProjectUrlBrowse.Enabled = false;
            sharedProjectUrlBrowse.Visible = false;

            slnBindBrowse.Enabled =
                (SolutionSettings.ProjectRootSvnItem != null)
                && SolutionSettings.ProjectRootSvnItem.WorkingCopy != null;
        }

        private string GetProjectLocation(SccProject project)
        {
            if (project.RawHandle == null)
                return null;

            IVsProject2 vsProject = project.RawHandle as IVsProject2;
            if (vsProject == null)
                return null;

            string document;
            if (!VSErr.Succeeded(vsProject.GetMkDocument(VSItemId.Root, out document)))
                return null;

            if (SvnItem.IsValidPath(document))
            {
                return SvnItem.MakeRelative(
                    SolutionSettings.SolutionFilename,
                    document);
            }

            return document;
        }

        private Uri GetProjectUri(ISccProjectInfo info)
        {
            if (info.SccBaseUri != null)
                return info.SccBaseUri;

            if (info.SccBaseDirectory == null)
                return null;

            SvnItem item = StatusCache[info.SccBaseDirectory];
            return item != null ? item.Uri : null;
        }

        private void slnBindBrowse_Click(object sender, EventArgs e)
        {
            using (ChangeSolutionRoot sr = new ChangeSolutionRoot())
            {
                sr.ShowDialog(Context);
                RefreshGrid();
            }
        }
    }
}
