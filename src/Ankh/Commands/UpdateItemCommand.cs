// $Id$
using System;
using EnvDTE;
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
            if (context.DTE.SelectedItems.Count > 0)
                return vsCommandStatus.vsCommandStatusEnabled | 
                vsCommandStatus.vsCommandStatusSupported;
            else
                return vsCommandStatus.vsCommandStatusEnabled;
        }

        public override void Execute(AnkhContext context)
        {
            new TestRepositoryExplorer().ShowDialog();
            //context.SolutionExplorer.UpdateSelectionStatus();
        }
    
        #endregion
    }
}



