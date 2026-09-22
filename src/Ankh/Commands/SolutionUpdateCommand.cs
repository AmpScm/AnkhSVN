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
using System.Diagnostics;
using System.Windows.Forms;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.UI.Commands;
using Ankh.VS;
using SharpSvn;

namespace Ankh.Commands
{
    [SvnCommand(AnkhCommand.PendingChangesUpdateLatest, HideWhenDisabled = false)]
    [SvnCommand(AnkhCommand.SolutionUpdateLatest)]
    [SvnCommand(AnkhCommand.SolutionUpdateSpecific)]
    [SvnCommand(AnkhCommand.ProjectUpdateLatest)]
    [SvnCommand(AnkhCommand.ProjectUpdateSpecific)]
    [SvnCommand(AnkhCommand.FolderUpdateSpecific)]
    [SvnCommand(AnkhCommand.FolderUpdateLatest)]
    class SolutionUpdateCommand : CommandBase
    {
        static bool IsSolutionCommand(AnkhCommand command)
        {
            return SolutionUpdateLogic.GetScope(command) == UpdateCommandScope.Solution;
        }

        static bool IsFolderCommand(AnkhCommand command)
        {
            return SolutionUpdateLogic.GetScope(command) == UpdateCommandScope.Folder;
        }

        static bool IsHeadCommand(AnkhCommand command)
        {
            return SolutionUpdateLogic.IsHeadCommand(command);
        }

        static IEnumerable<SccProject> GetSelectedProjects(BaseCommandEventArgs e)
        {
            foreach (SccProject p in e.Selection.GetSelectedProjects(false))
            {
                yield return p;
            }
        }

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.State.SolutionBuilding || e.State.Debugging || e.State.SolutionOpening)
            {
                e.Enabled = false;
                return;
            }

            if (IsSolutionCommand(e.Command))
            {
                IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();
                if (settings == null || string.IsNullOrEmpty(settings.ProjectRoot))
                {
                    e.Enabled = false;
                    return;
                }

                if (!settings.ProjectRootSvnItem.IsVersioned)
                    e.Enabled = false;
            }
            else if (IsFolderCommand(e.Command))
            {
                bool forHead = IsHeadCommand(e.Command);
                bool foundOne = false;
                Uri root = null;
                foreach (SvnItem dir in e.Selection.GetSelectedSvnItems(false))
                {
                    if (!dir.IsDirectory || !dir.IsVersioned)
                    {
                        e.Enabled = false;
                        break;
                    }

                    foundOne = true;

                    if (!forHead)
                    {
                        Uri reposRoot = dir.WorkingCopy.RepositoryRoot;

                        if (root != reposRoot)
                        {
                            if (root == null)
                                reposRoot = root;
                            else
                            {
                                e.Enabled = false;
                                break;
                            }
                        }
                    }
                }

                if (!foundOne)
                {
                    e.Enabled = false;
                }
            }
            else
            {
                IProjectFileMapper pfm = null;
                ISvnStatusCache fsc = null;

                Uri rootUrl = null;
                foreach (SccProject p in GetSelectedProjects(e))
                {
                    if (pfm == null)
                        pfm = e.GetService<IProjectFileMapper>();

                    ISccProjectInfo pi = pfm.GetProjectInfo(p);

                    if (pi == null || pi.ProjectDirectory == null)
                        continue;

                    if (fsc == null)
                        fsc = e.GetService<ISvnStatusCache>();

                    SvnItem rootItem = fsc[pi.ProjectDirectory];

                    if (!rootItem.IsVersioned)
                        continue;

                    if (IsHeadCommand(e.Command))
                        return; // Ok, we can update

                    if (rootUrl == null)
                        rootUrl = rootItem.WorkingCopy.RepositoryRoot;
                    else if (rootUrl != rootItem.WorkingCopy.RepositoryRoot)
                    {
                        // Multiple repositories selected; can't choose uniform version
                        e.Enabled = false;
                        return;
                    }
                }

                if (rootUrl == null)
                    e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IAnkhServiceEvents ci = e.GetService<IAnkhServiceEvents>();

            if (ci != null)
                ci.OnLastChanged(new LastChangedEventArgs(null, null));

            IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();
            ISvnStatusCache cache = e.GetService<ISvnStatusCache>();
            IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();

            UpdateExecutionSettings updateSettings;
            if (!TryGetUpdateSettings(
                    e,
                    settings,
                    cache,
                    mapper,
                    out updateSettings))
            {
                return;
            }

            SvnRevision rev = updateSettings.Revision;
            bool allowUnversionedObstructions = updateSettings.AllowUnversionedObstructions;
            bool updateExternals = updateSettings.UpdateExternals;
            bool setDepthInfinity = updateSettings.SetDepthInfinity;
            Uri reposRoot = updateSettings.RepositoryRoot;

            SolutionUpdatePlan plan = new SolutionUpdatePlan();

            // Get a list of all documents below the specified paths that are open in editors inside VS
            HybridCollection<string> lockPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            IAnkhOpenDocumentTracker documentTracker = e.GetService<IAnkhOpenDocumentTracker>();

            foreach (SvnItem item in GetAllUpdateRoots(e))
            {
                // GetAllUpdateRoots can (and probably will) return duplicates!
                SvnWorkingCopy wc = item.WorkingCopy;

                if (!plan.TryAddRoot(
                        item.FullPath,
                        item.IsVersioned,
                        IsHeadCommand(e.Command),
                        reposRoot,
                        wc == null ? null : wc.RepositoryRoot,
                        wc == null ? null : wc.FullPath))
                {
                    continue;
                }

                foreach (string file in documentTracker.GetDocumentsBelow(item.FullPath))
                {
                    if (!lockPaths.Contains(file))
                        lockPaths.Add(file);
                }
            }

            documentTracker.SaveDocuments(lockPaths); // Make sure all files are saved before updating/merging!

            using (DocumentLock lck = documentTracker.LockDocuments(lockPaths, DocumentLockType.NoReload))
            using (lck.MonitorChangesForReload())
            {
                SvnUpdateResult updateResult = null;

                ProgressRunnerArgs pa = new ProgressRunnerArgs();
                pa.CreateLog = true;

                string title;

                if (IsSolutionCommand(e.Command))
                    title = CommandStrings.UpdatingSolution;
                else if (IsFolderCommand(e.Command))
                    title = CommandStrings.UpdatingFolder;
                else
                    title = CommandStrings.UpdatingProject;

                IConflictHandler ih = e.Context.GetService<IConflictHandler>();

                e.GetService<IProgressRunner>().RunModal(title, pa,
                    delegate(object sender, ProgressWorkerArgs a)
                    {
                        PerformUpdate(e, a, rev, allowUnversionedObstructions, updateExternals, setDepthInfinity, plan.Groups, ih, out updateResult);
                    });

                if (ci != null && updateResult != null && IsSolutionCommand(e.Command))
                {
                    ci.OnLastChanged(new LastChangedEventArgs(CommandStrings.UpdatedToTitle, updateResult.Revision.ToString()));
                }
            }
        }

