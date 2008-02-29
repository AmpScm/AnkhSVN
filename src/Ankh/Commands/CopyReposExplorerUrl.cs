// $Id$
using System;
using EnvDTE;
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

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            // all we need is a selection in the repos explorer
            if ( context.RepositoryExplorer.SelectedNode != null )
                return Enabled;
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            INode node = context.RepositoryExplorer.SelectedNode;
            Clipboard.SetDataObject( node.Url );
        }

        #endregion
    }
}