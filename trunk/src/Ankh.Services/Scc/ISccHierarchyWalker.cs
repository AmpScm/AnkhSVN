using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    public enum ProjectWalkDepth
    {
        Empty,
        SpecialFiles,
        AllDescendantsInHierarchy,
        AllDescendants
    }

    [CLSCompliant(false)]
    public interface ISccProjectWalker
    {
        /// <summary>
        /// Gets the list of files specified by the hierarchy (IVsSccProject2 or IVsHierarchy)
        /// </summary>
        /// <param name="hierarchy"></param>
        /// <param name="id"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        /// <remarks>The list might contain duplicates if files are included more than once</remarks>
        IEnumerable<string> GetSccFiles(IVsHierarchy hierarchy, uint id, ProjectWalkDepth depth);

        void SetPrecreatedFilterItem(IVsHierarchy hierarchy, uint id);
    }
}
