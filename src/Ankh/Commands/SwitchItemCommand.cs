// $Id$
//
// Copyright 2003-2008 The AnkhSVN Project
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

// $Id$
using System;
using System.Collections;
using System.Windows.Forms;
using Ankh.UI;

using SharpSvn;
using Ankh.Ids;
using Ankh.VS;
using Ankh.Scc;
using Ankh.Selection;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to switch current item to a different URL.
    /// </summary>
    [Command(AnkhCommand.SwitchItem)]
    [Command(AnkhCommand.SolutionSwitchDialog)]
    [Command(AnkhCommand.SwitchProject)]
    class SwitchItemCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {

            IFileStatusCache statusCache;
            switch (e.Command)
            {
                case AnkhCommand.SolutionSwitchDialog:
                    IAnkhSolutionSettings solutionSettings = e.GetService<IAnkhSolutionSettings>();
                    if (solutionSettings == null || string.IsNullOrEmpty(solutionSettings.ProjectRoot))
                    {
                        e.Enabled = false;
                        return;
                    }
                    statusCache = e.GetService<IFileStatusCache>();
                    SvnItem solutionItem = statusCache[solutionSettings.ProjectRoot];
                    if (!solutionItem.IsVersioned)
                    {
                        e.Enabled = false;
                        return;
                    }
                    break;

                case AnkhCommand.SwitchProject:
                    statusCache = e.GetService<IFileStatusCache>();
                    IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();
                    foreach (SvnProject item in e.Selection.GetSelectedProjects(true))
                    {
                        ISvnProjectInfo pi = pfm.GetProjectInfo(item);

                        if (pi == null || pi.ProjectDirectory == null)
                        {
                            e.Enabled = false;
                            return;
                        }

                        SvnItem projectItem = statusCache[pi.ProjectDirectory];
                        if (!projectItem.IsVersioned)
                        {
                            e.Enabled = false;
                            return;
                        }
                    }
                    break;

                case AnkhCommand.SwitchItem:
                    bool foundOne = false, error = false;
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (item.IsVersioned && !foundOne)
                            foundOne = true;
                        else
                        {
                            error = true;
                            break;
                        }
                    }

                    e.Enabled = foundOne && !error;
                    break;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            SvnItem theItem = null;
            string path;

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

            Uri uri;

            using (SvnClient cl = e.GetService<ISvnClientPool>().GetNoUIClient())
            {
                uri = cl.GetUriFromWorkingCopy(path);
            }

            SvnUriTarget target;
            SvnRevision revision = SvnRevision.None;

            if (e.Argument is string)
                target = SvnUriTarget.FromString((string)e.Argument);
            else if (e.Argument is Uri)
                target = (Uri)e.Argument;
            else
            {
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
                }
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
            {
                lck.MonitorChanges();

                // TODO: Monitor conflicts!!

                e.GetService<IProgressRunner>().RunModal(
                    "Switching",
                    delegate(object sender, ProgressWorkerArgs a)
                    {
                        SvnSwitchArgs args = new SvnSwitchArgs();
                        if (revision != SvnRevision.None)
                            args.Revision = revision;

                        e.GetService<IConflictHandler>().RegisterConflictHandler(args, a.Synchronizer);
                        a.Client.Switch(path, target, args);
                    });


                // This fixes the PC 'Working on' combo 
                string solution = e.GetService<IAnkhSolutionSettings>().SolutionFilename;
                IFileStatusCache cache = e.GetService<IFileStatusCache>();

                if (!string.IsNullOrEmpty(solution))
                    cache.MarkDirty(solution);
                if (!string.IsNullOrEmpty(projectRoot))
                    cache.MarkDirty(projectRoot);
                // Working on fix


                lck.ReloadModified();
            }
        }
    }
}
