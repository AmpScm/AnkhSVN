using System;
using System.Threading;
using SharpSvn;

using Ankh.ContextServices;
using Ankh.UI;
using Ankh.VS;
using System.ComponentModel;
using System.Windows.Forms;

namespace Ankh
{
    public class AnkhWorkerArgs : EventArgs
    {
        readonly AnkhContext _context;
        readonly SvnClient _client;
        Exception _exception;

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

        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
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
        readonly IAnkhServiceProvider _context;
        readonly IProgressWorker _worker;
        Form _invoker;
        bool _done;
        bool _cancelled;
        Exception _exception;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressRunner"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="worker">The worker.</param>
        public ProgressRunner(IAnkhServiceProvider context, IProgressWorker worker)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (worker == null)
                throw new ArgumentNullException("worker");

            _context = context;
            _worker = worker;
        }

        /// <summary>
        /// Whether the operation was cancelled.
        /// </summary>
        public bool Cancelled
        {
            get { return _cancelled; }
        }

        /// <summary>
        /// Call this to start the operation.
        /// </summary>
        /// <param name="caption">The caption to use in the progress dialog.</param>
        public void Start(string caption)
        {
            Thread thread = new Thread(new ParameterizedThreadStart(this.Run));
            ISvnClientPool pool = _context.GetService<ISvnClientPool>();
            IAnkhDialogOwner dialogOwner = _context.GetService<IAnkhDialogOwner>();

            using (ProgressDialog dialog = new ProgressDialog())
            using (SvnClient client = pool.GetClient())
            using (dialog.Bind(client))
            {
                dialog.Caption = caption;

                thread.Start(client);

                _invoker = dialog;

                dialog.ShowDialog(dialogOwner.DialogOwner);

                thread.Join();
            }
            if (_cancelled)
            {
                IAnkhOperationLogger logger = _context.GetService<IAnkhOperationLogger>();

                if (logger != null)
                {
                    logger.WriteLine("Cancelled");
                }
            }
            else if (_exception != null)
                throw new ProgressRunnerException(this._exception);
        }

        private void Run(object arg)
        {
            SvnClient client = (SvnClient)arg;
            try
            {
                AnkhWorkerArgs awa = new AnkhWorkerArgs(_context, client);
                _worker.Work(awa);

                if (_exception == null && awa.Exception != null)
                    _exception = awa.Exception;
            }
            catch (SvnOperationCanceledException)
            {
                _cancelled = true;
            }
            catch (Exception ex)
            {
                this._exception = ex;
            }
            finally
            {
                _done = true;
                OnDone(this, EventArgs.Empty);
            }
        }

        private void OnDone(object sender, EventArgs e)
        {
            Form si = _invoker;

            if (si != null && si.InvokeRequired)
            {
                EventHandler eh = new EventHandler(OnDone);
                si.Invoke(eh, new object[] { sender, e });
                return;
            }

            if (si.Visible)
            {
                si.Close();
            }
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
    }
}
