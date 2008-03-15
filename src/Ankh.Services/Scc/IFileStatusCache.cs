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

        void UpdateStatus(string path, SvnDepth depth);

        /// <summary>
        /// Clears the whole statuscache; called when closing the solution
        /// </summary>
        void ClearCache();
    }
}
