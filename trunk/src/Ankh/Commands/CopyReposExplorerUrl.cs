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
    [VSNetCommand(AnkhCommand.CopyReposExplorerUrl,
		"CopyReposExplorerUrl",
        Text = "Copy &URL",
        Tooltip = "Copy the URL of this item to the clipboard.", 
         Bitmap = ResourceBitmaps.CopyUrlToClipboard ),
         VSNetControl( "ReposExplorer", Position = 1)]
    public class CopyReposExplorerUrl : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            // all we need is a selection in the repos explorer
            if (e.Context.RepositoryExplorer.SelectedNode == null)
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            INode node = context.RepositoryExplorer.SelectedNode;
            Clipboard.SetDataObject( node.Url );
        }

        #endregion
    }
}