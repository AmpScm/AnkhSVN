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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using SharpSvn;

using Ankh.Commands;
using Ankh.Selection;
using Ankh.UI;
using Ankh.Scc.Engine;

namespace Ankh.Scc
{
    [GlobalService(typeof(IFileStatusMonitor))]
    sealed class ProjectNotifier : AnkhService, IFileStatusMonitor, IVsBroadcastMessageEvents
    {
        readonly object _lock = new object();
        bool _posted;
        bool _onIdle;
        List<SccProject> _dirtyProjects;
        HybridCollection<string> _maybeAdd;
        uint _cookie;

        public ProjectNotifier(IAnkhServiceProvider context)
            : base(context)
        {
            uint cookie;
            if (VSErr.Succeeded(Shell.AdviseBroadcastMessages(this, out cookie)))
                _cookie = cookie;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_cookie != 0)
                {
                    uint cookie = _cookie;
                    _cookie = 0;
                    Shell.UnadviseBroadcastMessages(cookie);
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        IVsShell _shell;
        IVsShell Shell
        {
            get { return _shell ?? (_shell = GetService<IVsShell>(typeof(SVsShell))); }
        }

        IAnkhCommandService _commandService;
        /// <summary>
        /// Gets the command service.
        /// </summary>
        /// <value>The command service.</value>
        IAnkhCommandService CommandService
        {
            [DebuggerStepThrough]
            get { return _commandService ?? (_commandService = GetService<IAnkhCommandService>()); }
        }

        ISccStatusCache _statusCache;
        ISccStatusCache Cache
        {
            [DebuggerStepThrough]
            get { return _statusCache ?? (_statusCache = GetService<ISvnStatusCache>()); }
        }

        ISvnStatusCache _svnCache;
        ISvnStatusCache SvnCache
        {
            get { return _svnCache ?? (_svnCache = GetService<ISvnStatusCache>()); }
        }

        IProjectFileMapper _mapper;
        IProjectFileMapper Mapper
        {
            [DebuggerStepThrough]
            get { return _mapper ?? (_mapper = GetService<IProjectFileMapper>()); }
        }

        PendingChangeManager _changeManager;
        PendingChangeManager ChangeManager
        {
            [DebuggerStepThrough]
            get { return _changeManager ?? (_changeManager = GetService<PendingChangeManager>(typeof(IPendingChangesManager))); }
        }

        IAnkhOpenDocumentTracker _tracker;
        IAnkhOpenDocumentTracker DocumentTracker
        {
            [DebuggerStepThrough]
            get { return _tracker ?? (_tracker = GetService<IAnkhOpenDocumentTracker>()); }
        }

        ISelectionContextEx _selection;
        ISelectionContextEx Selection
        {
            [DebuggerStepThrough]
            get { return _selection ?? (_selection = GetService<ISelectionContextEx>(typeof(ISelectionContext))); }
        }

        SvnSccProvider _sccProvider;
        SvnSccProvider SccProvider
        {
            [DebuggerStepThrough]
            get { return _sccProvider ?? (_sccProvider = GetService<SvnSccProvider>(typeof(IAnkhSccService))); }
        }

        void PostDirty(bool checkDelay)
        {
            if (!_posted)
            {
                if (checkDelay)
                    Selection.MaybeInstallDelayHandler();

                CommandService.PostTickCommand(ref _posted, AnkhCommand.MarkProjectDirty);
            }
        }

        void PostIdle()
        {
            if (!_onIdle)
            {
                CommandService.PostIdleCommand(AnkhCommand.MarkProjectDirty);
                _onIdle = true;
            }
        }

        /// <summary>
        /// Schedules a glyph refresh of all specified projects
        /// </summary>
        /// <param name="projects"></param>
        public void ScheduleGlyphOnlyUpdate(IEnumerable<SccProject> projects)
        {
            if (projects == null)
                throw new ArgumentNullException("projects");

            lock (_lock)
            {
                if (_dirtyProjects == null)
                    _dirtyProjects = new List<SccProject>();

                foreach (SccProject project in projects)
                {
                    if (!_dirtyProjects.Contains(project))
                        _dirtyProjects.Add(project);
                }

                PostDirty(false);
            }
        }


        public void ScheduleAddFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            lock (_lock)
            {
                if (_maybeAdd == null)
                    _maybeAdd = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

                if (!_maybeAdd.Contains(path))
                    _maybeAdd.Add(path);

                PostDirty(true);
            }
        }

        HybridCollection<string> _dirtyCheck = null;

