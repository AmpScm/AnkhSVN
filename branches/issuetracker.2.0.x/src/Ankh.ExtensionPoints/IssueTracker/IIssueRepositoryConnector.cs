using System;
using System.Windows.Forms;

namespace Ankh.Interop.IssueTracker
{
    [System.Runtime.InteropServices.Guid("0E080D53-D4A1-4609-8AB1-45ABD217BA4B")]
    public interface IIssueRepositoryConnector
    {
        /// <summary>
        /// Gets the registered connector's unique name 
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Creates an Issue Repository based on the settings.
        /// </summary>
        IIssueRepository Create(IIssueRepositorySettings settings);

        /// <summary>
        /// Gets the IWin32Window (to edit/setup an issue repository)
        /// </summary>
        IIssueRepositoryConfigurationPage ConfigurationPage { get; }
    }
}
