using System;
using System.Collections;
using System.Collections.Generic;
using SharpSvn;

using Ankh.Ids;
using Ankh.Scc;
using Ankh.UI;
using Ankh.VS;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to unlock the selected items.
    /// </summary>
    [Command(AnkhCommand.Unlock, HideWhenDisabled=true)] 
	public class UnlockCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsLocked)
                    return;

            }
            e.Enabled = false; // No need to unlock anything if we are not versioned or not locked
        }

        public override void OnExecute(CommandEventArgs e)
        {
            PathSelectorInfo psi = new PathSelectorInfo("Select Files to Unlock", e.Selection.GetSelectedSvnItems(true));

            psi.VisibleFilter += delegate(SvnItem item)
            {
				return item.IsLocked;
            };

            psi.CheckedFilter += delegate(SvnItem item)
            {
				return item.IsLocked;
            };
            
            PathSelectorResult psr;
            if (!CommandBase.Shift)
            {
                IUIShell uiShell = e.GetService<IUIShell>();

                psr = uiShell.ShowPathSelector(psi);
            }
            else
                psr = psi.DefaultResult;

            if (!psr.Succeeded)
                return;

            List<string> files = new List<string>();

            foreach (SvnItem item in psr.Selection)
            {
                files.Add(item.FullPath);
            }

            if(files.Count == 0)
                return;

            try
            {
                e.GetService<IProgressRunner>().Run("Unlocking",
                    delegate(object sender, ProgressWorkerArgs ee)
                    {
                        SvnUnlockArgs ua = new SvnUnlockArgs();

                        ee.Client.Unlock(files, ua);
                    });
            }
            finally
            {
                // Subversion 1.5.0 bug. Invalid path passed to notify handler :(
                e.GetService<IFileStatusMonitor>().ScheduleSvnStatus(files);
            }
        }
	}
}