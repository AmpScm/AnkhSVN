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
    [VSNetCommand("AddItem", Text = "Add", Tooltip = "Adds selected item to a working copy",
         Bitmap = ResourceBitmaps.Add),
    VSNetControl( "Item.Ankh", Position = 1 ),
    VSNetControl( "Project.Ankh", Position = 1 ),
    VSNetControl( "Folder.Ankh", Position = 1 ),
    VSNetControl( "Solution.Ankh", Position = 1)]
    
	internal class AddItemCommand : CommandBase
	{
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            UnversionedVisitor a = new UnversionedVisitor();
            context.SolutionExplorer.VisitSelectedItems( a, true );
            
            if ( !a.IsVersioned )
                return vsCommandStatus.vsCommandStatusEnabled |
                    vsCommandStatus.vsCommandStatusSupported;
            else
                return vsCommandStatus.vsCommandStatusEnabled;
        }

        public override void Execute(Ankh.AnkhContext context)
        {
            context.OutputPane.StartActionText( "Add" );
            context.SolutionExplorer.VisitSelectedItems( new AddVisitor(), true );
            context.OutputPane.EndActionText();
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



