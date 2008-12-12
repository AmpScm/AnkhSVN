using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public enum CommandTarget
    {
        /// <summary>
        /// No command targets
        /// </summary>
        None=0,
        /// <summary>
        /// The selected files (in the hierarchy)
        /// </summary>
        /// <remarks>When selecting a project this returns its file, not its directory</remarks>
        SelectedFiles,
        /// <summary>
        /// The selected files and everything below (in the hierarchy)
        /// </summary>
        /// <remarks>When selecting a project this returns its file, not its directory</remarks>
        SelectedFilesRecursive,
        /// <summary>
        /// The selected files (in the hierarchy)
        /// </summary>
        /// <remarks>When selecting a project this returns its directory and its file</remarks>
        SelectedPaths,
        /// <summary>
        /// The selected files and everything below (in the hierarchy)
        /// </summary>
        /// <remarks>When selecting a project this returns its directory and its file</remarks>
        SelectedPathsRecursive,

        /// <summary>
        /// Selects only the solution file (regardless of context)
        /// </summary>
        SolutionFileAlways,

        /// <summary>
        /// Selects the solution file when it is selected
        /// </summary>
        SelectedSolutionFile,

        /// <summary>
        /// Selects the project files of the selected projects
        /// </summary>
        /// <remarks>Doesn't retrieve the directory when the project 'file' is a directory</remarks>
        SelectedProjectFiles,

        /// <summary>
        /// Selects the project files of the selected projects (and the projects below)
        /// </summary>
        /// <remarks>Doesn't retrieve the directory when the project 'file' is a directory</remarks>
        SelectedProjectFilesRecursive,

        /// <summary>
        /// Selects the project directories of the selected projects
        /// </summary>
        SelectedProjectDirectories,

        /// <summary>
        /// Select the project directories of the selected projects (and the projects below)
        /// </summary>
        SelectedProjectDirectoriesRecursive,
        /// <summary>
        /// The owner project of the currently selected nodes
        /// </summary>
        SelectedOwnerProjectFiles,
        /// <summary>
        /// The project directory of the currently selected nodes
        /// </summary>
        SelectedOwnerProjectDirectories,
    }
}
