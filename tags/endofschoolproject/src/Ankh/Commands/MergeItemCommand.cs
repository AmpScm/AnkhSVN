// $Id$
using System;
using EnvDTE;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for MergeItem.
    /// </summary>
    [VSNetCommand("MergeItem", Text = "Update", Tooltip = "Merges the local item",
         Bitmap = ResourceBitmaps.Add),
     VSNetControl( "ReposExplorer", Position = 1 ) ]
    internal class MergeItem : CommandBase
    {
		
        public override void Execute(Ankh.AnkhContext context)
        {
        
        }

        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            return vsCommandStatus.vsCommandStatusEnabled | 
                vsCommandStatus.vsCommandStatusSupported;

        }
    }
}



