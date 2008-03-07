// $Id$
using System;
using System.Collections;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to cleanup the working copy.
    /// </summary>
    [VSNetCommand(AnkhCommand.Cleanup,
		"Cleanup",
         Text="Cle&anup",
         Tooltip = "Cleanup the working copy.", 
         Bitmap = ResourceBitmaps.Cleanup ),
         VSNetItemControl( VSNetControlAttribute.AnkhSubMenu, Position = 11)]
    public class Cleanup : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            context.StartOperation( "Running cleanup" );

            IList resources = context.Selection.GetSelectionResources( false,
                new ResourceFilterCallback(SvnItem.DirectoryFilter) );
            foreach( SvnItem item in resources )
                context.Client.CleanUp( item.Path );

            context.EndOperation();
        }
    }
}



