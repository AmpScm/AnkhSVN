// $Id: ExportCommand.cs 1410 2004-06-24 01:45:04Z Arild $
using System;
using Ankh.UI;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Ids;
using System.Windows.Forms.Design;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to export a Subversion repository or local folder.
    /// </summary>
    [Command(AnkhCommand.Export,HideWhenDisabled=false)]
    class ExportCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool foundOne = false;
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if(foundOne || !item.IsVersioned)
                {
                    e.Enabled = false;
                    break;
                }
                foundOne = true;
            }

            if(!foundOne)
                e.Enabled = false;
        }
        public override void OnExecute(CommandEventArgs e)
        {
            using (ExportDialog dlg = new ExportDialog(e.Context))
            {
                IUIService ui = e.GetService<IUIService>();

                foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
                {
                    dlg.OriginUri = item.Status.Uri;
                    dlg.OriginPath = item.FullPath;
                }

                DialogResult dr;

                if (ui != null)
                    dr = ui.ShowDialog(dlg);
                else
                    dr = dlg.ShowDialog();

                if (dr != DialogResult.OK)
                    return;

                e.GetService<IProgressRunner>().RunModal("Exporting",
                    delegate(object sender, ProgressWorkerArgs wa)
                    {
                        SvnExportArgs args = new SvnExportArgs();
                        args.Depth = dlg.NonRecursive ? SvnDepth.Infinity : SvnDepth.Empty;
                        args.Revision = dlg.Revision;

                        wa.Client.Export(dlg.ExportSource, dlg.LocalPath, args);
                    });
            }
        }
    }
}