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
    [VSNetCommand("ViewInWindows",
         Text = "In Wi&ndows",
         Tooltip="Have Windows launch the associated application.", 
         Bitmap = ResourceBitmaps.ViewInWindows ),
         VSNetControl( "ReposExplorer.View", Position = 1 )]
    public class ViewInWindowsCommand : ViewRepositoryFileCommand
    {
        public override void Execute(IContext context, string parameters)
        {
            try
            {
                context.StartOperation( "Opening" );

                // make the catrunner get it on a separate thread.
                INode node = context.RepositoryExplorer.SelectedNode;
                CatRunner runner = new CatRunner( node.Name, 
                    node.Revision, new Uri(node.Url) );

                context.UIShell.RunWithProgressDialog( runner, "Retrieving" );
                 
                // now have windows try to start it.
                Process process = new Process();
                process.StartInfo.FileName = runner.Path;
                process.StartInfo.UseShellExecute = true;

                try
                {
                    process.Start();
                }
                catch( Win32Exception ex )
                {
                    // no application is associated with the file type
                    if ( ex.NativeErrorCode == NOASSOCIATEDAPP )
                        MessageBox.Show( "Windows could not find an application associated with the file type", 
                            "No associated application", MessageBoxButtons.OK );
                    else
                        throw;
                }
            }
            finally
            {
                context.EndOperation();
            }
        }        

        private const int NOASSOCIATEDAPP = 1155;
    }
}
