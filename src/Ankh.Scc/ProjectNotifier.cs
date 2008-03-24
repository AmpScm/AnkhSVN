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
    class ProjectNotifier : IProjectNotifier, IFileStatusMonitor
    {
        readonly IAnkhServiceProvider _context;

        public ProjectNotifier(IAnkhServiceProvider context)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            _context = context;
        }

        IAnkhCommandService _commandService;
        /// <summary>
        /// Gets the command service.
        /// </summary>
        /// <value>The command service.</value>
        IAnkhCommandService CommandService
        {
            get { return _commandService ?? (_commandService = _context.GetService<IAnkhCommandService>()); }
        }

        public void MarkDirty(SvnProject project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            MarkDirty(new SvnProject[] { project });
        }

        public void MarkDirty(IEnumerable<SvnProject> projects)
        {
            if (projects == null)
                throw new ArgumentNullException("projects");

            IAnkhCommandService cmd = CommandService;

            if (cmd == null)
                return;

            // Don't pass an enumerator via a postexec.. It's context might not be valid after the call
            IList<SvnProject> allProjects = (projects as IList<SvnProject>) ?? new List<SvnProject>(projects);

            cmd.PostExecCommand(AnkhCommand.MarkProjectDirty, allProjects);
        }

        public void MarkFullRefresh(SvnProject project)
        {
            if(project == null)
                throw new ArgumentNullException("project");

            MarkFullRefresh(new SvnProject[] { project });
        }

        public void MarkFullRefresh(IEnumerable<SvnProject> projects)
        {
            if (projects == null)
                throw new ArgumentNullException("projects");

            IAnkhCommandService cmd = CommandService;

            if (cmd == null)
                return;

            // Don't pass an enumerator via a postexec.. It's context might not be valid after the call
            IList<SvnProject> allProjects = (projects as IList<SvnProject>) ?? new List<SvnProject>(projects);

            cmd.PostExecCommand(AnkhCommand.MarkProjectRefresh, allProjects);
        }

        public void ScheduleStatusUpdate(string path)
        {
            ScheduleStatusUpdate(new string[] { path });
        }

        public void ScheduleStatusUpdate(IList<string> paths)
        {
            IFileStatusCache cache = _context.GetService<IFileStatusCache>();

            if (cache != null)
                cache.MarkDirty(paths);

            IProjectFileMapper mapper = _context.GetService<IProjectFileMapper>();

            if (mapper != null)
                MarkDirty(mapper.GetAllProjectsContaining(paths));
        }
    }
}
