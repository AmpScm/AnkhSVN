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
    [VSNetCommand("ExportFolder", Tooltip="Export this folder", 
         Text = "Export Folder...", Bitmap = ResourceBitmaps.Export ),
    VSNetControl( "ReposExplorer", Position = 1 ) ]
    internal class ExportFolderCommand : 
        CommandBase
    {
        #region ICommand Members
        public override EnvDTE.vsCommandStatus QueryStatus(AnkhContext context)
        {
            if ( context.RepositoryExplorer.SelectedNode != null &&
                context.RepositoryExplorer.SelectedNode.IsDirectory )
            {
                return Enabled;
            } 
            else
                return Disabled;
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
                context.StartOperation( "Exporting" );

                INode node = context.RepositoryExplorer.SelectedNode;

                ExportRunner runner = new ExportRunner(context, browser.DirectoryPath, node.Revision, node.Url);
                runner.Start( "Exporting folder" );

            }
            finally
            {
                context.EndOperation();
            }
        }  
    }
}
