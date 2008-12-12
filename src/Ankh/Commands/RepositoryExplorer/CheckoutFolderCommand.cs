using System;
using System.IO;
using System.Windows.Forms;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;

namespace Ankh.Commands.RepositoryExplorer
{
	/// <summary>
	/// Command to check out current folder in the Repository Explorer.
	/// </summary>
    [Command(AnkhCommand.CheckoutFolder)]
    class CheckoutFolderCommand : CommandBase
    {
        

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            e.Enabled = false;
            /*IExplorersShell shell = e.GetService<IExplorersShell>();

            if (shell.RepositoryExplorerService.SelectedNode == null ||
                !shell.RepositoryExplorerService.SelectedNode.IsDirectory)
            {
                e.Enabled = false;
            } */
        }

        public override void OnExecute(CommandEventArgs e)
        {
/*            IExplorersShell shell = e.GetService<IExplorersShell>();
            IContext context = e.Context.GetService<IContext>();

            /// first get the parent folder
			using (FolderBrowserDialog browser = new FolderBrowserDialog())
			{
				/// give a chance to the user to bail
				if (browser.ShowDialog() != DialogResult.OK)
					return;

				using(context.StartOperation("Checking out"))
                {
					INode node = shell.RepositoryExplorerService.SelectedNode;

					CheckoutRunner runner = new CheckoutRunner(
						browser.SelectedPath, node.Revision, new Uri(node.Url));
                    e.GetService<IProgressRunner>().Run(
                        "Checking out folder",
                        runner.Work);

				}
			}*/
        }
    }
}
