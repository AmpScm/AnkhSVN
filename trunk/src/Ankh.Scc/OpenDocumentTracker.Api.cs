using System;
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
                    files.Add(SvnTools.GetNormalizedFullPath(path));
            }

            return files.ToArray();
        }

        public DocumentLock LockDocuments(IEnumerable<string> paths, DocumentLockType lockType)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            HybridCollection<string> ignoring = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            HybridCollection<string> readOnly = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
            foreach (string path in paths)
            {
                SccDocumentData dd;
                if (_docMap.TryGetValue(path, out dd))
                {
                    if (!ignoring.Contains(dd.Name) && dd.IgnoreFileChanges(true))
                        ignoring.Add(dd.Name);

                    if (lockType >= DocumentLockType.ReadOnly && !readOnly.Contains(dd.Name) && !dd.IsReadOnly())
                    {
                        // Don't set read-only twice!!!
                        if (dd.SetReadOnly(true))
                            readOnly.Add(dd.Name);
                    }
                }
            }
            return new SccDocumentLock(this, ignoring, readOnly);
        }

        class SccDocumentLock : DocumentLock, IVsFileChangeEvents
        {
            readonly OpenDocumentTracker _tracker;
            readonly HybridCollection<string> _ignoring, _readonly;
            readonly Dictionary<uint, string> _monitor;
            readonly HybridCollection<string> _changedPaths;
            IVsFileChangeEx _change;

            public SccDocumentLock(OpenDocumentTracker tracker, HybridCollection<string> ignoring, HybridCollection<string> readOnly)
            {
                if (tracker == null)
                    throw new ArgumentNullException("tracker");
                else if (ignoring == null)
                    throw new ArgumentNullException("ignoring");
                else if (readOnly == null)
                    throw new ArgumentNullException("readOnly");

                _tracker = tracker;
                _ignoring = ignoring;
                _readonly = readOnly;
                _changedPaths = new HybridCollection<string>();
                _monitor = new Dictionary<uint, string>();
            }

            public override void MonitorChanges()
            {
                if (_monitor.Count > 0)
                    return;
                _change = _tracker.GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));

                if (_change == null)
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

                List<string> changed = new List<string>(_changedPaths);
                _changedPaths.Clear();

                foreach (string path in changed)
                {
                    SccDocumentData dd;
                    if (_tracker._docMap.TryGetValue(path, out dd))
                    {
                        if (!dd.GetIsDirty())
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
                if (_change == null)
                    _change = _tracker.GetService<IVsFileChangeEx>(typeof(SVsFileChangeEx));

                if (_change != null)
                {
                    // Sync all files for the last time
                    // to make sure they are not reloaded for old changes after disposing

                    foreach (string path in _ignoring)
                        _change.SyncFile(path);

                    foreach (uint v in _monitor.Keys)
                        _change.UnadviseFileChange(v);

                    _monitor.Clear();
                }

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

