using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc
{
    public abstract class DocumentLock : IDisposable
    {
        /// <summary>
        /// Reloads the specified files instead of just unlocking them
        /// </summary>
        /// <param name="paths"></param>
        public abstract void Reload(IEnumerable<string> paths);

        /// <summary>
        /// Reloads the specified file instead of just unlocking it
        /// </summary>
        /// <param name="path">The path.</param>
        public void Reload(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Reload(new string[] { path });
        }

        /// <summary>
        /// Releases the locks placed on the paths
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Registers a file listener on all locked paths; allowing selective reload
        /// </summary>
        public abstract void MonitorChanges();

        /// <summary>
        /// Reloads all files modified since MonitorChanges()
        /// </summary>
        public abstract void ReloadModified();
    }

    public enum DocumentLockType
    {
        NoReload,
        ReadOnly,
    }

    public interface IAnkhOpenDocumentTracker
    {
        bool IsDocumentOpen(string path);
        bool IsDocumentDirty(string path);

        bool SaveDocument(string path);
        bool PromptSaveDocument(string path);
        bool SaveDocuments(IEnumerable<string> paths);

        ICollection<string> GetDocumentsBelow(string path);

        void CheckDirty(string path);

        /// <summary>
        /// Locks the specified documents for processing until the lock is disposed
        /// </summary>
        /// <param name="paths">The paths to lock. If a path ends with a '\' all files below that path will be locked</param>
        /// <returns></returns>
        DocumentLock LockDocuments(IEnumerable<string> paths, DocumentLockType lockType);

        /// <summary>
        /// Locks the specified documents for processing until the lock is disposed
        /// </summary>
        /// <param name="paths">The paths to lock. If a path ends with a '\' all files below that path will be locked</param>
        /// <returns></returns>
        DocumentLock LockDocument(string path, DocumentLockType lockType);

        /// <summary>
        /// Marks the specified path dirty
        /// </summary>
        /// <param name="ProjectFile">The project file.</param>
        /// <param name="sure">if sure set to <c>true</c>.. <c>false</c> if the editory should be queried.</param>
        void SetDirty(string path, bool sure);

        /// <summary>
        /// Reloads the specified file if the document is open and not dirty
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="clearUndo">if set to <c>true</c> [clear undo].</param>
        /// <returns></returns>
        bool ReloadIfNotDirty(string file, bool clearUndo);

        /// <summary>
        /// Determines whether the document is open in a standard editor
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if [is document open in text editor] [the specified p]; otherwise, <c>false</c>.
        /// </returns>
        bool IsDocumentOpenInTextEditor(string path);

        /// <summary>
        /// Refreshes all dirty flags
        /// </summary>
        void RefreshDirtyState();
    }
}
