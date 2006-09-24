using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Ankh.RepositoryExplorer;

namespace Ankh.Commands
{
    /// <summary>
    /// Lets the user cat a file from a repos and have Windows open it.
    /// </summary>
    [VSNetCommand("SaveToFile", Tooltip="Save the file to disk.", 
         Text = "Save to file", Bitmap = ResourceBitmaps.SaveToFile ),
    VSNetControl( "ReposExplorer.View", Position = 1 ) ]
    public class SaveToFileCommand : ViewRepositoryFileCommand
    {
        public SaveToFileCommand(IServiceProvider serviceProvider)
            : base(serviceProvider)
        { }

        public override void Execute(IContext context, string parameters)
        {
            using(OperationManager.RunOperation( "Saving" ))
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

                
                CatRunner runner = new CatRunner( node.Revision, node.Url, 
                    filename );
                context.UIShell.RunWithProgressDialog( runner, "Retrieving file" );
            }
        } 
    }
}
