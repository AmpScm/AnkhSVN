using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Messaging;
using PostCommit.Remoting;
using System.Configuration;
using System.Runtime.Remoting;

namespace PostCommit.Service
{
    partial class PostCommitService : ServiceBase
    {
        public PostCommitService()
        {
            InitializeComponent();
        }

        protected override void OnStart( string[] a )
        {
            RemotingConfiguration.Configure( AppDomain.CurrentDomain.SetupInformation.ConfigurationFile );

            this.runtime = PostCommitRuntime.Instance;
            this.runtime.Error += delegate( object o, ErrorEventArgs args )
            {
                this.EventLog.WriteEntry( args.Exception.Message, EventLogEntryType.Error );
            };

            this.runtime.Start();
        }

        protected override void OnStop()
        {
            this.runtime.Stop();
        }

        public static void Main()
        {
            PostCommitService service = new PostCommitService();

            if ( Debugger.IsAttached )
            {
                DebugRun( service );
            }
            else
            {
                ServiceBase.Run( new ServiceBase[] { service } );
            }
        }

        private static void DebugRun( PostCommitService service )
        {
            service.OnStart( new string[] { } );
            while ( true )
            {
                Thread.Sleep( 1000 );
            }

            service.OnStop();
        }

        private PostCommitRuntime runtime;
    }
}
