using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh
{
    public class ProgressWorkerArgs : EventArgs
    {
        readonly AnkhContext _context;
        readonly SvnClient _client;
        Exception _exception;

        public ProgressWorkerArgs(IAnkhServiceProvider context, SvnClient client)
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
