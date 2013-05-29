using System;
using System.Windows.Forms;

namespace Ankh.ExtensionPoints.RepositoryProvider
{
    /// <summary>
    /// Base class for SCM repository selection pages contributed by the repository providers.
    /// </summary>
    public abstract class ScmRepositorySelectionControl
    {
        /// <summary>
        /// Raised when this control needs to notify containers, such as when selection changes, or when an exception is thrown.
        /// </summary>
        public event EventHandler<ScmRepositorySelectionControlEventArgs> ScmRepositorySelectionControlEvent;

        /// <summary>
        /// Gets the string representation of the selected SCM repository URI
        /// </summary>
        public abstract string SelectedUri { get; }

        /// <summary>
        /// Gets the IWin32Window instance which provides the UI for listing and selecting a SCM repository
        /// </summary>
        /// <remarks>Default implementation returns this as IWin32Window.</remarks>        
        public virtual IWin32Window Window
        {
            get { return this as IWin32Window; }
        }

        /// <summary>
        /// Raises ScmRepositorySelectionControlEvent when this control needs to notify containers, such as when selection changes, or when an exception is thrown.
        /// </summary>
        public virtual void OnScmRepositorySelectionControlEvent(ScmRepositorySelectionControlEventArgs e)
        {
            if (ScmRepositorySelectionControlEvent != null)
            {
                ScmRepositorySelectionControlEvent(this, e);
            }
        }
    }

    public class ScmRepositorySelectionControlEventArgs : EventArgs
    {
        // TODO
        private string message;
        private Exception exception;
    }
}
