using System;
using System.Threading;
using Ankh.UI;
using NSvn.Core;

namespace Ankh
{
    public delegate void ProgressRunnerCallback( IContext context );

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
        public ProgressRunner( IContext context, ProgressRunnerCallback callback )
        {
            this.context = context;
            this.callback = callback;
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
                dialog.ProgressStatus +=new ProgressStatusDelegate(this.ProgressStatus);

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
      
        protected IContext Context
        {
            get{ return this.context; }
        }


        /// <summary>
        /// Override this to perform the operation in a derived class.
        /// </summary>
        protected virtual void DoRun()
        {
            if ( this.callback != null )
                this.callback( this.Context );
        }

        private void Run()
        {
            try
            {
                this.Context.Client.Cancel += new NSvn.Core.CancelDelegate(this.Cancel);
                this.DoRun();
            }
            catch( OperationCancelledException )
            {
                this.cancelled = true;
            }
            catch( Exception ex )
            {
                this.exception = ex;
            }
            finally
            {
                this.done = true;
                this.Context.Client.Cancel -= new CancelDelegate(this.Cancel);
            }
        }


        private void ProgressStatus( object sender, ProgressStatusEventArgs args )
        {
            args.Done = this.done;
            this.cancel = args.Cancelled;
        }

        private void Cancel(object sender, CancelEventArgs args)
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
        private ProgressRunnerCallback callback;
    }
}
