// $Id$
using System;
using EnvDTE;
using NSvn;

namespace Ankh.Commands
{
    /// <summary>
    /// Cleans up a working copy directory.
    /// </summary>
    [VSNetCommand( "Cleanup", Text="Cleanup", Tooltip = "Cleans up the working copy", 
         Bitmap = ResourceBitmaps.Cleanup ),
     VSNetControl( "Folder.Ankh", Position = 1 ),
     VSNetControl( "Project.Ankh", Position = 1 ),
     VSNetControl( "Solution.Ankh", Position = 1 )]
    internal class Cleanup : CommandBase
    {  
    
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            return vsCommandStatus.vsCommandStatusEnabled | 
                vsCommandStatus.vsCommandStatusSupported;
        }
    
        public override void Execute(AnkhContext context)
        {
            context.OutputPane.StartActionText( "Running cleanup" );

            CleanupVisitor v = new CleanupVisitor();
            context.SolutionExplorer.VisitSelectedItems( v, false );

            context.OutputPane.EndActionText();
        }

        private class CleanupVisitor : LocalResourceVisitorBase
        {            
            public override void VisitWorkingCopyDirectory(WorkingCopyDirectory dir)
            {
                dir.Cleanup();
            }
        }
    }
}



