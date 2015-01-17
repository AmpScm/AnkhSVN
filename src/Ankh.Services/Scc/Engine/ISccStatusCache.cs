using System;
using System.Collections.Generic;

namespace Ankh.Scc.Engine
{
    public interface ISccStatusCache : IAnkhServiceProvider
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

        void MarkDirtyRecursive(string path);

        /// <summary>
        /// Clears the whole statuscache; called when closing the solution
        /// </summary>
        void ClearCache();

        IEnumerable<string> GetCachedBelow(string path);
        IEnumerable<string> GetCachedBelow(IEnumerable<string> paths);

        SccItem this[string path] { get; }
    }

    public interface ISccStatusCache<T> : ISccStatusCache
        where T : SccItem<T>
    {
        new T this[string path] { get; }

        /// <summary>
        /// Like this[], but without the normalization step
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        T GetAlreadyNormalizedItem(string path);
    }
}
