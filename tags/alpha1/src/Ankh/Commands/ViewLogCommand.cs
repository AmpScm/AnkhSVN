// $Id$
using System;
using Ankh.UI;
using EnvDTE;

namespace Ankh.Commands
{
	/// <summary>
	/// Summary description for ViewLog.
	/// </summary>
    [VSNetCommand("ViewLog", Text = "Repos Explorer", Tooltip = "Show the repository explorer",
         Bitmap = ResourceBitmaps.ReposExplorer),
    VSNetControl( "MenuBar.View", Position = 1 )]
	internal class ViewLog : CommandBase
	{
        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            return vsCommandStatus.vsCommandStatusEnabled |
                vsCommandStatus.vsCommandStatusSupported;
        }
		
	    public override void Execute(Ankh.AnkhContext context)
        {
           context.RepositoryExplorerWindow.Visible = true;
        }        
    }
}



