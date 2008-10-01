using System;
using System.Collections.Generic;
using System.Text;

using Ankh.Commands;
using Ankh.Ids;

using WizardFramework;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using Ankh.Scc;
using Ankh.VS;
using Ankh.Selection;

namespace Ankh.UI.MergeWizard.Commands
{
    [Command(AnkhCommand.ItemMerge)]
    [Command(AnkhCommand.ProjectMerge)]
    [Command(AnkhCommand.SolutionMerge)]
    class Merge : ICommandHandler
    {
        /// <see cref="Ankh.Commands.ICommandHandler.OnUpdate" />
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            IFileStatusCache statusCache = e.GetService<IFileStatusCache>();
            int n = 0;
            bool ok = true;
            switch (e.Command)
            {
                case AnkhCommand.ItemMerge:
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                    {
                        if (!item.IsVersioned)
                        {
                            e.Enabled = false;
                            return;
                        }

                        n++;

                        if (n > 1)
                            break;
                    }
                    break;
                case AnkhCommand.ProjectMerge:
                    IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();
                    foreach (SvnProject project in e.Selection.GetSelectedProjects(false))
                    {
                        ISvnProjectInfo projInfo = pfm.GetProjectInfo(project);
                        if (projInfo == null)
                        {
                            e.Enabled = false;
                            return;
                        }
                        SvnItem projectDir = statusCache[projInfo.ProjectDirectory];
                        if (!projectDir.IsVersioned)
                        {
                            e.Enabled = false;
                            return;
                        }

                        n++;

                        if (n > 1)
                            break;
                    }
                    break;
                case AnkhCommand.SolutionMerge:
                    IAnkhSolutionSettings solutionSettings = e.GetService<IAnkhSolutionSettings>();
                    if (solutionSettings == null || string.IsNullOrEmpty(solutionSettings.ProjectRoot))
                    {
                        e.Enabled = false;
                        return;
                    }
                    SvnItem solutionItem = statusCache[solutionSettings.ProjectRoot];
                    if (solutionItem.IsVersioned)
                        n = 1;
                    break;
                default:
                    throw new InvalidOperationException();
            }

            if (!ok || n == 0 || n > 1)
                e.Enabled = false;
            else
                e.Enabled = true;
        }

        /// <see cref="Ankh.Commands.ICommandHandler.OnExecute" />
        public void OnExecute(CommandEventArgs e)
        {
            List<SvnItem> svnItems = new List<SvnItem>();
            IFileStatusCache cache = e.GetService<IFileStatusCache>();

            switch (e.Command)
            {
                case AnkhCommand.ItemMerge:
                    // TODO: Check for solution and/or project selection to use the folder instead of the file
                    foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                    {
                        svnItems.Add(item);
                    }
                    break;
                case AnkhCommand.ProjectMerge:
                    foreach (SvnProject p in e.Selection.GetSelectedProjects(false))
                    {
                        IProjectFileMapper pfm = e.GetService<IProjectFileMapper>();

                        ISvnProjectInfo info = pfm.GetProjectInfo(p);
                        if (info != null)
                        {
                            svnItems.Add(cache[info.ProjectDirectory]);
                        }
                    }
                    break;
                case AnkhCommand.SolutionMerge:
                    svnItems.Add(cache[e.GetService<IAnkhSolutionSettings>().ProjectRoot]);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            using (MergeWizardDialog dialog = new MergeWizardDialog(e.Context, new MergeUtils(e.Context), svnItems[0]))
            {
                DialogResult result;

                IUIService uiService = e.GetService<IUIService>();
                // TODO: Use
                //result = uiService.ShowDialog(dialog);
                result = dialog.ShowDialog(uiService.GetDialogOwnerWindow());

                if (result == DialogResult.OK)
                {
                    MergeResultsDialog mrd = new MergeResultsDialog();

                    mrd.MergeActions = ((MergeWizard)dialog.Wizard).MergeActions;
                    mrd.ResolvedMergeConflicts = ((MergeWizard)dialog.Wizard).ResolvedMergeConflicts;

                    mrd.ShowDialog(uiService.GetDialogOwnerWindow());
                }
            }
        }
    }
}
