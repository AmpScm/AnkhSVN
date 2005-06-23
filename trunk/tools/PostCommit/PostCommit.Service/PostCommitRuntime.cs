using System;
using System.Collections.Generic;
using System.Text;
using PostCommit.Remoting;
using System.Messaging;
using System.Threading;
using System.Configuration;

namespace PostCommit.Service
{
    
    public class PostCommitRuntime 
    {
        public event CommittedEventHandler Committed;
        public event ErrorEventHandler Error;

        private PostCommitRuntime()
        {
            this.queueName = ConfigurationSettings.AppSettings["messagequeue"];
        }

        public static PostCommitRuntime Instance
        {
            get
            {
                if ( instance == null )
                    instance = new PostCommitRuntime();
                return instance;
            }
        }


        public void Start()
        {
            this.running = true;
            this.workerThread = new Thread( WorkerMethod );
            this.workerThread.Start();
        }

        public void Stop()
        {
            this.running = false;
        }

        public void Ping()
        {
            // empty
        }

        private void WorkerMethod()
        {
            // create the message queue if we have to
            MessageQueue queue;
            if ( !MessageQueue.Exists( queueName ) )
                queue = MessageQueue.Create( queueName );
            else
                queue = new MessageQueue( queueName );

            ( (XmlMessageFormatter)queue.Formatter ).TargetTypes = new Type[] { typeof( Commit ) };

            // run along
            while ( running )
            {
                try
                {
                    Message msg = queue.Receive( new TimeSpan( 0, 0, 10 ) );

                    Commit commit = (Commit)msg.Body;
                    this.RaiseCommitted( new CommitEventArgs( commit ) );
                    
                }
                catch ( MessageQueueException ex )
                {
                    if ( ex.MessageQueueErrorCode == MessageQueueErrorCode.IOTimeout )
                        continue;

                    if ( this.Error != null )
                        this.Error( this, new ErrorEventArgs( ex ) );
                }
            }
        }

        private void RaiseCommitted( CommitEventArgs args )
        {
            foreach ( CommittedEventHandler handler in this.Committed.GetInvocationList() )
            {
                try
                {
                    handler.Invoke( null, args );
                }
                catch ( Exception /* ex */ )
                {
                    //this.Error( this, new ErrorEventArgs( ex ) );
                    Delegate.Remove( this.Committed, handler );
                }
            }
        }

        

        private string queueName;
        private Thread workerThread;
        private bool running = false;

        private static PostCommitRuntime instance;

    }

    public delegate void ErrorEventHandler( object sender, ErrorEventArgs args );
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs( Exception ex )
        {
            this.exception = ex;
        }

        public Exception Exception
        {
            get { return exception; }
        }

        private Exception exception;
    }
}
