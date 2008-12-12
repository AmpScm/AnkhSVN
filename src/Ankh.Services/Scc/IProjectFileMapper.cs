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
using Ankh.Selection;

namespace Ankh.Scc
{
    public interface IProjectFileMapper
    {
        /// <summary>
        /// Gets an IEnumerable over all projects containing <paramref name="path"/>
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        IEnumerable<SvnProject> GetAllProjectsContaining(string path);
        /// <summary>
        /// Gets an IEnumerable over all projects containing one or more of the specified <paramref name="paths"/>
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        IEnumerable<SvnProject> GetAllProjectsContaining(IEnumerable<string> paths);

        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns></returns>
        IEnumerable<SvnProject> GetAllProjects();

        /// <summary>
        /// Gets a list of all files contained within <paramref name="project"/>
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        IEnumerable<string> GetAllFilesOf(SvnProject project);
        /// <summary>
        /// Gets a list of all files contained within the list of <paramref name="projects"/>
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        IEnumerable<string> GetAllFilesOf(ICollection<SvnProject> projects);

        /// <summary>
        /// Gets all files of all projects.
        /// </summary>
        /// <returns></returns>
        ICollection<string> GetAllFilesOfAllProjects();

        SvnProject ResolveRawProject(SvnProject project);

        /// <summary>
        /// Gets a boolean indicating whether one or more projects (or the solution) contains path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool ContainsPath(string path);

        /// <summary>
        /// Gets the solution path.
        /// </summary>
        /// <value>The solution path.</value>
        string SolutionFilename { get; }

        /// <summary>
        /// Gets a boolean indicating whether the specified path is of a project or the solution
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool IsProjectFileOrSolution(string path);

        /// <summary>
        /// Gets the project info.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns></returns>
        ISvnProjectInfo GetProjectInfo(SvnProject project);

        /// <summary>
        /// Gets the icon of the file in the first project containing the file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        ProjectIconReference GetPathIconHandle(string path);
    }
}
