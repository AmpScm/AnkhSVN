using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;
using Microsoft.VisualStudio.Shell.Interop;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.OLE.Interop;
using Ankh.VS;
using Ankh.Commands;

namespace Ankh.Scc
{
    class ProjectNotifier : AnkhService, IProjectNotifier, IFileStatusMonitor
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


                foreach (SvnProject p in projects)
                {
                    if (!_dirtyProjects.Contains(p))
                        _dirtyProjects.Add(p);
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

                foreach (SvnProject p in projects)
                {
                    if (!_fullRefresh.Contains(p))
                        _fullRefresh.Add(p);
                }

                if (!_posted && CommandService != null && CommandService.PostExecCommand(AnkhCommand.MarkProjectDirty))
                    _posted = true;
            }
        }

        public void ScheduleStatusUpdate(string path)
        {
            ScheduleStatusUpdate(new string[] { path });
        }

        public void ScheduleStatusUpdate(IList<string> paths)
        {
            IFileStatusCache cache = Context.GetService<IFileStatusCache>();

            if (cache != null)
                cache.MarkDirty(paths);

            IProjectFileMapper mapper = Context.GetService<IProjectFileMapper>();

            if (mapper != null)
                MarkDirty(mapper.GetAllProjectsContaining(paths));
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
    }
}
