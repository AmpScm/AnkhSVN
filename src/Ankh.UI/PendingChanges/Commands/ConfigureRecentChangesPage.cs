using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Configuration;

namespace Ankh.UI.PendingChanges.Commands
{
    [Command(AnkhCommand.PendingChangesConfigureRecentChangesPage)]
    class ConfigureRecentChangesPage : ICommandHandler
    {
        #region ICommandHandler Members

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            bool disable = true;
#if DEBUG
            RecentChangesPage rcPage = e.Context.GetService<RecentChangesPage>();
            disable = rcPage == null || !rcPage.Visible;
#endif
            if (disable)
            {
                e.Enabled = false;
                return;
            }
        }

        public void OnExecute(CommandEventArgs e)
        {
            IAnkhConfigurationService configSvc = e.GetService<IAnkhConfigurationService>();
            if (configSvc != null)
            {
                AnkhConfig cfg = configSvc.Instance;
                bool save = false;
                using (ConfigureRecentChangesPageDialog dlg = new ConfigureRecentChangesPageDialog())
                {
                    double miliseconds = cfg.RecentChangesRefreshInterval;
                    dlg.RefreshInterval = (int) (miliseconds < 0 ? 0 : TimeSpan.FromMilliseconds(miliseconds).TotalMinutes);
                    if (dlg.ShowDialog(e.Context) == System.Windows.Forms.DialogResult.OK)
                    {
                        double mins = dlg.RefreshInterval;
                        cfg.RecentChangesRefreshInterval = mins < 0 ? 0 : TimeSpan.FromMinutes(mins).TotalMilliseconds;
                        save = true;
                    }
                }
                if (save)
                {
                    configSvc.SaveConfig(cfg);
                    RecentChangesPage rcPage = e.Context.GetService<RecentChangesPage>();
                    if (rcPage != null)
                    {
                        rcPage.ResetRefreshSchedule();
                    }
                }
            }
        }

        #endregion
    }
}
