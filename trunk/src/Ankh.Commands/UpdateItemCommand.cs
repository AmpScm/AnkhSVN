// $Id$
using System;
using EnvDTE;

namespace Ankh.Commands
{
	/// <summary>
	/// A command that updates an item.
	/// </summary>
	[VSNetCommand("UpdateItem", Text = "Update", Tooltip = "Updates the local item"),
     VSNetControl( "Item", Position = 2 ),
     VSNetControl( "Tools", Position = 4 )]
	internal class UpdateItem : ICommand
	{	
	
        #region Implementation of ICommand
        public EnvDTE.vsCommandStatus QueryStatus(EnvDTE._DTE dte)
        {
            return vsCommandStatus.vsCommandStatusEnabled | 
                vsCommandStatus.vsCommandStatusSupported;
        }

        public void Execute(EnvDTE._DTE dte)
        {   
            System.Windows.Forms.MessageBox.Show( "Update invoked" );
        }
    
        #endregion
    }
}



