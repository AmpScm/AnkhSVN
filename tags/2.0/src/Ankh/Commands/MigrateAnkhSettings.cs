using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;

namespace Ankh.Commands
{
    [Command(Ankh.Ids.AnkhCommand.MigrateSettings, AlwaysAvailable=true)]
    class MigrateAnkhSettings : CommandBase
    {
        public override void OnUpdate(CommandUpdateEventArgs e)
        {
            // Always available
        }

        public override void OnExecute(CommandEventArgs e)
        {
            int upgradeFrom = 0;
            bool incremental = false;
            if (e.Argument is int)
            {
                incremental = true;
                upgradeFrom = (int)e.Argument;
            }

			DTE dte = e.GetService<DTE>();
			Ankh.Migrate.Cleanup.RemoveOldUI(dte, !incremental);
        }
    }
}
