using System.Windows.Forms;

namespace Ankh.ExtensionPoints.IssueTracker
{
    /// <summary>
    /// Base class for IssueRepository implementation
    /// </summary>
    public abstract class IssueRepository : IssueRepositorySettings
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectorName">Unique connector name(as registered with the registry)</param>
        public IssueRepository(string connectorName)
            : base(connectorName)
        {
        }

        /// <summary>
        /// Label for the issue repository.
        /// </summary>
        public abstract string Label { get; }

        /// <summary>
        /// Handles the request to open the issue.
        /// </summary>
        /// <param name="issueId">Issue identifier</param>
        public virtual void NavigateTo(string issueId)
        {
        }

        /// <summary>
        /// Called just before the changes are committed.
        /// </summary>
        public virtual void PreCommit(PreCommitArgs args)
        {
            args.Cancel = false;
        }

        /// <summary>
        /// Called just after the changes are committed.
        /// </summary>
        /// <remarks>Provider is responsible for any post-commit issue related operations</remarks>
        public virtual void PostCommit(PostCommitArgs args)
        {
        }

        /// <summary>
        /// Gets the RegEx pattern to recognize issues in a text.
        /// </summary>
        public virtual string IssueIdPattern
        {
            get { return string.Empty; }
        }

        /// <summary>
        /// Gets the IWin32Window instance for issue presentation
        /// </summary>
        /// <remarks>Default implementation returns this as IWin32Window.</remarks>
        public virtual IWin32Window Window
        {
            get { return this as IWin32Window; }
        }
    }
}
