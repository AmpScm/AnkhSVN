using System;
using EnvDTE;
using System.Diagnostics;
using System.Collections;
using System.IO; 

namespace Ankh
{
    public class ConflictManager
    {           
        
        /// <summary>
        /// ConflictManager
        /// Class to handle issues related to conflicts
        /// </summary>
        public ConflictManager (AnkhContext context)
        {                         
            this.context = context;
        }

        /// <summary>
        ///  Add a Conflict item the the Task List
        /// </summary>
        /// <param name="path">Path to the file containing the conflict.</param>
        public void AddTask(string path)
        {
            // Get the line number for adding to the task so that when the user clicks on 
            // the task it opens the file and goes to the line the conflict text starts on
            int lineNumber = this.GetConflictLine(path);

            // At the task item 
            Window win =  this.context.DTE.Windows.Item(Constants.vsWindowKindTaskList);
            TaskList taskList = (TaskList) win.Object;
            TaskItem taskListItem;
            taskListItem = taskList.TaskItems.Add(ConflictTaskItemCategory, " ", 
                "AnkhSVN: file has a conflict. ", 
                vsTaskPriority.vsTaskPriorityHigh, 
                vsTaskIcon.vsTaskIconUser, true, path, lineNumber, true, true);

            // Get the task event list and add an event for this task list item to it.
            TaskListEvents taskListEvents = (TaskListEvents)  this.context.DTE.Events.get_TaskListEvents(
                "ConflictTaskItemCategory");
            taskListEvents.TaskNavigated +=new 
                _dispTaskListEvents_TaskNavigatedEventHandler(TaskNavigated);

        }


        /// <summary>
        ///  Find all the files with conflicts and create conflict items in the task list for them
        /// </summary>
        public void CreateTaskItems()
        {

            IList conflictItems =  this.context.SolutionExplorer.GetAllResources(true,   new ResourceFilterCallback(ConflictedFilter)); 
            foreach(SvnItem item in conflictItems)
            {
                this.AddTask(item.Path);
            }
        }

        /// <summary>
        ///  Find all the tasks for conflicts and delete
        ///          /// </summary>
        public void RemoveAllTaskItems()
        {
            Window win =  this.context.DTE.Windows.Item(Constants.vsWindowKindTaskList);
            TaskList taskList = (TaskList) win.Object;
            foreach(TaskItem item in taskList.TaskItems)
            {
                if(item.Category == ConflictTaskItemCategory) 
                    item.Delete(); 
            }
        }

        /// <summary>   
        ///  Look through the task items and delete the item
        ///  associated with path
        /// <param name="path">string: Path to file with a conflict</param>   
        /// <returns>void</returns>   
        public void RemoveTaskItem(string path)   
        {   
            Window win =  this.context.DTE.Windows.Item(Constants.vsWindowKindTaskList);
            TaskList taskList = (TaskList) win.Object;

            foreach(TaskItem item in taskList.TaskItems)   
            {   
                if(item.Category == ConflictTaskItemCategory &&   
                    item.FileName == path)   
                {
                    item.Delete();
                    break;
                }
            }   
        } 


        /// <summary>
        ///  Filter for getting conflicted items from the solution
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private  bool ConflictedFilter( SvnItem item )
        {
            return (item.Status.TextStatus == NSvn.Core.StatusKind.Conflicted);
        }


        /// <summary>
        ///  Event handler for when a conflict task item is clicked in VS
        ///       
        private void  TaskNavigated(TaskItem taskItem, ref bool navigateHandled)
        {
            // Debug.WriteLine("A task named '" + taskItem.Description  + "' was navigated to in the Task List.");
            Window win = taskItem.DTE.ItemOperations.OpenFile( taskItem.FileName, Constants.vsViewKindTextView);
            TextSelection sel = (TextSelection) win.DTE.ActiveDocument.Selection; 
            sel.GotoLine(taskItem.Line, true);

            navigateHandled = true;
        }

        /// <summary>
        ///  Find the line that the conflict occurs on in the file with a conflict
        /// </summary>
        /// <param name="path">string: Path to file with a conflict</param>
        /// <returns>int: Line number conflict </returns>
        private int GetConflictLine(string path) 
        {
            int lineNumber = 1; 
            StreamReader sr = null;
            int index = NotFound; 
            try 
            {
                // Create an instance of StreamReader to read from a file.
                // The using statement also closes the StreamReader.
                sr = new StreamReader(path);

                string line;
                // Read and display lines from the file until the end of 
                // the file is reached and not match
                while ((line = sr.ReadLine()) != null && 
                    (index = line.IndexOf(SvnConflictString)) == NotFound)
                    lineNumber++;
                if(index == NotFound) 
                    lineNumber = 0;
            }
            catch (Exception ) 
            {
                lineNumber = 0; 
            }
            sr.Close();
            return lineNumber;            
        }

        private AnkhContext context; 
        private const string ConflictTaskItemCategory = "Conflict"; 
        private const string SvnConflictString = "<<<<<<< .mine"; 
        private const int NotFound = -1;
    }
}
