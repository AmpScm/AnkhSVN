// $Id$
using System;
using EnvDTE;

namespace Ankh.Commands
{
	/// <summary>
	/// Commits an item.
	/// </summary>
	[VSNetCommand("CommitItem", Text = "Commit", Tooltip = "Commits an item"),
     VSNetControl( "Item", Position = 2 )]
	internal class CommitItem : CommandBase
	{	
        #region Implementation of ICommand
        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            if (context.DTE.SelectedItems.Count > 0)
                return vsCommandStatus.vsCommandStatusEnabled | 
                    vsCommandStatus.vsCommandStatusSupported;
            else
                return vsCommandStatus.vsCommandStatusEnabled;
        }
        public override void Execute(Ankh.AnkhContext context)
        {
//            context.SolutionExplorer.ChangeStatus( 
//                context.DTE.SelectedItems );
        }
        
        #endregion
    }
}



