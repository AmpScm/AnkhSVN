using System;
using System.Collections.Generic;
using System.Text;
using Ankh.VS;
using Ankh.UI;
using Ankh.Scc.UI;
using System.Windows.Forms;

namespace Ankh.Commands
{
    [Command(Ankh.Ids.AnkhCommand.SolutionApplyPatch)]
    [Command(Ankh.Ids.AnkhCommand.PendingChangesApplyPatch, HideWhenDisabled=false)]
    public class ApplyPatch : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();

            if (ss != null && !string.IsNullOrEmpty(ss.ProjectRoot) && ss.ProjectRootSvnItem.IsVersioned)
            {
                IAnkhConfigurationService cs = e.GetService<IAnkhConfigurationService>();

                if (!string.IsNullOrEmpty(cs.Instance.PatchExePath))
                    return;
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();
            IAnkhConfigurationService cs = e.GetService<IAnkhConfigurationService>();
            IAnkhDiffHandler diff = e.GetService<IAnkhDiffHandler>();

            AnkhPatchArgs args = new AnkhPatchArgs();
            args.ApplyTo = ss.ProjectRoot;

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Patch files( *.patch)|*.patch|Diff files (*.diff)|*.diff|" +
                    "Text files (*.txt)|*.txt|All files (*.*)|*";

                if (ofd.ShowDialog(e.Context.DialogOwner) != DialogResult.OK)
                    return;

                args.PatchFile = ofd.FileName;
            }

            diff.RunPatch(args);
        }
    }
}
