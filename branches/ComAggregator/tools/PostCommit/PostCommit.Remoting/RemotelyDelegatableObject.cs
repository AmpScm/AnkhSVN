using System;
using System.Collections.Generic;
using System.Text;

namespace PostCommit.Remoting
{
    public class RemotelyDelegatableObject : MarshalByRefObject
    {
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void CommittedCallback( object sender, CommitEventArgs args )
        {
            InternalCommittedCallback( sender, args );
        }

        protected virtual void InternalCommittedCallback( object sender, CommitEventArgs args )
        {
            // empty
        }
    }
}
