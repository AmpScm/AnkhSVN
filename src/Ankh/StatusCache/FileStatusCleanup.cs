using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Scc;
using AnkhSvn.Ids;

namespace Ankh.StatusCache
{
    [Command(AnkhCommand.FileCacheFinishTasks)]
    public class FileStatusCleanup : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {            
        }

        public void OnExecute(CommandEventArgs e)
        {
            FileStatusCache cache = e.Context.GetService<FileStatusCache>(typeof(IFileStatusCache));

            if(cache != null)
                cache.OnCleanup();
        }
    }
}
