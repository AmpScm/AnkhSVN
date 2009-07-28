using System.Windows.Forms;
using System;

namespace Ankh.ExtensionPoints.IssueTracker
{
    [System.Runtime.InteropServices.Guid("7C7C4E55-3551-488b-B8BA-714F4A96E75D")]
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
    [System.Runtime.InteropServices.Guid("B0ECBD34-2CA9-49fc-8198-242B470CA1BF")]
    public interface IIssueRepositoryConfigurationPageEvents
    {
        /// <summary>
        /// Raised with ConfigPageEventArgs by Issue Tracker Connector configuration page
        /// </summary>
        event EventHandler<ConfigPageEventArgs> OnPageEvent;
    }

    [System.Runtime.InteropServices.Guid("5FC53092-5C4A-434f-A424-F306E6402BF9")]
    public class ConfigPageEventArgs : EventArgs
    {
        private bool _isComplete;
        private Exception _exception;

        public bool IsComplete
        {
            get { return _isComplete; }
            set { _isComplete = value; }
        }

        public Exception Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }
    }
}
