using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using SharpSvn;
using Ankh.VS;
using System.IO;
using System.CodeDom.Compiler;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Ids;
using Ankh.Scc.UI;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogShowChanges)]
    class ShowChanges : ICommandHandler
    {
        TempFileCollection _collection = new TempFileCollection();
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            ILogControl logWindow = e.Selection.ActiveDialogOrFrameControl as ILogControl;

            if ((logWindow == null) || !logWindow.HasWorkingCopyItems)
            {
                e.Enabled = false;
                return;
            }

            // TODO: Remove this code when we're able to handle directories
            SvnItem first = null;
            foreach (SvnItem i in logWindow.WorkingCopyItems)
            {
                first = i;
            }
            if (first == null || first.IsDirectory)
            {
                e.Enabled = false;
                return;
            }

            foreach (Ankh.Scc.ISvnLogItem item in e.Selection.GetSelection<Ankh.Scc.ISvnLogItem>())
            {
                return;
            }

            e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            long min = long.MaxValue;
            long max = long.MinValue;

            bool touched = false;
            foreach (Ankh.Scc.ISvnLogItem item in e.Selection.GetSelection<Ankh.Scc.ISvnLogItem>())
            {
                min = Math.Min(min, item.Revision);
                max = Math.Max(max, item.Revision);
                touched = true;
            }
            if (!touched)
                return;
            if (min == max)
                min--;

            ILogControl logWindow = e.Selection.ActiveDialogOrFrameControl as ILogControl;

            SvnItem[] files = logWindow.WorkingCopyItems;
            if (files == null || files.Length == 0)
                return;

            SvnItem workingCopyItem = files[0];

            SvnRevisionRange range = new SvnRevisionRange(min, max);

            SvnTarget diffTarget = new SvnPathTarget(workingCopyItem.FullPath);
            IAnkhDiffHandler diff = e.GetService<IAnkhDiffHandler>();
            AnkhDiffArgs da = new AnkhDiffArgs();
            da.BaseFile = diff.GetTempFile(diffTarget, range.StartRevision, false);
            da.BaseTitle = diff.GetTitle(diffTarget, range.StartRevision);
            da.MineFile = diff.GetTempFile(diffTarget, range.EndRevision, false);
            da.MineTitle = diff.GetTitle(diffTarget, range.EndRevision);
            diff.RunDiff(da);
        }
    }
}
