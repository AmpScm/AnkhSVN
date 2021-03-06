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
using Ankh.Scc.ProjectMap;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Ankh.Scc
{
    partial class OpenDocumentTracker
    {
        public bool IsDocumentOpen(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return false;
            else
                return true;
        }

        public bool IsDocumentOpenInTextEditor(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return false;
            else
            {
                object o = data.RawDocument;

                bool isEditor = (o is IVsTextBuffer);

                return isEditor;
            }
        }

        public bool IsDocumentDirty(string path, bool poll)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return false;

            if (poll)
                data.CheckDirty(_poller);

            return data.IsDirty;
        }

        public bool PromptSaveDocument(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return false;

            data.CheckDirty(_poller);

            if (!data.IsDirty || (data.Cookie == 0))
                return true; // Not/never modified, no need to save

            // Save the document if it is dirty
            return VSErr.Succeeded(RunningDocumentTable.SaveDocuments((uint)__VSRDTSAVEOPTIONS.RDTSAVEOPT_PromptSave,
                data.Hierarchy, data.ItemId, data.Cookie));
        }

        public bool SaveDocument(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return true;

            data.CheckDirty(_poller);

            // Save the document if it is dirty
            return data.SaveDocument(RunningDocumentTable);
        }

        public void CheckDirty(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return;

            data.CheckDirty(_poller);
        }

        public bool SaveDocuments(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            bool ok = true;

            foreach (string path in paths)
            {
                if (!SaveDocument(path))
                    ok = false;
            }

            return ok;
        }

        public bool SaveAllDocumentsExcept(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            HybridCollection<string> openDocs = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            HybridCollection<string> dontSave = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

            openDocs.UniqueAddRange(_docMap.Keys);
            dontSave.UniqueAddRange(paths);

            bool ok = true;
            foreach(string name in openDocs)
            {
                SccDocumentData data;

                if (dontSave.Contains(name))
                    continue;

                if (!_docMap.TryGetValue(name, out data))
                    continue; // File closed through saving another one

                if (!data.SaveDocument(RunningDocumentTable))
                    ok = false;
            }

            return ok;
        }

        void SaveAllDocuments()
        {
            foreach (SccDocumentData dd in _docMap.Values)
            {
                dd.SaveDocument(RunningDocumentTable);
            }
        }

        /// <summary>
        /// Marks the specified path dirty
        /// </summary>
        /// <param name="ProjectFile">The project file.</param>
        /// <param name="sure">if sure set to <c>true</c>.. <c>false</c> if the editory should be queried.</param>
        public void ForceDirty(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (_docMap.TryGetValue(path, out data))
            {
                data.SetDirty(true);
            }
        }

        public ICollection<string> GetDocumentsBelow(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            path = SvnTools.GetNormalizedFullPath(path);

            List<string> files = new List<string>();
            SccDocumentData dd;
            if (_docMap.TryGetValue(path, out dd))
            {
                files.Add(path);
            }

            if (!path.EndsWith(Path.DirectorySeparatorChar.ToString()))
                path += Path.DirectorySeparatorChar;

            foreach (SccDocumentData d in _docMap.Values)
            {
                string docPath = d.FullPath;
                if (docPath == null)
                    continue; // Not file based

                if (SvnItem.IsBelowRoot(docPath, path))
                    files.Add(SvnTools.GetNormalizedFullPath(d.Name));
            }

            return files.ToArray();
        }

        public DocumentLock LockDocument(string path, DocumentLockType lockType)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            return LockDocuments(new string[] { path }, lockType);
        }

        public DocumentLock LockDocuments(IEnumerable<string> paths, DocumentLockType lockType)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            HybridCollection<string> locked = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            HybridCollection<string> ignoring = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            HybridCollection<string> readOnly = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string path in paths)
            {
                SccDocumentData dd;
                if (_docMap.TryGetValue(path, out dd))
                {
                    if (!locked.Contains(path))
                        locked.Add(path);

                    if (!ignoring.Contains(path) && dd.IgnoreFileChanges(true))
                        ignoring.Add(path);

                    if (lockType >= DocumentLockType.ReadOnly && !readOnly.Contains(path) && !dd.IsReadOnly())
                    {
                        // Don't set read-only twice!!!
                        if (dd.SetReadOnly(true))
                            readOnly.Add(path);
                    }
                }
            }
            return new SccDocumentLock(this, locked, ignoring, readOnly);
        }

        class SccDocumentLock : DocumentLock, IVsFileChangeEvents
        {
            readonly OpenDocumentTracker _tracker;
            readonly HybridCollection<string> _locked, _ignoring, _readonly;
            readonly HybridCollection<string> _fsIgnored;
            readonly Dictionary<uint, string> _monitor;
            readonly Dictionary<string, FileInfo> _altMonitor;
            readonly HybridCollection<string> _changedPaths;
            readonly IVsFileChangeEx _change;

            public SccDocumentLock(OpenDocumentTracker tracker, HybridCollection<string> locked, HybridCollection<string> ignoring, HybridCollection<string> readOnly)
            {
                if (tracker == null)
                    throw new ArgumentNullException("tracker");
                else if (locked == null)
                    throw new ArgumentNullException("locked");
                else if (ignoring == null)
                    throw new ArgumentNullException("ignoring");
                else if (readOnly == null)
                    throw new ArgumentNullException("readOnly");

                _tracker = tracker;
                _locked = locked;
                _ignoring = ignoring;
                _readonly = readOnly;
                _fsIgnored = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                _changedPaths = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                _monitor = new Dictionary<uint, string>();
                _altMonitor = new Dictionary<string, FileInfo>();

                _change = tracker.GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));

                foreach (string file in locked)
                {
                    // This files auto reload could not be suspended by calling Ignore on the document
                    // We must therefore stop posting messages to it by stopping it in the change monitor

                    // But to be able to tell if there are changes.. We keep some stats ourselves

                    if (!ignoring.Contains(file) &&
                        VSErr.Succeeded(_change.IgnoreFile(0, file, 1)))
                    {
                        _fsIgnored.Add(file);
                        FileInfo info = new FileInfo(file);
                        info.Refresh();
                        if (info.Exists)
                        {
                            GC.KeepAlive(info.LastWriteTime);
                            GC.KeepAlive(info.CreationTime);
                            GC.KeepAlive(info.Length);
                        }
                        _altMonitor.Add(file, info);
                    }
                }
            }

            public override IDisposable MonitorChangesForReload()
            {
                if (_monitor.Count > 0)
                    return null;

                foreach (string path in _ignoring)
                {
                    uint cky;

                    // BH: We don't monitor the attributes as some SVN actions put files temporary on read only!
                    if (VSErr.Succeeded(_change.AdviseFileChange(path, (uint)(/*_VSFILECHANGEFLAGS.VSFILECHG_Attr |*/
                        _VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time), this, out cky)))
                    {
                        _monitor.Add(cky, path);
                    }
                }

                return new ReloadModifiedDisposer(this);
            }

            sealed class ReloadModifiedDisposer : IDisposable
            {
                readonly SccDocumentLock _lck;

                public ReloadModifiedDisposer(SccDocumentLock lck)
                {
                    if (lck == null)
                        throw new ArgumentNullException("lck");

                    _lck = lck;
                }

                public void Dispose()
                {
                    _lck.ReloadModified();
                }
            }

            internal void ReloadModified()
            {
                if (_monitor == null || _change == null)
                    return;

                foreach (string file in _monitor.Values)
                {
                    _change.SyncFile(file);
                }

                foreach (KeyValuePair<string, FileInfo> item in new List<KeyValuePair<string, FileInfo>>(_altMonitor))
                {
                    string file = item.Key;
                    FileInfo from = item.Value;
                    FileInfo to = new FileInfo(file);

                    if (from.Exists && to.Exists &&
                        ((from.LastWriteTime != to.LastWriteTime) || (from.Length != to.Length) ||
                        (from.CreationTime != to.CreationTime)))
                    {
                        if (!_changedPaths.Contains(file))
                        {
                            _changedPaths.Add(file);
                            _altMonitor[file] = to;
                        }
                    }
                }

                List<string> changed = new List<string>(_changedPaths);
                _changedPaths.Clear();

                Reload(changed);
            }

            #region IVsFileChangeEvents
            // Called by the file monitor when a monitored directory has changed
            int IVsFileChangeEvents.DirectoryChanged(string pszDirectory)
            {
                if (string.IsNullOrEmpty(pszDirectory))
                    return VSErr.S_OK;

                if (!_changedPaths.Contains(pszDirectory))
                    _changedPaths.Add(pszDirectory);

                return VSErr.S_OK;
            }

            // Called by the file monitor when a monitored file has changed
            int IVsFileChangeEvents.FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
            {
                if (cChanges == 0 || rgpszFile == null)
                    return VSErr.S_OK;

                for (int i = 0; i < cChanges && i < rgpszFile.Length; i++)
                {
                    string file = rgpszFile[i];
                    if (string.IsNullOrEmpty(file))
                        continue;

                    if (!_changedPaths.Contains(file))
                        _changedPaths.Add(file);
                }

                return VSErr.S_OK;
            }
            #endregion

            public override void Reload(IEnumerable<string> paths)
            {
                if (paths == null)
                    throw new ArgumentNullException("paths");

                StopMonitor(); // Make sure we have no further locks while reloading!

                HybridCollection<string> changed = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
                changed.AddRange(paths);

                IProjectFileMapper mapper = _tracker.GetService<IProjectFileMapper>();

                if (!string.IsNullOrEmpty(mapper.SolutionFilename) && changed.Contains(mapper.SolutionFilename))
                {
                    // Ok; we are going to reload the solution itself
                    _tracker.SaveAllDocumentsExcept(changed); // Make sure everything that is dirty is saved

                    // let's remove all documents that are in the solution from the changed list
                    foreach (string file in mapper.GetAllFilesOfAllProjects())
                    {
                        changed.Remove(file);
                    }

                    // The solution was just removed; add it back
                    changed.Add(mapper.SolutionFilename);
                }

                for (int i = 0; i < changed.Count; i++)
                {
                    string ch = changed[i];
                    SccDocumentData dd;
                    if (_tracker._docMap.TryGetValue(ch, out dd))
                    {
                        if (!dd.Reload(true))
                        {
                            string parentDocument = _tracker.GetParentDocument(dd);

                            if (string.IsNullOrEmpty(parentDocument))
                                parentDocument = mapper.SolutionFilename;

                            if (!string.IsNullOrEmpty(parentDocument) && !changed.Contains(parentDocument))
                            {
                                if (!_locked.Contains(parentDocument))
                                {
                                    // The parent is not on our changed or locked list.. so make sure it is saved
                                    _tracker.SaveDocument(parentDocument);
                                }

                                changed.Add(parentDocument);
                            }
                        }
                    }
                }
            }

            public override void Dispose()
            {
                StopMonitor();
            }

            void StopMonitor()
            {
                // Stop monitoring
                foreach (uint v in _monitor.Keys)
                    _change.UnadviseFileChange(v);

                _monitor.Clear();

                foreach (string path in _fsIgnored)
                {
                    _change.SyncFile(path);
                    _change.IgnoreFile(0, path, 0);
                }

                _fsIgnored.Clear();

                // Sync all files for the last time
                // to make sure they are not reloaded for old changes after disposing
                foreach (string path in _locked)
                    _change.SyncFile(path);

                _locked.Clear();

                foreach (string path in _readonly)
                {
                    SccDocumentData dd;
                    if (_tracker._docMap.TryGetValue(path, out dd))
                    {
                        dd.SetReadOnly(false);
                    }
                }
                _readonly.Clear();

                foreach (string path in _ignoring)
                {
                    SccDocumentData dd;
                    if (_tracker._docMap.TryGetValue(path, out dd))
                    {
                        dd.IgnoreFileChanges(false);
                    }
                }

                _ignoring.Clear();
            }
        }

        /// <summary>
        /// Gets the parent document of a document (normally a project or the solution)
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        internal string GetParentDocument(SccDocumentData document)
        {
            IVsHierarchy hier = document.Hierarchy;

            foreach (SccDocumentData dd in _docMap.Values)
            {
                IVsHierarchy hh = dd.RawDocument as IVsHierarchy;

                if (hh != null && dd.Hierarchy == hier)
                {
                    return dd.FullPath;
                }
            }

            return null;
        }

        public bool IgnoreChanges(string path, bool ignore)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData dd;
            if (_docMap.TryGetValue(path, out dd))
                return dd.IgnoreFileChanges(ignore);

            return false;
        }

        public void RefreshDirtyState()
        {
            foreach (SccDocumentData data in _docMap.Values)
            {
                data.CheckDirty(_poller);
            }
        }

        public bool NoReloadNecessary(string file)
        {
            if (string.IsNullOrEmpty(file))
                throw new ArgumentNullException("file");

            SccDocumentData dd;
            if (!_docMap.TryGetValue(file, out dd))
                return false;

            if (dd.Saving.HasValue && (DateTime.Now - dd.Saving) < new TimeSpan(0, 0, 2))
            {
                return true;
            }
            return false;
        }

        public bool NoDirtyCheck(SvnItem item)
        {
            SccDocumentData dd;

            if (!_docMap.TryGetValue(item.FullPath, out dd))
                return false;

            return dd.NeedsNoDirtyCheck;
        }

    }
}

