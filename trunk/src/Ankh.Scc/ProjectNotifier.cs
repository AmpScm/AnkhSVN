using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Selection;
using Microsoft.VisualStudio.Shell.Interop;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.OLE.Interop;
using Ankh.VS;

namespace Ankh.Scc
{
    public class ProjectNotifier : IProjectNotifier
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

            IVsUIShell uiShell = (IVsUIShell)_context.GetService(typeof(SVsUIShell));

            if (uiShell != null)
            {
                // After marking the item dirty, force the SccGlyphs to be reloaded. The SvnItems know if they need refreshing at that point
                Guid commandSet = AnkhId.CommandSetGuid;
                object projectsObj = new List<SvnProject>(projects); // Cache a list instead of the enumerator
                uiShell.PostExecCommand(ref commandSet, (uint)AnkhCommand.MarkProjectDirty, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref projectsObj);
            }
        }        
    }
}
