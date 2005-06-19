using System;
using System.Collections.Generic;
using System.Text;
using Meebey.SmartIrc4net;

namespace AnkhBot
{
    class Program
    {
        static void Main( string[] args )
        {
            AnkhBot bot = new AnkhBot();
            bot.Nick = args[0];
            bot.Network = args[1];
            bot.Connect();
            bot.JoinChannel( args[2] );
            bot.Listen();
        }
    }
}
