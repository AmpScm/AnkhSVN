using System;
using System.IO;
using System.Windows.Forms;
using Utils;
using EnvDTE;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
	/// <summary>
	/// Summary description for ViewInVSNetCommand.
	/// </summary>
    [VSNetCommand("CheckoutSolution", Tooltip="Checkout this solution", Text = "Checkout Solution..." ),
    VSNetControl( "ReposExplorer", Position = 2 ) ]
	internal class CheckoutSolutionCommand : 
        CheckoutCommand
	{
        #region ICommand Members
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            return context.RepositoryExplorer.SelectedNode.IsDirectory ? 
                vsCommandStatus.vsCommandStatusSupported :
                context.RepositoryExplorer.SelectedNode.Name.ToLower().EndsWith(".sln") ?
                vsCommandStatus.vsCommandStatusSupported | vsCommandStatus.vsCommandStatusEnabled :
                vsCommandStatus.vsCommandStatusSupported;
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

                INode node = (INode)context.RepositoryExplorer.SelectedNode;
                INode parent = node.Parent;

                CheckoutRunner runner = new CheckoutRunner(context, browser.DirectoryPath, parent.Revision, parent.Url);
                runner.Start( "Checking out solution" );

                context.DTE.Solution.Open( System.IO.Path.Combine( browser.DirectoryPath, node.Name ) );
            }
            finally
            {
                context.EndOperation();
            }
        }  
	}
}
