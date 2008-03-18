using System;
using System.Collections.Generic;
using System.Text;
using PostCommit.Remoting;

namespace PostCommit.Service
{
    class Remote : MarshalByRefObject, IRepository
    {
        public event CommittedEventHandler Committed
        {
            add { runtime.Committed += value; }
            remove { runtime.Committed -= value; }
        }

        public void Ping()
        {
            // empty
        }

        private PostCommitRuntime runtime = PostCommitRuntime.Instance;

    }
}
