using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;
using System.ComponentModel;

namespace Ankh
{
    public class ProgressWorkerArgs : EventArgs
    {
        readonly AnkhContext _context;
        readonly SvnClient _client;
        readonly ISynchronizeInvoke _sync;
        Exception _exception;

        public ProgressWorkerArgs(IAnkhServiceProvider context, SvnClient client, ISynchronizeInvoke sync)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context.GetService<AnkhContext>();
            _client = client;
            _sync = sync;
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

        public ISynchronizeInvoke Synchronizer
        {
            get { return _sync; }
        }
    }

    public class ProgressRunnerResult
    {
        readonly bool _succeeded;

        public ProgressRunnerResult(bool succeeded)
        {
            _succeeded = succeeded;
        }

        public bool Succeeded
        {
            get { return _succeeded; }
        }
    }

    public interface IProgressRunner
    {
        ProgressRunnerResult Run(string caption, EventHandler<ProgressWorkerArgs> handler);
    }
}
