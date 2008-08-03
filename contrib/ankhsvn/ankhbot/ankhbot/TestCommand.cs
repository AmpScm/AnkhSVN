using System;
using System.Collections.Generic;
using System.Text;

namespace AnkhBot
{
    [Command( "test" )]
    class TestCommand : ICommand
    {
        public void Execute( CommandArgs args )
        {
            args.SendMessage( "Ack" );
        }

    }
}
