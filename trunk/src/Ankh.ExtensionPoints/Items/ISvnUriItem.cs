using System;

namespace Ankh.ExtensionPoints.Items
{
    /// <summary>
    /// Interface to allow Subversion Info integration
    /// </summary>
    public interface ISvnUriItem
    {
        /// <summary>
        /// The absolute path to the item on disk or <c>null</c> if the item has no local representation
        /// </summary>
        /// <remarks>Can be <c>null</c> if <see cref="Uri"/> is not null</remarks>
        string FullPath { get; }

        /// <summary>
        /// The uri to the item in the repository
        /// </summary>
        /// <remarks>Can be <c>null</c> if <see cref="FullPath"/> is a valid path</remarks>
        Uri Uri { get; }

        /// <summary>
        /// The repository root of <see cref="Uri"/> or <c>null</c>
        /// </summary>
        /// <remarks>It's better to provide <c>null</c> than a potentially invalid value</remarks>
        Uri RepositoryRoot { get; }
    }
}
