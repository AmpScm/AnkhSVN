// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to add a new URL to the Repository Explorer.
    /// </summary>
    [VSNetCommand("AddRepositoryRootCommand",
       Text = "A&dd Repository URL",
        Tooltip = "Add a new URL to the Repository Explorer.",
        Bitmap = ResourceBitmaps.AddURL)]
        [VSNetControl( "ReposExplorer", Position = 1 )]
    public class AddRepositoryRootCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            return Enabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            RepositoryRootInfo info = context.UIShell.ShowAddRepositoryRootDialog();
            if ( info == null )
                return;

            context.RepositoryExplorer.AddRoot( info );
        }

        #endregion
    }
}
