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
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    public enum ProjectWalkDepth
    {
        Empty,
        /// <summary>
        /// The file and its SCC items. (Walks only SCC items)
        /// </summary>
        SpecialFiles,
        /// <summary>
        /// All descendants in the specified hierarchy only (Walks only SCC items)
        /// </summary>
        AllDescendantsInHierarchy,
        AllDescendants
    }

    [CLSCompliant(false)]
    public interface ISccProjectWalker
    {
        /// <summary>
        /// Gets the list of files specified by the hierarchy (IVsSccProject2 or IVsHierarchy)
        /// </summary>
        /// <param name="hierarchy">The hierarchy.</param>
        /// <param name="id">The id.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="map">The map to receive ids or null if not interested.</param>
        /// <returns></returns>
        /// <remarks>The list might contain duplicates if files are included more than once</remarks>
        IEnumerable<string> GetSccFiles(IVsHierarchy hierarchy, uint id, ProjectWalkDepth depth, IDictionary<string, uint> map);

        void SetPrecreatedFilterItem(IVsHierarchy hierarchy, uint id);
    }
}
