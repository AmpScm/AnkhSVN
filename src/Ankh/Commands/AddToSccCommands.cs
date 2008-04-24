using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.Ids;
using Ankh.Scc;
using Ankh.Selection;

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


            if (e.Command == AnkhCommand.FileSccAddSolutionToSubversion)
            {
                if (!scc.IsProjectManaged(null))
                    return;
                
                return; // Already added
            }
            else
            {
                bool foundOne = false;
                foreach (SvnProject p in e.Selection.GetSelectedProjects(true))
                {
                    if (!scc.IsProjectManaged(p))
                        return; // Something to enable

                    foundOne = true;
                }

                if (!foundOne)
                {
                    foreach (SvnProject p in e.Selection.GetOwnerProjects(false))
                    {
                        if (!scc.IsProjectManaged(p))
                            return;
                    }
                }

                e.Visible = e.Enabled = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
