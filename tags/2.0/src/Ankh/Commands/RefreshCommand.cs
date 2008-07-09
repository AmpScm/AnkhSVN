// $Id$
using System;
using Ankh.UI;
using Ankh.Ids;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to refresh this view.
    /// </summary>
    [Command(AnkhCommand.Refresh)]
    public class RefreshCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (!e.State.SccProviderActive)
            {
                e.Visible = e.Enabled = false;
                return;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            using (context.StartOperation("Refreshing"))
            {
                IFileStatusMonitor monitor = e.GetService<IFileStatusMonitor>();

                monitor.ScheduleSvnStatus(e.Selection.GetSelectedFiles(true));

                IPendingChangesManager pm = e.GetService<IPendingChangesManager>();

                pm.Refresh((string)null); // Perform a full incremental refresh on the PC window
            }
        }
    }
}