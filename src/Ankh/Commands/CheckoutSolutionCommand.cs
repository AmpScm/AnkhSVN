using System;
using System.IO;
using System.Windows.Forms;
using Utils;
using EnvDTE;
using Ankh.RepositoryExplorer;
using Ankh.UI;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to checkout current solution in Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.CheckoutSolution)]
    public class CheckoutSolutionCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            IRepositoryTreeNode node = context.RepositoryExplorer.SelectedNode;
            if (node != null && !node.IsDirectory &&
                node.Name.EndsWith(".sln", StringComparison.OrdinalIgnoreCase))
            {
                // e.Enabled = true;
            }
            else
                e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            /// first get the parent folder
			using (FolderBrowserDialog browser = new FolderBrowserDialog())
			{
				/// give a chance to the user to bail
				if (browser.ShowDialog() != DialogResult.OK)
					return;

				using(context.StartOperation("Checking out"))
				{
					INode node = context.RepositoryExplorer.SelectedNode;
					INode parent = node.Parent;

					CheckoutRunner runner = new CheckoutRunner(browser.SelectedPath, parent.Revision, new Uri(parent.Url));
					context.UIShell.RunWithProgressDialog(runner, "Checking out solution");

					((IDTEContext)context).DTE.Solution.Open(Path.Combine(browser.SelectedPath, node.Name));
				}
			}
        }

        #endregion
    }
}
