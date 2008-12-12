using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Ankh.Ids;
using Ankh.WorkingCopyExplorer;
using Ankh.Scc;
using System.Windows.Forms.Design;
using System.IO;
using SharpSvn;

namespace Ankh.Commands.RepositoryExplorer
{
    /// <summary>
    /// Command to save currnet file to disk from Repository Explorer.
    /// </summary>
    [Command(AnkhCommand.SaveToFile)]
    class SaveToFileCommand : ViewRepositoryFileCommand
    {
        public override void OnExecute(CommandEventArgs e)
        {
            ISvnRepositoryItem ri = null;

            foreach (ISvnRepositoryItem i in e.Selection.GetSelection<ISvnRepositoryItem>())
            {
                ri = i;
                break;
            }
            if (ri == null)
                return;

            IUIService ui = e.GetService<IUIService>();

            string toFile;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "All Files (*.*)|*";
                sfd.FileName = ri.Name;               

                if (sfd.ShowDialog(ui.GetDialogOwnerWindow()) != DialogResult.OK)
                    return;

                toFile = sfd.FileName;
            }

            SaveFile(e, ri, toFile);
        }
    }
}
