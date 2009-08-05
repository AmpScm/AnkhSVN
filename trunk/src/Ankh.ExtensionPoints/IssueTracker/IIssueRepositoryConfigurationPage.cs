using System.Windows.Forms;
using System;

namespace Ankh.ExtensionPoints.IssueTracker
{
    // ### BH: I think we should replace these interfaces with a new abstract baseclass. That way we can add
    // properties, methods and events whenever we want. (An interface can never change)

    /// <summary>
    /// 
    /// </summary>
    public interface IIssueRepositoryConfigurationPage : IWin32Window, IIssueRepositoryConfigurationPageEvents
    {
        /// <summary>
        /// Gets or sets the current repository settings.
        /// </summary>
        IIssueRepositorySettings Settings { get; set; }
    }

    /// <summary>
    /// This interface exposes the config page events raised by connector configuration logic.
    /// </summary>
    public interface IIssueRepositoryConfigurationPageEvents
    {
        /// <summary>
        /// Raised on a config page event
        /// </summary>
        event EventHandler<ConfigPageEventArgs> OnPageEvent;
    }

    /// <summary>
    /// 
    /// </summary>
    public class ConfigPageEventArgs : EventArgs
    {
        private bool _isComplete;
        private Exception _exception;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is complete.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is complete; otherwise, <c>false</c>.
        /// </value>
        public bool IsComplete
        {
            get { return _isComplete; }
            set { _isComplete = value; }
        }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>The exception.</value>
        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }
    }
}
