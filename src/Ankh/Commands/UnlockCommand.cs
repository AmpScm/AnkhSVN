using System;
using System.Collections;
using SharpSvn;
using Ankh.Ids;
using Ankh.VS;
using System.Collections.Generic;
using Ankh.UI;

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
                if (item.IsVersioned && item.IsLocked)
                    return;

            }
            e.Enabled = false;
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

            e.GetService<IProgressRunner>().Run("Unlocking",
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    SvnUnlockArgs ua = new SvnUnlockArgs();
                    ee.Client.Unlock(files, ua);
                });
        }
	}
}