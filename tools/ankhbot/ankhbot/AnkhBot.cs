using System;
using System.Collections.Generic;
using System.Text;
using Meebey.SmartIrc4net;
using System.IO;
using System.Threading;

namespace AnkhBot
{
    public class AnkhBot
    {
        public AnkhBot( TextWriter output, TextWriter error )
        {
            this.output = output;
            this.error = error;
            this.dispatcher = new CommandDispatcher( this );
            this.serviceProvider = new ServiceProvider( this );

            this.timer = new Timer( delegate
            {
                this.client.RfcPing( this.Network );
            } );
            this.timer.Change( 5000, 5000 );
        }

        public AnkhBot()
            : this( Console.Out, Console.Error )
        {
        }

        public string Nick
        {
            get { return nick; }
            set { nick = value; }
        }

        public string Network
        {
            get { return this.network; }
            set { this.network = value; }
        }

        public ServiceProvider ServiceProvider
        {
            get { return this.serviceProvider; }
        }

        public void Connect()
        {
            this.client = new IrcClient();
            this.client.ActiveChannelSyncing = true;
            this.client.OnConnecting += delegate
            {
                this.output.WriteLine( "Connecting to {0} as {1}", this.network, this.nick );
            };

            this.client.OnConnected += delegate
            {
                this.output.WriteLine( "Connected to {0} as {1}", this.network, this.nick );
            };

            this.client.OnConnectionError += delegate
            {
                this.error.WriteLine( "Error connecting to {0}", this.network );
            };

            this.client.OnJoin += delegate( object sender, JoinEventArgs args )
            {
                this.output.WriteLine( "{0} joined {1}", args.Who, args.Channel );
            };


            this.client.OnChannelMessage += OnChannelMessage;
            this.client.OnQueryMessage += OnQueryMessage;

            try
            {
                this.client.Connect( this.network, Port );
            }
            catch (ConnectionException ex)
            {
                this.error.WriteLine( "Could not connect: " + ex.Message );
                throw;
            }

            try
            {
                this.client.Login( this.Nick, this.Nick );
            }
            catch (ConnectionException ex)
            {
                this.error.WriteLine( "Could not log in: " + ex.Message );
                throw;
            }
        }

        public void JoinChannel( string channel )
        {
            this.client.RfcJoin( channel );
        }

        public void Listen()
        {
            this.client.Listen();
            if (this.client.IsConnected)
                this.client.Disconnect();
        }

        public void SendPM( string user, string message )
        {
            string[] lines = message.Trim().Split( '\n' );
            foreach ( string line in lines )
            {
                this.client.RfcPrivmsg( user, line );
                Thread.Sleep( Sleep );
            }
        }

        public void SendMessage( string channel, string message )
        {
            string[] lines = message.Trim().Split( '\n' );
            foreach ( string line in lines )
            {
                this.client.SendMessage( SendType.Message, channel, line );
                Thread.Sleep( Sleep );
            }
        }

        public void Broadcast( string msg )
        {
            foreach (string channel in this.client.GetChannels())
                this.SendMessage( channel, msg );
        }

        public void Quit()
        {
            this.client.RfcQuit();
        }


        private void OnChannelMessage( object sender, IrcEventArgs args )
        {
            this.output.WriteLine( "{0} - {1}: {2}", args.Data.Channel,
                args.Data.Nick, args.Data.Message );
            this.dispatcher.Dispatch( args.Data.Message, args.Data.Nick, args.Data.Channel, false );
        }

        private void OnQueryMessage( object sender, IrcEventArgs args )
        {
            this.dispatcher.Dispatch( args.Data.Message, args.Data.Nick, null, false );
        }

        private ServiceProvider serviceProvider;
        private CommandDispatcher dispatcher;
        private TextWriter output;
        private TextWriter error;
        private IrcClient client;
        private Timer timer;
        private string nick = "";
        private string network = "";
        private const int Port = 6667;

        private const int Sleep = 500;






    }
}
