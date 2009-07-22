using System;
using System.Windows.Forms;

namespace Ankh.Interop.IssueTracker
{
    [System.Runtime.InteropServices.Guid("E1E1B50C-366C-4f3e-934A-6BCCF78C96E8")]
    public interface IIssueRepository
    {
        /// <summary>
        /// Gets the issue repository connector's registered name
        /// </summary>
        string ConnectorName { get; }

        /// <summary>
        /// Issue repository URL.
        /// </summary>
        Uri RepositoryUri { get; }

        /// <summary>
        /// Label for the issue repository.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the control to be hosted in AnkhSVN PendingChanges window.
        /// </summary>
        Control Control { get; }

        /// <summary>
        /// Handles the request to open the issue.
        /// </summary>
        /// <param name="IssueId">Issue identifier</param>
        void NavigateTo(string issueId);

        /// <summary>
        /// Called just before the changes are committed.
        /// </summary>
        void PreCommit(PreCommitArgs args);

        /// <summary>
        /// Called just after the changes are committed.
        /// </summary>
        /// <remarks>Provider is responsible for any post-commit issue related operations</remarks>
        void PostCommit(PostCommitArgs args);

        /// <summary>
        /// Gets the RegEx pattern to recognize issues in a text.
        /// </summary>
        string IssueIdPattern { get; }
    }
}
