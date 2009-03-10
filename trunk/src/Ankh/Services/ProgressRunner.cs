// $Id$
//
// Copyright 2004-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;

using SharpSvn;

using Ankh.UI;
using Ankh.VS;
using Ankh.Ids;
using System.Text;
using System.IO;
using Ankh.VS.OutputPane;

namespace Ankh
{
    [GlobalService(typeof(IProgressRunner))]
    sealed class ProgressRunnerService : AnkhService, IProgressRunner
    {
        public ProgressRunnerService(IAnkhServiceProvider context)
            : base(context)
        {
        }

        public ProgressRunnerResult RunModal(string caption, EventHandler<ProgressWorkerArgs> action)
        {
            return RunModal(caption, action, false);
        }

        public ProgressRunnerResult RunModal(string caption, EventHandler<ProgressWorkerArgs> action, bool log)
        {
            if (action == null)
                throw new ArgumentNullException("action");
            else if (string.IsNullOrEmpty(caption))
                caption = AnkhId.PlkProduct;

            ProgressRunner pr = new ProgressRunner(this, action);
            pr.LogOutput = log;
            pr.Start(caption);

            return new ProgressRunnerResult(!pr.Cancelled);
        }

        public void RunNonModal(string caption, EventHandler<ProgressWorkerArgs> action, EventHandler<ProgressWorkerDoneArgs> completer)
        {
            ProgressRunnerResult r = null;
            // Temporary implementation
            try
            {
                r = RunModal(caption, action);
            }
            catch (Exception e)
            {
                r = new ProgressRunnerResult(false, e);
            }
            finally
            {
                if (completer != null)
                    completer(this, new ProgressWorkerDoneArgs(r));
            }
        }

        /// <summary>
        /// Used to run lengthy operations in a separate thread while 
        /// displaying a modal progress dialog in the main thread.
        /// </summary>
        sealed class ProgressRunner
        {
            readonly IAnkhServiceProvider _context;
            readonly EventHandler<ProgressWorkerArgs> _action;
            Form _invoker;
            bool _cancelled;
            bool _closed;
            Exception _exception;
            bool _logOutput;

            /// <summary>
            /// Initializes a new instance of the <see cref="ProgressRunner"/> class.
            /// </summary>
            /// <param name="context">The context.</param>
            /// <param name="action">The action.</param>
            public ProgressRunner(IAnkhServiceProvider context, EventHandler<ProgressWorkerArgs> action)
            {
                if (context == null)
                    throw new ArgumentNullException("context");
                else if (action == null)
                    throw new ArgumentNullException("action");

                _context = context;
                _action = action;
            }

            public bool LogOutput
            {
                get { return _logOutput; }
                set { _logOutput = value; }
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
                    IDisposable reporter = LogOutput ? BindOutputPane(client) : null;
                    using (reporter)
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

                        do
                        {
                            if (!_closed)
                            {
                                dialog.ShowDialog(_context);
                            }

                            // Show the dialog again if the thread join times out
                            // Do this to handle the acase where the service wants to
                            // pop up a dialog before canceling.

                            // BH: Experienced this 2008-09-29 when our repository server
                            //     accepted http connections but didn't handle them in time
                        }
                        while (!thread.Join(2500));
                    }
                }
                if (_cancelled)
                {
                    // NOOP
                }
                else if (_exception != null)
                    throw new ProgressRunnerException(this._exception);
            }

            private IDisposable BindOutputPane(SvnClient client)
            {
                return new OutputPaneReporter(client, _context);
            }

            private void Run(object arg)
            {
                SvnClient client = (SvnClient)arg;
                try
                {
                    ProgressWorkerArgs awa = new ProgressWorkerArgs(_context, client, _sync);
                    _action(null, awa);

                    if (_exception == null && awa.Exception != null)
                        _exception = awa.Exception;

                    if (_exception is SvnOperationCanceledException)
                        _cancelled = true;
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

        sealed class OutputPaneReporter : IDisposable
        {
            readonly IOutputPaneManager _mgr;
            readonly SvnClientReporter _reporter;
            readonly StringBuilder _sb;

            public OutputPaneReporter(SvnClient client, IAnkhServiceProvider context)
            {
                _mgr = context.GetService<IOutputPaneManager>();
                _sb = new StringBuilder();
                _reporter = new SvnClientReporter(client, _sb);
            }

            public void Dispose()
            {
                try
                {
                    _mgr.WriteToPane(_sb.ToString());
                    _mgr.WriteToPane("\r\n");
                }
                finally
                {
                    _reporter.Dispose();
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
