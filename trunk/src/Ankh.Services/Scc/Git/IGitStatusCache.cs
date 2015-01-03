using System;
using System.Collections.Generic;
using Ankh.Scc.Engine;

namespace Ankh.Scc
{
    public interface IGitStatusCache : ISccStatusCache<GitItem>
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

        void MarkDirtyRecursive(string path);


        /// <summary>
        /// Updates the in-solution and Scc excluded state
        /// </summary>
        /// <param name="path"></param>
        /// <param name="inSolution"></param>
        /// <param name="sccExcluded"></param>
        void SetSolutionContained(string path, bool inSolution, bool sccExcluded);

        void RefreshItem(GitItem gitItem, SharpSvn.SvnNodeKind svnNodeKind);
    }
}
