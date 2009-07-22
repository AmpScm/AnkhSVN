using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Interop.IssueTracker;

namespace Ankh
{
    public interface IAnkhIssueService
    {
        /// <summary>
        /// Gets all the registered Issue repository connectors.
        /// </summary>
        /// <remarks>This call DOES NOT trigger connector package initialization.</remarks>
        ICollection<IIssueRepositoryConnector> Connectors { get; }

        /// <summary>
        /// Tries to find a registered connector with the given name.
        /// </summary>
        /// <remarks>This call DOES NOT trigger connector package initialization.</remarks>
        bool TryGetConnector(string name, out IIssueRepositoryConnector connector);

        /// <summary>
        /// Gets the issue repository associated with the current solution.
        /// </summary>
        IIssueRepository CurrentIssueRepository { get; set; }
    }
}
