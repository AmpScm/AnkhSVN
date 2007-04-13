// $Id$
using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh.Commands
{
    /// <summary>
    /// Summary description for AddRepositoryRootCommand.
    /// </summary>
    [VSNetCommand("AddRepositoryRootCommand", Tooltip = "Add a new URL to the repository explorer",
         Text="A&dd Repository URL", Bitmap = ResourceBitmaps.AddURL )]
    [VSNetControl( "ReposExplorer", Position = 1 )]
    public class AddRepositoryRootCommand : CommandBase
    {		
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

    }
}
