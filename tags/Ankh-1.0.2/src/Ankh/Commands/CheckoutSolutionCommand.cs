using System;
using System.IO;
using System.Windows.Forms;
using Utils;
using EnvDTE;
using Ankh.RepositoryExplorer;
using Ankh.UI;

namespace Ankh.Commands
{
	/// <summary>
	/// Summary description for ViewInVSNetCommand.
	/// </summary>
    [VSNetCommand("CheckoutSolution", Tooltip="Checkout this solution", 
         Bitmap = ResourceBitmaps.CheckoutSolution, Text = "Checkout Solution..." ),
    VSNetControl( "ReposExplorer", Position = 2 ) ]
	public class CheckoutSolutionCommand : 
        CommandBase
	{
        #region ICommand Members
        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            IRepositoryTreeNode node = context.RepositoryExplorer.SelectedNode;
            if ( node!= null && !node.IsDirectory && 
                node.Name.ToLower().EndsWith(".sln" ) )
            {
                return Enabled;
            }
            else
                return Disabled;
        }
        #endregion

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
                INode parent = node.Parent;

                CheckoutRunner runner = new CheckoutRunner( browser.DirectoryPath, parent.Revision, parent.Url);
                context.UIShell.RunWithProgressDialog( runner, "Checking out solution" );

                context.DTE.Solution.Open( System.IO.Path.Combine( browser.DirectoryPath, node.Name ) );
            }
            finally
            {
                context.EndOperation();
            }
        }  
	}
}
