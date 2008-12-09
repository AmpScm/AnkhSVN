using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

using SharpSvn;

using Ankh.Commands;
using Ankh.Ids;
using Ankh.Selection;
using Ankh.UI;

namespace Ankh.Scc
{
    [GlobalService(typeof(IProjectNotifier))]
    [GlobalService(typeof(IFileStatusMonitor))]
    sealed class ProjectNotifier : AnkhService, IProjectNotifier, IFileStatusMonitor, IVsBroadcastMessageEvents
    {
        readonly object _lock = new object();
        volatile bool _posted;
        List<SvnProject> _dirtyProjects;
        List<SvnProject> _fullRefresh;
        uint _cookie;

        public ProjectNotifier(IAnkhServiceProvider context)
            : base(context)
        {
            uint cookie;
            if (ErrorHandler.Succeeded(context.GetService<IVsShell>(typeof(SVsShell)).AdviseBroadcastMessages(this, out cookie)))
                _cookie = cookie;
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

        IFileStatusCache _statusCache;
        IFileStatusCache Cache
        {
            [DebuggerStepThrough]
            get { return _statusCache ?? (_statusCache = GetService<IFileStatusCache>()); }
        }

        IProjectFileMapper _mapper;
        IProjectFileMapper Mapper
        {
            [DebuggerStepThrough]
            get { return _mapper ?? (_mapper = GetService<IProjectFileMapper>()); }
        }

        IPendingChangesManager _changeManager;
        IPendingChangesManager ChangeManager
        {
            [DebuggerStepThrough]
            get { return _changeManager ?? (_changeManager = GetService<IPendingChangesManager>()); }
        }

        IAnkhOpenDocumentTracker _tracker;
        IAnkhOpenDocumentTracker DocumentTracker
        {
            [DebuggerStepThrough]
            get { return _tracker ?? (_tracker = GetService<IAnkhOpenDocumentTracker>()); }
        }

        void PostDirty()
        {
            if (!_posted && CommandService != null && CommandService.PostExecCommand(AnkhCommand.MarkProjectDirty))
                _posted = true;
        }

        public void MarkDirty(SvnProject project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            lock (_lock)
            {
                if (_dirtyProjects == null)
                    _dirtyProjects = new List<SvnProject>();

                if (!_dirtyProjects.Contains(project))
                    _dirtyProjects.Add(project);

                PostDirty();
            }
        }

        public void MarkDirty(IEnumerable<SvnProject> projects)
        {
            if (projects == null)
                throw new ArgumentNullException("projects");

            lock (_lock)
            {
                if (_dirtyProjects == null)
                    _dirtyProjects = new List<SvnProject>();

                foreach (SvnProject project in projects)
                {
                    if (!_dirtyProjects.Contains(project))
                        _dirtyProjects.Add(project);
                }

                PostDirty();
            }
        }

        public void MarkFullRefresh(SvnProject project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            lock (_lock)
            {
                if (_fullRefresh == null)
                    _fullRefresh = new List<SvnProject>();

                if (!_fullRefresh.Contains(project))
                    _fullRefresh.Add(project);

                PostDirty();
            }
        }

        public void MarkFullRefresh(IEnumerable<SvnProject> projects)
        {
            if (projects == null)
                throw new ArgumentNullException("projects");

            lock (_lock)
            {
                if (_fullRefresh == null)
                    _fullRefresh = new List<SvnProject>();

                foreach (SvnProject project in projects)
                {
                    if (!_fullRefresh.Contains(project))
                        _fullRefresh.Add(project);
                }

                PostDirty();
            }
        }

        internal void HandleEvent(AnkhCommand command)
        {
            List<SvnProject> dirtyProjects;
            List<SvnProject> fullRefresh;

            AnkhSccProvider provider = Context.GetService<AnkhSccProvider>();

            lock (_lock)
            {
                _posted = false;

                if (provider == null)
                    return;

                dirtyProjects = _dirtyProjects;
                fullRefresh = _fullRefresh;
                _dirtyProjects = null;
                _fullRefresh = null;
            }

            if (fullRefresh != null)
            {
                foreach (SvnProject project in fullRefresh)
                {
                    // Will handle glyphs and all
                    if (project.RawHandle == null)
                    {
                        if (project.IsSolution)
                            provider.UpdateSolutionGlyph();

                        continue;
                    }

                    provider.RefreshProject(project.RawHandle);
                }
            }

            if (dirtyProjects != null)
            {
                foreach (SvnProject project in dirtyProjects)
                {
                    if (project.RawHandle == null)
                    {
                        if (project.IsSolution)
                            provider.UpdateSolutionGlyph();

                        continue; // All IVsSccProjects have a RawHandle
                    }

                    if (fullRefresh == null || !fullRefresh.Contains(project))
                    {
                        project.RawHandle.SccGlyphChanged(0, null, null, null);
                    }
                }
            }
        }

        #region IFileStatusMonitor Members

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

        public void ScheduleGlyphUpdate(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            MarkDirty(Mapper.GetAllProjectsContaining(path));
            ChangeManager.Refresh(path);
        }

        public void ScheduleGlyphUpdate(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            MarkDirty(Mapper.GetAllProjectsContaining(paths));
            ChangeManager.Refresh(paths);
        }

        #endregion

        #region IFileStatusMonitor Members

        public void ScheduleMonitor(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            ((PendingChangeManager)ChangeManager).ScheduleMonitor(path);
        }

        public void ScheduleMonitor(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            ((PendingChangeManager)ChangeManager).ScheduleMonitor(paths);
        }

        #endregion

        #region IFileStatusMonitor Members

        readonly Dictionary<string, DocumentLock> _externallyChanged = new Dictionary<string, DocumentLock>(StringComparer.OrdinalIgnoreCase);

        public void ExternallyChanged(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            ScheduleSvnStatus(path);

            lock (_externallyChanged)
            {
                if (!_externallyChanged.ContainsKey(path)
                    && !DocumentTracker.IsDocumentDirty(path, true))
                {
                    // Locking will trigger a file change!
                    _externallyChanged.Add(path, null);

                    DocumentLock dl = DocumentTracker.LockDocument(path, DocumentLockType.ReadOnly);

                    _externallyChanged.Remove(path);
                    _externallyChanged.Add(path, dl);
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
                foreach (KeyValuePair<string, DocumentLock> file in modified)
                {
                    ScheduleSvnStatus(file.Key);
                    SvnItem item = Cache[file.Key];

                    if (item.IsConflicted)
                    {
                        AnkhMessageBox mb = new AnkhMessageBox(Context);

                        DialogResult dr = mb.Show(string.Format(Resources.YourMergeToolSavedXWouldYouLikeItMarkedAsResolved, file), Resources.MergeCompleted,
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

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
                                break;
                        }
                    }
                    else if (!item.IsDocumentDirty)
                    {
                        // Reload?
                        file.Value.Reload(file.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler handler = GetService<IAnkhErrorHandler>();

                if (handler != null)
                    handler.OnError(ex);
            }
            finally
            {
                foreach (DocumentLock dl in modified.Values)
                {
                    dl.Dispose();
                }
            }
        }

        #endregion

        #region IVsBroadcastMessageEvents Members

        const uint WM_ACTIVATE = 0x0006;
        const uint WM_ACTIVATEAPP = 0x001C;

        int IVsBroadcastMessageEvents.OnBroadcastMessage(uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_ACTIVATEAPP:
                    if (wParam != IntPtr.Zero)
                        ReleaseExternalWrites();
                    break;
            }
            return VSConstants.S_OK;
        }

        #endregion
    }
}
