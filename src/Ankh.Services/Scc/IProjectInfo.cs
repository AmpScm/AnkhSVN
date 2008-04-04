using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISvnProjectInfo
    {
        /// <summary>
        /// Gets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        string ProjectName { get; }

        /// <summary>
        /// Gets the project file.
        /// </summary>
        /// <value>The project file.</value>
        string ProjectFile { get; }

        /// <summary>
        /// Gets the full name of the project (the project prefixed by the folder it is under)
        /// </summary>
        /// <value>The full name of the project.</value>
        string UniqueProjectName { get; }

        /// <summary>
        /// Gets the project directory.
        /// </summary>
        /// <value>The project directory.</value>
        string ProjectDirectory { get; }
    }
}
