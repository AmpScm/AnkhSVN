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
    [VSNetCommand("SaveToFile", Tooltip="Save the file to disk.", Text = "Save to file" ),
    VSNetControl( "ReposExplorer.View", Position = 1 ) ]
    internal class SaveToFileCommand : ViewRepositoryFileCommand
    {
        public override void Execute(AnkhContext context, string parameters)
        {
            context.StartOperation( "Saving" );
            try
            {
                INode node = (INode)context.RepositoryExplorer.SelectedNode;
                string filename = null;
                using( SaveFileDialog sfd = new SaveFileDialog() )
                {
                    sfd.FileName = node.Name;
                    if ( sfd.ShowDialog() == DialogResult.OK )
                        filename = sfd.FileName;
                    else
                        return;
                }

                
                CatRunner runner = new CatRunner( context, node.Revision, node.Url, 
                    filename );
                runner.Start( "Retrieving file" );
            }
            finally
            {
                context.EndOperation();
            }
        } 
    }
}
