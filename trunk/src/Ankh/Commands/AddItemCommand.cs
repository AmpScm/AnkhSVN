// $Id$
using System;
using NSvn;
using NSvn.Core;
using NSvn.Common;
using EnvDTE;
namespace Ankh.Commands
{
	/// <summary>
	/// Summary description for AddItem.
	/// </summary>
	[VSNetCommand( "AddItem", Text = "Add", Tooltip = "Add files to revision control"),
     VSNetControl( "Item", Position = 3)]

    
	internal class AddItemCommand : CommandBase
	{
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            ILocalResource[] resources = context.SolutionExplorer.GetSelectedItems();        

            bool addAble = true;
            foreach( ILocalResource resource in resources )
            {
               if (resource.IsVersioned)
                   addAble = false;
            }
            if (addAble)
                return vsCommandStatus.vsCommandStatusEnabled |
                    vsCommandStatus.vsCommandStatusSupported;
            else return vsCommandStatus.vsCommandStatusUnsupported ;
        }

        public override void Execute(Ankh.AnkhContext context)
        {
            ILocalResource[] resources = context.SolutionExplorer.GetSelectedItems();        

            foreach( ILocalResource resource in resources )
            {
                context.SolutionExplorer.UpdateItem( resource, 
                    (( UnversionedResource) resource).Add( false ) );
            }
        }
        #endregion
    }
}



