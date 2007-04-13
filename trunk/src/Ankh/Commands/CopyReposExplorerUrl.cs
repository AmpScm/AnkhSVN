// $Id$
using System;
using EnvDTE;
using Ankh.UI;
using Clipboard = System.Windows.Forms.Clipboard;
using Ankh.RepositoryExplorer;


namespace Ankh.Commands
{
	/// <summary>
	/// Copies the URL from the selected node in the Repository Explorer
	/// to the system clipboard
	/// </summary>
	[VSNetCommand("CopyReposExplorerUrl", 
         Tooltip = "Copy the URL of this item to the system clipboard", 
         Text = "Copy &URL to clipboard", Bitmap = ResourceBitmaps.CopyUrlToClipboard ),
     VSNetControl( "ReposExplorer", Position=1 )]
	public class CopyReposExplorerUrl : CommandBase
	{
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
	}
}
