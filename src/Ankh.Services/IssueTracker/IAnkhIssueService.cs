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
        ICollection<IIssueRepositoryConnector> Connectors { get; }

        /// <summary>
        /// Tries to find a registered connector with the given name.
        /// </summary>
        /// <remarks>This call DOES NOT trigger connector package initialization.</remarks>
        bool TryGetConnector(string name, out IIssueRepositoryConnector connector);

        /// <summary>
        /// Gets the issue repository settings associated with the current solution.
        /// </summary>
        IIssueRepositorySettings CurrentIssueRepositorySettings { get; }

        /// <summary>
        /// Gets or Sets the issue repository associated with the current solution.
        /// </summary>
        IIssueRepository CurrentIssueRepository { get; set; }

        /// <summary>
        /// Occurs when current solution's Issue Tracker Repository association settings are changed
        /// </summary>
        event EventHandler IssueRepositoryChanged;

        #region VS Solution persistence

        /// <summary>
        /// Gets or sets a boolean indicating whether te solution should be saved for changed scc settings
        /// </summary>
        bool IsSolutionDirty { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has solution property data.
        /// </summary>
        bool HasSolutionData { get; }

        /// <summary>
        /// Writes the Issue Repository Settings to the solution
        /// </summary>
        /// <param name="propertyBag">The property bag.</param>
        void WriteSolutionProperties(IPropertyMap propertyBag);

        /// <summary>
        /// Loads the Issue Repository Settings
        /// </summary>
        /// <param name="propertyBag">The property bag.</param>
        void ReadSolutionProperties(IPropertyMap propertyBag);

        #endregion
    }
}
