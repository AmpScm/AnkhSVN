using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Ankh.RepositoryExplorer;
using AnkhSvn.Ids;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Lets the user cat a file from a repos and have Windows open it.
    /// </summary>
    [Command(AnkhCommand.ViewInWindows)]
    public class ViewInWindowsCommand : ViewRepositoryFileCommand
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            using (context.StartOperation("Opening"))
            {

                // make the catrunner get it on a separate thread.
                INode node = context.RepositoryExplorer.SelectedNode;
                CatRunner runner = new CatRunner(node.Name,
                    node.Revision, new Uri(node.Url));

                e.GetService<IProgressRunner>().Run("Retrieving", runner.Work);

                // now have windows try to start it.
                Process process = new Process();
                process.StartInfo.FileName = runner.Path;
                process.StartInfo.UseShellExecute = true;

                try
                {
                    process.Start();
                }
                catch (Win32Exception ex)
                {
                    // no application is associated with the file type
                    if (ex.NativeErrorCode == NOASSOCIATEDAPP)
                        MessageBox.Show("Windows could not find an application associated with the file type",
                            "No associated application", MessageBoxButtons.OK);
                    else
                        throw;
                }
            }
        }

        private const int NOASSOCIATEDAPP = 1155;
    }
}
