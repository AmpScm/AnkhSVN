using System;
using System.Threading;
using Ankh.UI;
using SharpSvn;

namespace Ankh
{
    public interface IProgressWorker
    {        
        void Work( IContext context );
    }

    public delegate void SimpleProgressWorkerCallback( IContext context );


    public class SimpleProgressWorker : IProgressWorker
    {
        public SimpleProgressWorker( SimpleProgressWorkerCallback cb )
        {
            this.callback = cb;
        }
        public void Work(IContext context)
        {
            this.callback( context );
        }

        private SimpleProgressWorkerCallback callback;
    }

    /// <summary>
    /// Used to run lengthy operations in a separate thread while 
    /// displaying a modal progress dialog in the main thread.
    /// </summary>
    public class ProgressRunner
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="callback">The callback which performs 
        /// the actual operation.</param>
        public ProgressRunner( IContext context, IProgressWorker worker )
        {
            this.context = context;
            this.worker = worker;
        }

        public ProgressRunner( IContext context ) : this( context, null )
        {}

        /// <summary>
        /// Whether the operation was cancelled.
        /// </summary>
        public bool Cancelled
        {
            get{ return this.cancelled; }
        }
            

        /// <summary>
        /// Call this to start the operation.
        /// </summary>
        /// <param name="caption">The caption to use in the progress dialog.</param>
        public void Start( string caption )
        {
            Thread thread = new Thread( new ThreadStart(this.Run) );

            using( ProgressDialog dialog = new ProgressDialog() )
            {
                dialog.Caption = caption;
                dialog.ProgressStatus +=new EventHandler<ProgressStatusEventArgs>(this.ProgressStatus);

                thread.Start();

                dialog.ShowDialog( this.Context.HostWindow );
            }
            if ( this.cancelled )
            {
                this.Context.OutputPane.WriteLine( "Cancelled" );
            }
            else if ( this.exception != null )
                throw new ProgressRunnerException(this.exception);
        }  
      
        private IContext Context
        {
            get{ return this.context; }
        }

        private void Run()
        {
            try
            {
                this.Context.Client.Cancel += new EventHandler<SvnCancelEventArgs>(this.Cancel);
                this.worker.Work(this.Context);
            }
            catch (SvnOperationCanceledException)
            {
                this.cancelled = true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            finally
            {
                this.done = true;
                this.Context.Client.Cancel -= new EventHandler<SvnCancelEventArgs>(this.Cancel);
            }
        }


        private void ProgressStatus( object sender, ProgressStatusEventArgs args )
        {
            args.Done = this.done;
            this.cancel = args.Cancelled;
        }

        private void Cancel(object sender, SvnCancelEventArgs args)
        {
            args.Cancel = this.cancel;
        }

        /// <summary>
        /// To be used to wrap exceptions thrown from the other thread.
        /// </summary>
        public class ProgressRunnerException : ApplicationException
        {
            public ProgressRunnerException( Exception realException ) : 
                base( "Exception thrown in progress runner thread", realException )
            {}            
        }

        private bool done = false;
        private bool cancel = false;
        private bool cancelled = false;
        private Exception exception;
        private IContext context;
        private IProgressWorker worker;
    }
}
