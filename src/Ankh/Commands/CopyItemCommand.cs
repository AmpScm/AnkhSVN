// $Id$
using System;
using EnvDTE;
using Utils;

namespace Ankh.Commands
{
	/// <summary>
	/// Summary description for CopyItem.
	/// </summary>
    [VSNetCommand("CopyItem", Text = "GC Collect", Tooltip = "Commits an item"),
    VSNetControl( "Tools", Position = 2 )]
	internal class CopyItem : CommandBase
	{
		
	    public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            return vsCommandStatus.vsCommandStatusEnabled | 
                vsCommandStatus.vsCommandStatusSupported;
        }

        public override void Execute(Ankh.AnkhContext context)
        {
            GC.Collect();
        
        }
    }
}