        sealed class UpdateExecutionSettings
        {
            public SvnRevision Revision { get; set; }
            public bool AllowUnversionedObstructions { get; set; }
            public bool UpdateExternals { get; set; }
            public bool SetDepthInfinity { get; set; }
            public Uri RepositoryRoot { get; set; }
        }

        static UpdateExecutionSettings SettingsFromDialog(
            UpdateDialog dialog,
            Uri repositoryRoot)
        {
            return new UpdateExecutionSettings
            {
                Revision = dialog.Revision,
                AllowUnversionedObstructions = dialog.AllowUnversionedObstructions,
                UpdateExternals = dialog.UpdateExternals,
                SetDepthInfinity = dialog.SetDepthInfinty,
                RepositoryRoot = repositoryRoot
            };
        }

        static bool TryGetUpdateSettings(
            CommandEventArgs e,
            IAnkhSolutionSettings settings,
            ISvnStatusCache cache,
            IProjectFileMapper mapper,
            out UpdateExecutionSettings result)
        {
            if (SolutionUpdateLogic.UsesImplicitHeadRevision(e.Command, e.DontPrompt))
            {
                result = new UpdateExecutionSettings
                {
                    Revision = SvnRevision.Head,
                    UpdateExternals = true,
                    SetDepthInfinity = true
                };
                return true;
            }

            switch (SolutionUpdateLogic.GetScope(e.Command))
            {
                case UpdateCommandScope.Solution:
                    return TryGetSolutionUpdateSettings(e, settings, out result);

                case UpdateCommandScope.Folder:
                    return TryGetFolderUpdateSettings(e, out result);

                default:
                    return TryGetProjectUpdateSettings(e, cache, mapper, out result);
            }
        }

        static bool TryGetSolutionUpdateSettings(
            CommandEventArgs e,
            IAnkhSolutionSettings settings,
            out UpdateExecutionSettings result)
        {
            SvnItem projectItem = settings.ProjectRootSvnItem;
            Debug.Assert(projectItem != null, "Has item");

            using (UpdateDialog dialog = new UpdateDialog())
            {
                dialog.ItemToUpdate = projectItem;
                dialog.Revision = SvnRevision.Head;

                if (dialog.ShowDialog(e.Context) != DialogResult.OK)
                {
                    result = null;
                    return false;
                }

                result = SettingsFromDialog(dialog, null);
                return true;
            }
        }

