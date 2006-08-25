using System;
using System.Collections.Generic;
using System.Text;

namespace AnkhBot
{
    public class CommandArgs
    {
        public CommandArgs( AnkhBot bot, string[] args, string user, string channel, bool isPM )
        {
            this.bot = bot;
            this.args = args;
            this.user = user;
            this.channel = channel;
            this.isPM = isPM;
        }


        public bool IsPM
        {
            get { return isPM; }
        }

        public string Channel
        {
            get { return channel; }
        }

        public string User
        {
            get { return User; }
        }

        public string[] Args
        {
            get { return args; }
        }

        public AnkhBot Bot
        {
            get { return bot; }
        }

        public void SendMessage( string message )
        {
            if (isPM)
                this.Bot.SendPM( this.User, message );
            else
                this.bot.SendMessage( this.Channel, message );
        }


        private AnkhBot bot;
        private string[] args;

        private string user;
        private string channel;
        private bool isPM;
    }
}
