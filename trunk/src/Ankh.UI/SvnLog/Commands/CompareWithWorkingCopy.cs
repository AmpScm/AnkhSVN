using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Ids;
using Ankh.Scc.UI;
using Ankh.Scc;
using SharpSvn;

namespace Ankh.UI.SvnLog.Commands
{
    [Command(AnkhCommand.LogCompareWithWorkingCopy, AlwaysAvailable = true)]
    public class CompareWithWorkingCopy : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
            ISvnLogItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());

            if (item != null)
            {
                ILogControl logWindow = e.Selection.ActiveDialogOrFrameControl as ILogControl;

                if (logWindow != null)
                {
                    SvnOrigin origin = EnumTools.GetSingle(logWindow.Origins);

                    if (origin != null)
                    {
                        SvnPathTarget pt = origin.Target as SvnPathTarget;

                        SvnItem svnItem = e.GetService<IFileStatusCache>()[pt.FullPath];

                        if (svnItem != null && !svnItem.IsDirectory)
                            return;
                    }
                }
            }

            e.Enabled = false;
        }
   
        public void OnExecute(CommandEventArgs e)
        {
            // All checked in OnUpdate            
            ILogControl logWindow = (ILogControl)e.Selection.ActiveDialogOrFrameControl;
            SvnOrigin origin = EnumTools.GetSingle(logWindow.Origins);
            ISvnLogItem item = EnumTools.GetSingle(e.Selection.GetSelection<ISvnLogItem>());

            IAnkhDiffHandler diff = e.GetService<IAnkhDiffHandler>();

            AnkhDiffArgs da = new AnkhDiffArgs();
            da.BaseFile = diff.GetTempFile(origin.Target, item.Revision, true); 
            da.MineFile = ((SvnPathTarget)origin.Target).FullPath;
            da.BaseTitle = string.Format("Base (r{0})", item.Revision);
            da.MineTitle = "Mine/Working";

            diff.RunDiff(da);
        }
    }
}
