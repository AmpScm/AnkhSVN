// $Id$

using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc.Engine;
using SharpSvn;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>The default implementation of this service is thread safe</remarks>
    public interface ISvnStatusCache : ISccStatusCache<SvnItem>
    {
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
        void RefreshWCRoot(SvnItem item);

        /// <summary>
        /// Gets the <see cref="SvnDirectory"/> of the specified path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        SvnDirectory GetDirectory(string path);

        void MarkDirtyRecursive(string path);

        bool EnableUpgradeCommand { get; }

        /// <summary>
        /// Updates the in-solution and Scc excluded state
        /// </summary>
        /// <param name="path"></param>
        /// <param name="inSolution"></param>
        /// <param name="sccExcluded"></param>
        void SetSolutionContained(string path, bool inSolution, bool sccExcluded);

        void ResetUpgradeWarning();
    }
}
