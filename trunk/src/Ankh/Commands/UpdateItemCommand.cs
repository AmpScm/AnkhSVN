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
     VSNetControl( "Project", Position = 2 ),
     VSNetControl( "Solution", Position = 2 ),
     VSNetControl( "Folder", Position = 2 )]
	internal class UpdateItem : CommandBase
	{		
        #region Implementation of ICommand
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            // all items must be versioned if we are going to run update.
            VersionedVisitor v = new VersionedVisitor();
            context.SolutionExplorer.VisitSelectedItems( v );
            
            if ( v.IsVersioned )
                return vsCommandStatus.vsCommandStatusEnabled |
                vsCommandStatus.vsCommandStatusSupported;
            else
                return vsCommandStatus.vsCommandStatusUnsupported;
        }

        public override void Execute(AnkhContext context)
        {
            // we assume by now that all items are working copy resources.
            context.SolutionExplorer.VisitSelectedItems( new UpdateVisitor() );
            context.SolutionExplorer.UpdateSelectionStatus();
        }    
        #endregion

        private class UpdateVisitor : LocalResourceVisitorBase
        {
            public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
            {
                resource.Update();
            }
        }
    }
}



