using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    public enum ProjectWalkDepth
    {
        Empty,
        /// <summary>
        /// The file and its SCC items. (Walks only SCC items)
        /// </summary>
        SpecialFiles,
        /// <summary>
        /// All descendants in the specified hierarchy only (Walks only SCC items)
        /// </summary>
        AllDescendantsInHierarchy,
        AllDescendants
    }

    [CLSCompliant(false)]
    public interface ISccProjectWalker
    {
        /// <summary>
        /// Gets the list of files specified by the hierarchy (IVsSccProject2 or IVsHierarchy)
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <param name="id">The id.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="map">The map to receive ids or null if not interested.</param>
        /// <returns></returns>
        /// <remarks>The list might contain duplicates if files are included more than once</remarks>
        IEnumerable<string> GetSccFiles(IVsHierarchy hierarchy, uint id, ProjectWalkDepth depth, IDictionary<string, uint> map);

        void SetPrecreatedFilterItem(IVsHierarchy hierarchy, uint id);
    }
}
