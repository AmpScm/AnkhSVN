using System.Windows.Forms;

namespace Ankh.Interop.IssueTracker
{
    [System.Runtime.InteropServices.Guid("7C7C4E55-3551-488b-B8BA-714F4A96E75D")]
    public interface IIssueRepositoryConfigurationPage : IWin32Window
    {
        /// <summary>
        /// Gets or sets the current repository settings.
        /// </summary>
        IIssueRepositorySettings Settings { get; set; }
    }
}
