using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    public interface IPendingChangeHandler
    {
        bool Commit(IEnumerable<PendingChange> changes, PendingChangeCommitArgs args);
    }

    public class PendingChangeCommitArgs
    {
        string _logMessage;
        bool _keepLocks;
        bool _keepChangeLists;
        bool _storeMessageOnError;

        /// <summary>
        /// Gets or sets the log message.
        /// </summary>
        /// <value>The log message.</value>
        public string LogMessage
        {
            get { return _logMessage; }
            set { _logMessage = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [keep locks].
        /// </summary>
        /// <value><c>true</c> if [keep locks]; otherwise, <c>false</c>.</value>
        public bool KeepLocks
        {
            get { return _keepLocks; }
            set { _keepLocks = value; }
        }

        public bool KeepChangeLists
        {
            get { return _keepChangeLists; }
            set { _keepChangeLists = value; }
        }

        public bool StoreMessageOnError
        {
            get { return _storeMessageOnError; }
            set { _storeMessageOnError = value; }
        }
    }
}
