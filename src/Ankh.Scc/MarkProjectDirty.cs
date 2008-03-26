using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.UI;
using Ankh.Selection;
using Ankh.Commands;
using AnkhSvn.Ids;
using Ankh.Scc.ProjectMap;
using System.Diagnostics;

namespace Ankh.Scc
{
    [Command(AnkhCommand.MarkProjectDirty)]
    [Command(AnkhCommand.MarkProjectRefresh)]
    public class MarkProjectDirty : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
            ProjectNotifier pn = e.Context.GetService<IProjectNotifier>() as ProjectNotifier;

            Debug.Assert(pn != null, "ProjectNotifier must be available!", "ProjectNotifier service not available");
            if (pn != null)
            {
                pn.HandleEvent(e.Command);
            }
        }
    }
}
