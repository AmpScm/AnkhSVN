// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add a new URL to the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.AddRepositoryRoot)]
    public class AddRepositoryRootCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            RepositoryRootInfo info = context.UIShell.ShowAddRepositoryRootDialog();
            if ( info == null )
                return;

            context.RepositoryExplorer.AddRoot( info );
        }
    }
}
