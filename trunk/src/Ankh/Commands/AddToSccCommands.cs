﻿using System;
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

namespace Ankh.Commands
{
    [Command(AnkhCommand.FileSccAddProjectToSubversion)]
    [Command(AnkhCommand.FileSccAddSolutionToSubversion)]
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
            if (scc == null)
            {
                e.Enabled = false;
                return;
            }


            if (!scc.IsProjectManaged(null))
                return; // Nothing is added unless the solution is added

            if (e.Command == AnkhCommand.FileSccAddSolutionToSubversion)
            {
                e.Visible = e.Enabled = false;
                return;
            }

            foreach (SvnProject p in GetSelection(e.Selection))
            {
                if (!scc.IsProjectManaged(p))
                    return; // Something to enable
            }

            e.Visible = e.Enabled = false;
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

            foreach (SvnProject pr in iSelectionContext.GetOwnerProjects(false))
            {
                yield return pr;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IAnkhSccService scc = e.GetService<IAnkhSccService>();
            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            if (scc == null || cache == null || e.Selection.SolutionFilename == null)
                return;

            AnkhMessageBox mb = new AnkhMessageBox(e.Context);

            if (!scc.IsProjectManaged(null))
            {
                bool confirmed = false;
                SvnItem item = cache[e.Selection.SolutionFilename];

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

                        // For now just throw the error

                        cl.Add(e.Selection.SolutionFilename, aa);
                    }
                }
                else
                {
                    new AddSolutionToRepositoryCommand().OnExecute(e);
                    return;
                }

                if (!confirmed && !e.IsInAutomation &&
                    DialogResult.Yes != mb.Show(string.Format(CommandResources.MarkXAsManaged,
                    Path.GetFileName(e.Selection.SolutionFilename)), AnkhId.PlkProduct, MessageBoxButtons.YesNo))
                {
                    return;
                }

                scc.SetProjectManaged(null, true);
                item.MarkDirty(); // This clears the solution settings cache to retrieve its properties
            }

            if (e.Command == AnkhCommand.FileSccAddSolutionToSubversion)
                return;

            IProjectFileMapper mapper = e.GetService<IProjectFileMapper>();

            if (mapper != null)
            {
                if (!e.IsInAutomation)
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
                    }
                }
            }
        }
    }
}
