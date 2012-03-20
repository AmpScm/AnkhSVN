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
using System.Text;
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
    [Command(AnkhCommand.FileSccAddProjectToSubversion, HideWhenDisabled = false)]
    [Command(AnkhCommand.FileSccAddSolutionToSubversion, AlwaysAvailable = true, HideWhenDisabled = false)]
    sealed class AddToSccCommands : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SolutionExists || (e.Command == AnkhCommand.FileSccAddProjectToSubversion && e.State.EmptySolution))
            {
                e.Visible = e.Enabled = false;
                return;
            }

            if (e.State.OtherSccProviderActive)
            {
                e.Visible = e.Enabled = false;
                return; // Only one scc provider can be active at a time
            }

            IAnkhSccService scc = e.GetService<IAnkhSccService>();
            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            if (scc == null || cache == null)
            {
                e.Visible = e.Enabled = false;
                return;
            }

            string solutionFilename = e.Selection.SolutionFilename;

            if (string.IsNullOrEmpty(solutionFilename) || !SvnItem.IsValidPath(solutionFilename))
                solutionFilename = null;

            if (e.Command == AnkhCommand.FileSccAddSolutionToSubversion)
            {
                if (solutionFilename == null || scc.IsSolutionManaged)
                {
                    e.Visible = e.Enabled = false; // Already handled
                    return;
                }
                SvnItem item = cache[solutionFilename];

                if (!item.Exists || !item.IsFile || item.ParentDirectory.NeedsWorkingCopyUpgrade)
                {
                    // Decide where you store the .sln first
                    e.Visible = e.Enabled = false;
                    return;
                }

                if (!item.IsVersioned)
                {
                    // If the .sln is ignored hide it in the context menus
                    // but don't hide it on the node itself
                    e.HideOnContextMenu = item.IsIgnored && !e.Selection.IsSolutionSelected;
                }
                return;
            }

            IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();

            int n = 0;
            bool foundOne = false;
            foreach (IEnumerable<SvnProject> projects in
                new IEnumerable<SvnProject>[] 
                { 
                    e.Selection.GetSelectedProjects(true),
                    e.Selection.GetSelectedProjects(false) 
                })
            {
                foreach (SvnProject p in projects)
                {
                    foundOne = true;

                    ISvnProjectInfo pi = pfm.GetProjectInfo(p);

                    if (pi == null || !pi.IsSccBindable)
                        continue; // Not an SCC project

                    // A project is managed if the file says its managed
                    // and the project dir is managed
                    if (pi.ProjectDirectory != null && cache[pi.ProjectDirectory].IsVersioned
                        && scc.IsProjectManaged(p))
                        continue; // Nothing to do here

                    string projectFile = pi.ProjectFile;

                    if (n > 1 && projectFile != null && cache[projectFile].IsIgnored)
                        e.HideOnContextMenu = true;

                    return;
                }
                n++;
                if (foundOne)
                    break;
            }

            e.Visible = e.Enabled = false;
        }

        private static IEnumerable<SvnProject> GetSelection(ISelectionContext iSelectionContext)
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

            if (cache == null || e.Selection.SolutionFilename == null)
                return;

            SvnItem item = cache[e.Selection.SolutionFilename];

            if (!HandleUnmanagedOrUnversionedSolution(e, item))
                return;

            if (e.Command == AnkhCommand.FileSccAddSolutionToSubversion)
                return;

            SetProjectsManaged(e);
        }

        static bool HandleUnmanagedOrUnversionedSolution(CommandEventArgs e, SvnItem solutionItem)
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
                if (!CheckoutWorkingCopyForSolution(e, ref confirmed))
                    return false;
            }

            if (!confirmed && !e.DontPrompt && !e.IsInAutomation &&
                DialogResult.Yes != mb.Show(string.Format(CommandResources.MarkXAsManaged,
                Path.GetFileName(e.Selection.SolutionFilename)), "", MessageBoxButtons.YesNo))
            {
                return false;
            }

            SetSolutionManaged(shouldActivate, solutionItem, scc);

            return true;
        }

        static SvnItem GetVersionedParent(SvnItem child)
        {
            if (!child.IsVersionable)
                return null;

            if (!child.IsVersioned)
                return GetVersionedParent(child.Parent);
            return child;
        }

        static bool AddVersionableSolution(CommandEventArgs e, SvnItem solutionItem, ref bool confirmed)
        {
            AnkhMessageBox mb = new AnkhMessageBox(e.Context);
            SvnItem parentDir = GetVersionedParent(solutionItem);

            // File is not versioned but is inside a versioned directory
            if (!e.DontPrompt && !e.IsInAutomation)
            {
                if (!solutionItem.Parent.IsVersioned)
                {
                    AddPathToSubversion(e, e.Selection.SolutionFilename);

                    return true;
                }

                DialogResult rslt = mb.Show(string.Format(CommandResources.AddXToExistingWcY,
                                                          Path.GetFileName(e.Selection.SolutionFilename),
                                                          parentDir.FullPath), AnkhId.PlkProduct, MessageBoxButtons.YesNoCancel);

                if (rslt == DialogResult.Cancel)
                    return false;
                if (rslt == DialogResult.No)
                {
                    // Checkout new working copy
                    return CheckoutWorkingCopyForSolution(e, ref confirmed);
                }
                if (rslt == DialogResult.Yes)
                {
                    // default case: Add to existing workingcopy
                    AddPathToSubversion(e, e.Selection.SolutionFilename);

                    return true;
                }
                return false;
            }

            confirmed = true;
            return true;
        }

        static void AddPathToSubversion(CommandEventArgs e, string path)
        {
            using (SvnClient cl = e.GetService<ISvnClientPool>().GetNoUIClient())
            {
                SvnAddArgs aa = new SvnAddArgs();
                aa.AddParents = true;
                aa.AddExpectedError(SvnErrorCode.SVN_ERR_ENTRY_EXISTS); // Don't fail on already added nodes. (<= 1.7)
                aa.AddExpectedError(SvnErrorCode.SVN_ERR_WC_PATH_FOUND); // Don't fail on already added nodes. (1.8+?)
                cl.Add(path, aa);
            }
        }

        static void SetSolutionManaged(bool shouldActivate, SvnItem item, IAnkhSccService scc)
        {
            if (shouldActivate)
                scc.RegisterAsPrimarySccProvider();

            scc.SetProjectManaged(null, true);
            item.MarkDirty(); // This clears the solution settings cache to retrieve its properties
        }

        static bool CheckoutWorkingCopyForSolution(CommandEventArgs e, ref bool confirmed)
        {
            using (SvnClient cl = e.GetService<ISvnClientPool>().GetClient())
            using (AddToSubversion dialog = new AddToSubversion())
            {
                dialog.PathToAdd = e.Selection.SolutionFilename;
                if (dialog.ShowDialog(e.Context) != DialogResult.OK)
                    return false; // Don't set as managed by AnkhSVN

                confirmed = true;

                if (dialog.CommitAllFiles)
                {
                    HybridCollection<string> allFiles = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

                    string logMessage;
                    string wcPath = dialog.WorkingCopyDir;
                    Uri reposUrl = dialog.RepositoryAddUrl;

                    allFiles.UniqueAddRange(e.GetService<IProjectFileMapper>().GetAllFilesOfAllProjects());
                    using (CreateDirectoryDialog dlg = new CreateDirectoryDialog())
                    {
                        dlg.Text = CommandStrings.ImportingTitle;
                        dlg.NewDirectoryName = dialog.RepositoryAddUrl.ToString();
                        dlg.NewDirectoryReadonly = true;

                        if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                            return false;

                        logMessage = dlg.LogMessage;
                    }


                    e.GetService<IProgressRunner>().RunModal(CommandStrings.ImportingTitle,
                        delegate(object sender, ProgressWorkerArgs a)
                        {
                            SvnImportArgs importArgs = new SvnImportArgs();
                            importArgs.Filter +=
                                delegate(object ieSender, SvnImportFilterEventArgs ie)
                                {
                                    if (ie.NodeKind != SvnNodeKind.Directory)
                                        ie.Filter = allFiles.Contains(ie.FullPath);
                                    else
                                    {
                                        bool filter = true;
                                        foreach (string p in allFiles)
                                        {
                                            if (SvnItem.IsBelowRoot(p, ie.FullPath))
                                            {
                                                filter = false;
                                                break;
                                            }
                                        }
                                        if (filter)
                                            ie.Filter = true;
                                    }
                                };
                            a.Client.Import(wcPath, reposUrl, importArgs);
                        });
                }
                else
                {
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

                    // Add solutionfile so we can set properties (set managed)
                    AddPathToSubversion(e, e.Selection.SolutionFilename);

                    IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();
                    IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
                    IFileStatusMonitor monitor = e.GetService<IFileStatusMonitor>();

                    settings.ProjectRoot = SvnTools.GetNormalizedFullPath(dialog.WorkingCopyDir);

                    if (monitor != null && mapper != null)
                    {
                        // Make sure all visible glyphs are updated to reflect a new working copy
                        monitor.ScheduleSvnStatus(mapper.GetAllFilesOfAllProjects());
                    }
                }

                return true;
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
        static bool RemoteCreateDirectory(CommandEventArgs e, string title, Uri uri, SvnClient cl)
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

                return false; // bail out, we cannot continue without directory in the repository
            }
        }

        static void SetProjectsManaged(CommandEventArgs e)
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
            Uri solutionReposRoot = null;
            if (slnItem.WorkingCopy != null)
            {
                solutionReposRoot = slnItem.WorkingCopy.RepositoryRoot;

                foreach (SvnProject project in GetSelection(e.Selection))
                {
                    ISvnProjectInfo projInfo = mapper.GetProjectInfo(project);

                    if (projInfo == null || projInfo.ProjectDirectory == null
                        || !projInfo.IsSccBindable)
                        continue; // Some projects can't be managed

                    SvnItem projectDir = cache[projInfo.ProjectDirectory];

                    if (projectDir.WorkingCopy == slnItem.WorkingCopy)
                    {
                        // This is a 'normal' project, part of the solution and in the same working copy
                        projectsToBeManaged.Add(project);
                        continue;
                    }

                    bool markAsManaged;
                    bool writeReference;

                    if (projectDir.IsVersioned)
                        continue; // We don't have to add this one
                    if (projectDir.IsVersionable)
                    {
                        SvnItem parentDir = GetVersionedParent(projectDir);
                        Debug.Assert(parentDir != null);

                        DialogResult rslt = mb.Show(string.Format(CommandResources.AddXToExistingWcY,
                                                                  Path.GetFileName(projInfo.ProjectName),
                                                                  parentDir.FullPath), AnkhId.PlkProduct, MessageBoxButtons.YesNoCancel);

                        switch (rslt)
                        {
                            case DialogResult.Cancel:
                                return;
                            case DialogResult.No:
                                if (CheckoutWorkingCopyForProject(e, project, projInfo, solutionReposRoot, out markAsManaged, out writeReference))
                                {
                                    if (markAsManaged)
                                        scc.SetProjectManaged(project, true);
                                    if (writeReference)
                                        scc.EnsureCheckOutReference(project);

                                    continue;
                                }
                                break;
                            case DialogResult.Yes:
                                projectsToBeManaged.Add(project);
                                AddPathToSubversion(e, projInfo.ProjectFile ?? projInfo.ProjectDirectory);
                                continue;
                        }
                    }
                    else
                    {
                        // We have to checkout (and create repository location)
                        if (CheckoutWorkingCopyForProject(e, project, projInfo, solutionReposRoot, out markAsManaged, out writeReference))
                        {
                            if (markAsManaged)
                                scc.SetProjectManaged(project, true);
                            if (writeReference)
                                scc.EnsureCheckOutReference(project);

                            continue;
                        }
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
        static bool AskSetManagedSelectionProjects(CommandEventArgs e, IProjectFileMapper mapper, IAnkhSccService scc, IEnumerable<SvnProject> succeededProjects)
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

            if (!foundOne)
                return false; // No need to add when there are no projects

            string txt = sb.ToString();
            int li = txt.LastIndexOf("', '");
            if (li > 0)
                txt = txt.Substring(0, li + 1) + CommandResources.FileAnd + txt.Substring(li + 3);

            return DialogResult.Yes == mb.Show(string.Format(CommandResources.MarkXAsManaged,
                txt), AnkhId.PlkProduct, MessageBoxButtons.YesNo);
        }

        /// <summary>
        /// Returns false if the AddToSubversionDialog has been cancelled, true otherwise
        /// </summary>
        /// <param name="e"></param>
        /// <param name="projectInfo"></param>
        /// <param name="solutionReposRoot"></param>
        /// <param name="shouldMarkAsManaged"></param>
        /// <param name="storeReference"></param>
        /// <returns></returns>
        static bool CheckoutWorkingCopyForProject(CommandEventArgs e, SvnProject project, ISvnProjectInfo projectInfo, Uri solutionReposRoot, out bool shouldMarkAsManaged, out bool storeReference)
        {
            shouldMarkAsManaged = false;
            storeReference = false;
            using (SvnClient cl = e.GetService<ISvnClientPool>().GetClient())
            using (AddProjectToSubversion dialog = new AddProjectToSubversion())
            {
                dialog.Context = e.Context;
                dialog.PathToAdd = projectInfo.ProjectDirectory;
                dialog.RepositoryAddUrl = solutionReposRoot;
                if (dialog.ShowDialog(e.Context) != DialogResult.OK)
                    return false; // User cancelled the "Add to subversion" dialog, don't set as managed by Ankh

                if (!dialog.CommitAllFiles)
                {
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
                }
                else
                {
                    // Cache some values before thread marshalling
                    HybridCollection<string> projectFiles = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                    string wcDir = dialog.WorkingCopyDir;
                    Uri reposUrl = dialog.RepositoryAddUrl;
                    string logMessage;

                    projectFiles.UniqueAddRange(e.GetService<IProjectFileMapper>().GetAllFilesOf(project));
                    using (CreateDirectoryDialog dlg = new CreateDirectoryDialog())
                    {
                        dlg.Text = CommandStrings.ImportingTitle;
                        dlg.NewDirectoryName = reposUrl.ToString();
                        dlg.NewDirectoryReadonly = true;

                        if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                            return false;

                        logMessage = dlg.LogMessage;
                    }
                    e.GetService<IProgressRunner>().RunModal(CommandStrings.ImportingTitle,
                        delegate(object sender, ProgressWorkerArgs a)
                        {
                            SvnImportArgs importArgs = new SvnImportArgs();
                            importArgs.LogMessage = logMessage;
                            importArgs.Filter +=
                                delegate(object ieSender, SvnImportFilterEventArgs ie)
                                {
                                    if (ie.NodeKind != SvnNodeKind.Directory)
                                        ie.Filter = projectFiles.Contains(ie.FullPath);
                                    else
                                    {
                                        bool filter = true;
                                        foreach(string p in projectFiles)
                                        {
                                            if (SvnItem.IsBelowRoot(p, ie.FullPath))
                                            {
                                                filter = false;
                                                break;
                                            }
                                        }
                                        if (filter)
                                            ie.Filter = true;
                                    }
                                };
                            a.Client.Import(wcDir, reposUrl, importArgs);
                        });
                }

                shouldMarkAsManaged = dialog.MarkAsManaged;
                storeReference = dialog.WriteCheckOutInformation;
            }
            return true;
        }
    }
}
