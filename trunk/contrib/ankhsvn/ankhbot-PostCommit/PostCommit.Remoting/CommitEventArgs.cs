using System;
using System.Collections.Generic;
using System.Text;

namespace PostCommit.Remoting
{
    [Serializable]
    public class CommitEventArgs : EventArgs
    {
        public CommitEventArgs( Commit commit )
        {
            this.commit = commit;
        }

        public Commit Commit
        {
            get { return commit; }
        }

        private Commit commit;
    }
}
