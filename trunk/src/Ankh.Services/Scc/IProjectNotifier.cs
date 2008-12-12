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
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>The default implementation of this service is thread safe</remarks>
    public interface IProjectNotifier
    {
        /// <summary>
        /// Schedules a glyph refresh of a project
        /// </summary>
        /// <param name="project"></param>
        void MarkDirty(SvnProject project);
        /// <summary>
        /// Schedules a glyph refresh of all specified projects
        /// </summary>
        /// <param name="project"></param>
        void MarkDirty(IEnumerable<SvnProject> projects);

        /// <summary>
        /// Schedules a full data reload and glyph refresh of a project
        /// </summary>
        /// <param name="project"></param>
        /// <remarks>Should only be initiated at the users request or after a known-invalid state</remarks>
        void MarkFullRefresh(SvnProject project);
        /// <summary>
        /// Schedules a full data reload and glyph refresh of all specified projects
        /// </summary>
        /// <param name="project"></param>
        /// <remarks>Should only be initiated at the users request or after a known-invalid state</remarks>
        void MarkFullRefresh(IEnumerable<SvnProject> project);
    }
}
