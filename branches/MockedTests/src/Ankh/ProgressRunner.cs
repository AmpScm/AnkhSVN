using System;
using System.Threading;
using Ankh.UI;
using NSvn.Core;
using System.Windows.Forms;

namespace Ankh
{
    public interface IProgressWorker
    {        
        void Work( IServiceProvider serviceProvider );
    }

    public delegate void SimpleProgressWorkerCallback(IServiceProvider serviceProvider);


    public class SimpleProgressWorker : IProgressWorker
    {
        public SimpleProgressWorker( SimpleProgressWorkerCallback cb )
        {
            this.callback = cb;
        }
        public void Work(IServiceProvider serviceProvider)
        {
            this.callback(serviceProvider);
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
        public ProgressRunner( IServiceProvider serviceProvider, IProgressWorker worker )
        {
            this.serviceProvider = serviceProvider;
            this.worker = worker;
        }

        public ProgressRunner( IServiceProvider serviceProvider ) 
            : this( serviceProvider, null )
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

                dialog.ShowDialog( this.HostWindow );
            }
            if ( this.cancelled )
            {
                this.OutputPane.WriteLine( "Cancelled" );
            }
            else if ( this.exception != null )
                throw new ProgressRunnerException(this.exception);
        }

        private void Run()
        {
            try
            {
                this.Client.Cancel += new NSvn.Core.CancelDelegate(this.Cancel);
                this.worker.Work( this.serviceProvider );
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
                this.Client.Cancel -= new CancelDelegate(this.Cancel);
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

        IWin32Window HostWindow
        {
            get { return ((IHostWindowService)this.serviceProvider.GetService(typeof(IHostWindowService))).HostWindow; }
        }

        Client Client
        {
            get { return ((IClientProvider)this.serviceProvider.GetService(typeof(IClientProvider))).Client; }
        }

        OutputPaneTextWriter OutputPane
        {
            get { return ((IOutputPaneProvider)this.serviceProvider.GetService(typeof(IOutputPaneProvider))).OutputPaneWriter; }
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
        private IServiceProvider serviceProvider;
        private IProgressWorker worker;
    }
}
