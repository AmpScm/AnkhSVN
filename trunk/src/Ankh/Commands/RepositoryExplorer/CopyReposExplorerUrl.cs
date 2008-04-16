// $Id$
using System;
using Ankh.UI;
using Clipboard = System.Windows.Forms.Clipboard;
using Ankh.RepositoryExplorer;
using AnkhSvn.Ids;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to copy the URL of this item to the clipboard in Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.CopyReposExplorerUrl)]
    public class CopyReposExplorerUrl : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IExplorersShell shell = e.GetService<IExplorersShell>();
            // all we need is a selection in the repos explorer
            if (shell.RepositoryExplorerService.SelectedNode == null)
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IExplorersShell shell = e.GetService<IExplorersShell>();

            INode node = shell.RepositoryExplorerService.SelectedNode;
            Clipboard.SetDataObject( node.Url );
        }

        #endregion
    }
}