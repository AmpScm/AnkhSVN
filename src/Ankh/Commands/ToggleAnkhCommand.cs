using System;
using System.IO;
using EnvDTE;
using NSvn.Core;
using Microsoft.Office.Core;

namespace Ankh.Commands
{
    /// <summary>
    /// Enables or disables Ankh for a solution.
    /// </summary>
    [VSNetCommand("ToggleAnkh", Text="Enable Ankh for this solution", 
         Tooltip= "Enable Ankh for this solution", 
         Bitmap=ResourceBitmaps.Default ),
    VSNetControl( "Solution.Ankh", Position=1 )]
    internal class ToggleAnkhCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            // QueryStatus gets called when we set the Caption
            // we must prevent infinite recursion
            if ( this.updating ) 
            {
                return Enabled;
            }

            this.updating = true;

            try
            {
                 CommandBarControl cntl = this.GetControl( context, "Solution.Ankh", 
                    "ToggleAnkh" );

                string solutionPath = context.DTE.Solution.FullName;
                
                // if this path isn't valid, we don't wanna enable anything
                if ( !File.Exists( solutionPath ) )
                    return Disabled;

                if ( ( !context.SolutionIsOpen || (!SvnUtils.IsWorkingCopyPath(
                    Path.GetDirectoryName(solutionPath)))))
                {
                    // we want it to show "Enable" if we're not in a wc
                    cntl.Caption = cntl.TooltipText = "Enable Ankh for this solution";
                    return Disabled;
                }

                // now we have to figure out what text to set for the command           
                string adminDir = Path.Combine( Path.GetDirectoryName(solutionPath),
                    Client.AdminDirectoryName );
                if ( File.Exists( Path.Combine( adminDir, "Ankh.Load" ) ) )            
                    cntl.Caption = cntl.TooltipText = "Disable Ankh for this solution";
                else 
                    cntl.Caption = cntl.TooltipText = "Enable Ankh for this solution";

                return Enabled;
            }
            finally
            {
                this.updating = false;
            }
        }

        public override void Execute(AnkhContext context, string parameters)
        {
            string adminDir = Path.Combine( 
                Path.GetDirectoryName(context.DTE.Solution.FullName), 
                Client.AdminDirectoryName );
            string noLoad = Path.Combine(adminDir, "Ankh.NoLoad");
            string load = Path.Combine(adminDir, "Ankh.Load");

            // disable or enable?
            if ( File.Exists( load ) )
            {
                File.Delete( load );
                File.Create( noLoad ).Close();
                context.SolutionClosing();
            }
            else
            {
                // delete doesnt throw if the file doesn't exist
                File.Delete( noLoad );
                File.Create( load ).Close();
                context.SolutionOpened();
            }
        }

        private bool updating = false;


    }
}
