// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
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
        public abstract IDisposable MonitorChangesForReload();
    }

    public enum DocumentLockType
    {
        NoReload,
        ReadOnly,
    }

    public interface IAnkhOpenDocumentTracker
    {
        bool IsDocumentOpen(string path);
        bool IsDocumentDirty(string path, bool poll);

        bool SaveDocument(string path);
        bool PromptSaveDocument(string path);
        bool SaveDocuments(IEnumerable<string> paths);

        ICollection<string> GetDocumentsBelow(string path);

        void CheckDirty(string path);

        /// <summary>
        /// Locks the specified documents for processing until the lock is disposed
        /// </summary>
        /// <param name="paths">The paths to lock. If a path ends with a '\' all files below that path will be locked</param>
        /// <param name="lockType"></param>
        /// <returns></returns>
        DocumentLock LockDocuments(IEnumerable<string> paths, DocumentLockType lockType);

        /// <summary>
        /// Locks the specified documents for processing until the lock is disposed
        /// </summary>
        /// <param name="path">The paths to lock. If a path ends with a '\' all files below that path will be locked</param>
        /// <param name="lockType"></param>
        /// <returns></returns>
        DocumentLock LockDocument(string path, DocumentLockType lockType);

        /// <summary>
        /// Marks the specified path dirty
        /// </summary>
        /// <param name="path">The project file.</param>
        void ForceDirty(string path);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ignore"></param>
        /// <returns></returns>
        bool IgnoreChanges(string path, bool ignore);
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

        /// <summary>
        /// Gets a boolean indicating whether this document is just saved by the VS Editor (and not some
        /// outside change)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        bool NoReloadNecessary(string path);

        /// <summary>
        /// Provide API to avoid dirty scheduling
        /// </summary>
        bool NoDirtyCheck(SvnItem item);

        void DoDispose(ProjectMap.SccDocumentData sccDocumentData);
    }
}
