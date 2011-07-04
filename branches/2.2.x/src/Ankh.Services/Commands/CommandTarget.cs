// $Id$
//
// Copyright 2008 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

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
