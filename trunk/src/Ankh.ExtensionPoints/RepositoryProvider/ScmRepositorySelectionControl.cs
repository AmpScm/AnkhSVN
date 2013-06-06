using System;
using System.ComponentModel;
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

        /// <summary>
        /// Populates the username, password information for a SCM repository
        /// </summary>
        /// <param name="e">Arguments</param>
        public virtual void UserNamePasswordCallback(ScmUserNamePasswordEventArgs e)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ScmRepositorySelectionControlEventArgs : EventArgs
    {
        private string msg;
        private Exception exc;
        private string repoUri;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repositoryUri"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public ScmRepositorySelectionControlEventArgs(string repositoryUri, string message, Exception exception)
        {
            this.repoUri = repositoryUri;
            this.msg = message;
            this.exc = exception;
        }

        /// <summary>
        /// Gets the repository URI string
        /// </summary>
        /// <remarks>this property is exposed here for convenience, and it must not be return a different value from ScmRepositorySelectionControl's SelectedUri property</remarks>
        public string RepositoryUri
        {
            get { return this.repoUri; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Message
        {
            get { return this.msg; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Exception Exception
        {
            get { return this.exc; }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ScmUserNamePasswordEventArgs : CancelEventArgs
    {
        private string userName;
        private string password;
        private string repoUri;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repositoryUri"></param>
        public ScmUserNamePasswordEventArgs(string repositoryUri)
        {
            this.repoUri = repositoryUri;
        }

        /// <summary>
        /// 
        /// </summary>
        public string UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }

        /// <summary>
        /// Gets the repository URI string
        /// </summary>
        public string RepositoryUri
        {
            get { return this.repoUri; }
        }
    }
}
