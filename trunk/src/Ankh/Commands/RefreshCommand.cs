// $Id$
using System;
using Ankh.UI;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to refresh this view.
    /// </summary>
    [VSNetCommand(AnkhCommand.Refresh,
		"Refresh",
         Text = "Refres&h",
         Tooltip = "Refresh this view.", 
         Bitmap = ResourceBitmaps.Refresh),
         VSNetItemControl( VSNetControlAttribute.AnkhSubMenu, Position = 2 )]
    public class RefreshCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            GC.KeepAlive(e.Selection.GetOwnerProjects(true));
            IContext context = e.Context;

            try
            {
                context.StartOperation( "Refreshing" );
                context.Selection.RefreshSelection();
            }
            finally
            {
                context.EndOperation();
            }
        }
    }
}