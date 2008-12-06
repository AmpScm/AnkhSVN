using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>The default implementation of this service is thread safe</remarks>
    public interface IFileStatusCache : IAnkhServiceProvider
    {
        /// <summary>
        /// Gets the <see cref="Ankh.SvnItem"/> with the specified path.
        /// </summary>
        /// <value></value>
        SvnItem this[string path] { get; }

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="depth">The depth.</param>
        void UpdateStatus(string directory, SvnDepth depth);

        /// <summary>
        /// Marks the specified path dirty
        /// </summary>
        /// <param name="path">A file of directory</param>
        /// <remarks>If the file is in the cache</remarks>
        void MarkDirty(string path);

        /// <summary>
        /// Marks the specified paths dirty
        /// </summary>
        /// <param name="paths">The paths.</param>
        void MarkDirty(IEnumerable<string> paths);

        /// <summary>
        /// Clears the whole statuscache; called when closing the solution
        /// </summary>
        void ClearCache();

        /// <summary>
        /// Called from <see cref="SvnItem.Refresh()"/>
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="diskNodeKind">The on-disk node kind if it is known to be correct.</param>
        void RefreshItem(SvnItem item, SvnNodeKind diskNodeKind);

        /// <summary>
        /// Refreshes the nested status of the <see cref="SvnItem"/>
        /// </summary>
        /// <param name="item"></param>
        void RefreshNested(SvnItem item);

        /// <summary>
        /// Gets the <see cref="SvnDirectory"/> of the specified path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        SvnDirectory GetDirectory(string path);

        void MarkDirtyRecursive(string path);

        IList<SvnItem> GetCachedBelow(string path);
        IList<SvnItem> GetCachedBelow(IEnumerable<string> paths);

        void SetSolutionContained(string path, bool contained);
    }
}
