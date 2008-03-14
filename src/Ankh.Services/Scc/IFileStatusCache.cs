using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    public interface IFileStatusCache
    {
        /// <summary>
        /// Gets the <see cref="Ankh.SvnItem"/> with the specified path.
        /// </summary>
        /// <value></value>
        SvnItem this[string path] { get; }

        /// <summary>
        /// Clears the whole statuscache; called when closing the solution
        /// </summary>
        void ClearCache();
    }
}
