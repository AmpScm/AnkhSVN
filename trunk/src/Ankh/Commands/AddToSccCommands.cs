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
            IAnkhSccService scc = e.GetService<IAnkhSccService>();
            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();
            IFileStatusMonitor monitor = e.GetService<IFileStatusMonitor>();
            if (scc == null || cache == null || e.Selection.SolutionFilename == null)
                return;

            bool shouldActivate = false;
            if (!scc.IsActive)
            {
                if (e.State.OtherSccProviderActive)
                    return; // Can't switch in this case.. Nothing to do

                shouldActivate = true;
            }

            AnkhMessageBox mb = new AnkhMessageBox(e.Context);

            if (!scc.IsSolutionManaged || !cache[e.Selection.SolutionFilename].IsVersioned)
            {
                bool confirmed = false;
                SvnItem item = cache[e.Selection.SolutionFilename];

                IAnkhSolutionSettings settings = e.GetService<IAnkhSolutionSettings>();

                if (item.IsVersioned)
                { /* File is in subversion; just enable */ }
                else if (item.IsVersionable)
                {
                    if (e.IsInAutomation)
                        confirmed = true;
                    else if (DialogResult.Yes != mb.Show(string.Format(CommandResources.AddSolutionXToSubversion,
                        Path.GetFileName(e.Selection.SolutionFilename)), AnkhId.PlkProduct, MessageBoxButtons.YesNo))
                    {
                        return;
                    }
                    else
                        confirmed = true;

                    using (SvnClient cl = e.GetService<ISvnClientPool>().GetNoUIClient())
                    {
                        SvnAddArgs aa = new SvnAddArgs();
                        aa.AddParents = true;
                        cl.Add(e.Selection.SolutionFilename, aa);

                        //settings.ProjectRoot = Path.GetFullPath(dialog.WorkingCopyDir);
                    }
                }
                else
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
                                using (CreateDirectoryDialog createDialog = new CreateDirectoryDialog())
                                {
                                    createDialog.Text = dialog.Text; // Override dialog title with text from other dialog

                                    createDialog.NewDirectoryName = dialog.RepositoryAddUrl.ToString();
                                    createDialog.NewDirectoryReadonly = true;
                                    if (createDialog.ShowDialog(e.Context) == DialogResult.OK)
                                    {
                                        // Create uri (including optional /trunk if required)
                                        SvnCreateDirectoryArgs cdArg = new SvnCreateDirectoryArgs();
                                        cdArg.CreateParents = true;
                                        cdArg.LogMessage = createDialog.LogMessage;

                                        cl.RemoteCreateDirectory(dialog.RepositoryAddUrl, cdArg);
                                    }
                                    else
                                        return; // bail out, we cannot continue without directory in the repository
                                }
                            }

                            // Create working copy
                            SvnCheckOutArgs coArg = new SvnCheckOutArgs();
                            coArg.AllowObstructions = true;
                            cl.CheckOut(dialog.RepositoryAddUrl, dialog.WorkingCopyDir, coArg);

                            // Add solutionfile so we can set properties (set managed)
                            SvnAddArgs aa = new SvnAddArgs();
                            aa.AddParents = true;
                            cl.Add(e.Selection.SolutionFilename, aa);

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
                            return; // User cancelled the "Add to subversion" dialog, don't set as managed by Ankh or anything else
                        }
                    }
                }

                if (!confirmed && !e.IsInAutomation &&
                    DialogResult.Yes != mb.Show(string.Format(CommandResources.MarkXAsManaged,
                    Path.GetFileName(e.Selection.SolutionFilename)), AnkhId.PlkProduct, MessageBoxButtons.YesNo))
                {
                    return;
                }

                if (shouldActivate)
                    scc.RegisterAsPrimarySccProvider();

                scc.SetProjectManaged(null, true);
                item.MarkDirty(); // This clears the solution settings cache to retrieve its properties
            }

            if (e.Command == AnkhCommand.FileSccAddSolutionToSubversion)
                return;

            if (mapper != null)
            {
                if (!e.DontPrompt && !e.IsInAutomation)
                {
                    StringBuilder sb = new StringBuilder();
                    bool foundOne = false;
                    foreach (SvnProject project in GetSelection(e.Selection))
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

                    if (foundOne && DialogResult.Yes != mb.Show(string.Format(CommandResources.MarkXAsManaged,
                        txt), AnkhId.PlkProduct, MessageBoxButtons.YesNo))
                    {
                        return;
                    }
                }

                foreach (SvnProject project in GetSelection(e.Selection))
                {
                    if (!scc.IsProjectManaged(project))
                    {
                        scc.SetProjectManaged(project, true);

                        monitor.ScheduleSvnStatus(mapper.GetAllFilesOf(project)); // Update for 'New' status
                    }
                }
            }
        }
    }
}
