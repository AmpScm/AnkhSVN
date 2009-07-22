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

namespace Ankh.Selection
{
    public interface IAnkhTaskManager
    {
        /// <summary>
        /// Navigates to the task list.
        /// </summary>
        void NavigateTaskList();

        /// <summary>
        /// Adds the task.
        /// </summary>
        /// <param name="p">The p.</param>
        void AddConflictTask(string path);


        /// <summary>
        /// Called by the scc provider when the solution closes
        /// </summary>
        void OnCloseSolution();
    }
}
