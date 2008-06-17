using System;
using System.Collections.Generic;
using System.Text;

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

            // TODO: Handle migration here (And don't throw an exception ;)
            throw new NotImplementedException("Migration not implemented yet");
        }
    }
}
