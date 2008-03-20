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

        public void MarkDirty(Ankh.Selection.SvnProject project)
        {
            // TODO: We could probably group the projects and only post the command once
            MarkDirty(new SvnProject[] { project });
        }

        public void MarkDirty(IEnumerable<SvnProject> projects)
        {
            // TODO: We could probably group the projects and only post the command once

            IAnkhCommandService cmd = _context.GetService<IAnkhCommandService>();

            if (cmd == null)
                return;

            cmd.PostExecCommand(AnkhCommand.MarkProjectDirty, new List<SvnProject>(projects));
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
