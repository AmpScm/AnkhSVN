// $Id$
using System;
using NSvn;
using NSvn.Core;
using NSvn.Common;
using EnvDTE;

namespace Ankh.Commands
{
	/// <summary>
	/// Summary description for RevertItem.
	/// </summary>

    [VSNetCommand("RevertItem", Text = "Revert", Tooltip = "Reverts selected item"),
    VSNetControl( "Item", Position = 4 ),
    VSNetControl( "Project", Position = 4 ),
    VSNetControl( "Folder", Position = 4 ),
    VSNetControl( "Solution", Position = 4)]

    public class RevertItem
	{
        internal class RevertItemCommand : CommandBase
        {
        #region Implementation of ICommand

            public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
            {
                ModifiedVisitor m = new ModifiedVisitor();
                context.SolutionExplorer.VisitSelectedItems( m );
            
                if ( m.Modified )
                    return vsCommandStatus.vsCommandStatusEnabled |
                        vsCommandStatus.vsCommandStatusSupported;
                else
                    return vsCommandStatus.vsCommandStatusEnabled;
            }

            public override void Execute(Ankh.AnkhContext context)
            {
                context.SolutionExplorer.VisitSelectedItems( new RevertVisitor() );
                context.SolutionExplorer.UpdateSelectionStatus();
            }
        #endregion
        
            /// <summary>
            /// A visitor reverts visited item in the Working copy.
            /// </summary>
            private class RevertVisitor : LocalResourceVisitorBase
            {
                public override void VisitWorkingCopyResource(NSvn.WorkingCopyResource resource)
                {
                    resource.Revert( true );
                }
            }
        }
	}
}



