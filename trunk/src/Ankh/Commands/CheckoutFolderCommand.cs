using System;
using System.IO;
using System.Windows.Forms;
using EnvDTE;
using NSvn;

namespace Ankh.Commands
{
	/// <summary>
	/// Summary description for ViewInVSNetCommand.
	/// </summary>
    [VSNetCommand("Checkout", Tooltip="Checkout this folder", Text = "Checkout" ),
    VSNetControl( "ReposExplorer", Position = 2 ) ]
	internal class CheckoutFolderCommand : CheckoutCommand
	{
        public override void Execute(AnkhContext context)
        {
            /// first get the parent folder
            FolderBrowserDialog browser = new FolderBrowserDialog();
            browser.Description = "Select the location where to checkout";
            browser.ShowNewFolderButton = true;

            /// give a chance to the user to bail
            if ( browser.ShowDialog() != DialogResult.OK) 
                return;

            /// save parent folder path
            this.parentPath = browser.SelectedPath;

            CheckoutVisitor v = new CheckoutVisitor( );
            context.RepositoryController.VisitSelectedNodes( v );

            /// checkout all selected folders recurively
            foreach( RepositoryDirectory directory in v.Directories )
            {
                string directoryPath = GetPath( directory.Name );
                directory.Checkout( directoryPath, true );
            }
        }  

        /// <summary>
        /// Compines the current parent path with a foldername
        /// </summary>
        /// <param name="foldername">The foldername to combine</param>
        /// <returns>The full path</returns>
        protected virtual string GetPath( string foldername )
        {
            return Path.Combine( this.parentPath, foldername );
        }

        /// <summary>
        /// This is where the checkout wil be placed
        /// </summary>
        private string parentPath;
	}
}
