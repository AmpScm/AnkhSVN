// $Id$
using System;

namespace Ankh.Commands
{
	/// <summary>
	/// Commits an item.
	/// </summary>
	[VSNetCommand("CommitItem", Text = "Commit", Tooltip = "Commits an item")]
	internal class CommitItem : CommandBase
	{	
        #region Implementation of ICommand
        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            return new EnvDTE.vsCommandStatus();
        }
        public override void Execute(Ankh.AnkhContext context)
        {
        
        }
        
        #endregion
    }
}



