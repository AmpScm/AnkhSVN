// $Id$
//
// Copyright 2004-2008 The AnkhSVN Project
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
using System.Diagnostics;
using System.Collections;
using System.IO;
using SharpSvn;
using Ankh.Selection;

namespace Ankh
{
    // TODO: Re-implement using IVsTaskProvider, IVsTaskProvider2 and IVsTaskProvider3
    [GlobalService(typeof(IAnkhTaskManager))]
    sealed class ConflictManager : AnkhService, IAnkhTaskManager
    {
        /// <summary>
        /// ConflictManager
        /// Class to handle issues related to conflicts
        /// </summary>
        public ConflictManager(IAnkhServiceProvider context)
            : base(context)
        {
        }

        /// <summary>
        ///  Add a Conflict item the the Task List
        /// </summary>
        /// <param name="path">Path to the file containing the conflict.</param>
        public void AddConflictTask(string path)
        {
            // TODO: Handle
        }


        /// <summary>
        ///  Find all the files with conflicts and create conflict items in the task list for them
        /// </summary>
        public void CreateTaskItems()
        {
        }

        /// <summary>
        ///  Find all the tasks for conflicts and delete
        ///          /// </summary>
        public void RemoveAllTaskItems()
        {
        }

        /// <summary>   
        ///  Look through the task items and delete the item
        ///  associated with path
        /// <param name="path">string: Path to file with a conflict</param>   
        /// <returns>void</returns>   
        public void RemoveTaskItem(string path)
        {
        }



        /// <summary>
        ///  Navigate to task list wondow and filter for conflicts
        ///       
        public void NavigateTaskList()
        {
        }

        /// <summary>
        ///  Filter for getting conflicted items from the solution
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool ConflictedFilter(SvnItem item)
        {
            return (item.Status.LocalContentStatus == SvnStatus.Conflicted);
        }


        /// <summary>
        ///  Find the line that the conflict occurs on in the file with a conflict
        /// </summary>
        /// <param name="path">string: Path to file with a conflict</param>
        /// <returns>int: Line number conflict </returns>
        private ArrayList GetConflictLines(string path)
        {
            return new ArrayList();
        }        

        const string ConflictTaskItemCategory = "Conflict";
        const string SvnConflictString = "<<<<<<< .mine";
        const int NotFound = -1;

        #region IAnkhConflictManager Members

        public void OnCloseSolution()
        {
            this.RemoveAllTaskItems();
        }

        #endregion
    }
}
