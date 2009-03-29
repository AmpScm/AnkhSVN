using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using Ankh.Scc;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemAddToPending)]
    [Command(AnkhCommand.ItemRemoveFromPending)]
    class ItemAddToPending : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool add = (e.Command == AnkhCommand.ItemAddToPending);
            IPendingChangesManager pcm= null;

            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
            {
                if (i.InSolution)
                    continue;

                if (!PendingChange.IsPending(i))
                    continue;

                if (pcm == null)
                {
                    pcm = e.GetService<IPendingChangesManager>();
                    if (pcm == null)
                        break;
                }

                if (pcm.Contains(i.FullPath) != add)
                    return;
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IFileStatusMonitor fsm = e.GetService<IFileStatusMonitor>();

            foreach (SvnItem i in e.Selection.GetSelectedSvnItems(false))
            {
                if (i.InSolution)
                    continue;

                if (e.Command == AnkhCommand.ItemAddToPending)
                    fsm.ScheduleMonitor(i.FullPath);
                else
                    fsm.StopMonitoring(i.FullPath);
            }            
        }
    }
}
