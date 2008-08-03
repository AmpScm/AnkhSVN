using System;
using System.Collections.Generic;
using System.Text;
using Meebey.SmartIrc4net;
using System.Runtime.Remoting;

namespace AnkhBot
{
    class Program
    {
        static void Main( string[] args )
        {
            RemotingConfiguration.Configure( AppDomain.CurrentDomain.SetupInformation.ConfigurationFile );

            AnkhBot bot = new AnkhBot();
            bot.Nick = args[0];
            bot.Network = args[1];
            bot.Connect();
            bot.JoinChannel( args[2] );
            bot.Listen();
        }
    }
}
