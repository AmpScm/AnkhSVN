using System;
using EnvDTE;

namespace Ankh.Commands
{
	/// <summary>
	/// Command that refreshes the tree view
	/// </summary>
    [VSNetCommand("Refresh", Text = "Refresh", Tooltip = "Refresh this view."),
    VSNetControl( "Solution.Ankh", Position = 1 )]
	internal class RefreshCommand : CommandBase
	{
		
	    public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            return vsCommandStatus.vsCommandStatusEnabled | 
                vsCommandStatus.vsCommandStatusSupported;
        }

        public override void Execute(Ankh.AnkhContext context)
        {
            context.SolutionExplorer.SyncWithTreeView();
        }
    }
}
