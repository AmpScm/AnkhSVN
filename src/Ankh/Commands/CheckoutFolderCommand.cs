using System;
using System.IO;
using System.Windows.Forms;
using Utils;
using EnvDTE;
using NSvn;

namespace Ankh.Commands
{
	/// <summary>
	/// Summary description for ViewInVSNetCommand.
	/// </summary>
    [VSNetCommand("CheckoutFolder", Tooltip="Checkout this folder", Text = "Checkout Folder..." ),
    VSNetControl( "ReposExplorer", Position = 1 ) ]
	internal class CheckoutFolderCommand : 
        CheckoutCommand
	{
        public override void Execute(AnkhContext context)
        {
            /// first get the parent folder
            FolderBrowser browser = new FolderBrowser();

            /// give a chance to the user to bail
            if ( browser.ShowDialog() != DialogResult.OK) 
                return;

            context.StartOperation( "Checking out..." );

            try
            {
                CheckoutVisitor v = new CheckoutVisitor( );
                context.RepositoryController.VisitSelectedNodes( v );

                /// checkout all selected folders recurively
                foreach( RepositoryDirectory directory in v.Directories )
                {
                    context.OutputPane.WriteLine( "Checking out {0}", directory.Url );
                    directory.Checkout( browser.DirectoryPath, true );
                }
            }
            finally
            {
                context.EndOperation();
            }
        }  
	}
}
