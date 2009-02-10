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
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Ids;
using Ankh.Scc;
using Ankh.Selection;
using Ankh.UI;
using System.Windows.Forms;
using System.IO;
using SharpSvn;
using Ankh.VS;
using Ankh.UI.SccManagement;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Ankh.Commands
{
    [Command(AnkhCommand.FileSccAddProjectToSubversion)]
    [Command(AnkhCommand.FileSccAddSolutionToSubversion, AlwaysAvailable = true)]
    sealed class AddToSccCommands : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SolutionExists || (e.Command == AnkhCommand.FileSccAddProjectToSubversion && e.State.EmptySolution))
            {
                e.Enabled = false;
                return;
            }

            if (e.State.OtherSccProviderActive)
            {
                e.Enabled = false;
                return; // Only one scc provider can be active at a time
            }

            IAnkhSccService scc = e.GetService<IAnkhSccService>();
            IFileStatusCache fcc = e.GetService<IFileStatusCache>();
            if (scc == null || fcc == null)
            {
                e.Enabled = false;
                return;
            }

            if (!scc.IsSolutionManaged || !fcc[e.Selection.SolutionFilename].IsVersioned)
                return; // Nothing is added unless the solution is added

            if (e.Command == AnkhCommand.FileSccAddSolutionToSubversion)
            {
                e.Enabled = false;
                return;
            }

            foreach (SvnProject p in GetSelection(e.Selection))
            {
                if (!scc.IsProjectManaged(p))
                    return; // Something to enable
            }

            e.Enabled = false;
        }

        private IEnumerable<SvnProject> GetSelection(ISelectionContext iSelectionContext)
        {
            bool foundOne = false;
            foreach (SvnProject pr in iSelectionContext.GetSelectedProjects(true))
            {
                yield return pr;
                foundOne = true;
            }

            if (foundOne)
                yield break;

            foreach (SvnProject pr in iSelectionContext.GetOwnerProjects())
            {
                yield return pr;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
            
            if (cache == null || e.Selection.SolutionFilename == null)
                return;

            SvnItem item = cache[e.Selection.SolutionFilename];

            if (!HandleUnmanagedOrUnversionedSolution(e, item))
                return;

            if (e.Command == AnkhCommand.FileSccAddSolutionToSubversion)
                return;

            SetProjectsManaged(e);
        }

        bool HandleUnmanagedOrUnversionedSolution(CommandEventArgs e, SvnItem solutionItem)
        {
            IAnkhSccService scc = e.GetService<IAnkhSccService>();
            AnkhMessageBox mb = new AnkhMessageBox(e.Context);

            bool shouldActivate = false;
            if (!scc.IsActive)
            {
                if (e.State.OtherSccProviderActive)
                    return false; // Can't switch in this case.. Nothing to do

                // Ankh is not the active provider, we should register as active
                shouldActivate = true;
            }

            if (scc.IsSolutionManaged && solutionItem.IsVersioned)
                return true; // Projects should still be checked

            bool confirmed = false;
            

            if (solutionItem.IsVersioned)
            { /* File is in subversion; just enable */ }
            else if (solutionItem.IsVersionable)
            {
                if (!AddVersionableSolution(e, solutionItem, ref confirmed))
                    return false;
            }
            else
            {
                CheckoutWorkingCopyForSolution(e, ref confirmed);
            }

            if (!confirmed && !e.DontPrompt && !e.IsInAutomation &&
                DialogResult.Yes != mb.Show(string.Format(CommandResources.MarkXAsManaged,
                Path.GetFileName(e.Selection.SolutionFilename)), AnkhId.PlkProduct, MessageBoxButtons.YesNo))
            {
                return false;
            }

            SetSolutionManaged(shouldActivate, solutionItem, scc);

            return true;
        }

        static SvnItem GetVersionedParent(SvnItem child)
        {
            if(!child.IsVersionable)
                return null;

            if (!child.IsVersioned)
                return GetVersionedParent(child.Parent);
            return child;
        }

        bool AddVersionableSolution(CommandEventArgs e, SvnItem solutionItem, ref bool confirmed)
        {
            AnkhMessageBox mb = new AnkhMessageBox(e.Context);
            SvnItem parentDir = GetVersionedParent(solutionItem);

            DialogResult rslt;
            // File is not versioned but is inside a versioned directory
            if (!e.DontPrompt && !e.IsInAutomation)
            {
                if (!solutionItem.Parent.IsVersioned)
                {
                    AddPathToSubversion(e, e.Selection.SolutionFilename);

                    return true;
                }

                rslt = mb.Show(string.Format(CommandResources.AddXToExistingWcY,
                    Path.GetFileName(e.Selection.SolutionFilename),
                    parentDir.FullPath), AnkhId.PlkProduct, MessageBoxButtons.YesNoCancel);

                if (rslt == DialogResult.Cancel)
                    return false;
                else if (rslt == DialogResult.No)
                {
                    // Checkout new working copy
                    CheckoutWorkingCopyForSolution(e, ref confirmed);
                    return true;
                }
                else if (rslt == DialogResult.Yes)
                {
                    // default case: Add to existing workingcopy
                    AddPathToSubversion(e, e.Selection.SolutionFilename);

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                confirmed = true;
                return true;
            }
        }

        void AddPathToSubversion(CommandEventArgs e, string path)
        {
            using (SvnClient cl = e.GetService<ISvnClientPool>().GetNoUIClient())
            {
                SvnAddArgs aa = new SvnAddArgs();
                aa.AddParents = true;
                cl.Add(path, aa);
            }
        }

        void SetSolutionManaged(bool shouldActivate, SvnItem item, IAnkhSccService scc)
        {
            if (shouldActivate)
                scc.RegisterAsPrimarySccProvider();

            scc.SetProjectManaged(null, true);
            item.MarkDirty(); // This clears the solution settings cache to retrieve its properties
        }

        void CheckoutWorkingCopyForSolution(CommandEventArgs e, ref bool confirmed)
        {
            using (SvnClient cl = e.GetService<ISvnClientPool>().GetClient())
            using (Ankh.UI.SccManagement.AddToSubversion dialog = new Ankh.UI.SccManagement.AddToSubversion())
            {
                dialog.PathToAdd = e.Selection.SolutionFilename;
                if (dialog.ShowDialog(e.Context) == DialogResult.OK)
                {
                    confirmed = true;
                    Collection<SvnInfoEventArgs> info;
                    SvnInfoArgs ia = new SvnInfoArgs();
                    ia.ThrowOnError = false;
                    if (!cl.GetInfo(dialog.RepositoryAddUrl, ia, out info))
                    {
                        // Target uri doesn't exist in the repository, let's create
                        if (!RemoteCreateDirectory(e, dialog.Text, dialog.RepositoryAddUrl, cl))
                            return; // Create failed; bail out
                    }

                    // Create working copy
                    SvnCheckOutArgs coArg = new SvnCheckOutArgs();
                    coArg.AllowObstructions = true;
                    cl.CheckOut(dialog.RepositoryAddUrl, dialog.WorkingCopyDir, coArg);

                    // Add solutionfile so we can set properties (set managed)
                    AddPathToSubversion(e, e.Selection.SolutionFilename);

                    IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();
                    IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
                    IFileStatusMonitor monitor = e.GetService<IFileStatusMonitor>();

                    settings.ProjectRoot = Path.GetFullPath(dialog.WorkingCopyDir);

                    if (monitor != null && mapper != null)
                    {
                        // Make sure all visible glyphs are updated to reflect a new working copy
                        monitor.ScheduleSvnStatus(mapper.GetAllFilesOfAllProjects());
                    }

                    e.Result = true;
                }
                else
                {
                    return; // User cancelled the "Add to subversion" dialog, don't set as managed by Ankh
                }
            }
        }

        /// <summary>
        /// Creates the directory specified by <see cref="uri"/>
        /// Returns false if the user cancelled creating the directory, true otherwise
        /// </summary>
        /// <param name="e"></param>
        /// <param name="title">The title of the Create dialog</param>
        /// <param name="uri">The directory to be created</param>
        /// <param name="cl"></param>
        /// <returns></returns>
        bool RemoteCreateDirectory(CommandEventArgs e, string title, Uri uri, SvnClient cl)
        {
            using (CreateDirectoryDialog createDialog = new CreateDirectoryDialog())
            {
                createDialog.Text = title; // Override dialog title with text from other dialog

                createDialog.NewDirectoryName = uri.ToString();
                createDialog.NewDirectoryReadonly = true;
                if (createDialog.ShowDialog(e.Context) == DialogResult.OK)
                {
                    // Create uri (including optional /trunk if required)
                    SvnCreateDirectoryArgs cdArg = new SvnCreateDirectoryArgs();
                    cdArg.CreateParents = true;
                    cdArg.LogMessage = createDialog.LogMessage;

                    cl.RemoteCreateDirectory(uri, cdArg);
                    return true;
                }
                else
                    return false; // bail out, we cannot continue without directory in the repository
            }
        }

        void SetProjectsManaged(CommandEventArgs e)
        {
            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            IFileStatusMonitor monitor = e.GetService<IFileStatusMonitor>();
            IAnkhSccService scc = e.GetService<IAnkhSccService>();
            IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
            AnkhMessageBox mb = new AnkhMessageBox(e.Context);

            if (mapper == null)
                return;

            List<SvnProject> projectsToBeManaged = new List<SvnProject>();
            SvnItem slnItem = cache[e.Selection.SolutionFilename];
            Uri solutionReposRoot = slnItem.WorkingCopy.RepositoryRoot;

            foreach (SvnProject project in GetSelection(e.Selection))
            {
                ISvnProjectInfo projInfo = mapper.GetProjectInfo(project);
                SvnItem projectFile = cache[projInfo.ProjectFile];

                if (projectFile.WorkingCopy == slnItem.WorkingCopy)
                {
                    // This is a 'normal' project, part of the solution and in the same working copy
                    projectsToBeManaged.Add(project);
                    continue; 
                }

                if (projectFile.IsVersioned)
                    continue; // We don't have to add this one
                else if (projectFile.Parent.IsVersioned)
                    continue; // Project file is inside a WC, we can't check out a new one here, just add
                else if (projectFile.IsVersionable)
                {
                    SvnItem parentDir = GetVersionedParent(projectFile);
                    Debug.Assert(parentDir != null);

                    DialogResult rslt = mb.Show(string.Format(CommandResources.AddXToExistingWcY,
                        Path.GetFileName(projInfo.ProjectName),
                        parentDir.FullPath), AnkhId.PlkProduct, MessageBoxButtons.YesNoCancel);

                    if (rslt == DialogResult.Cancel)
                        return;
                    else if (rslt == DialogResult.No)
                    {
                        // No means we have to checkout a new working copy
                        if (CheckoutWorkingCopyForProject(e, projInfo, projectFile, solutionReposRoot))
                        {
                            projectsToBeManaged.Add(project);
                            continue;
                        }
                    }
                    else if (rslt == DialogResult.Yes)
                    {
                        // Yes means we have to add the file to the current WC
                        projectsToBeManaged.Add(project);

                        AddPathToSubversion(e, projectFile.FullPath);
                        continue;
                    }
                }
                else
                {
                    // We have to checkout (and create repository location)
                    if (CheckoutWorkingCopyForProject(e, projInfo, projectFile, solutionReposRoot))
                    {
                        projectsToBeManaged.Add(project);
                        continue;
                    }
                }
            }

            if (!AskSetManagedSelectionProjects(e, mapper, scc, projectsToBeManaged))
                return;

            foreach (SvnProject project in projectsToBeManaged)
            {
                if (!scc.IsProjectManaged(project))
                {
                    scc.SetProjectManaged(project, true);

                    monitor.ScheduleSvnStatus(mapper.GetAllFilesOf(project)); // Update for 'New' status
                }
            }
        }

        /// <summary>
        /// Returns true if <see cref="succeededProjects"/> should be set managed, false otherwise
        /// </summary>
        /// <param name="e"></param>
        /// <param name="mapper"></param>
        /// <param name="scc"></param>
        /// <param name="succeededProjects"></param>
        /// <returns></returns>
        bool AskSetManagedSelectionProjects(CommandEventArgs e, IProjectFileMapper mapper, IAnkhSccService scc, IList<SvnProject> succeededProjects)
        {
            if (e.DontPrompt || e.IsInAutomation)
                return true;

            AnkhMessageBox mb = new AnkhMessageBox(e.Context);
            StringBuilder sb = new StringBuilder();
            bool foundOne = false;
            foreach (SvnProject project in succeededProjects)
            {
                ISvnProjectInfo info;
                if (!scc.IsProjectManaged(project) && null != (info = mapper.GetProjectInfo(project)))
                {
                    if (sb.Length > 0)
                        sb.Append("', '");

                    sb.Append(info.ProjectName);
                }

                foundOne = true;
            }
            string txt = sb.ToString();
            int li = txt.LastIndexOf("', '");
            if (li > 0)
                txt = txt.Substring(0, li + 1) + CommandResources.FileAnd + txt.Substring(li + 3);

            if (!foundOne)
                return false; // No need to add when there are no projects

            return DialogResult.Yes == mb.Show(string.Format(CommandResources.MarkXAsManaged,
                txt), AnkhId.PlkProduct, MessageBoxButtons.YesNo);
        }

        /// <summary>
        /// Returns false if the AddToSubversionDialog has been cancelled, true otherwise
        /// </summary>
        /// <param name="e"></param>
        /// <param name="projectInfo"></param>
        /// <param name="projectItem"></param>
        /// <param name="solutionReposRoot"></param>
        /// <returns></returns>
        bool CheckoutWorkingCopyForProject(CommandEventArgs e, ISvnProjectInfo projectInfo, SvnItem projectItem, Uri solutionReposRoot)
        {
            using (SvnClient cl = e.GetService<ISvnClientPool>().GetClient())
            // TODO: Use dialog specific for projects
            using (Ankh.UI.SccManagement.AddToSubversion dialog = new Ankh.UI.SccManagement.AddToSubversion())
            {
                dialog.Context = e.Context;
                dialog.PathToAdd = projectInfo.ProjectDirectory;
                dialog.RepositoryAddUrl = solutionReposRoot;
                if (dialog.ShowDialog(e.Context) != DialogResult.OK)
                    return false; // User cancelled the "Add to subversion" dialog, don't set as managed by Ankh


                Collection<SvnInfoEventArgs> info;
                SvnInfoArgs ia = new SvnInfoArgs();
                ia.ThrowOnError = false;
                if (!cl.GetInfo(dialog.RepositoryAddUrl, ia, out info))
                {
                    // Target uri doesn't exist in the repository, let's create
                    if (!RemoteCreateDirectory(e, dialog.Text, dialog.RepositoryAddUrl, cl))
                        return false; // Create failed; bail out
                }

                // Create working copy
                SvnCheckOutArgs coArg = new SvnCheckOutArgs();
                coArg.AllowObstructions = true;
                cl.CheckOut(dialog.RepositoryAddUrl, dialog.WorkingCopyDir, coArg);

                e.Result = true;
            }
            return true;
        }
    }
}
