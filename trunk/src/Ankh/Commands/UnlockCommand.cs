using System;
using System.Collections;
using SharpSvn;
using Ankh.Ids;
using Ankh.VS;
using System.Collections.Generic;
using Ankh.UI;
using Ankh.Scc;
using SharpSvn.Implementation;

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
                //if (item.IsVersioned && item.IsLocked)
                    return;

            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            PathSelectorInfo psi = new PathSelectorInfo("Select Files to Unlock", e.Selection.GetSelectedSvnItems(true));

            psi.VisibleFilter += delegate(SvnItem item)
            {
				return true;// item.IsLocked;
            };

            psi.CheckedFilter += delegate(SvnItem item)
            {
				return true;// item.IsLocked;
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
					ua.SvnError += delegate(object errorSender, SvnErrorEventArgs error)
					{
						if (error.Exception.SvnErrorCode == SvnErrorCode.SVN_ERR_CLIENT_MISSING_LOCK_TOKEN)
						{
							error.Cancel = true; // File is already unlocked, probably by another client, cancel this exception.

							// This schedule should not be removed, the error indicates our working copy state was wrong
							e.GetService<IFileStatusMonitor>().ScheduleSvnStatus(files);
						}
					};
                    ee.Client.Unlock(files, ua);
                });

			// TODO: this can be removed when switching to Subversion 1.6
			e.GetService<IFileStatusMonitor>().ScheduleSvnStatus(files);
        }
	}
}