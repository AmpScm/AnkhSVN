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
