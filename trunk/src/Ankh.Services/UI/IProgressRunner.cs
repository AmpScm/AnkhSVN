using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.ComponentModel;

namespace Ankh
{
    public class ProgressWorkerArgs : EventArgs
    {
        readonly IAnkhServiceProvider _context;
        readonly SvnClient _client;
        readonly ISynchronizeInvoke _sync;
        Exception _exception;

        public ProgressWorkerArgs(IAnkhServiceProvider context, SvnClient client, ISynchronizeInvoke sync)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
            _client = client;
            _sync = sync;
        }

        public SvnClient Client
        {
            get { return _client; }
        }

        public IAnkhServiceProvider Context
        {
            get { return _context; }
        }

        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }

        public ISynchronizeInvoke Synchronizer
        {
            get { return _sync; }
        }
    }

    public class ProgressRunnerResult
    {
        readonly bool _succeeded;
        readonly Exception _ex;

        public ProgressRunnerResult(bool succeeded)
        {
            _succeeded = succeeded;
        }

        public ProgressRunnerResult(bool succeeded, Exception e)
        {
            _ex = e;
        }

        public bool Succeeded
        {
            get { return _succeeded; }
        }

        public Exception Exception
        {
            get { return _ex; }
        }
    }

    public class ProgressWorkerDoneArgs : EventArgs
    {
        readonly ProgressRunnerResult _result;
        public ProgressWorkerDoneArgs(ProgressRunnerResult result)
        {
            if (result == null)
                throw new ArgumentNullException("result");

            _result = result;
        }

        public ProgressRunnerResult Result
        {
            get { return _result; }
        }
    }

    public interface IProgressRunner
    {
        /// <summary>
        /// Runs the specified action.
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        ProgressRunnerResult RunModal(string caption, EventHandler<ProgressWorkerArgs> action);

        /// <summary>
        /// Runs the specified action and when the action completes completer. (Currently implemented synchronously!)
        /// </summary>
        /// <param name="caption">The caption.</param>
        /// <param name="action">The action.</param>
        /// <param name="completer">The completer.</param>
        void RunNonModal(string caption, EventHandler<ProgressWorkerArgs> action, EventHandler<ProgressWorkerDoneArgs> completer);
    }
}
