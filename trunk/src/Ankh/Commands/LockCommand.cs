using System;
using System.Collections;

using System.Text;
using SharpSvn;
using Ankh.Ids;
using System.Collections.Generic;
using Ankh.UI;
using System.Windows.Forms;
using Ankh.Scc;
using System.Windows.Forms.Design;

namespace Ankh.Commands
{
    /// <summary>
    /// Command to lock the selected item.
    /// </summary>
    [Command(AnkhCommand.Lock, HideWhenDisabled=true)]
    public class LockCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsFile && item.IsVersioned && !item.IsLocked)
                {
                    return;
                }

            }
            e.Enabled = false;
        }

        #region Implementation of ICommand


        public override void OnExecute(CommandEventArgs e)
        {
            IEnumerable<SvnItem> items = e.Argument as IEnumerable<SvnItem>;

            PathSelectorInfo psi = new PathSelectorInfo("Select Files to Lock", items != null ? items : e.Selection.GetSelectedSvnItems(true));
            psi.VisibleFilter += delegate(SvnItem item)
            {
                return item.IsFile && !item.IsLocked;
            };

            psi.CheckedFilter += delegate(SvnItem item)
            {
                return item.IsFile && !item.IsLocked;
            };

            PathSelectorResult psr;
            bool stealLocks = false;
            string comment = "";

            if (!CommandBase.Shift)
            {
                IUIService uiService = e.GetService<IUIService>();
                using (LockDialog dlg = new LockDialog(psi))
                {
                    dlg.Context = e.Context;
                    bool succeeded = uiService.ShowDialog(dlg) == DialogResult.OK;
                    psr = new PathSelectorResult(succeeded, dlg.CheckedItems);
                    stealLocks = dlg.StealLocks;
                    comment = dlg.Message;
                }

            }
            else
                psr = psi.DefaultResult;

            if (!psr.Succeeded)
                return;

            List<string> files = new List<string>();

            foreach (SvnItem item in psr.Selection)
            {
                if (item.IsFile) // svn lock is only for files
                {
                    files.Add(item.FullPath);
                }
            }

            if (files.Count == 0)
                return;

            e.GetService<IProgressRunner>().Run("Locking",
                delegate(object sender, ProgressWorkerArgs ee)
                {
                    SvnLockArgs la = new SvnLockArgs();
                    la.StealLock = stealLocks;
                    la.Comment = comment;
                    ee.Client.Lock(files, la);
                });
            // TODO: this can be removed when switching to Subversion 1.6
            e.GetService<IFileStatusMonitor>().ScheduleSvnStatus(files);

        } // OnExecute

        #endregion

        
    }
}