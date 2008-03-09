using System;
using System.IO;

using Utils.Services;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Enables or disables Ankh for a solution.
    /// </summary>
    [VSNetCommand(AnkhCommand.ToggleAnkh,
        "ToggleAnkh",
         Text = "Enable AnkhSVN for this solution",
         Tooltip = "Enable Ankh for this solution.",
         Bitmap = ResourceBitmaps.ToggleAnkh),
         VSNetControl("Solution." + VSNetControlAttribute.AnkhSubMenu, Position = 1)]
    public class ToggleAnkhCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (this.updating)
            {
                return; // Enabled
            }

            this.updating = true;

            try
            {
                string solutionPath = e.Selection.SolutionFilename;

                // if this path isn't valid, we don't wanna enable anything
                if (!File.Exists(solutionPath))
                {
                    e.Enabled = false;
                    return;
                }

                string solutionDir = Path.GetDirectoryName(solutionPath);

                if ((!e.Context.SolutionIsOpen))
                {
                    // we want it to show "Enable" if we're not in a wc
                    e.Text = "Enable AnkhSVN for this solution";
                    e.Enabled = false;
                    return;
                }

                // now we have to figure out what text to set for the command    
                if (File.Exists(Path.Combine(solutionDir, "Ankh.Load")))
                {
                    e.Text = "Disable AnkhSVN for this solution";
                }
                else
                {
                    // we will allow the user to load for a solution where the 
                    // solution dir is not versioned
                    if (!AnkhServices.GetService<IWorkingCopyOperations>().IsWorkingCopyPath(
                        solutionDir))
                    {
                        e.Text = "Force AnkhSVN to load for this solution";
                    }
                    else
                    {
                        e.Text = "Enable AnkhSVN for this solution";
                    }
                }

                return;
            }
            finally
            {
                this.updating = false;
            }
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            string solutionDir = Path.GetDirectoryName(e.Selection.SolutionFilename);
            string noLoad = Path.Combine(solutionDir, "Ankh.NoLoad");
            string load = Path.Combine(solutionDir, "Ankh.Load");

            // disable or enable?
            if (File.Exists(load))
            {
                File.Delete(load);
                File.Create(noLoad).Close();
                context.SolutionClosing();
            }
            else
            {
                // delete doesnt throw if the file doesn't exist
                File.Delete(noLoad);
                File.Create(load).Close();
                context.EnableAnkhForLoadedSolution();
            }
        }

        #endregion

        private void SetToolTipAndCaption(IContext context, object ctrl, string text)
        {
            // TODO: Use new command routing for this
            //context.CommandBars.SetControlCaption( ctrl, text );
            //context.CommandBars.SetControlToolTip( ctrl, text + ".");
        }

        private bool updating = false;
    }
}
