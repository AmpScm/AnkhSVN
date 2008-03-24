using System;
using System.IO;

using Ankh.Scc;
using AnkhSvn.Ids;
using Ankh.Selection;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// Enables or disables Ankh for a solution.
    /// </summary>
    [Command(AnkhCommand.ToggleAnkh)]
    public class ToggleAnkhCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IAnkhSccService scc = e.Context.GetService<IAnkhSccService>();

            if (scc == null || !scc.IsActive)
            {
                e.Enabled = false;
                return;
            }

            bool enable = false;
            bool first = true;
            foreach (SvnProject project in e.Selection.GetOwnerProjects())
            {
                if (first)
                {
                    enable = !scc.IsProjectManaged(project);
                    first = false;
                }
                else if (enable == scc.IsProjectManaged(project))
                {
                    // Some projects managed and some not. Disable command
                    e.Enabled = false;
                    break;
                }
            }

            e.Text = enable ? "Enable Ank&hSVN" : "Disable Ank&hSVN";
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IAnkhSccService scc = e.Context.GetService<IAnkhSccService>();

            if (scc == null || !scc.IsActive)
                return;

            bool enable = false;
            bool first = true;
            foreach (SvnProject project in e.Selection.GetOwnerProjects())
            {
                if (first)
                {
                    enable = !scc.IsProjectManaged(project);
                    first = false;
                }

                scc.SetProjectManaged(project, enable);
            }            
        }
    }
}
