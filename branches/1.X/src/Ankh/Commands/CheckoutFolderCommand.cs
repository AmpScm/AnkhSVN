using System;
using System.IO;
using System.Windows.Forms;
using Utils;
using EnvDTE;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
	/// <summary>
	/// Command to check out current folder in the Repository Explorer.
	/// </summary>
    [VSNetCommand("CheckoutFolder",
         Text = "Chec&kout Folder...",
         Tooltip = "Checkout this folder.", 
         Bitmap = ResourceBitmaps.CheckoutDirectory ),
         VSNetControl( "ReposExplorer", Position = 1 )]
	public class CheckoutFolderCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            if ( context.RepositoryExplorer.SelectedNode != null &&
                context.RepositoryExplorer.SelectedNode.IsDirectory )
            {
                return Enabled;
            } 
            else
                return Disabled;
        }

        public override void Execute(IContext context, string parameters)
        {
            /// first get the parent folder
            FolderBrowser browser = new FolderBrowser();

            /// give a chance to the user to bail
            if ( browser.ShowDialog() != DialogResult.OK) 
                return;

            try
            {
                context.StartOperation( "Checking out" );

                INode node = context.RepositoryExplorer.SelectedNode;

                CheckoutRunner runner = new CheckoutRunner( 
                    browser.DirectoryPath, node.Revision, node.Url);
                context.UIShell.RunWithProgressDialog( runner, "Checking out folder" );

            }
            finally
            {
                context.EndOperation();
            }
        }

        #endregion
    }
}