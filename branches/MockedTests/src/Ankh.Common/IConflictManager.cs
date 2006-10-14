using System;
namespace Ankh
{
    public interface IConflictManager
    {
        void AddTask(string path);
        void CreateTaskItems();
        void NavigateTaskList();
        void RemoveAllTaskItems();
        void RemoveTaskItem(string path);
    }
}
