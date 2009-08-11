using System;
using System.Collections.Generic;
using Ankh.ExtensionPoints.IssueTracker;
using Ankh.Scc;

namespace Ankh
{
    [CLSCompliant(false)]
    public interface IAnkhIssueService
    {
        /// <summary>
        /// Gets all the registered Issue repository connectors.
        /// </summary>
        /// <remarks>This call DOES NOT trigger connector package initialization.</remarks>
        ICollection<IssueRepositoryConnectorBase> Connectors { get; }

        /// <summary>
        /// Tries to find a registered connector with the given name.
        /// </summary>
        /// <remarks>This call DOES NOT trigger connector package initialization.</remarks>
        bool TryGetConnector(string name, out IssueRepositoryConnectorBase connector);

        /// <summary>
        /// Gets the issue repository settings associated with the current solution.
        /// </summary>
        IssueRepositorySettingsBase CurrentIssueRepositorySettings { get; }

        /// <summary>
        /// Gets or Sets the issue repository associated with the current solution.
        /// </summary>
        IssueRepositoryBase CurrentIssueRepository { get; set; }

        /// <summary>
        /// Occurs when current solution's Issue Tracker Repository association settings are changed
        /// </summary>
        event EventHandler IssueRepositoryChanged;

        /// <summary>
        /// Marks Issue Service as dirty which signals Issues page to refresh itself.
        /// </summary>
        void MarkDirty();
    }
}
