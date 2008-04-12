using System;
using System.IO;
using System.Windows.Forms;
using Utils;
using Ankh.RepositoryExplorer;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command for exporting a folder
    /// </summary>
    [Command(AnkhCommand.ExportFolder)]
    public class ExportFolderCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            if (context.RepositoryExplorer.SelectedNode == null ||
                !context.RepositoryExplorer.SelectedNode.IsDirectory)
            {
                // BH: Why don't we allow exporting single files?
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

                using (context.StartOperation("Exporting"))
                {
                    INode node = context.RepositoryExplorer.SelectedNode;

                    ExportRunner runner = new ExportRunner(browser.SelectedPath, node.Revision, node.Url);
                    e.GetService<IProgressRunner>().Run(
                        "Exporting folder",
                        runner.Work);
                }
            }
        }

        #endregion
    }
}