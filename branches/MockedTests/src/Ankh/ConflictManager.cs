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
        public ConflictManager (IContext context)
        {                         
            this.context = context;

            // Get the task event list and add an event for this task list item to it.
            // This needs to be kept around so that there is a reference to it and
            // won't get garbage collected because it's in unmanaged code
            this.taskListEvents = (TaskListEvents)  this.context.DTE.Events.get_TaskListEvents(
                ConflictTaskItemCategory);

            // add an event for this task list item to the taskEventList.
            this.taskListEvents.TaskNavigated +=new 
                _dispTaskListEvents_TaskNavigatedEventHandler(TaskNavigated);
        }

        /// <summary>
        ///  Add a Conflict item the the Task List
        /// </summary>
        /// <param name="path">Path to the file containing the conflict.</param>
        public void AddTask(string path)
        {
            // Get the line number for adding to the task so that when the user clicks on 
            // the task it opens the file and goes to the line the conflict text starts on
            ArrayList conflictLines = this.GetConflictLines(path);
            

            Window win =  this.context.DTE.Windows.Item(Constants.vsWindowKindTaskList);
            TaskList taskList = (TaskList) win.Object;

            // add a task item for every conflict in the file
            foreach(int lineNumber in conflictLines)
            {
                if(!TaskExists(path, lineNumber, taskList))
                    taskList.TaskItems.Add(ConflictTaskItemCategory, " ", 
                        "AnkhSVN: file has a conflict. ", 
                        vsTaskPriority.vsTaskPriorityHigh, 
                        vsTaskIcon.vsTaskIconUser, true, path, lineNumber, true, true);
            }
        }


        /// <summary>
        ///  Find all the files with conflicts and create conflict items in the task list for them
        /// </summary>
        public void CreateTaskItems()
        {
            IList conflictItems =  this.context.SolutionExplorer.GetAllResources(new ResourceFilterCallback(ConflictedFilter)); 
            foreach(SvnItem item in conflictItems)
            {
                this.AddTask(item.Path);
            }
            if(conflictItems.Count > 0) 
                this.NavigateTaskList(); 
        }

        /// <summary>
        ///  Find all the tasks for conflicts and delete
        ///          /// </summary>
        public void RemoveAllTaskItems()
        {
            Window win;
            try
            {
                win = this.context.DTE.Windows.Item(Constants.vsWindowKindTaskList);
            }
            catch(ArgumentException)
            {
                // Swallow (this occurs on VS2005 RTM shutdown)
                return;
            }

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
                }
            }   
        }
 
 

        /// <summary>
        ///  Navigate to task list wondow and filter for conflicts
        ///       
        public void  NavigateTaskList()
        {
            //object o = null;
            Window win = this.context.DTE.Windows.Item(Constants.vsWindowKindTaskList);
            win.Activate();
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
        private ArrayList GetConflictLines(string path) 
        {
            ArrayList conflictLines = new ArrayList();
            int lineNumber = 0; 
            int index = NotFound; 
            bool nothingFound = true;
            string line;
            // Create an instance of StreamReader to read from a file.
            // The using statement also closes the StreamReader.
            using (StreamReader sr  = new StreamReader(path))
            {


                // Read and display lines from the file until the end of 
                // the file is reached and not match
                while ((line = sr.ReadLine()) != null)
                {
                    lineNumber++;
                    index = line.IndexOf(SvnConflictString); 
                    if(index != NotFound) 
                    {
                        nothingFound = false;
                        conflictLines.Add(lineNumber);
                    }
                }
            }
            if(nothingFound) 
                conflictLines.Add(0);

            return conflictLines;
        }
        /// <summary>
        ///  Look through the task items and return true is a conflict task item already exists for a file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool TaskExists(String path, int lineNumber, TaskList taskList)
        {
            bool exists = false; 
            foreach(TaskItem item in taskList.TaskItems)
            {
                if(item.Category == ConflictTaskItemCategory &&
                    item.FileName == path && 
                    item.Line == lineNumber)
                {
                    exists = true; 
                    break;
                }
            }
            return exists;
        }



        private IContext context; 
        private const string ConflictTaskItemCategory = "Conflict"; 
        private const string SvnConflictString = "<<<<<<< .mine"; 
        private const int NotFound = -1;
        private TaskListEvents taskListEvents; 
    }
}
