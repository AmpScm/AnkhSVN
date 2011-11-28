using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc;
using System.ComponentModel;
using SharpSvn;
using System.IO;

namespace Ankh.Commands
{
    [Command(AnkhCommand.UpgradeWorkingCopy)]
    sealed class UpgradeWorkingCopy : CommandBase, IComponent
    {
        ISite _site;

        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            if (StatusCache == null || !StatusCache.EnableUpgradeCommand)
            {
                e.Enabled = false;
                return;
            }
            
            foreach(SvnItem item in e.Selection.GetSelectedSvnItems(true))
            {
                SvnDirectory dir = item.ParentDirectory;
                if (dir != null && dir.NeedsWorkingCopyUpgrade)
                    return;
            }
            e.Enabled = false;
        }
        
        public override void OnExecute(CommandEventArgs e)
        {
            HybridCollection<string> dirs = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

            foreach(SvnItem i in e.Selection.GetSelectedSvnItems(true))
            {
                SvnDirectory dir = i.ParentDirectory;
                if (dir != null && dir.NeedsWorkingCopyUpgrade && !dirs.Contains(dir.FullPath))
                    dirs.Add(dir.FullPath);
            }

            e.GetService<IProgressRunner>().RunModal(CommandStrings.UpgradingWorkingCopy,
                delegate(object sender, ProgressWorkerArgs a)
                {
                    HybridCollection<string> done = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                    foreach(string dir in dirs)
                    {
                        SvnInfoArgs ia = new SvnInfoArgs();
                        ia.ThrowOnError = false;

                        if (done.Contains(dir))
                            continue;
                        
                        if (a.Client.Info(dir, ia, null) || ia.LastException.SvnErrorCode != SvnErrorCode.SVN_ERR_WC_UPGRADE_REQUIRED)
                            continue;

                        SvnUpgradeArgs ua = new SvnUpgradeArgs();
                        ua.ThrowOnError = false;

                        /* Capture the already upgraded directories to avoid a lot of work */
                        ua.Notify += delegate(object sender2, SvnNotifyEventArgs n)
                            {
                                if (n.Action == SvnNotifyAction.UpgradedDirectory)
                                {
                                    if (!done.Contains(n.FullPath))
                                        done.Add(n.FullPath);
                                }
                            };

                        string tryDir = dir;
                        while(true)
                        {
                            if (a.Client.Upgrade(tryDir, ua)
                                || ua.LastException.SvnErrorCode != SvnErrorCode.SVN_ERR_WC_INVALID_OP_ON_CWD)
                                break;

                            string pd = Path.GetDirectoryName(tryDir);

                            if (pd == tryDir || string.IsNullOrEmpty(pd))
                                break;

                            tryDir = pd;
                        }
                    }
                });
        }

        event EventHandler IComponent.Disposed
        {
            add { throw new NotImplementedException(); }
            remove { throw new NotImplementedException(); }
        }

        public ISite Site
        {
            get { return _site; }
            set { _site = value; }
        }

        IFileStatusCache _cache;

        IFileStatusCache StatusCache
        {
            get
            {
                if (_cache == null && _site != null)
                    _cache = (IFileStatusCache)Site.GetService(typeof(IFileStatusCache));
                
                return _cache;
            }
        }

        void IDisposable.Dispose()
        {
        }
    }
}
