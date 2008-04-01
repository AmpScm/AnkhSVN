using System;
using System.Collections.Generic;
using System.Text;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.Shell;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemOpenVisualStudio)]
    class OpenInVisualStudio : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems())
            {
                if (item.Exists)
                    return;
            }

            e.Enabled = false;
        }
        public override void OnExecute(CommandEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems())
            {
                if (item.Exists)
                    VsShellUtilities.OpenDocument(e.Context, item.FullPath);
            }            
        }
    }
}
