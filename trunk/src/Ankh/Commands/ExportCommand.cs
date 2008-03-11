// $Id: ExportCommand.cs 1410 2004-06-24 01:45:04Z Arild $
using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;
using SharpSvn;
using AnkhSvn.Ids;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to export a Subversion repository or local folder.
    /// </summary>
    [Command(AnkhCommand.Export)]
    public class ExportCommand : CommandBase
    {
        public override void OnExecute(CommandEventArgs e)
        {
            IContext context = e.Context.GetService<IContext>();

            using (ExportDialog dlg = new ExportDialog())
            {
                if (dlg.ShowDialog(context.HostWindow) != DialogResult.OK)
                    return;

                using (context.StartOperation("Exporting"))
                {
                    ExportRunner runner = new ExportRunner(
                        dlg.LocalPath, dlg.Revision, dlg.Source, dlg.NonRecursive ? SvnDepth.Infinity : SvnDepth.Empty);
                    context.UIShell.RunWithProgressDialog(runner, "Exporting");

                    // make sure it's remembered
                    RegistryUtils.CreateNewTypedUrl(dlg.Source);
                }
            }
        }
    }
}