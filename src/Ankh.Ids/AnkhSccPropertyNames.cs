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

namespace Ankh.Ids
{
    // Disable Missing XML comment warning for this file
#pragma warning disable 1591 

    public static class AnkhSccPropertyNames
    {
        /// <summary>
        /// When this svn property is specified on the solution file, use this relative path as the
        /// update and checkout root of the solution.
        /// </summary>
        public const string ProjectRoot = "vs:project-root";

        /// <summary>
        /// When this svn property is specified on a solution/project file, use the relative paths
        /// that are specified on different lines in this property as if they were included in the project
        /// </summary>
        /// <remarks>Files that are already in the project are ignored</remarks>
        public const string ProjectInclude = "vs:scc-include";

        /// <summary>
        /// When this svn property is specified on a solution/project file, use the relative paths
        /// that are specified on different lines in this property as if they were not included in the project
        /// </summary>
        /// <remarks>Files that are not in the project are ignored</remarks>
        public const string ProjectExclude = "vs:scc-excluded";
    }
}
