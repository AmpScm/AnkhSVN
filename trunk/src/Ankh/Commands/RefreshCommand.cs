// $Id$
using System;
using Ankh.UI;
using AnkhSvn.Ids;
using Ankh.Scc;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to refresh this view.
    /// </summary>
    [Command(AnkhCommand.Refresh)]
    public class RefreshCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            using(context.StartOperation( "Refreshing" ))
            {
                IFileStatusCache cache = e.Context.GetService<IFileStatusCache>();
                IProjectNotifier pn = e.Context.GetService<IProjectNotifier>();

                if(cache != null)
                    cache.MarkDirty(e.Selection.GetSelectedFiles(true));
                
                if(pn != null)
                    pn.MarkFullRefresh(e.Selection.GetOwnerProjects(true));
            }
        }
    }
}