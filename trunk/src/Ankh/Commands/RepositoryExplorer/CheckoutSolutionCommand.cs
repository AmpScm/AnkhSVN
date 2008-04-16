using System;
using System.IO;
using System.Windows.Forms;
using Utils;
using EnvDTE;
using Ankh.RepositoryExplorer;
using Ankh.UI;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;

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
            IExplorersShell shell = e.GetService<IExplorersShell>();

            IRepositoryTreeNode node = shell.RepositoryExplorerService.SelectedNode;
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
            IExplorersShell shell = e.GetService<IExplorersShell>();
            IContext context = e.GetService<IContext>();
            EnvDTE._DTE dte = e.GetService<EnvDTE._DTE>(typeof(Microsoft.VisualStudio.Shell.Interop.SDTE));

            /// first get the parent folder
			using (FolderBrowserDialog browser = new FolderBrowserDialog())
			{
				/// give a chance to the user to bail
				if (browser.ShowDialog() != DialogResult.OK)
					return;

				using(context.StartOperation("Checking out"))
				{
					INode node = shell.RepositoryExplorerService.SelectedNode;
					INode parent = node.Parent;

					CheckoutRunner runner = new CheckoutRunner(browser.SelectedPath, parent.Revision, new Uri(parent.Url));
                    e.GetService<IProgressRunner>().Run(
                        "Checking out solution",
                        runner.Work);

					dte.Solution.Open(Path.Combine(browser.SelectedPath, node.Name));
				}
			}
        }

        #endregion
    }
}
