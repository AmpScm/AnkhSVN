// $Id$
using System;
using NSvn.Core;

namespace NSvn
{
    /// <summary>
    /// This represents an object that can provide a log message.
    /// </summary>
    [Obsolete("Will be removed in a future version")]
    public interface ILogMessageProvider
    {
        /// <summary>
        /// Retrieve the log message.
        /// </summary>
        /// <param name="targets">The items that will be committed.</param>
        /// <returns>The log message.</returns>
        string GetLogMessage( CommitItem[] targets );
    }

    /// <summary>
    /// Represents a simple ILogMessageProvider that can be initialized with a 
    /// string.
    /// </summary>
    [Obsolete("Will be removed in a future version")]
    public class SimpleLogMessageProvider : ILogMessageProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="targets">The items that will be committed.</param>
        /// <param name="logMessage">The log message</param>
        public SimpleLogMessageProvider( string logMessage )
        {
            this.message = logMessage;
        }

        /// <summary>
        /// Retrieve the log message.
        /// </summary>
        /// <param name="targets">The items that will be committed.</param>
        /// <returns>The log message.</returns>
        public string GetLogMessage( CommitItem[] targets )
        {
            return this.message; 
        }

        private string message;
    }
}
