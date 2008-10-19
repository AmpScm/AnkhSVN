using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Scc.UI;
using Ankh.Scc;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogCompareWithWorkingCopy, AlwaysAvailable = true)]
    public class CompareWithWorkingCopy : ICommandHandler
    {
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

            bool one = false;
            foreach (ISvnLogItem li in e.Selection.GetSelection<ISvnLogItem>())
            {
                if (one)
                {
                    e.Enabled = false;
                    return;
                }
                one = true;
            }

            if (!one)
                e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            ILogControl logWindow = (ILogControl)e.Selection.ActiveDialogOrFrameControl;
            ISvnLogItem logItem = null;
            foreach (ISvnLogItem li in e.Selection.GetSelection<ISvnLogItem>())
            {
                logItem = li;
                break;
            }

            if(logItem == null || logWindow == null || !logWindow.HasWorkingCopyItems)
                return;

            SvnItem[] files = logWindow.WorkingCopyItems;

            if(files == null || files.Length == 0)
                return;

            SvnItem firstFile = files[0];

            IAnkhDiffHandler diff = e.GetService<IAnkhDiffHandler>();

            AnkhDiffArgs da = new AnkhDiffArgs();
            da.BaseFile = diff.GetTempFile(firstFile, logItem.Revision, true); 
            da.MineFile = firstFile.FullPath;
            da.BaseTitle = "Base";
            da.MineTitle = "Mine";

            diff.RunDiff(da);
        }
    }
}
