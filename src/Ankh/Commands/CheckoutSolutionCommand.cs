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
    /// Command to checkout current solution in Repository Explorer.
    /// </summary>
    [VSNetCommand("CheckoutSolution",
         Text = "Checkout &Solution...",
         Tooltip = "Checkout this solution.", 
         Bitmap = ResourceBitmaps.CheckoutSolution),
         VSNetControl( "ReposExplorer", Position = 2 )]
    public class CheckoutSolutionCommand : CommandBase
    {
        #region Implementation of ICommand

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

                CheckoutRunner runner = new CheckoutRunner( browser.DirectoryPath, parent.Revision, new Uri(parent.Url));
                context.UIShell.RunWithProgressDialog( runner, "Checking out solution" );

                context.DTE.Solution.Open( Path.Combine( browser.DirectoryPath, node.Name ) );
            }
            finally
            {
                context.EndOperation();
            }
        }

        #endregion
    }
}