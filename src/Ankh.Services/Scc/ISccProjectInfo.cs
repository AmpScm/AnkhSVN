// Copyright 2008-2009 The AnkhSVN Project
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

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISccProjectInfo
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

        /// <summary>
        /// Gets the SCC base directory.
        /// </summary>
        /// <value>The SCC base directory.</value>
        /// <remarks>The SCC base directory is the project directory or one of its parents</remarks>
        string SccBaseDirectory { get; set;  }

        /// <summary>
        /// Gets or sets the SCC base URI.
        /// </summary>
        /// <value>The SCC base URI.</value>
        Uri SccBaseUri { get; set; }

        /// <summary>
        /// Gets a boolean indicating whether the project support binding to SCC
        /// </summary>
        bool IsSccBindable { get; }
    }
}
