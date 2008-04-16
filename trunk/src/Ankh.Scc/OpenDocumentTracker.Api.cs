﻿using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Scc.ProjectMap;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using SharpSvn;
using System.IO;

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

        public bool IsDocumentDirty(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return false;

            return data.IsDirty;
        }

        public bool PromptSaveDocument(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return false;

            data.CheckDirty();

            if (!data.IsDirty || (data.Cookie == 0))
                return true; // Not/never modified, no need to save

            // Save the document if it is dirty
            return ErrorHandler.Succeeded(RunningDocumentTable.SaveDocuments((uint)__VSRDTSAVEOPTIONS.RDTSAVEOPT_PromptSave,
                data.Hierarchy, data.ItemId, data.Cookie));
        }

        public bool SaveDocument(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return true;

            data.CheckDirty();

            // Save the document if it is dirty
            return ErrorHandler.Succeeded(RunningDocumentTable.SaveDocuments(0, data.Hierarchy, data.ItemId, data.Cookie));
        }

        public void CheckDirty(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (!_docMap.TryGetValue(path, out data))
                return;

            data.CheckDirty();
        }

        public bool SaveDocuments(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            foreach (string path in paths)
            {
                if (!SaveDocument(path))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Marks the specified path dirty
        /// </summary>
        /// <param name="ProjectFile">The project file.</param>
        /// <param name="sure">if sure set to <c>true</c>.. <c>false</c> if the editory should be queried.</param>
        public void SetDirty(string path, bool sure)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            SccDocumentData data;

            if (_docMap.TryGetValue(path, out data))
            {
                if (sure)
                    data.SetDirty(true);
                else
                    data.CheckDirty();
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
                if (d.Name.StartsWith(path, StringComparison.OrdinalIgnoreCase) && d.Name.Length > path.Length)
                    files.Add(SvnTools.GetNormalizedFullPath(d.Name));
            }

            return files.ToArray();
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
                _changedPaths = new HybridCollection<string>();
                _monitor = new Dictionary<uint, string>();
                _altMonitor = new Dictionary<string,FileInfo>();

                _change = tracker.GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));

                foreach(string file in locked)
                {
                    // This files updates could not be suspended by calling Ignore on the document
                    // We must therefore stop posting messages to it by stopping it in the monitor

                    if(!ignoring.Contains(file) &&
                        ErrorHandler.Succeeded(_change.IgnoreFile(0, file, 1)))
                    {
                        FileInfo info = new FileInfo(file);
                        info.Refresh();
                        if (info.Exists)
                        {
                            GC.KeepAlive(info.LastWriteTime);
                            GC.KeepAlive(info.Length);
                        }
                        _altMonitor.Add(file, info);
                    }
                }
            }

            public override void MonitorChanges()
            {
                if (_monitor.Count > 0)
                    return;

                foreach (string path in _ignoring)
                {
                    uint cky;

                    // BH: We don't monitor the attributes as some SVN actions put files temporary on read only!
                    if (ErrorHandler.Succeeded(_change.AdviseFileChange(path, (uint)(/*_VSFILECHANGEFLAGS.VSFILECHG_Attr |*/
                        _VSFILECHANGEFLAGS.VSFILECHG_Size | _VSFILECHANGEFLAGS.VSFILECHG_Time), this, out cky)))
                    {
                        _monitor.Add(cky, path);
                    }
                }
            }

            public override void ReloadModified()
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
                        ((from.LastWriteTime != to.LastWriteTime) || (from.Length != to.Length)))
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

                foreach (string path in changed)
                {
                    SccDocumentData dd;
                    if (_tracker._docMap.TryGetValue(path, out dd))
                    {
                        dd.Reload(true);
                    }
                }
            }

            #region IVsFileChangeEvents
            // Called by the file monitor when a monitored directory has changed
            int IVsFileChangeEvents.DirectoryChanged(string pszDirectory)
            {
                if (string.IsNullOrEmpty(pszDirectory))
                    return VSConstants.S_OK;

                if (!_changedPaths.Contains(pszDirectory))
                    _changedPaths.Add(pszDirectory);

                return VSConstants.S_OK;
            }

            // Called by the file monitor when a monitored file has changed
            int IVsFileChangeEvents.FilesChanged(uint cChanges, string[] rgpszFile, uint[] rggrfChange)
            {
                if (cChanges == 0 || rgpszFile == null)
                    return VSConstants.S_OK;

                for (int i = 0; i < cChanges && i < rgpszFile.Length; i++)
                {
                    string file = rgpszFile[i];
                    if (string.IsNullOrEmpty(file))
                        continue;

                    if (!_changedPaths.Contains(file))
                        _changedPaths.Add(file);
                }

                return VSConstants.S_OK;
            }
            #endregion

            public override void Reload(IEnumerable<string> paths)
            {
                if (paths == null)
                    throw new ArgumentNullException("paths");

                foreach (string path in paths)
                {
                    SccDocumentData dd;
                    if (_tracker._docMap.TryGetValue(path, out dd))
                    {
                        if (!dd.GetIsDirty())
                            dd.Reload(true);
                    }
                }
            }

            public override void Dispose()
            {
                // Stop monitoring
                foreach (uint v in _monitor.Keys)
                    _change.UnadviseFileChange(v);

                _monitor.Clear();

                foreach (string path in _locked)
                {
                    _change.SyncFile(path);
                    _change.IgnoreFile(0, path, 0);
                }

                // Sync all files for the last time
                // to make sure they are not reloaded for old changes after disposing

                foreach (string path in _locked)
                    _change.SyncFile(path);

                foreach (string path in _readonly)
                {
                    SccDocumentData dd;
                    if (_tracker._docMap.TryGetValue(path, out dd))
                    {
                        dd.SetReadOnly(false);
                    }
                }

                foreach (string path in _ignoring)
                {
                    SccDocumentData dd;
                    if (_tracker._docMap.TryGetValue(path, out dd))
                    {
                        dd.IgnoreFileChanges(false);
                    }
                }
            }
        }
    }
}

