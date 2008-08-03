using System;
using System.Collections.Generic;
using System.Text;

namespace PostCommit.Remoting
{
    public delegate void CommittedEventHandler( object sender, CommitEventArgs args );

    public interface IRepository
    {
        event CommittedEventHandler Committed;

        void Ping();
    }
}
