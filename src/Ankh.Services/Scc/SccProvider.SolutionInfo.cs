using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;

namespace Ankh.Scc
{
    partial class SccProvider
    {
        protected void ClearSolutionInfo()
        {
            ProjectMap.ClearSolutionInfo();
        }

        public string SolutionFilename
        {
            get { return ProjectMap.SolutionFilename; }
        }

        public string SolutionDirectory
        {
            get { return ProjectMap.SolutionDirectory; }
        }

        public string RawSolutionDirectory
        {
            get { return ProjectMap.RawSolutionDirectory; }
        }
    }
}
