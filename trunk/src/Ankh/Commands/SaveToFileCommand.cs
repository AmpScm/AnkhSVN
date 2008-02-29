using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Ankh.RepositoryExplorer;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to save currnet file to disk from Repository Explorer.
    /// </summary>
    [VSNetCommand(AnkhCommand.SaveToFile,
		"SaveToFile",
        Text = "Save to &File",
        Tooltip="Save the file to disk.", 
        Bitmap = ResourceBitmaps.SaveToFile ),
        VSNetControl( "ReposExplorer.View", Position = 1 ) ]
    public class SaveToFileCommand : ViewRepositoryFileCommand
    {
        #region Implementation of ICommand

        public override void Execute(IContext context, string parameters)
        {
            context.StartOperation( "Saving" );
            try
            {
                INode node = context.RepositoryExplorer.SelectedNode;
                string filename = null;
                using( SaveFileDialog sfd = new SaveFileDialog() )
                {
                    sfd.FileName = node.Name;
                    if ( sfd.ShowDialog() == DialogResult.OK )
                        filename = sfd.FileName;
                    else
                        return;
                }

                
                CatRunner runner = new CatRunner( node.Revision, new Uri(node.Url), 
                    filename );
                context.UIShell.RunWithProgressDialog( runner, "Retrieving file" );
            }
            finally
            {
                context.EndOperation();
            }
        }

        #endregion
    }
}