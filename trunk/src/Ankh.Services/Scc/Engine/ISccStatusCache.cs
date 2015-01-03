using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ankh.Scc.Engine
{
    public interface ISccStatusCache<T> : IAnkhServiceProvider
        where T : SccItem<T>
    {
        T this[string path] { get; }

        IList<T> GetCachedBelow(string path);
        IList<T> GetCachedBelow(IEnumerable<string> paths);

        /// <summary>
        /// Like this[], but without the normalization step
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        T GetAlreadyNormalizedItem(string path);
    }
}
