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
            /*
#if DEBUG
            RecentChangesPage rcPage = e.Context.GetService<RecentChangesPage>();
            disable = rcPage == null || !rcPage.Visible;
#endif
            */
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
                using (ConfigureRecentChangesPageDialog dlg = new ConfigureRecentChangesPageDialog())
                {
                    int seconds = Math.Max(0, cfg.RecentChangesRefreshInterval);
                    dlg.RefreshInterval = seconds / 60;
                    if (dlg.ShowDialog(e.Context) == System.Windows.Forms.DialogResult.OK)
                    {
                        cfg.RecentChangesRefreshInterval = Math.Max(dlg.RefreshInterval * 60, 0);

                        configSvc.SaveConfig(cfg);
                        RecentChangesPage rcPage = e.GetService<RecentChangesPage>();
                        if (rcPage != null)
                            rcPage.RefreshIntervalConfigModified();
                    }
                }
            }
        }

        #endregion
    }
}
