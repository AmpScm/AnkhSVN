using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Ids;
using Microsoft.VisualStudio.OLE.Interop;
using Ankh.VS;
using Ankh.Commands;

namespace Ankh.Scc
{
    sealed class ProjectNotifier : AnkhService, IProjectNotifier, IFileStatusMonitor
    {
        readonly object _lock = new object();
        volatile bool _posted;
        List<SvnProject> _dirtyProjects;
        List<SvnProject> _fullRefresh;        

        public ProjectNotifier(IAnkhServiceProvider context)
            : base(context)
        {
        }

        IAnkhCommandService _commandService;
        /// <summary>
        /// Gets the command service.
        /// </summary>
        /// <value>The command service.</value>
        IAnkhCommandService CommandService
        {
            get { return _commandService ?? (_commandService = Context.GetService<IAnkhCommandService>()); }
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

                if (!_posted && CommandService != null && CommandService.PostExecCommand(AnkhCommand.MarkProjectDirty))
                    _posted = true;
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

                if (!_posted && CommandService != null && CommandService.PostExecCommand(AnkhCommand.MarkProjectDirty))
                    _posted = true;
            }
        }

        public void MarkFullRefresh(SvnProject project)
        {
            if(project == null)
                throw new ArgumentNullException("project");

            lock (_lock)
            {
                if (_fullRefresh == null)
                    _fullRefresh = new List<SvnProject>();

                if (!_fullRefresh.Contains(project))
                    _fullRefresh.Add(project);

                if (!_posted && CommandService != null && CommandService.PostExecCommand(AnkhCommand.MarkProjectDirty))
                    _posted = true;
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

                if (!_posted && CommandService != null && CommandService.PostExecCommand(AnkhCommand.MarkProjectDirty))
                    _posted = true;
            }
        }        

        internal void HandleEvent(AnkhCommand command)
        {
            List<SvnProject> dirtyProjects;
            List<SvnProject> fullRefresh;            
            
            AnkhSccProvider provider = Context.GetService<AnkhSccProvider>();            

            lock(_lock)
            {
                _posted = false;

                if(provider == null)
                    return;
                    
                dirtyProjects = _dirtyProjects;
                fullRefresh = _fullRefresh;
                _dirtyProjects = null;
                _fullRefresh = null;
            }
                
            if(fullRefresh != null)
            {
                foreach(SvnProject project in fullRefresh)
                {
                    // Will handle glyphs and all
                    provider.RefreshProject(project.RawHandle);
                }
            }

            if(dirtyProjects != null)
            {
                foreach (SvnProject project in dirtyProjects)
                {
                    if(project.RawHandle == null)
                        continue; // All IVsSccProjects have a RawHandle

                    if (fullRefresh == null || !fullRefresh.Contains(project))
                    {
                        project.RawHandle.SccGlyphChanged(0, null, null, null);
                    }
                }
            }
        }

        IFileStatusCache _statusCache;
        IFileStatusCache Cache
        {
            get { return _statusCache ?? (_statusCache = GetService<IFileStatusCache>()); }
        }

        IProjectFileMapper _mapper;
        IProjectFileMapper Mapper
        {
            get { return _mapper ?? (_mapper = GetService<IProjectFileMapper>()); }
        }

        IPendingChangesManager _changeManager;
        IPendingChangesManager ChangeManager
        {
            get { return _changeManager ?? (_changeManager = GetService<IPendingChangesManager>()); }
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
            if(paths == null)
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
            MarkDirty(Mapper.GetAllProjectsContaining(paths));
            ChangeManager.Refresh(paths);
        }

        #endregion

        #region IFileStatusMonitor Members

        public void ScheduleMonitor(string path)
        {
            if(string.IsNullOrEmpty("path"))
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
    }
}
