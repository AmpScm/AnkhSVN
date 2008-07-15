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
                shell.AddRepositoryRoot(info);
            }
        }
    }
}
