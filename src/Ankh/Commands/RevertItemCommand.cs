// $Id$
using System;
using NSvn;
using NSvn.Core;
using NSvn.Common;
using EnvDTE;
using Ankh.UI;

namespace Ankh.Commands
{
	/// <summary>
	/// Summary description for RevertItem.
	/// </summary>

    [VSNetCommand("RevertItem", Text = "Revert", Tooltip = "Reverts selected item"),
    VSNetControl( "Item.Ankh", Position = 1 ),
    VSNetControl( "Project.Ankh", Position = 1 ),
    VSNetControl( "Folder.Ankh", Position = 1 ),
    VSNetControl( "Solution.Ankh", Position = 1)]
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



