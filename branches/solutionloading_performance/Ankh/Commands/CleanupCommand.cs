// $Id$
using System;
using EnvDTE;
using System.Collections;


namespace Ankh.Commands
{
    /// <summary>
    /// Cleans up a working copy directory.
    /// </summary>
    [VSNetCommand( "Cleanup", Text="Cleanup", Tooltip = "Cleans up the working copy", 
         Bitmap = ResourceBitmaps.Cleanup ),
    VSNetFolderNodeControl( "Ankh", Position = 1),
    VSNetProjectNodeControl( "Ankh", Position = 1 ),
    VSNetControl( "Solution.Ankh", Position = 1 )]
    public class Cleanup : CommandBase
    {  
    
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            return Enabled;
        }
    
        public override void Execute(IContext context, string parameters)
        {
            context.StartOperation( "Running cleanup" );

            IList resources = context.SolutionExplorer.GetSelectionResources( false,
                new ResourceFilterCallback(CommandBase.DirectoryFilter) );
            foreach( SvnItem item in resources )
                context.Client.Cleanup( item.Path );

            context.EndOperation();
        }
    }
}



