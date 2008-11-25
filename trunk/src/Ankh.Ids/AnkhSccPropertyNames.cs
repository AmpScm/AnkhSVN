using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Ids
{
    public static class AnkhSccPropertyNames
    {
        /// <summary>
        /// When this svn property is specified on the solution file, use this relative path as the
        /// update and checkout root of the solution.
        /// </summary>
        public const string ProjectRoot = "vs:project-root";

        /// <summary>
        /// When this svn property is specified on a solution/project file, treat the relative paths
        /// that are specified on different lines in this property as if they were included in the project
        /// </summary>
        /// <remarks>Files that are already in the project are ignored</remarks>
        public const string ProjectInclude = "vs:scc-include";

        /// <summary>
        /// When this svn property is specified on a solution/project file, treat the relative paths
        /// that are specified on different lines in this property as if they were not included in the project
        /// </summary>
        /// <remarks>Files that are not in the project are ignored</remarks>
        public const string ProjectExclude = "vs:scc-excluded";
    }
}
