using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Scc;
using Ankh.Ids;

namespace Ankh.Scc.StatusCache.Commands
{
    [Command(AnkhCommand.FileCacheFinishTasks)]
    [Command(AnkhCommand.TickRefreshSvnItems)]
    public class FileStatusCleanup : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {            
        }

        public void OnExecute(CommandEventArgs e)
        {
            FileStatusCache cache = e.Context.GetService<FileStatusCache>(typeof(IFileStatusCache));

            if (cache != null)
            {
                if (e.Command == AnkhCommand.FileCacheFinishTasks)
                    cache.OnCleanup();
                else
                    cache.BroadcastChanges();
            }
        }
    }
}