        /// <summary>
        /// Schedules a dirty check for the specified document
        /// </summary>
        /// <param name="item">The item.</param>
        public void ScheduleDirtyCheck(SvnItem item)
        {
            if (item == null)
                throw new ArgumentNullException("path");

            if (!item.IsVersioned || item.IsModified || DocumentTracker.NoDirtyCheck(item))
                return; // Not needed

            lock (_lock)
            {
                if (_dirtyCheck == null)
                    _dirtyCheck = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);

                if (!_dirtyCheck.Contains(item.FullPath))
                    _dirtyCheck.Add(item.FullPath);

                PostIdle();
            }
        }

        internal void HandleEvent(AnkhCommand command)
        {
            List<SccProject> dirtyProjects;
            HybridCollection<string> dirtyCheck;
            HybridCollection<string> maybeAdd;

            SvnSccProvider provider = GetService<SvnSccProvider>();

            lock (_lock)
            {
                _posted = false;
                _onIdle = false;

                if (provider == null)
                    return;

                dirtyProjects = _dirtyProjects;
                dirtyCheck = _dirtyCheck;
                maybeAdd = _maybeAdd;
                _dirtyProjects = null;
                _dirtyCheck = null;
                _maybeAdd = null;
            }

            if (dirtyCheck != null)
                foreach (string file in dirtyCheck)
                {
                    DocumentTracker.CheckDirty(file);
                }

            if (dirtyProjects != null)
            {
                foreach (SccProject project in dirtyProjects)
                {
                    if (project.IsSolution)
                        provider.UpdateSolutionGlyph();
                    else
                        project.NotifyGlyphChanged();
                }
            }

            if (maybeAdd != null)
            {
                using (SvnClient cl = GetService<ISvnClientPool>().GetNoUIClient())
                {
                    foreach (string file in maybeAdd)
                    {
                        SvnItem item = SvnCache[file];
                        // Only add
                        // * files
                        // * that are unversioned
                        // * that are addable
                        // * that are not ignored
                        // * and just to be sure: that are still part of the solution
                        if (item.IsFile && !item.IsVersioned &&
                            item.IsVersionable && !item.IsIgnored &&
                            item.InSolution && !item.IsSccExcluded)
                        {
                            SvnAddArgs aa = new SvnAddArgs();
                            aa.ThrowOnError = false; // Just ignore errors here; make the user add them themselves
                            aa.AddParents = true;

                            if (cl.Add(item.FullPath, aa))
                            {
                                item.MarkDirty();

                                // Detect if we have a file that Subversion might detect as binary
                                if (item.IsVersioned && !item.IsTextFile)
                                {
                                    // Only check small files, avoid checking big binary files
                                    FileInfo fi = new FileInfo(item.FullPath);
                                    if (fi.Length < 10)
                                    {
                                        // We're sure it's at most 10 bytes here, so just read all
                                        byte[] fileBytes = File.ReadAllBytes(item.FullPath);

                                        // If the file starts with a UTF8 BOM, we're sure enough it's a text file, keep UTF16 & 32 binary
                                        if (StartsWith(fileBytes, new byte[] { 0xEF, 0xBB, 0xBF }))
                                        {
                                            // Delete the mime type property, so it's detected as a text file again
                                            SvnSetPropertyArgs pa = new SvnSetPropertyArgs();
                                            pa.ThrowOnError = false;
                                            cl.DeleteProperty(item.FullPath, SvnPropertyNames.SvnMimeType, pa);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        static bool StartsWith(byte[] haystack, byte[] needle)
        {
            if (haystack == null)
                return false;
            if (needle == null)
                return false;
            if (needle.Length > haystack.Length)
                return false;

            for (int i = 0; i < needle.Length; i++)
            {
                if (haystack[i] != needle[i])
                    return false;
            }

            return true;
        }

        public void ScheduleSvnStatus(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Cache.MarkDirty(path);

            ScheduleGlyphUpdate(path);
        }

        public void ScheduleSvnStatus(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            Cache.MarkDirty(paths);

            ScheduleGlyphUpdate(paths);
        }

        public void HandleSvnResult(IDictionary<string, SvnClientAction> actions)
        {
            List<SvnClientAction> sccRefreshItems = null;
            ScheduleMonitor(actions.Keys);

            ScheduleSvnStatus(actions.Keys);

            foreach (SvnClientAction action in actions.Values)
            {
                if (action.Recursive)
                    ScheduleGlyphUpdate(Cache.GetCachedBelow(action.FullPath));

                if (action.AddOrRemove)
                {
                    if(sccRefreshItems == null)
                        sccRefreshItems = new List<SvnClientAction>();

                    sccRefreshItems.Add(action);
                }
            }

            if (sccRefreshItems != null)
                SccProvider.ScheduleSvnRefresh(sccRefreshItems);
        }

        public void ScheduleGlyphUpdate(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            ScheduleGlyphOnlyUpdate(Mapper.GetAllProjectsContaining(path));
            ChangeManager.Refresh(path);
        }

        public void ScheduleGlyphUpdate(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            ScheduleGlyphOnlyUpdate(Mapper.GetAllProjectsContaining(paths));
            ChangeManager.Refresh(paths);
        }

        public void ScheduleMonitor(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            ChangeManager.ScheduleMonitor(path);
        }

        public void ScheduleMonitor(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            ChangeManager.ScheduleMonitor(paths);
        }

        public void StopMonitoring(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            ChangeManager.StopMonitor(path);
        }

        readonly Dictionary<string, DocumentLock> _externallyChanged = new Dictionary<string, DocumentLock>(StringComparer.OrdinalIgnoreCase);

        public void ExternallyChanged(string path, out bool isDirty)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            isDirty = false;

            if (DocumentTracker.NoReloadNecessary(path))
                return;

            lock (_externallyChanged)
            {
                if (DocumentTracker.IsDocumentOpenInTextEditor(path))
                {
                    if (!DocumentTracker.IsDocumentDirty(path, true))
                    {
                        if (!_externallyChanged.ContainsKey(path))
                            _externallyChanged[path] = DocumentTracker.LockDocument(path, DocumentLockType.ReadOnly);
                    }
                    else
                    {
                        DocumentLock dl;
                        if (_externallyChanged.TryGetValue(path, out dl))
                        {
                            _externallyChanged.Remove(path);
                            dl.Dispose();
                        }
                        isDirty = true; // Unhook external change handling: Make VS ask user
                    }
                }
            }
        }

        private void ReleaseExternalWrites()
        {
            Dictionary<string, DocumentLock> modified;
            lock (_externallyChanged)
            {
                if (_externallyChanged.Count == 0)
                    return;

                modified = new Dictionary<string, DocumentLock>(_externallyChanged, StringComparer.OrdinalIgnoreCase);
                _externallyChanged.Clear();
            }

            try
            {
                ScheduleSvnStatus(modified.Keys);
                foreach (KeyValuePair<string, DocumentLock> file in modified)
                {
                    SvnItem item = SvnCache[file.Key];

                    if (item.IsConflicted)
                    {
                        AnkhMessageBox mb = new AnkhMessageBox(Context);

                        DialogResult dr = mb.Show(string.Format(Resources.YourMergeToolSavedXWouldYouLikeItMarkedAsResolved, file.Key),
                            Resources.MergeCompleted, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

                        switch (dr)
                        {
                            case DialogResult.Yes:
                                using (SvnClient c = Context.GetService<ISvnClientPool>().GetNoUIClient())
                                {
                                    SvnResolveArgs ra = new SvnResolveArgs();
                                    ra.ThrowOnError = false;

                                    c.Resolve(file.Key, SvnAccept.Merged, ra);
                                }
                                goto case DialogResult.No;
                            case DialogResult.No:
                                if (!item.IsModified)
                                {
                                    // Reload?
                                }
                                break;
                            default:
                                // Let VS handle the file
                                return; // No reload
                        }
                    }

                    if (!item.IsDocumentDirty)
                    {
                        if (file.Value != null)
                            file.Value.Reload(file.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = GetService<IAnkhErrorHandler>();

                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
            finally
            {
                foreach (DocumentLock dl in modified.Values)
                {
                    if (dl != null)
                        dl.Dispose();
                }
            }
        }

        #region IVsBroadcastMessageEvents Members

        const uint WM_ACTIVATE = 0x0006;
        const uint WM_ACTIVATEAPP = 0x001C;
        const uint WM_SYSCOLORCHANGE = 0x0015;
        const uint WM_THEMECHANGED = 0x031A;

        int IVsBroadcastMessageEvents.OnBroadcastMessage(uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_ACTIVATEAPP:
                    if (wParam != IntPtr.Zero)
                        ReleaseExternalWrites();
                    break;
                case VSConstants.VSM_ENTERMODAL:
                case VSConstants.VSM_EXITMODAL:
                case VSConstants.VSM_TOOLBARMETRICSCHANGE:
                    break;

                case WM_THEMECHANGED:
                case WM_SYSCOLORCHANGE:
                    {
                        IAnkhServiceEvents services = GetService<IAnkhServiceEvents>();

                        if (services != null && VSVersion.VS2012OrLater)
                            services.OnThemeChanged(EventArgs.Empty);
                    }
                    break;
            }
            return VSErr.S_OK;
        }

        #endregion

        public void SetDocumentDirty(string FullPath, bool dirty)
        {
            SvnItem item = SvnCache[FullPath];

            if (item == null)
                return;

            ISvnItemStateUpdate sisu = item;
            sisu.SetDocumentDirty(dirty);

            if (item.IsModified)
                return; // No need to update glyph!

            ScheduleGlyphUpdate(FullPath);
        }
    }
}
