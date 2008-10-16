// $Id$
using System;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;
using Ankh.Scc;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to refresh the current item in the Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.RefreshRepositoryItem)]
    class RefreshRepositoryItemCommand : CommandBase
    {
        
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (ISvnRepositoryItem it in e.Selection.GetSelection<ISvnRepositoryItem>())
            {
                if (it.RepositoryRoot != null)
                    return;
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            foreach (ISvnRepositoryItem it in e.Selection.GetSelection<ISvnRepositoryItem>())
            {
                if (it.Uri != null)
                    it.RefreshItem(false);
            }
        }
    }
}