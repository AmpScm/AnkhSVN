// $Id$
using System;
using EnvDTE;
using Ankh.UI;

namespace Ankh.Commands
{
    /// <summary>
    /// Command that refreshes the tree view
    /// </summary>
    [VSNetCommand("Refresh", Text = "Refres&h", Tooltip = "Refresh this view.", 
         Bitmap = ResourceBitmaps.Refresh),
    VSNetItemControl( "Ankh", Position = 1 )]
    public class RefreshCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.IContext context)
        {
            return Enabled;
        }

        public override void Execute(Ankh.IContext context, string parameters)
        {
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
