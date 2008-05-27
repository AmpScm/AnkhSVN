// $Id$
using System;
using Ankh.RepositoryExplorer;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to refresh the current item in the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.RefreshRepositoryItem)]
    public class RefreshRepositoryItemCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
        }

        #endregion
    }
}