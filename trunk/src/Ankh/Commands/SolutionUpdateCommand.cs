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
using Ankh.VS;
using Ankh.Selection;
using SharpSvn;
using Ankh.Scc;
using Ankh.UI.Commands;
using System.Diagnostics;
using System.Windows.Forms;

namespace Ankh.Commands
{
    [Command(AnkhCommand.PendingChangesUpdateLatest, HideWhenDisabled = false)]
    [Command(AnkhCommand.SolutionUpdateLatest)]
    [Command(AnkhCommand.SolutionUpdateSpecific)]
    [Command(AnkhCommand.ProjectUpdateLatest)]
    [Command(AnkhCommand.ProjectUpdateSpecific)]
    [Command(AnkhCommand.FolderUpdateSpecific)]
    [Command(AnkhCommand.FolderUpdateLatest)]
    class SolutionUpdateCommand : CommandBase
    {
        static bool IsSolutionCommand(AnkhCommand command)
        {
            switch (command)
            {
                case AnkhCommand.SolutionUpdateLatest:
                case AnkhCommand.SolutionUpdateSpecific:
                case AnkhCommand.PendingChangesUpdateLatest:
                    return true;
                default:
                    return false;
            }
        }

        static bool IsFolderCommand(AnkhCommand command)
        {
            switch (command)
            {
                case AnkhCommand.FolderUpdateLatest:
                case AnkhCommand.FolderUpdateSpecific:
                    return true;
                default:
                    return false;
            }
        }

        static bool IsHeadCommand(AnkhCommand command)
        {
            switch (command)
            {
                case AnkhCommand.SolutionUpdateLatest:
                case AnkhCommand.ProjectUpdateLatest:
                case AnkhCommand.PendingChangesUpdateLatest:
                case AnkhCommand.FolderUpdateLatest:
                    return true;
                default:
                    return false;
            }
        }

