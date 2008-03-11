// $Id$
using System;
using Ankh.UI;
using AnkhSvn.Ids;

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
            GC.KeepAlive(e.Selection.GetOwnerProjects(true));
            IContext context = e.Context.GetService<IContext>();

            using(context.StartOperation( "Refreshing" ))
            {
                context.Selection.RefreshSelection();
            }
        }
    }
}