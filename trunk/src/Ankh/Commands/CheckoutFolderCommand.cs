using System;
using System.IO;
using System.Windows.Forms;
using Ankh.RepositoryExplorer;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
	/// <summary>
	/// Command to check out current folder in the Repository Explorer.
	/// </summary>
    [Command(AnkhCommand.CheckoutFolder)]
    public class CheckoutFolderCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            if (context.RepositoryExplorer.SelectedNode == null ||
                !context.RepositoryExplorer.SelectedNode.IsDirectory )
            {
                e.Enabled = false;
            } 
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

					CheckoutRunner runner = new CheckoutRunner(
						browser.SelectedPath, node.Revision, new Uri(node.Url));
                    e.GetService<IProgressRunner>().Run(
                        "Checking out folder",
                        runner.Work);

				}
			}
        }

        #endregion
    }
}