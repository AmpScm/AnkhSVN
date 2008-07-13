using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemOpenFolderInWorkingCopyExplorer)]
    [Command(AnkhCommand.ItemOpenFolderInRepositoryExplorer)]
    class OpenInXExplorer : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = e.Visible = false;
        }
        public override void OnExecute(CommandEventArgs e)
        {
            
        }
    }
}
