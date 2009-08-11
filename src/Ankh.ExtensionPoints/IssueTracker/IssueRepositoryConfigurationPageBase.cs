using System.Windows.Forms;
using System;

namespace Ankh.ExtensionPoints.IssueTracker
{
    /// <summary>
    /// Base class for Issue Repository configuration page
    /// </summary>
    public abstract class IssueRepositoryConfigurationPageBase : IIssueRepositoryConfigurationPageEvents
    {
        private IssueRepositorySettingsBase _settings;

        /// <summary>
        /// Gets or sets the current repository settings.
        /// </summary>
        public virtual IssueRepositorySettingsBase Settings
        {
            get { return _settings; }
            set { _settings = value; }
        }

        /// <summary>
        /// Gets the IWin32Window instance for issue repository configuration UI.
        /// </summary>
        /// <remarks>Default implementation returns this as IWin32Window.</remarks>
        public virtual IWin32Window Window
        {
            get
            {
                return this as IWin32Window;
            }
        }

        /// <summary>
        /// Notifies ragistered OnPageEvent handlers.
        /// </summary>
        public virtual void ConfigurationPageChanged(ConfigPageEventArgs e)
        {
            if (OnPageEvent != null)
            {
                OnPageEvent(this, e);
            }
        }

        #region IIssueRepositoryConfigurationPageEvents Members

        /// <summary>
        /// Raised on a configuration page event 
        /// </summary>
        public event System.EventHandler<ConfigPageEventArgs> OnPageEvent;

        #endregion
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
