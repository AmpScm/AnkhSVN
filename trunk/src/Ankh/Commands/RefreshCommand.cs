// $Id$
using System;
using Ankh.UI;
using Ankh.Ids;
using Ankh.Scc;
using Microsoft.VisualStudio;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to refresh this view.
    /// </summary>
    [Command(AnkhCommand.Refresh)]
    class RefreshCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IFileStatusMonitor monitor = e.GetService<IFileStatusMonitor>();

            monitor.ScheduleSvnStatus(e.Selection.GetSelectedFiles(true));

            IAnkhOpenDocumentTracker dt = e.GetService<IAnkhOpenDocumentTracker>();

            dt.RefreshDirtyState();

            IPendingChangesManager pm = e.GetService<IPendingChangesManager>();

            pm.Refresh((string)null); // Perform a full incremental refresh on the PC window            
        }
    }
}