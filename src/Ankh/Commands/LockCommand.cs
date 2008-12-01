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
    [Command(AnkhCommand.Lock)]
    [Command(AnkhCommand.LockMustLock)]
    class LockCommand : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            bool mustOnly = (e.Command == AnkhCommand.LockMustLock);
            foreach (SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                if (item.IsFile && item.IsVersioned && !item.IsLocked)
                {
                    if(!mustOnly || item.ReadOnlyMustLock)
                        return;
                }
            }
            e.Enabled = false;
        }

        public override void OnExecute(CommandEventArgs e)
        {
            IEnumerable<SvnItem> items = e.Argument as IEnumerable<SvnItem>;

            PathSelectorInfo psi = new PathSelectorInfo("Select Files to Lock", items != null ? items : e.Selection.GetSelectedSvnItems(true));
            psi.VisibleFilter += delegate(SvnItem item)
            {
                return item.IsFile && item.IsVersioned && !item.IsLocked;
            };

            psi.CheckedFilter += delegate(SvnItem item)
            {
                return item.IsFile && item.IsVersioned && !item.IsLocked;
            };

            PathSelectorResult psr;
            bool stealLocks = false;
            string comment = "";

            if (e.PromptUser || !(CommandBase.Shift || e.DontPrompt))
            {
                IUIService uiService = e.GetService<IUIService>();
                using (LockDialog dlg = new LockDialog(psi))
                {
                    bool succeeded = (dlg.ShowDialog(e.Context)== DialogResult.OK);
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

            try
            {
                e.GetService<IProgressRunner>().RunModal("Locking",
                    delegate(object sender, ProgressWorkerArgs ee)
                    {
                        SvnLockArgs la = new SvnLockArgs();
                        la.StealLock = stealLocks;
                        la.Comment = comment;
                        ee.Client.Lock(files, la);
                    });
            }
            finally
            {
                // TODO: this can be removed when switching to Subversion 1.6
                e.GetService<IFileStatusMonitor>().ScheduleSvnStatus(files);
            }

        } // OnExecute        
    }
}