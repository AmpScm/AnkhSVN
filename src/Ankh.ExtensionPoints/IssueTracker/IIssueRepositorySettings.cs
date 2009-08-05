using System;
using System.Collections.Generic;

namespace Ankh.ExtensionPoints.IssueTracker
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIssueRepositorySettings
    {
        /// <summary>
        /// Gets the issue repository connector's registered name
        /// </summary>
        string ConnectorName { get; }

        /// <summary>
        /// Gets Issue Repository URI
        /// </summary>
        Uri RepositoryUri { get; }

        /// <summary>
        /// Gets the unique id of the repository.
        /// </summary>
        /// <remarks>This value can be used to identify a specific (sub) issue repository on RepositoryUri</remarks>
        string RepositoryId { get; }

        /// <summary>
        /// Custom properties specific to the connector.
        /// </summary>
        Dictionary<string, object> CustomProperties { get; }
    }
}
