using System;
using System.IO;
using EnvDTE;
using NSvn.Core;

namespace Ankh.Commands
{
    /// <summary>
    /// Enables or disables Ankh for a solution.
    /// </summary>
    [VSNetCommand("ToggleAnkh", Text="Enable Ankh for this solution", 
         Tooltip= "Enable Ankh for this solution", 
         Bitmap=ResourceBitmaps.ToggleAnkh ),
    VSNetControl( "Solution.Ankh", Position=1 )]
    public class ToggleAnkhCommand : CommandBase
    {
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
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
                 object cntl = this.GetControl( context, "Solution.Ankh", 
                    "ToggleAnkh" );

                string solutionPath = context.DTE.Solution.FullName;
                
                // if this path isn't valid, we don't wanna enable anything
                if ( !File.Exists( solutionPath ) )
                    return Disabled;

                string solutionDir = Path.GetDirectoryName(solutionPath);

                if ( ( !context.SolutionIsOpen ) )
                {
                    // we want it to show "Enable" if we're not in a wc
                    this.SetToolTipAndCaption( context, cntl, 
                        "Enable Ankh for this solution" );
                    return Disabled;
                }

                // now we have to figure out what text to set for the command    
                if ( File.Exists( Path.Combine( solutionDir, "Ankh.Load" ) ) ) 
                {
                    this.SetToolTipAndCaption( context, cntl, 
                        "Disable Ankh for this solution" );
                }
                else 
                {
                    // we will allow the user to load for a solution where the 
                    // solution dir is not versioned
                    if (!SvnUtils.IsWorkingCopyPath(
                        solutionDir))
                    {
                        this.SetToolTipAndCaption( context, cntl, 
                            "Force Ankh to load for this solution" );
                    }
                    else
                    { 
                        this.SetToolTipAndCaption( context, cntl, 
                            "Enable Ankh for this solution" );
                    }
                }               

                return Enabled;
            }
            finally
            {
                this.updating = false;
            }
        }

        public override void Execute(IContext context, string parameters)
        {
            string solutionDir = Path.GetDirectoryName(context.DTE.Solution.FullName);
            string noLoad = Path.Combine(solutionDir, "Ankh.NoLoad");
            string load = Path.Combine(solutionDir, "Ankh.Load");

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
                context.EnableAnkhForLoadedSolution();
            }
        }

        private void SetToolTipAndCaption( IContext context, object ctrl, string text )
        {
            context.CommandBars.SetControlCaption( ctrl, text );
            context.CommandBars.SetControlToolTip( ctrl, text );
        }

        private bool updating = false;


    }
}
