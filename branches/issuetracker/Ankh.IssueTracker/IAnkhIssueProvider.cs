using System;
using System.Windows.Forms;

namespace Ankh
{
    [System.Runtime.InteropServices.Guid("E1E1B50C-366C-4f3e-934A-6BCCF78C96E8")]
    public interface IAnkhIssueProvider
    {
        /// <summary>
        /// Determines if this provider is associated with the <paramref name="RepositoryUri"/>.
        /// </summary>
        /// <param name="RepositoryUri"></param>
        /// <returns>boolean</returns>
        bool IsProviderFor(Uri RepositoryUri);

        /// <summary>
        /// Label for the issue provider.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Gets the control to be hosted.
        /// </summary>
        Control Control { get; }

        /// <summary>
        /// Handles the request to open the issue.
        /// </summary>
        /// <param name="IssueId">Issue identifier</param>
        void NavigateTo(string IssueId);

        /// <summary>
        /// Called just before the changes are committed.
        /// </summary>
        /// <param name="CommitMessage">Commit message to be updated.</param>
        /// <param name="Cancel"></param>
        void PreCommit(Uri[] Paths, ref string CommitMessage, ref bool Cancel);

        /// <summary>
        /// Called just after the changes are committed.
        /// </summary>
        /// <param name="Paths"></param>
        /// <param name="RevisionNumber">New revision number after commit</param>
        /// <remarks>Provider is responsible for any post-commit issue related operations</remarks>
        void PostCommit(Uri[] Paths, long RevisionNumber);

        /// <summary>
        /// Gets the RegEx pattern to recognize issues in a text.
        /// </summary>
        string IssueIdPattern { get; }
    }
}
