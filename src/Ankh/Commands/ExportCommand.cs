// $Id: ExportCommand.cs 1410 2004-06-24 01:45:04Z Arild $
using System;
using Ankh.UI;
using System.Windows.Forms;
using Utils;
using SharpSvn;
using Ankh.Ids;
using System.Windows.Forms.Design;

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
            using (ExportDialog dlg = new ExportDialog(e.Context))
            {
                IUIService ui = e.GetService<IUIService>();

                DialogResult dr;

                if (ui != null)
                    dr = ui.ShowDialog(dlg);
                else
                    dr = dlg.ShowDialog();

                if (dr != DialogResult.OK)
                    return;

                e.GetService<IProgressRunner>().Run("Exporting",
                    delegate(object sender, ProgressWorkerArgs wa)
                    {
                        SvnExportArgs args = new SvnExportArgs();
                        args.Depth = dlg.NonRecursive ? SvnDepth.Infinity : SvnDepth.Empty;
                        args.Revision = dlg.Revision;

                        wa.Client.Export(new Uri(dlg.Source), dlg.LocalPath, args);
                    });
            }
        }
    }
}