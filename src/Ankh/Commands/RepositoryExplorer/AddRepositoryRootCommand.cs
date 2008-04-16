// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to add a new URL to the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.AddRepositoryRoot)]
    public class AddRepositoryRootCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IExplorersShell shell = e.GetService<IExplorersShell>();

            RepositoryRootInfo info = shell.ShowAddRepositoryRootDialog();
            if ( info == null )
                return;

            shell.RepositoryExplorerService.AddRoot( info );
        }
    }
}
