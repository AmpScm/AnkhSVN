// $Id$
using System;

namespace Ankh.Commands
{
	/// <summary>
	/// Commits an item.
	/// </summary>
	[VSNetCommand("CommitItem", Text = "Commit", Tooltip = "Commits an item")]
	internal class CommitItem : ICommand
	{		
	
        #region Implementation of ICommand
        public EnvDTE.vsCommandStatus QueryStatus(EnvDTE._DTE dte)
        {
            return new EnvDTE.vsCommandStatus();
        }
        public void Execute(EnvDTE._DTE dte)
        {
        
        }
    
        #endregion
    }
}



