using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Ids;
using SharpSvn;
using Ankh.Scc.UI;
using System.IO;

namespace Ankh.Commands
{
    [Command(AnkhCommand.ItemConflictEdit)]
    [Command(AnkhCommand.ItemConflictEditVisualStudio)]
    class ItemConflictEdit : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsConflicted)
                    return;
            }

            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            // TODO: Choose which conflict to edit if we have more than one!
            SvnItem conflict = null;

            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(false))
            {
                if (item.IsConflicted)
                {
                    conflict = item;
                    break;
                }
            }

            if (conflict == null)
                return;

            SvnInfoEventArgs conflictInfo = null;

            bool ok = false;
            ProgressRunnerResult r = e.GetService<IProgressRunner>().Run("Retrieving Conflict Information",
                delegate(object sender, ProgressWorkerArgs a)
                {
                    ok = a.Client.GetInfo(conflict.FullPath, out conflictInfo);
                });

            if (!ok || !r.Succeeded || conflictInfo == null)
                return;

            AnkhMergeArgs da = new AnkhMergeArgs();
            string dir = conflict.Directory;

            da.BaseFile = Path.Combine(dir, conflictInfo.ConflictOld);
            da.TheirsFile = Path.Combine(dir, conflictInfo.ConflictNew);

            if (!string.IsNullOrEmpty(conflictInfo.ConflictWork))
                da.MineFile = Path.Combine(dir, conflictInfo.ConflictWork);
            else
                da.MineFile = conflict.FullPath;

            da.MergedFile = conflict.FullPath;

            da.BaseTitle = "Base";
            da.TheirsTitle = "Theirs";
            da.MineTitle = "Mine";
            da.MergedTitle = conflict.Name;
            

            e.GetService<IAnkhDiffHandler>().RunMerge(da);
        }
    }
}