        static bool TryGetFolderUpdateSettings(
            CommandEventArgs e,
            out UpdateExecutionSettings result)
        {
            SvnItem dirItem = EnumTools.GetFirst(
                e.Selection.GetSelectedSvnItems(false));

            Debug.Assert(
                dirItem != null && dirItem.IsDirectory && dirItem.IsVersioned);

            using (UpdateDialog dialog = new UpdateDialog())
            {
                dialog.Text = CommandStrings.UpdateFolder;
                dialog.FolderLabelText = CommandStrings.UpdateFolderLabel;
                dialog.ItemToUpdate = dirItem;
                dialog.Revision = SvnRevision.Head;

                if (dialog.ShowDialog(e.Context) != DialogResult.OK)
                {
                    result = null;
                    return false;
                }

                result = SettingsFromDialog(dialog, null);
                return true;
            }
        }

        static bool TryGetProjectUpdateSettings(
            CommandEventArgs e,
            ISvnStatusCache cache,
            IProjectFileMapper mapper,
            out UpdateExecutionSettings result)
        {
            SvnItem singleItem = null;
            SvnOrigin origin = null;
            Uri repositoryRoot = null;

            foreach (SccProject project in GetSelectedProjects(e))
            {
                ISccProjectInfo projectInfo = mapper.GetProjectInfo(project);
                if (projectInfo == null || projectInfo.ProjectDirectory == null)
                    continue;

                SvnItem item = cache[projectInfo.ProjectDirectory];
                if (!item.IsVersioned)
                    continue;

                if (singleItem == null && origin == null)
                {
                    singleItem = item;
                    origin = new SvnOrigin(item);
                    repositoryRoot = item.WorkingCopy.RepositoryRoot;
                }
                else
                {
                    singleItem = null;
                    origin = new SvnOrigin(
                        SolutionUpdateLogic.GetCommonAncestorUri(origin.Uri, item.Uri),
                        origin.RepositoryRoot);
                }
            }

            Debug.Assert(origin != null);

            using (UpdateDialog dialog = new UpdateDialog())
            {
                dialog.Text = CommandStrings.UpdateProject;

                if (singleItem != null)
                    dialog.ItemToUpdate = singleItem;
                else
                {
                    dialog.SvnOrigin = origin;
                    dialog.SetMultiple(true);
                }

                dialog.Revision = SvnRevision.Head;

                if (dialog.ShowDialog(e.Context) != DialogResult.OK)
                {
                    result = null;
                    return false;
                }

                result = SettingsFromDialog(dialog, repositoryRoot);
                return true;
            }
        }

        private static void PerformUpdate(CommandEventArgs e, ProgressWorkerArgs wa, SvnRevision rev, bool allowUnversionedObstructions, bool updateExternals, bool setDepthInfinity, IEnumerable<UpdateGroup> groups, IConflictHandler ih, out SvnUpdateResult updateResult)
        {
            SvnUpdateArgs ua = new SvnUpdateArgs();
            ua.Revision = rev;
            ua.AllowObstructions = allowUnversionedObstructions;
            ua.IgnoreExternals = !updateExternals;
            ua.KeepDepth = setDepthInfinity;
            updateResult = null;

            HybridCollection<string> handledExternals = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            ua.Notify += delegate(object ss, SvnNotifyEventArgs ee)
            {
                if (ee.Action == SvnNotifyAction.UpdateExternal)
                {
                    if (!handledExternals.Contains(ee.FullPath))
                        handledExternals.Add(ee.FullPath);
                }
            };

            ih?.RegisterConflictHandler(ua, wa.Synchronizer);

            foreach (UpdateGroup group in groups)
            {
                if (handledExternals.Contains(group.WorkingCopyRoot))
                    continue;

                group.Nodes.Sort(StringComparer.OrdinalIgnoreCase);

                // Currently Subversion runs update per item passed and in
                // Subversion 1.6 passing each item separately is actually 
                // a tiny bit faster than passing them all at once. 
                // (sleep_for_timestamp fails its fast route)
                foreach (string path in group.Nodes)
                {
                    if (handledExternals.Contains(path))
                        continue;

                    SvnUpdateResult result;
                    wa.Client.Update(path, ua, out result);

                    if (updateResult == null)
                        updateResult = result; // Return the primary update as version for output
                }
            }
        }

        private static IEnumerable<SvnItem> GetAllUpdateRoots(CommandEventArgs e)
        {
            // Duplicate handling is handled above this method!
            ISvnSolutionLayout pls = e.GetService<ISvnSolutionLayout>();
            if (IsSolutionCommand(e.Command))
                foreach (SvnItem item in pls.GetUpdateRoots(null))
                {
                    yield return item;
                }
            else if (IsFolderCommand(e.Command))
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                {
                    // Everything is checked in the OnUpdate
                    yield return item;
                }
            else
                foreach (SccProject project in GetSelectedProjects(e))
                {
                    foreach (SvnItem item in pls.GetUpdateRoots(project))
                    {
                        yield return item;
                    }
                }
        }
    }
}
