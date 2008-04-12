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
    [Command(AnkhCommand.SaveToFile)]
    public class SaveToFileCommand : ViewRepositoryFileCommand
    {
        #region Implementation of ICommand

        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            using (context.StartOperation("Saving"))
            {
                INode node = context.RepositoryExplorer.SelectedNode;
                string filename = null;
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.FileName = node.Name;
                    if (sfd.ShowDialog() == DialogResult.OK)
                        filename = sfd.FileName;
                    else
                        return;
                }


                CatRunner runner = new CatRunner(node.Revision, new Uri(node.Url),
                    filename);

                e.GetService<IProgressRunner>().Run(
                                    "Retrieving file",
                                    runner.Work);
            }
        }

        #endregion
    }
}