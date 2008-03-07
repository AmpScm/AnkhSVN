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
    [VSNetCommand(AnkhCommand.ExportFolder,
		"ExportFolder",
         Text = "E&xport Folder...",
         Tooltip="Export this folder.",          
         Bitmap = ResourceBitmaps.Export ),
         VSNetControl( "ReposExplorer", Position = 1 ) ]
    public class ExportFolderCommand : CommandBase
    {
        #region Implementation of ICommand

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if ( e.Context.RepositoryExplorer.SelectedNode == null ||
                !e.Context.RepositoryExplorer.SelectedNode.IsDirectory)
            {
                // BH: Why don't we allow exporting single files?
                e.Enabled = false;
            } 
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context;

            /// first get the parent folder
			using (FolderBrowserDialog browser = new FolderBrowserDialog())
			{

				/// give a chance to the user to bail
				if (browser.ShowDialog() != DialogResult.OK)
					return;

				try
				{
					context.StartOperation("Exporting");

					INode node = context.RepositoryExplorer.SelectedNode;

					ExportRunner runner = new ExportRunner(browser.SelectedPath, node.Revision, node.Url);
					context.UIShell.RunWithProgressDialog(runner, "Exporting folder");

				}
				finally
				{
					context.EndOperation();
				}
			}
        }

        #endregion
    }
}