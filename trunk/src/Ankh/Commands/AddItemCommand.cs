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
	/// Adds an unversioned item to a working copy
	/// </summary>
    [VSNetCommand("AddItem", Text = "Add", Tooltip = "Adds selected item to a working copy"),
    VSNetControl( "Item", Position = 3 ),
    VSNetControl( "Project", Position = 3 ),
    VSNetControl( "Folder", Position = 3 ),
    VSNetControl( "Solution", Position = 3)]
    
	internal class AddItemCommand : CommandBase
	{
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            UnversionedVisitor a = new UnversionedVisitor();
            context.SolutionExplorer.VisitSelectedItems( a );
            
            if ( !a.IsVersioned )
                return vsCommandStatus.vsCommandStatusEnabled |
                    vsCommandStatus.vsCommandStatusSupported;
            else
                return vsCommandStatus.vsCommandStatusEnabled;
        }

        public override void Execute(Ankh.AnkhContext context)
        {
            context.SolutionExplorer.VisitSelectedItems( new AddVisitor() );
            
            context.SolutionExplorer.RefreshSelectionParents();
        }
        #endregion
        
        /// <summary>
        /// A visitor that adds visited item to the Working copy.
        /// </summary>
        private class AddVisitor : LocalResourceVisitorBase
        {
            public override void VisitUnversionedResource(NSvn.UnversionedResource resource)
            {
                resource.Add( true );
            }
       
        }
    }
}



