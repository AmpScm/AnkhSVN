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
	internal class UpdateItem : CommandBase
	{	
	
        #region Implementation of ICommand
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            return vsCommandStatus.vsCommandStatusEnabled | 
                vsCommandStatus.vsCommandStatusSupported;
        }

        public override void Execute(AnkhContext context)
        {   
            System.Windows.Forms.MessageBox.Show( "Update invoked" );
        }
    
        #endregion
    }
}



