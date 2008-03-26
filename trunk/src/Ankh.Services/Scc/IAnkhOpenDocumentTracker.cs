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
        /// Releases the locks placed on the paths
        /// </summary>
        public abstract void Dispose();
    }

    public interface IAnkhOpenDocumentTracker
    {
        bool IsDocumentOpen(string path);
        bool IsDocumentDirty(string path);

        bool SaveDocument(string path);
        bool PromptSaveDocument(string path);
        bool SaveDocuments(IEnumerable<string> paths);

        /// <summary>
        /// Locks the specified documents for processing until the lock is disposed
        /// </summary>
        /// <param name="paths">The paths to lock. If a path ends with a '\' all files below that path will be locked</param>
        /// <returns></returns>
        DocumentLock LockDocuments(IEnumerable<string> paths);
    }
}
