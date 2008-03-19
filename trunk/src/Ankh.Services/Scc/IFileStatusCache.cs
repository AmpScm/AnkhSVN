﻿using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>The default implementation of this service is thread safe</remarks>
    public interface IFileStatusCache
    {
        /// <summary>
        /// Gets the <see cref="Ankh.SvnItem"/> with the specified path.
        /// </summary>
        /// <value></value>
        SvnItem this[string path] { get; }


        IEnumerable<SvnItem> GetDeletions(string directory);

        /// <summary>
        /// Updates the status.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="depth">The depth.</param>
        void UpdateStatus(string directory, SvnDepth depth);

        /// <summary>
        /// Marks the specified path dirty
        /// </summary>
        /// <param name="path"></param>
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
    }
}
