// $Id$
using System;
using EnvDTE;
using Ankh.UI;

namespace Ankh.Commands
{
    /// <summary>
    /// Command that refreshes the tree view
    /// </summary>
    [VSNetCommand("Refresh", Text = "Refresh", Tooltip = "Refresh this view.", 
         Bitmap = ResourceBitmaps.Refresh),
    VSNetControl( "Solution.Ankh", Position = 1 ),
    VSNetProjectNodeControl( "Ankh", Position = 1 ),
    VSNetControl( "Folder.Ankh", Position = 1)]
    internal class RefreshCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(Ankh.AnkhContext context)
        {
            return context.AnkhLoadedForSolution ? Enabled : Disabled;
        }

        public override void Execute(Ankh.AnkhContext context, string parameters)
        {
            try
            {
                context.StartOperation( "Refreshing solution explorer" );

                context.SolutionExplorer.RefreshSelection();
            }
            finally
            {
                context.EndOperation();
            }

        }
    }
}
