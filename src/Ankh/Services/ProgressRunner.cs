using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using SharpSvn;

using Ankh.UI;
using Ankh.VS;

namespace Ankh
{
    [GlobalService(typeof(IProgressRunner))]
    sealed class ProgressRunnerService : AnkhService, IProgressRunner
    {
        public ProgressRunnerService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public ProgressRunnerResult Run(string caption, EventHandler<ProgressWorkerArgs> handler)
        {
            if (string.IsNullOrEmpty(caption))
                caption = "AnkhSVN";

            ProgressRunner pr = new ProgressRunner(this, handler);

            pr.Start(caption);

            return new ProgressRunnerResult(!pr.Cancelled);
        }


        /// <summary>
        /// Used to run lengthy operations in a separate thread while 
        /// displaying a modal progress dialog in the main thread.
        /// </summary>
        sealed class ProgressRunner
        {
            readonly IAnkhServiceProvider _context;
            readonly EventHandler<ProgressWorkerArgs> _worker;
            Form _invoker;
            bool _cancelled;
            bool _closed;
            Exception _exception;

            /// <summary>
            /// Initializes a new instance of the <see cref="ProgressRunner"/> class.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="worker">The worker.</param>
            public ProgressRunner(IAnkhServiceProvider context, EventHandler<ProgressWorkerArgs> worker)
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

            ISynchronizeInvoke _sync;

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
                    _sync = dialog;
                    dialog.Caption = caption;
                    dialog.Context = _context;
                    thread.Name = "AnkhSVN Worker";

                    dialog.HandleCreated += delegate
                    {
                        thread.Start(client);
                    };
                    _invoker = dialog;

                    IUIService service = _context.GetService<IUIService>();

                    do
                    {
                        if (!_closed)
                        {
                            if (service == null)
                                dialog.ShowDialog(dialogOwner.DialogOwner);
                            else
                                service.ShowDialog(dialog);
                        }

                        // Show the dialog again if the thread join times out
                        // Do this to handle the acase where the service wants to
                        // pop up a dialog before canceling.

                        // BH: Experienced this 2008-09-29 when our repository server
                        //     accepted http connections but didn't handle them in time
                    }
                    while (!thread.Join(2500)); 
                }
                if (_cancelled)
                {
                    // NOOP
                }
                else if (_exception != null)
                    throw new ProgressRunnerException(this._exception);
            }

            private void Run(object arg)
            {
                SvnClient client = (SvnClient)arg;
                try
                {
                    ProgressWorkerArgs awa = new ProgressWorkerArgs(_context, client, _sync);
                    _worker(null, awa);

                    if (_exception == null && awa.Exception != null)
                        _exception = awa.Exception;
                }
                catch (SvnOperationCanceledException)
                {
                    _cancelled = true;
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
                finally
                {
                    _closed = true;
                    try
                    {
                        OnDone(this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        if (_exception == null)
                            _exception = ex;
                    }
                }
            }

            private void OnDone(object sender, EventArgs e)
            {
                Form si = _invoker;

                if (si != null && si.InvokeRequired)
                {
                    EventHandler eh = new EventHandler(OnDone);
                    try
                    {
                        si.Invoke(eh, new object[] { sender, e });
                    }
                    catch(Exception ex)
                    { 
                        /* Not Catching this exception kills VS */
                        GC.KeepAlive(ex);
                    }
                    return;
                }

                if (si.Visible)
                {
                    si.Close();
                }
            }
        }
    }

    /// <summary>
    /// To be used to wrap exceptions thrown from the other thread.
    /// </summary>
    public class ProgressRunnerException : Exception
    {
        public ProgressRunnerException(Exception realException) :
            base("Exception thrown in progress runner thread", realException)
        { }
    }
}
