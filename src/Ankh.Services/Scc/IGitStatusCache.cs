using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankh.Scc
{
    public interface IGitStatusCache : IAnkhServiceProvider
    {
        /// <summary>
        /// Gets the <see cref="Ankh.GitItem"/> with the specified path.
        /// </summary>
        /// <value></value>
        GitItem this[string path] { get; }

    }
}