        static IEnumerable<SvnProject> GetSelectedProjects(BaseCommandEventArgs e)
        {
            foreach (SvnProject p in e.Selection.GetSelectedProjects(false))
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
                IFileStatusCache fsc = null;

                Uri rootUrl = null;
                foreach (SvnProject p in GetSelectedProjects(e))
                {
                    if (pfm == null)
                        pfm = e.GetService<IProjectFileMapper>();

                    ISvnProjectInfo pi = pfm.GetProjectInfo(p);

                    if (pi == null || pi.ProjectDirectory == null)
                        continue;

                    if (fsc == null)
                        fsc = e.GetService<IFileStatusCache>();

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
            ILastChangeInfo ci = e.GetService<ILastChangeInfo>();

            if (ci != null)
                ci.SetLastChange(null, null);

            SvnRevision rev;
            bool allowUnversionedObstructions = false;
            bool updateExternals = true;
            bool setDepthInfinity = true;

            IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();
            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
            Uri reposRoot = null;

            if (IsHeadCommand(e.Command) || e.DontPrompt)
                rev = SvnRevision.Head;
            else if (IsSolutionCommand(e.Command))
            {
                SvnItem projectItem = settings.ProjectRootSvnItem;

                Debug.Assert(projectItem != null, "Has item");

                using (UpdateDialog ud = new UpdateDialog())
                {
                    ud.ItemToUpdate = projectItem;
                    ud.Revision = SvnRevision.Head;

                    if (ud.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    rev = ud.Revision;
                    allowUnversionedObstructions = ud.AllowUnversionedObstructions;
                    updateExternals = ud.UpdateExternals;
                    setDepthInfinity = ud.SetDepthInfinty;
                }
            }
            else if (IsFolderCommand(e.Command))
            {
                SvnItem dirItem = EnumTools.GetFirst(e.Selection.GetSelectedSvnItems(false));

                Debug.Assert(dirItem != null && dirItem.IsDirectory && dirItem.IsVersioned);

                using (UpdateDialog ud = new UpdateDialog())
                {
                    ud.Text = CommandStrings.UpdateFolder;
                    ud.FolderLabelText = CommandStrings.UpdateFolderLabel;
                    ud.ItemToUpdate = dirItem;
                    ud.Revision = SvnRevision.Head;

                    if (ud.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    rev = ud.Revision;
                    allowUnversionedObstructions = ud.AllowUnversionedObstructions;
                    updateExternals = ud.UpdateExternals;
                    setDepthInfinity = ud.SetDepthInfinty;
                }
            }
            else
            {
                // We checked there was only a single repository to select a revision 
                // from in OnUpdate, so we can suffice with only calculate the path

                SvnItem si = null;
                SvnOrigin origin = null;
                foreach (SvnProject p in GetSelectedProjects(e))
                {
                    ISvnProjectInfo pi = mapper.GetProjectInfo(p);
                    if (pi == null || pi.ProjectDirectory == null)
                        continue;

                    SvnItem item = cache[pi.ProjectDirectory];
                    if (!item.IsVersioned)
                        continue;

                    if (si == null && origin == null)
                    {
                        si = item;
                        origin = new SvnOrigin(item);
                        reposRoot = item.WorkingCopy.RepositoryRoot;
                    }
                    else
                    {
                        si = null;
                        string urlPath1 = origin.Uri.AbsolutePath;
                        string urlPath2 = item.Uri.AbsolutePath;

                        int i = 0;
                        while (i < urlPath1.Length && i < urlPath2.Length
                            && urlPath1[i] == urlPath2[i])
                        {
                            i++;
                        }

                        while (i > 0 && urlPath1[i - 1] != '/')
                            i--;

                        origin = new SvnOrigin(new Uri(origin.Uri, urlPath1.Substring(0, i)), origin.RepositoryRoot);
                    }
                }

                Debug.Assert(origin != null);

                using (UpdateDialog ud = new UpdateDialog())
                {
                    ud.Text = CommandStrings.UpdateProject;

                    if (si != null)
                        ud.ItemToUpdate = si;
                    else
                    {
                        ud.SvnOrigin = origin;
                        ud.SetMultiple(true);
                    }

                    ud.Revision = SvnRevision.Head;

                    if (ud.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    rev = ud.Revision;
                    allowUnversionedObstructions = ud.AllowUnversionedObstructions;
                    updateExternals = ud.UpdateExternals;
                    setDepthInfinity = ud.SetDepthInfinty;
                }
            }

            Dictionary<string, SvnItem> itemsToUpdate = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);
            Dictionary<string, List<string>> groups = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

            // Get a list of all documents below the specified paths that are open in editors inside VS
            HybridCollection<string> lockPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            IAnkhOpenDocumentTracker documentTracker = e.GetService<IAnkhOpenDocumentTracker>();

            foreach (SvnItem item in GetAllUpdateRoots(e))
            {
                // GetAllUpdateRoots can (and probably will) return duplicates!

                if (itemsToUpdate.ContainsKey(item.FullPath) || !item.IsVersioned)
                    continue;

                SvnWorkingCopy wc = item.WorkingCopy;

                if (!IsHeadCommand(e.Command) && reposRoot != null)
                {
                    // Specific revisions are only valid on a single repository!
                    if (wc != null && wc.RepositoryRoot != reposRoot)
                        continue;
                }

                List<string> inWc;

                if (!groups.TryGetValue(wc.FullPath, out inWc))
                {
                    inWc = new List<string>();
                    groups.Add(wc.FullPath, inWc);
                }

                inWc.Add(item.FullPath);
                itemsToUpdate.Add(item.FullPath, item);

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

                e.GetService<IProgressRunner>().RunModal(title, pa,
                    delegate(object sender, ProgressWorkerArgs a)
                    {
                        PerformUpdate(e, a, rev, allowUnversionedObstructions, updateExternals, setDepthInfinity, groups.Values, out updateResult);
                    });

                if (ci != null && updateResult != null && IsSolutionCommand(e.Command))
                {
                    ci.SetLastChange("Updated to:", updateResult.Revision.ToString());
                }
            }
        }

        private static void PerformUpdate(CommandEventArgs e, ProgressWorkerArgs wa, SvnRevision rev, bool allowUnversionedObstructions, bool updateExternals, bool setDepthInfinity, IEnumerable<List<string>> groups, out SvnUpdateResult updateResult)
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
            e.Context.GetService<IConflictHandler>().RegisterConflictHandler(ua, wa.Synchronizer);

            foreach (List<string> group in groups)
            {
                // Currently Subversion runs update per item passed and in
                // Subversion 1.6 passing each item separately is actually 
                // a tiny bit faster than passing them all at once. 
                // (sleep_for_timestamp fails its fast route)
                foreach (string path in group)
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
            IAnkhProjectLayoutService pls = e.GetService<IAnkhProjectLayoutService>();
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
                foreach (SvnProject project in GetSelectedProjects(e))
                {
                    foreach (SvnItem item in pls.GetUpdateRoots(project))
                    {
                        yield return item;
                    }
                }
        }
    }
}
