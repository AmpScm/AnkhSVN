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
                foreach(SvnItem item in e.Selection.GetSelectedSvnItems(true))
                {
                    item.MarkDirty();
                }
                IProjectNotifier pn = e.Context.GetService<IProjectNotifier>();
                if(pn != null)
                    pn.MarkDirty(e.Selection.GetOwnerProjects(true));
            }
        }
    }
}