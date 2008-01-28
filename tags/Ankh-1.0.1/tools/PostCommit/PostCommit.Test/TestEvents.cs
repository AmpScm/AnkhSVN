using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using PostCommit.Remoting;
using System.Messaging;
using System.Configuration;
using System.Threading;

namespace PostCommit.Tests
{
    [TestFixture]
    class TestEvents : RemotelyDelegatableObject
    {
        [SetUp]
        public void SetUp()
        {
            //BinaryServerFormatterSinkProvider provider = new BinaryServerFormatterSinkProvider();
            //provider.TypeFilterLevel = System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;
            //Dictionary<string, int> dict = new Dictionary<string, int>();

            //TcpChannel channel = new TcpChannel(dict, null, provider);
            //ChannelServices.RegisterChannel( channel );
            RemotingConfiguration.Configure( AppDomain.CurrentDomain.SetupInformation.ConfigurationFile );


            this.repos = (IRepository)Activator.GetObject(
                typeof( IRepository ), Url );

            if ( !MessageQueue.Exists( QueueName ) )
                MessageQueue.Create( QueueName );

            this.queue = new MessageQueue( QueueName );
        }

        [Test]
        public void TestBasic()
        {
            this.repos.Ping();
            this.repos.Committed += this.CommittedCallback;

            Commit commit = null;

            this.committed = delegate( object o, CommitEventArgs args )
            {
                commit = args.Commit;
            };

            this.queue.Send( new Commit( "Arild", 42, "Log message", new string[]{ "C:\\foo\\bar.cs" },
                new string[]{ "C:\\foo\\" } ) );
            Thread.Sleep( 10000 );

            Assert.IsNotNull( commit );
            Assert.AreEqual( "Arild", commit.Author );
            Assert.AreEqual( 42, commit.Revision );
            Assert.AreEqual( "Log message", commit.LogMessage );
            Assert.AreEqual( "C:\\foo\\bar.cs", commit.ChangedPaths[0] );
            Assert.AreEqual( "C:\\foo\\", commit.ChangedDirs[0] );
        }

        protected override void InternalCommittedCallback( object sender, CommitEventArgs args )
        {
            if ( this.committed != null )
                this.committed( sender, args );
        }

        public static int Main()
        {
            try
            {
                TestEvents events = new TestEvents();
                events.SetUp();
                events.TestBasic();

                return 0;
            }
            catch ( Exception ex )
            {
                Console.Error.WriteLine( ex );
                return 1;
            }
        }


        private CommittedEventHandler committed;
        private MessageQueue queue;
        private IRepository repos;
        private const string Url = "tcp://localhost:9999/Repository";
        private const string QueueName = @".\Private$\test";
    }
}
