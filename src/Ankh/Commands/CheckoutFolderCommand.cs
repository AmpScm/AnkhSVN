using System;
using System.IO;
using System.Windows.Forms;
using Utils;
using EnvDTE;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
	/// <summary>
	/// Command for checking out a folder
	/// </summary>
    [VSNetCommand("CheckoutFolder", Tooltip="Checkout this folder", 
         Text = "Checkout Folder...", Bitmap = ResourceBitmaps.CheckoutDirectory ),
    VSNetControl( "ReposExplorer", Position = 1 ) ]
	public class CheckoutFolderCommand : 
        CommandBase
	{
        #region ICommand Members
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            if ( context.RepositoryExplorer.SelectedNode != null &&
                context.RepositoryExplorer.SelectedNode.IsDirectory )
            {
                return Enabled;
            } 
            else
                return Disabled;
        }
        #endregion

        public override void Execute(AnkhContext context, string parameters)
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

                CheckoutRunner runner = new CheckoutRunner(context, browser.DirectoryPath, node.Revision, node.Url);
                runner.Start( "Checking out folder" );

            }
            finally
            {
                context.EndOperation();
            }
        }  
	}
}
