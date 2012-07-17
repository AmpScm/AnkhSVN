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
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.UI.RepositoryOpen;
using System.Windows.Forms;
using Ankh.VS;
using SharpSvn;
using System.IO;
using Ankh.Scc;
using System.Collections.Generic;

namespace Ankh.Commands
{
    [Command(AnkhCommand.FileFileOpenFromSubversion, AlwaysAvailable = true, ArgumentDefinition = "u")]
    [Command(AnkhCommand.FileFileAddFromSubversion, AlwaysAvailable = true, ArgumentDefinition = "u")]
    [Command(AnkhCommand.FileSccOpenFromSubversion)]
    [Command(AnkhCommand.FileSccAddFromSubversion)]
    class OpenFromSubversion : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (e.Command == AnkhCommand.FileFileAddFromSubversion || e.Command == AnkhCommand.FileSccAddFromSubversion)
            {
                if (!e.State.SolutionExists || e.State.SolutionBuilding || e.State.Debugging)
                    e.Enabled = e.Visible = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            Uri selectedUri = null;
            Uri rootUri = null;

            bool addingProject = (e.Command == AnkhCommand.FileFileAddFromSubversion
                                  || e.Command == AnkhCommand.FileSccAddFromSubversion);

            if (e.Argument is string && Uri.TryCreate((string)e.Argument, UriKind.Absolute, out selectedUri))
            { }
            else if (e.Argument is SvnOrigin)
            {
                SvnOrigin origin = (SvnOrigin)e.Argument;
                selectedUri = origin.Uri;
                rootUri = origin.RepositoryRoot;
            }
            else if (e.Argument is Uri)
                selectedUri = (Uri)e.Argument;

            IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();

            if (e.PromptUser || selectedUri == null)
            {
                using (RepositoryOpenDialog dlg = new RepositoryOpenDialog())
                {
                    if (addingProject)
                        dlg.Text = CommandStrings.AddProjectFromSubversion;

                    dlg.Filter = settings.OpenProjectFilterName + "|" + settings.AllProjectExtensionsFilter + "|All Files (*.*)|*";

                    if (selectedUri != null)
                        dlg.SelectedUri = selectedUri;

                    if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                        return;

                    selectedUri = dlg.SelectedUri;
                    rootUri = dlg.SelectedRepositoryRoot;
                }
            }
            else if (rootUri == null)
            {
                if (!e.GetService<IProgressRunner>().RunModal(CommandStrings.RetrievingRepositoryRoot,
                    delegate(object sender, ProgressWorkerArgs a)
                    {
                        rootUri = a.Client.GetRepositoryRoot(selectedUri);

                    }).Succeeded)
                {
                    return;
                }
            }

            string defaultPath = settings.NewProjectLocation;

            if (addingProject)
            {
                IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();

                if (!string.IsNullOrEmpty(ss.ProjectRoot))
                    defaultPath = ss.ProjectRoot;
            }

            string name = Path.GetFileNameWithoutExtension(SvnTools.GetFileName(selectedUri));

            string newPath;
            int n = 0;
            do
            {
                newPath = Path.Combine(defaultPath, name);
                if (n > 0)
                    newPath += string.Format("({0})", n);
                n++;
            }
            while (File.Exists(newPath) || Directory.Exists(newPath));

            using (CheckoutProject dlg = new CheckoutProject())
            {
                dlg.Context = e.Context;

                if (addingProject)
                    dlg.Text = CommandStrings.AddProjectFromSubversion;
                dlg.ProjectUri = selectedUri;
                dlg.RepositoryRootUri = rootUri;
                dlg.SelectedPath = newPath;
                dlg.SvnOrigin = new SvnOrigin(selectedUri, rootUri);
                dlg.HandleCreated += delegate
                {
                    FindRoot(e.Context, selectedUri, dlg);
                };

                if (dlg.ShowDialog(e.Context) != DialogResult.OK)
                    return;

                if (!addingProject)
                    OpenSolution(e, dlg);
                else
                    CheckOutAndOpenProject(e, dlg.ProjectTop, dlg.Revision, dlg.ProjectTop, dlg.SelectedPath, dlg.ProjectUri);
            }
        }

