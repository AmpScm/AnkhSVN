using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    /// <summary>
    /// Provides utility operations on a working copy
    /// </summary>
    public interface IWorkingCopyOperations
    {
        /// <summary>
        /// Determines wether a given path is a working copy.
        /// </summary>
        /// <param name="path">The path to check</param>
        /// <returns><c>True</c> if the path is (is in) a working copy, <c>false</c> otherwise</returns>
        bool IsWorkingCopyPath(string path);

        /// <summary>
        /// Takes a full path and strips off all leading directories that are not
        /// working copies.
        /// </summary>
        /// <param name="path">The path</param>
        /// <returns>The top level working copy.</returns>
        string GetWorkingCopyRootedPath(string path);
    }
}
