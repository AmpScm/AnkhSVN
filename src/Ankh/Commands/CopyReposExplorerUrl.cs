// $Id$
using System;
using Ankh.UI;
using Clipboard = System.Windows.Forms.Clipboard;
using Ankh.RepositoryExplorer;
using AnkhSvn.Ids;

namespace Ankh.Commands
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
            IContext context = e.Context.GetService<IContext>();
            // all we need is a selection in the repos explorer
            if (context.RepositoryExplorer.SelectedNode == null)
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            INode node = context.RepositoryExplorer.SelectedNode;
            Clipboard.SetDataObject( node.Url );
        }

        #endregion
    }
}