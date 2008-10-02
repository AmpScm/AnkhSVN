using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.VS
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAnkhSolutionSettings
    {
        /// <summary>
        /// Gets or sets the full path of the solution root including a final '\'
        /// </summary>
        /// <remarks>The project root is stored as a relative path from the solution file</remarks>
        string ProjectRootWithSeparator { get; }

        /// <summary>
        /// Gets or sets the project root. (As a normalized path)
        /// </summary>
        /// <value>The project root.</value>
        string ProjectRoot { get; set; }

        /// <summary>
        /// Gets the solution filename.
        /// </summary>
        /// <value>The solution filename.</value>
        string SolutionFilename { get; }

        /// <summary>
        /// Default path where VS stores its new projects
        /// </summary>
        string NewProjectLocation { get; }


        Version VisualStudioVersion { get; }

        /// <summary>
        /// Gets the project root URI.
        /// </summary>
        /// <value>The project root URI.</value>
        Uri ProjectRootUri { get; }


        /// <summary>
        /// Gets all project extensions filter.
        /// </summary>
        /// <value>All project extensions filter.</value>
        string AllProjectExtensionsFilter { get; }

        /// <summary>
        /// Gets the name of the open project filter.
        /// </summary>
        /// <value>The name of the open project filter.</value>
        string OpenProjectFilterName { get; }


        /// <summary>
        /// Gets the open file filter.
        /// </summary>
        /// <value>The open file filter.</value>
        string OpenFileFilter { get; }

        /// <summary>
        /// Gets a value indicating whether [in ranu mode].
        /// </summary>
        /// <value><c>true</c> if [in ranu mode]; otherwise, <c>false</c>.</value>
        bool InRanuMode { get; }
        /// <summary>
        /// Gets the visual studio registry root.
        /// </summary>
        /// <value>The visual studio registry root.</value>
        string VisualStudioRegistryRoot { get; }
        /// <summary>
        /// Gets the visual studio user registry root.
        /// </summary>
        /// <value>The visual studio user registry root.</value>
        string VisualStudioUserRegistryRoot { get; }
        /// <summary>
        /// Gets the registry hive suffix.
        /// </summary>
        /// <value>The registry hive suffix.</value>
        string RegistryHiveSuffix { get; }


        /// <summary>
        /// Gets the solution filter.
        /// </summary>
        /// <value>The solution filter.</value>
        string SolutionFilter { get; }

        /// <summary>
        /// Gets a list of Uris for url dropdowns
        /// </summary>
        /// <param name="forBrowse">if set to <c>true</c> [for browse].</param>
        /// <returns></returns>
        IEnumerable<Uri> GetRepositoryUris(bool forBrowse);
    }
}
