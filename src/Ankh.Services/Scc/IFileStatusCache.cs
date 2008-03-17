using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.Scc
{
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
        /// Marks the specified file dirty
        /// </summary>
        /// <param name="file"></param>
        void MarkDirty(string file);

        /// <summary>
        /// Clears the whole statuscache; called when closing the solution
        /// </summary>
        void ClearCache();
    }
}
