using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Ankh.ExtensionPoints.IssueTracker
{
    // ### BH: If we make this a base class it will be easier to extend it in future versions

    /// <summary>
    /// 
    /// </summary>
    public interface IIssueRepository : IIssueRepositorySettings, IWin32Window
    {
        /// <summary>
        /// Label for the issue repository.
        /// </summary>
        string Label { get; }

        /// <summary>
        /// Handles the request to open the issue.
        /// </summary>
        /// <param name="issueId">Issue identifier</param>
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
