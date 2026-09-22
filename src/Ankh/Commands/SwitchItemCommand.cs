// Copyright 2003-2009 The AnkhSVN Project
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
using System.Windows.Forms;

using SharpSvn;

using Ankh.Scc;
using Ankh.Selection;
using Ankh.UI.Commands;
using Ankh.VS;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to switch current item to a different URL.
    /// </summary>
    [SvnCommand(AnkhCommand.SwitchItem)]
    [SvnCommand(AnkhCommand.SolutionSwitchDialog)]
    [SvnCommand(AnkhCommand.SwitchProject)]
    class SwitchItemCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SolutionExists || e.State.SolutionBuilding || e.State.Debugging || e.State.SolutionOpening)
            {
                e.Enabled = false;
                return;
            }
            switch (e.Command)
            {
                case AnkhCommand.SolutionSwitchDialog:
                    IAnkhSolutionSettings solutionSettings = e.GetService<IAnkhSolutionSettings>();
                    SvnItem solutionItem = solutionSettings != null ? solutionSettings.ProjectRootSvnItem : null;
                    if (solutionItem == null || !solutionItem.IsVersioned || solutionItem.IsNewAddition)
                    {
                        e.Enabled = false;
                        return;
                    }
                    break;

                case AnkhCommand.SwitchProject:
                    SccProject oneProject = EnumTools.GetSingle(e.Selection.GetSelectedProjects(false));

                    if (oneProject == null)
                    {
                        e.Enabled = false;
                        return;
                    }

                    IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();
                    ISccProjectInfo pi = pfm.GetProjectInfo(oneProject);

                    if (pi == null || pi.ProjectDirectory == null)
                    {
                        e.Enabled = false;
                        return;
                    }

                    SvnItem projectItem = e.GetService<ISvnStatusCache>()[pi.ProjectDirectory];

                    if (projectItem == null || !projectItem.IsVersioned || projectItem.IsNewAddition)
                        e.Enabled = false;
                    break;

                case AnkhCommand.SwitchItem:
                    SvnItem oneItem = EnumTools.GetSingle(e.Selection.GetSelectedSvnItems(false));

                    if (oneItem == null || !oneItem.IsVersioned || oneItem.IsNewAddition)
                        e.Enabled = false;
                    break;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            string projectRoot = e.GetService<IAnkhSolutionSettings>().ProjectRoot;

            string path;
            if (!TryGetSwitchPath(e, projectRoot, out path))
                return;

            ISvnStatusCache statusCache = e.GetService<ISvnStatusCache>();
            SvnItem pathItem = statusCache[path];
            Uri uri = pathItem.Uri;

            if (uri == null)
                return; // Should never happen on a real working copy

            SvnUriTarget target;
            SvnRevision revision;
            bool allowObstructions;
            if (!TryGetSwitchTarget(
                    e,
                    path,
                    pathItem,
                    uri,
                    out target,
                    out revision,
                    out allowObstructions))
            {
                return;
            }

            // Get a list of all documents below the specified paths that are open in editors inside VS
            HybridCollection<string> lockPaths =
                new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            IAnkhOpenDocumentTracker documentTracker =
                e.GetService<IAnkhOpenDocumentTracker>();

            foreach (string file in documentTracker.GetDocumentsBelow(path))
            {
                if (!lockPaths.Contains(file))
                    lockPaths.Add(file);
            }

            documentTracker.SaveDocuments(lockPaths);

            using (DocumentLock lck =
                documentTracker.LockDocuments(lockPaths, DocumentLockType.NoReload))
            using (lck.MonitorChangesForReload())
            {
                Uri newRepositoryRoot;
                RunInitialSwitch(
                    e,
                    path,
                    pathItem,
                    target,
                    revision,
                    allowObstructions,
                    out newRepositoryRoot);

                RelocateAndRetryIfRequested(
                    e,
                    statusCache,
                    path,
                    pathItem,
                    target,
                    revision,
                    allowObstructions,
                    newRepositoryRoot);
            }
        }

        static bool TryGetSwitchPath(
            CommandEventArgs e,
            string projectRoot,
            out string path)
        {
            if (e.Command == AnkhCommand.SolutionSwitchDialog)
            {
                path = projectRoot;
                return true;
            }

            if (e.Command == AnkhCommand.SwitchProject)
            {
                IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();

                foreach (SccProject project in
                    e.Selection.GetSelectedProjects(true))
                {
                    ISccProjectInfo projectInfo =
                        mapper.GetProjectInfo(project);

                    if (projectInfo == null)
                        continue;

                    path = projectInfo.ProjectDirectory;
                    return !string.IsNullOrEmpty(path);
                }

                path = null;
                return false;
            }

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsVersioned)
                {
                    path = item.FullPath;
                    return true;
                }

                path = null;
                return false;
            }

            path = null;
            return false;
        }

        static bool TryGetSwitchTarget(
            CommandEventArgs e,
            string path,
            SvnItem pathItem,
            Uri currentUri,
            out SvnUriTarget target,
            out SvnRevision revision,
            out bool allowObstructions)
        {
            allowObstructions = false;
            revision = SvnRevision.None;

            if (e.Argument is string)
            {
                target = SvnUriTarget.FromString(
                    (string)e.Argument,
                    true);
                revision =
                    target.Revision != SvnRevision.None
                        ? target.Revision
                        : SvnRevision.Head;
                return true;
            }

            if (e.Argument is Uri)
            {
                target = (Uri)e.Argument;
                return true;
            }

            using (SwitchDialog dialog = new SwitchDialog())
            {
                dialog.Context = e.Context;
                dialog.LocalPath = path;
                dialog.RepositoryRoot =
                    pathItem.WorkingCopy.RepositoryRoot;
                dialog.SwitchToUri = currentUri;
                dialog.Revision = SvnRevision.Head;

                if (dialog.ShowDialog(e.Context) != DialogResult.OK)
                {
                    target = null;
                    return false;
                }

                target = dialog.SwitchToUri;
                revision = dialog.Revision;
                allowObstructions =
                    dialog.AllowUnversionedObstructions;
                return true;
            }
        }

        static void RunInitialSwitch(
            CommandEventArgs e,
            string path,
            SvnItem pathItem,
            SvnUriTarget target,
            SvnRevision revision,
            bool allowObstructions,
            out Uri newRepositoryRoot)
        {
            Uri repositoryRoot = null;

            e.GetService<IProgressRunner>().RunModal(
                CommandStrings.SwitchingTitle,
                delegate(object sender, ProgressWorkerArgs a)
                {
                    SvnSwitchArgs args = new SvnSwitchArgs();
                    args.AllowObstructions = allowObstructions;
                    args.AddExpectedError(
                        SvnErrorCode.SVN_ERR_WC_INVALID_SWITCH);

                    if (revision != SvnRevision.None)
                        args.Revision = revision;

                    e.GetService<IConflictHandler>()
                        .RegisterConflictHandler(
                            args,
                            a.Synchronizer);

                    if (a.Client.Switch(path, target, args))
                        return;

                    if (args.LastException.SvnErrorCode
                        != SvnErrorCode.SVN_ERR_WC_INVALID_SWITCH)
                    {
                        return;
                    }

                    SvnInfoEventArgs info;
                    if (!a.Client.GetInfo(target, out info))
                        return;

                    if (pathItem.WorkingCopy.RepositoryId
                        != info.RepositoryId)
                    {
                        e.Context.GetService<IAnkhDialogOwner>()
                            .MessageBox.Show(
                                "Cannot switch to different repository because the repository UUIDs are different",
                                "Cannot switch",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                    }
                    else if (pathItem.WorkingCopy.RepositoryRoot
                        != info.RepositoryRoot)
                    {
                        repositoryRoot = info.RepositoryRoot;
                    }
                    else if (pathItem.WorkingCopy.RepositoryId
                        == Guid.Empty)
                    {
                        throw args.LastException;
                    }
                });

            newRepositoryRoot = repositoryRoot;
        }

        static void RelocateAndRetryIfRequested(
            CommandEventArgs e,
            ISvnStatusCache statusCache,
            string path,
            SvnItem pathItem,
            SvnUriTarget target,
            SvnRevision revision,
            bool allowObstructions,
            Uri newRepositoryRoot)
        {
            if (newRepositoryRoot == null)
                return;

            DialogResult relocate =
                e.Context.GetService<IAnkhDialogOwner>()
                    .MessageBox.Show(
                        string.Format(
                            "The repository root specified is different from the one in your "
                            + "working copy. Would you like to relocate '{0}' from '{1}' to '{2}'?",
                            pathItem.WorkingCopy.FullPath,
                            pathItem.WorkingCopy.RepositoryRoot,
                            newRepositoryRoot),
                        "Relocate",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

            if (relocate != DialogResult.Yes)
                return;

            string wcRoot = pathItem.WorkingCopy.FullPath;
            try
            {
                e.GetService<IProgressRunner>().RunModal(
                    CommandStrings.RelocatingTitle,
                    delegate(object sender, ProgressWorkerArgs a)
                    {
                        a.Client.Relocate(
                            wcRoot,
                            pathItem.WorkingCopy.RepositoryRoot,
                            newRepositoryRoot);
                    });
            }
            finally
            {
                statusCache.MarkDirtyRecursive(wcRoot);
                e.GetService<IFileStatusMonitor>()
                    .ScheduleGlyphUpdate(
                        statusCache.GetCachedBelow(wcRoot));
            }

            DialogResult retry =
                e.Context.GetService<IAnkhDialogOwner>()
                    .MessageBox.Show(
                        string.Format(
                            "Would you like to try to switch '{0}' to '{1}' again?",
                            path,
                            target),
                        "Switch",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

            if (retry != DialogResult.Yes)
                return;

            e.GetService<IProgressRunner>().RunModal(
                CommandStrings.SwitchingTitle,
                delegate(object sender, ProgressWorkerArgs a)
                {
                    SvnSwitchArgs args = new SvnSwitchArgs();

                    if (revision != SvnRevision.None)
                        args.Revision = revision;

                    args.AllowObstructions = allowObstructions;

                    e.GetService<IConflictHandler>()
                        .RegisterConflictHandler(
                            args,
                            a.Synchronizer);
                    a.Client.Switch(path, target, args);
                });
        }

    }
}
