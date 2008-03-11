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
    }
}
