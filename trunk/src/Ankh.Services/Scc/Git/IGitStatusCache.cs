using System;
using System.Collections.Generic;
using Ankh.Scc.Engine;

namespace Ankh.Scc
{
    public interface IGitStatusCache : ISccStatusCache<GitItem>
    {
        /// <summary>
        /// Updates the in-solution and Scc excluded state
        /// </summary>
        /// <param name="path"></param>
        /// <param name="inSolution"></param>
        /// <param name="sccExcluded"></param>
        void SetSolutionContained(string path, bool inSolution, bool sccExcluded);

        void RefreshItem(GitItem gitItem, SharpSvn.SvnNodeKind svnNodeKind);
    }
}
