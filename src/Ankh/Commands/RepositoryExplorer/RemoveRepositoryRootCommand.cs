// $Id$
using System;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to remove a URL from the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.RemoveRepositoryRoot)]
    public class RemoveRepositoryRootCommand : CommandBase
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