// $Id$
using System;
using NSvn.Core;

namespace NSvn
{
    /// <summary>
    /// Represents the EventArgs passed to the LogMessage event.
    /// </summary>
    public class LogMessageEventArgs : EventArgs
    {
        public LogMessageEventArgs( CommitItem[] targets )
        {
            this.commitTargets = targets;
        }
        /// <summary>
        /// The actual log message.
        /// </summary>
        public string LogMessage
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.logMessage; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.logMessage = value; }
        }

        /// <summary>
        /// Whether the commit should be canceled.
        /// </summary>
        public bool Canceled
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.canceled; }

            [System.Diagnostics.DebuggerStepThrough]
            set{ this.canceled = value; }
        }

        /// <summary>
        /// The resources that are scheduled for commit.
        /// </summary>
        public CommitItem[] CommitTargets
        {
            [System.Diagnostics.DebuggerStepThrough]
            get{ return this.commitTargets; }
        }

        private string logMessage = "";
        private bool canceled = false;
        private CommitItem[] commitTargets;
    }

    public delegate void LogMessageEventHandler( object sender, LogMessageEventArgs args );
}
