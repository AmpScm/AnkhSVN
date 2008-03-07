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
    [VSNetCommand(AnkhCommand.AddRepositoryRoot,
		"AddRepositoryRootCommand",
       Text = "A&dd Repository URL",
        Tooltip = "Add a new URL to the Repository Explorer.",
        Bitmap = ResourceBitmaps.AddURL)]
        [VSNetControl( "ReposExplorer", Position = 1 )]
    public class AddRepositoryRootCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            RepositoryRootInfo info = context.UIShell.ShowAddRepositoryRootDialog();
            if ( info == null )
                return;

            context.RepositoryExplorer.AddRoot( info );
        }
    }
}
