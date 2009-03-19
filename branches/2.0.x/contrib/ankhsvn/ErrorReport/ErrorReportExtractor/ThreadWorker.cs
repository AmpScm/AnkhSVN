using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;

namespace ErrorReportExtractor
{

    public delegate void WorkerDelegate();

    public class ThreadWorker
    {
        public event WorkerDelegate Work;
        public event ThreadExceptionEventHandler Exception;
        public event EventHandler WorkFinished;

        public ThreadWorker( ISynchronizeInvoke synchronizer )
        {
            this.synchronizer = synchronizer;
        }

        public void Start()
        {
            this.Work.BeginInvoke( delegate( IAsyncResult res )
            {
                try
                {
                    this.Work.EndInvoke( res );
                    this.OnWorkFinished();
                }
                catch ( Exception ex )
                {
                    this.InvokeException( ex );
                }
            }, null );
        }

        private void OnWorkFinished()
        {
            this.Invoke( delegate
            {
                if ( this.WorkFinished != null )
                {
                    this.WorkFinished( this, EventArgs.Empty );
                }

            } );
        }

        private void InvokeException( Exception ex )
        {
            this.Invoke( delegate
            {
                if ( this.Exception != null )
                {
                    this.Exception( this, new ThreadExceptionEventArgs(ex) );
                }
            } );
        }

        private void Invoke( WorkerDelegate d )
        {
            this.synchronizer.Invoke( d, null );
        }
        


        private ISynchronizeInvoke synchronizer;
    }
}
