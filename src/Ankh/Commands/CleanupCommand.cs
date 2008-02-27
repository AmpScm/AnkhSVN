// $Id$
using System;
using EnvDTE;
using System.Collections;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to cleanup the working copy.
    /// </summary>
    [VSNetCommand( "Cleanup",
         Text="Cle&anup",
         Tooltip = "Cleanup the working copy.", 
         Bitmap = ResourceBitmaps.Cleanup ),
         VSNetItemControl( VSNetControlAttribute.AnkhSubMenu, Position = 11)]
    public class Cleanup : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            return Enabled;
        }
    
        public override void Execute(IContext context, string parameters)
        {
            context.StartOperation( "Running cleanup" );

            IList resources = context.Selection.GetSelectionResources( false,
                new ResourceFilterCallback(SvnItem.DirectoryFilter) );
            foreach( SvnItem item in resources )
                context.Client.CleanUp( item.Path );

            context.EndOperation();
        }

        #endregion
    }
}



