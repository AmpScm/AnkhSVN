// $Id$
using EnvDTE;
using System.Collections;

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
            if ( context.SolutionExplorer.GetSelectionResources( false, 
                new ResourceFilterCallback(CommandBase.UnversionedFilter)).Count > 0 )
            {
                return vsCommandStatus.vsCommandStatusEnabled |
                    vsCommandStatus.vsCommandStatusSupported;
            }
            else
                return vsCommandStatus.vsCommandStatusSupported;
        }

        public override void Execute(Ankh.AnkhContext context, string parameters )
        {
            IList resources = context.SolutionExplorer.GetSelectionResources( false,
                new ResourceFilterCallback(CommandBase.UnversionedFilter) );

            context.StartOperation( "Adding" );

            foreach( SvnItem item in resources )
            {
                context.Context.Add( item.Path, true );
                item.Refresh( context.Context );
            }


            context.EndOperation();
        }
        #endregion
    }
}