        private static void CheckOutAndOpenProject(CommandEventArgs e, SvnUriTarget checkoutLocation, SvnRevision revision, Uri projectTop, string localDir, Uri projectUri)
        {
            IProgressRunner runner = e.GetService<IProgressRunner>();

            runner.RunModal(CommandStrings.CheckingOutSolution,
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    PerformCheckout(ee, checkoutLocation, revision, localDir);
                });

            Uri file = projectTop.MakeRelativeUri(projectUri);

            string projectFile = SvnTools.GetNormalizedFullPath(Path.Combine(localDir, SvnTools.UriPartToPath(file.ToString())));

            AddProject(e, projectFile);

            using (ProjectAddInfoDialog pai = new ProjectAddInfoDialog())
            {
                IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();
                IFileStatusCache cache = e.GetService<IFileStatusCache>();
                SvnItem rootItem;

                pai.EnableSlnConnection = false;

                if (ss == null || cache == null
                    || string.IsNullOrEmpty(ss.ProjectRoot)
                    || !SvnItem.IsBelowRoot(localDir, ss.ProjectRoot)
                    || null == (rootItem = cache[localDir]))
                {
                    pai.EnableExternal = false;
                    pai.EnableCopy = false;
                }
                else
                {
                    SvnItem dir = rootItem.Parent;

                    if (ss.ProjectRootSvnItem != null
                        && ss.ProjectRootSvnItem.IsVersioned)
                    {
                        HybridCollection<string> dirs = new HybridCollection<string>();
                        SvnItem exDir = dir;

                        while (exDir != null && exDir.IsBelowPath(ss.ProjectRoot))
                        {
                            if (exDir.IsVersioned && exDir.WorkingCopy == ss.ProjectRootSvnItem.WorkingCopy)
                                dirs.Add(exDir.FullPath);

                            exDir = exDir.Parent;
                        }
                        pai.SetExternalDirs(dirs);
                        pai.EnableExternal = true;
                    }
                    else
                        pai.EnableExternal = false;

                    if (rootItem.WorkingCopy != null && dir.WorkingCopy != null)
                    {
                        pai.EnableCopy = (rootItem.WorkingCopy.RepositoryRoot == dir.WorkingCopy.RepositoryRoot)
                                         && (rootItem.WorkingCopy.RepositoryId == dir.WorkingCopy.RepositoryId);
                    }
                    else
                        pai.EnableCopy = false;
                }

                if (pai.ShowDialog(e.Context) == DialogResult.OK)
                {
                    switch (pai.SelectedMode)
                    {
                        case ProjectAddMode.External:
                            using (SvnClient cl = e.GetService<ISvnClientPool>().GetNoUIClient())
                            {
                                string externals;
                                if (!cl.TryGetProperty(pai.ExternalLocation, SvnPropertyNames.SvnExternals, out externals))
                                    externals = "";

                                SvnExternalItem sei;
                                if (pai.ExternalLocked)
                                    sei = new SvnExternalItem(SvnItem.SubPath(localDir, pai.ExternalLocation), checkoutLocation.Uri, revision, revision);
                                else
                                    sei = new SvnExternalItem(SvnItem.SubPath(localDir, pai.ExternalLocation), checkoutLocation.Uri);

                                externals = sei.ToString(true) + Environment.NewLine + externals;
                                cl.SetProperty(pai.ExternalLocation, SvnPropertyNames.SvnExternals, externals);
                            }
                            break;
                        case ProjectAddMode.Copy:
                            using (SvnWorkingCopyClient cl = e.GetService<ISvnClientPool>().GetWcClient())
                            {
                                string tmpDir = localDir + "-Src-copyTmp";
                                Directory.CreateDirectory(tmpDir);
                                Directory.Move(Path.Combine(localDir, SvnClient.AdministrativeDirectoryName), Path.Combine(tmpDir, SvnClient.AdministrativeDirectoryName));
                                SvnWorkingCopyCopyArgs ma = new SvnWorkingCopyCopyArgs();
                                ma.MetaDataOnly = true;
                                cl.Copy(tmpDir, localDir, ma);
                                SvnItem.DeleteDirectory(tmpDir, true);
                            }
                            break;
                        case ProjectAddMode.Unversioned:
                            cache.MarkDirtyRecursive(localDir);
                            SvnItem.DeleteDirectory(Path.Combine(localDir, SvnClient.AdministrativeDirectoryName), true);
                            e.GetService<IFileStatusMonitor>().ScheduleGlyphUpdate(projectFile); // And everything else in the project
                            break;
                    }
                }
            }
        }

        private static void OpenSolution(CommandEventArgs e, CheckoutProject dlg)
        {
            IVsSolution2 sol = e.GetService<IVsSolution2>(typeof(SVsSolution));

            if (sol != null)
            {
                sol.CloseSolutionElement(VSConstants.VSITEMID_ROOT, null, 0); // Closes the current solution
            }

            IAnkhSccService scc = e.GetService<IAnkhSccService>();

            if (scc != null)
                scc.RegisterAsPrimarySccProvider(); // Make us the current SCC provider!

            CheckOutAndOpenSolution(e, dlg.ProjectTop, dlg.Revision, dlg.ProjectTop, dlg.SelectedPath, dlg.ProjectUri);

            sol = e.GetService<IVsSolution2>(typeof(SVsSolution));

            if (sol != null)
            {
                string file, user, dir;

                if (ErrorHandler.Succeeded(sol.GetSolutionInfo(out dir, out file, out user))
                    && !string.IsNullOrEmpty(file))
                {
                    scc.SetProjectManaged(null, true);
                }
            }
        }

        private static void CheckOutAndOpenSolution(CommandEventArgs e, SvnUriTarget checkoutLocation, SvnRevision revision, Uri projectTop, string localDir, Uri projectUri)
        {
            IProgressRunner runner = e.GetService<IProgressRunner>();

            runner.RunModal(CommandStrings.CheckingOutSolution,
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    PerformCheckout(ee, checkoutLocation, revision, localDir);
                });

            Uri file = projectTop.MakeRelativeUri(projectUri);

            string projectFile = SvnTools.GetNormalizedFullPath(Path.Combine(localDir, SvnTools.UriPartToPath(file.ToString())));

            OpenProject(e, projectFile);
        }

        private static void OpenProject(CommandEventArgs e, string projectFile)
        {
            IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();

            ss.OpenProjectFile(projectFile);
        }

        private static void AddProject(CommandEventArgs e, string projectFile)
        {
            IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();

            ss.AddProjectFile(projectFile);
        }

        private static void PerformCheckout(ProgressWorkerArgs e, SvnUriTarget projectTop, SvnRevision revision, string localDir)
        {
            SvnCheckOutArgs a = new SvnCheckOutArgs();
            a.Revision = revision;

            e.Client.CheckOut(projectTop, localDir, a);
        }

        private static void FindRoot(IAnkhServiceProvider context, Uri selectedUri, CheckoutProject dlg)
        {
            AnkhAction ds = delegate
            {
                using (SvnClient client = context.GetService<ISvnClientPool>().GetClient())
                {
                    string value;
                    if (client.TryGetProperty(selectedUri, AnkhSccPropertyNames.ProjectRoot, out value))
                    {
                        if (dlg.IsHandleCreated)
                            dlg.Invoke((AnkhAction)delegate
                            {
                                try
                                {
                                    dlg.ProjectTop = new Uri(selectedUri, value);
                                }
                                catch { };
                            });
                    }
                }
            };

            ds.BeginInvoke(null, null);
        }
    }
}
