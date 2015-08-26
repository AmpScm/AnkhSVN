// $Id$
//
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
                    SvnItem solutionItem = solutionSettings.ProjectRootSvnItem;
                    if (solutionItem == null || !solutionItem.IsVersioned || solutionItem.IsNewAddition)
                    {
                        e.Enabled = false;
                        return;
                    }
                    break;

                case AnkhCommand.SwitchProject:
                    SvnProject oneProject = EnumTools.GetSingle(e.Selection.GetSelectedProjects(false));

                    if (oneProject == null)
                    {
                        e.Enabled = false;
                        return;
                    }

                    IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();
                    ISvnProjectInfo pi = pfm.GetProjectInfo(oneProject);

                    if (pi == null || pi.ProjectDirectory == null)
                    {
                        e.Enabled = false;
                        return;
                    }

                    SvnItem projectItem = e.GetService<IFileStatusCache>()[pi.ProjectDirectory];

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
            SvnItem theItem = null;
            string path;
            bool allowObstructions = false;

            string projectRoot = e.GetService<IAnkhSolutionSettings>().ProjectRoot;

            if (e.Command == AnkhCommand.SolutionSwitchDialog)
                path = projectRoot;
            else if (e.Command == AnkhCommand.SwitchProject)
            {
                IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
                path = null;

                foreach (SvnProject item in e.Selection.GetSelectedProjects(true))
                {
                    ISvnProjectInfo pi = mapper.GetProjectInfo(item);

                    if (pi == null)
                        continue;

                    path = pi.ProjectDirectory;
                    break;
                }

                if (string.IsNullOrEmpty(path))
                    return;
            }
            else
            {
                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                {
                    if (item.IsVersioned)
                    {
                        theItem = item;
                        break;
                    }
                    return;
                }
                path = theItem.FullPath;
            }

            IFileStatusCache statusCache = e.GetService<IFileStatusCache>();

            SvnItem pathItem = statusCache[path];
            Uri uri = pathItem.Uri;

            if (uri == null)
                return; // Should never happen on a real workingcopy

            SvnUriTarget target;
            SvnRevision revision = SvnRevision.None;

            if (e.Argument is string)
            {
                target = SvnUriTarget.FromString((string)e.Argument, true);
                revision = (target.Revision != SvnRevision.None) ? target.Revision : SvnRevision.Head;
            }
            else if (e.Argument is Uri)
                target = (Uri)e.Argument;
            else
                using (SwitchDialog dlg = new SwitchDialog())
                {
                    dlg.Context = e.Context;

                    dlg.LocalPath = path;
                    dlg.RepositoryRoot = e.GetService<IFileStatusCache>()[path].WorkingCopy.RepositoryRoot;
                    dlg.SwitchToUri = uri;
                    dlg.Revision = SvnRevision.Head;

                    if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    target = dlg.SwitchToUri;
                    revision = dlg.Revision;
                    allowObstructions = dlg.AllowUnversionedObstructions;
                }

            // Get a list of all documents below the specified paths that are open in editors inside VS
            HybridCollection<string> lockPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            IAnkhOpenDocumentTracker documentTracker = e.GetService<IAnkhOpenDocumentTracker>();

            foreach (string file in documentTracker.GetDocumentsBelow(path))
            {
                if (!lockPaths.Contains(file))
                    lockPaths.Add(file);
            }

            documentTracker.SaveDocuments(lockPaths); // Make sure all files are saved before merging!

            using (DocumentLock lck = documentTracker.LockDocuments(lockPaths, DocumentLockType.NoReload))
            using (lck.MonitorChangesForReload())
            {
                Uri newRepositoryRoot = null;
                e.GetService<IProgressRunner>().RunModal(CommandStrings.SwitchingTitle,
                    delegate(object sender, ProgressWorkerArgs a)
                    {
                        SvnSwitchArgs args = new SvnSwitchArgs();
                        args.AllowObstructions = allowObstructions;
                        args.AddExpectedError(SvnErrorCode.SVN_ERR_WC_INVALID_SWITCH);

                        if (revision != SvnRevision.None)
                            args.Revision = revision;

                        e.GetService<IConflictHandler>().RegisterConflictHandler(args, a.Synchronizer);
                        if (!a.Client.Switch(path, target, args))
                        {
                            if (args.LastException.SvnErrorCode != SvnErrorCode.SVN_ERR_WC_INVALID_SWITCH)
                                return;

                            // source/target repository is different, check if we can fix this by relocating
                            SvnInfoEventArgs iea;
                            if (a.Client.GetInfo(target, out iea))
                            {
                                if (pathItem.WorkingCopy.RepositoryId != iea.RepositoryId)
                                {
                                    e.Context.GetService<IAnkhDialogOwner>()
                                        .MessageBox.Show("Cannot switch to different repository because the repository UUIDs are different",
                                        "Cannot switch", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                                else if (pathItem.WorkingCopy.RepositoryRoot != iea.RepositoryRoot)
                                {
                                    newRepositoryRoot = iea.RepositoryRoot;
                                }
                                else if (pathItem.WorkingCopy.RepositoryId == Guid.Empty)
                                {
                                    // No UUIDs and RepositoryRoot equal. Throw/show error?

                                    throw args.LastException;
                                }
                            }
                        }
                    });

                if (newRepositoryRoot != null && DialogResult.Yes == e.Context.GetService<IAnkhDialogOwner>()
                   .MessageBox.Show(string.Format("The repository root specified is different from the one in your " +
                   "working copy. Would you like to relocate '{0}' from '{1}' to '{2}'?",
                   pathItem.WorkingCopy.FullPath,
                   pathItem.WorkingCopy.RepositoryRoot, newRepositoryRoot),
                   "Relocate", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    // We can fix this by relocating
                    string wcRoot = pathItem.WorkingCopy.FullPath;
                    try
                    {
                        e.GetService<IProgressRunner>().RunModal(
                            CommandStrings.RelocatingTitle,
                            delegate(object sender, ProgressWorkerArgs a)
                            {
                                a.Client.Relocate(wcRoot, pathItem.WorkingCopy.RepositoryRoot, newRepositoryRoot);
                            });
                    }
                    finally
                    {
                        statusCache.MarkDirtyRecursive(wcRoot);
                        e.GetService<IFileStatusMonitor>().ScheduleGlyphUpdate(SvnItem.GetPaths(statusCache.GetCachedBelow(wcRoot)));
                    }


                    if (DialogResult.Yes == e.Context.GetService<IAnkhDialogOwner>()
                        .MessageBox.Show(string.Format("Would you like to try to switch '{0}' to '{1}' again?",
                        path, target),
                        "Switch", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                    {
                        // Try to switch again
                        e.GetService<IProgressRunner>().RunModal(
                        CommandStrings.SwitchingTitle,
                        delegate(object sender, ProgressWorkerArgs a)
                        {
                            SvnSwitchArgs args = new SvnSwitchArgs();

                            if (revision != SvnRevision.None)
                                args.Revision = revision;

                            args.AllowObstructions = allowObstructions;

                            e.GetService<IConflictHandler>().RegisterConflictHandler(args, a.Synchronizer);
                            a.Client.Switch(path, target, args);
                        });
                    }
                }
            }
        }
    }
}
