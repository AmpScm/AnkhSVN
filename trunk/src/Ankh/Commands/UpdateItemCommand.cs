// $Id$
using System;
using EnvDTE;
using NSvn;
using Ankh.UI;

namespace Ankh.Commands
{
	/// <summary>
	/// A command that updates an item.
	/// </summary>
	[VSNetCommand("UpdateItem", Text = "Update", Tooltip = "Updates the local item"),
     VSNetControl( "Item", Position = 2 ),
     VSNetControl( "Tools", Position = 4 )]
	internal class UpdateItem : CommandBase
	{		
        #region Implementation of ICommand
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            // all items must be versioned if we are going to run update.
            foreach( ILocalResource resource in context.SolutionExplorer.GetSelectedItems() )
                if ( !resource.IsVersioned )
                    return vsCommandStatus.vsCommandStatusUnsupported;

            return vsCommandStatus.vsCommandStatusEnabled |
                vsCommandStatus.vsCommandStatusSupported;            
        }

        public override void Execute(AnkhContext context)
        {
            // we assume by now that all items are working copy resources.
            foreach( WorkingCopyResource resource in 
                context.SolutionExplorer.GetSelectedItems() )
            {
                resource.Update();
            }
        }
    
        #endregion
    }
}



