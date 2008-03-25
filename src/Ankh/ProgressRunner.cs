using System;
using System.Threading;
using SharpSvn;

using Ankh.ContextServices;
using Ankh.UI;
using Ankh.VS;

namespace Ankh
{
    public class AnkhWorkerArgs : EventArgs
    {
        AnkhContext _context;
        SvnClient _client;

        public AnkhWorkerArgs(IAnkhServiceProvider context, SvnClient client)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context.GetService<AnkhContext>();
            _client = client;
        }

        public SvnClient Client
        {
            get { return _client; }
        }

        public AnkhContext Context
        {
            get { return _context; }
        }
    }

    public interface IProgressWorker
    {
        void Work(AnkhWorkerArgs e);
    }

    public delegate void SimpleProgressWorkerCallback(AnkhWorkerArgs e);


    public class SimpleProgressWorker : IProgressWorker
    {
        public SimpleProgressWorker(SimpleProgressWorkerCallback cb)
        {
            this.callback = cb;
        }
        public void Work(AnkhWorkerArgs e)
        {
            this.callback(e);
        }

        private SimpleProgressWorkerCallback callback;
    }

    /// <summary>
    /// Used to run lengthy operations in a separate thread while 
    /// displaying a modal progress dialog in the main thread.
    /// </summary>
    public class ProgressRunner
    {
        IAnkhServiceProvider _context;
        IProgressWorker _worker;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressRunner"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="worker">The worker.</param>
        public ProgressRunner(IAnkhServiceProvider context, IProgressWorker worker)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _worker = worker;
        }

        /// <summary>
        /// Whether the operation was cancelled.
        /// </summary>
        public bool Cancelled
        {
            get { return this.cancelled; }
        }

        /// <summary>
        /// Call this to start the operation.
        /// </summary>
        /// <param name="caption">The caption to use in the progress dialog.</param>
        public void Start(string caption)
        {
            Thread thread = new Thread(new ThreadStart(this.Run));

            using (ProgressDialog dialog = new ProgressDialog())
            {
                dialog.Caption = caption;
                dialog.ProgressStatus += new EventHandler<ProgressStatusEventArgs>(this.ProgressStatus);

                thread.Start();

                dialog.ShowDialog(_context.GetService<IAnkhDialogOwner>().DialogOwner);
            }
            if (this.cancelled)
            {
                IAnkhOperationLogger logger = _context.GetService<IAnkhOperationLogger>();

                if (logger != null)
                {
                    logger.WriteLine("Cancelled");
                }
            }
            else if (this.exception != null)
                throw new ProgressRunnerException(this.exception);
        }

        private void Run()
        {
            try
            {
                ISvnClientPool pool = _context.GetService<ISvnClientPool>();

                using (SvnClient client = (pool != null) ? pool.GetClient() : new SvnClient())
                {
                    client.Cancel += new EventHandler<SvnCancelEventArgs>(Cancel);
                    try
                    {
                        _worker.Work(new AnkhWorkerArgs(_context, client));
                    }
                    finally
                    {
                        client.Cancel -= new EventHandler<SvnCancelEventArgs>(Cancel);
                    }
                }
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
            }
        }

        private void ProgressStatus(object sender, ProgressStatusEventArgs e)
        {
            if (done)
                e.Done = true;

            if(e.Cancelled)
                this.cancel = true;
        }

        private void Cancel(object sender, SvnCancelEventArgs args)
        {
            if(this.cancel)
                args.Cancel = true;
        }

        /// <summary>
        /// To be used to wrap exceptions thrown from the other thread.
        /// </summary>
        public class ProgressRunnerException : ApplicationException
        {
            public ProgressRunnerException(Exception realException) :
                base("Exception thrown in progress runner thread", realException)
            { }
        }

        private bool done = false;
        private volatile bool cancel = false; // Modified out of thread
        private bool cancelled = false;
        private Exception exception;
    }
}
