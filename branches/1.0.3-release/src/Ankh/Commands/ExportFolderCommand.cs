using System;
using System.IO;
using System.Windows.Forms;
using Utils;
using EnvDTE;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// Command for exporting a folder
    /// </summary>
    [VSNetCommand("ExportFolder",
         Tooltip="Export this folder.", 
         Text = "E&xport Folder...",
         Bitmap = ResourceBitmaps.Export ),
         VSNetControl( "ReposExplorer", Position = 1 ) ]
    public class ExportFolderCommand : CommandBase
    {
        #region Implementation of ICommand

        public override EnvDTE.vsCommandStatus QueryStatus(IContext context)
        {
            if ( context.RepositoryExplorer.SelectedNode != null &&
                context.RepositoryExplorer.SelectedNode.IsDirectory )
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
                context.StartOperation( "Exporting" );

                INode node = context.RepositoryExplorer.SelectedNode;

                ExportRunner runner = new ExportRunner( browser.DirectoryPath, node.Revision, node.Url);
                context.UIShell.RunWithProgressDialog( runner, "Exporting folder" );

            }
            finally
            {
                context.EndOperation();
            }
        }

        #endregion
    }
}