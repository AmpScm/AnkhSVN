using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using Ankh.VS;
using Ankh.Scc;
using SharpSvn;
using Ankh.UI.SccManagement;
using System.Windows.Forms;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ProjectBranch)]
    [Command(AnkhCommand.SolutionBranch)]
    class BranchSolutionCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
#if DEBUG // Remove debug check if somewhat functional
            switch (e.Command)
            {
                case AnkhCommand.SolutionBranch:
                    IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();

                    string root = ss.ProjectRoot;

                    if (string.IsNullOrEmpty(root))
                    {
                        e.Enabled = false;
                        return;
                    }

                    SvnItem item = e.GetService<IFileStatusCache>()[root];

                    if (item == null || !item.IsVersioned || item.IsDeleteScheduled || item.Status.LocalContentStatus == SvnStatus.Added || item.Status.Uri == null)
                    {
                        e.Enabled = false;
                        return;
                    }
                    return;
            }
#endif
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            string path;

            switch (e.Command)
            {
                case AnkhCommand.SolutionBranch:
                    IAnkhSolutionSettings ss = e.GetService<IAnkhSolutionSettings>();

                    path = ss.ProjectRoot;
                    break;
                default:
                    return;
            }

            if (string.IsNullOrEmpty(path))
                return;

            IFileStatusCache cache = e.GetService<IFileStatusCache>();
            SvnItem root;

            if (cache == null || null == (root = cache[path]) || root.Status.Uri == null)
                return;

            using (CreateBranch dlg = new CreateBranch())
            {
                dlg.SrcFolder = root.FullPath;
                dlg.SrcUri = root.Status.Uri;
                dlg.EditSource = false;

                dlg.Revision = root.Status.Revision;

                if (DialogResult.OK != dlg.ShowDialog(e.Context))
                    return;
            }
        }
    }
}
