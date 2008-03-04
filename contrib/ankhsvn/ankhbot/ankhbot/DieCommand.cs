using System;
using System.Collections.Generic;
using System.Text;

namespace AnkhBot
{
    [Command( "die" )]
    class DieCommand : ICommand
    {
        public void Execute( CommandArgs args )
        {
            args.Bot.Quit();
        }
    }
}
