// $Id$
using System;
using EnvDTE;

namespace Ankh.Commands
{
	/// <summary>
	/// Summary description for MergeItem.
	/// </summary>
	[RepositoryExplorerMenu("Merge", Position = 0)]
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



