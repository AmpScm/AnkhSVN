// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;
using SharpSvn;
using Ankh.UI;
using Ankh.UI.RepositoryExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to add a new URL to the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.RepositoryBrowse, ArgumentDefinition="u", AlwaysAvailable=true)]
    class RepositoryBrowseCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IUIShell shell = e.GetService<IUIShell>();
            Uri info;

            if (e.Argument is string)
            {
                // Allow opening from
                info = new Uri((string)e.Argument);
            }
            else if (e.Argument is Uri)
                info = (Uri)e.Argument;
            else
                info = shell.ShowAddRepositoryRootDialog();

            if (info != null)
            {
                RepositoryExplorerControl ctrl = e.Selection.ActiveDialogOrFrameControl as RepositoryExplorerControl;

                if (ctrl == null)
                {
                    IAnkhPackage pkg = e.GetService<IAnkhPackage>();
                    pkg.ShowToolWindow(AnkhToolWindow.RepositoryExplorer);
                }

                ctrl = e.Selection.ActiveDialogOrFrameControl as RepositoryExplorerControl;

                if (ctrl != null)
                    ctrl.AddRoot(info);
            }
        }
    }
}
