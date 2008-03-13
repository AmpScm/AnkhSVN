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
        public ProjectNotifier(IAnkhServiceProvider serviceProvider)
        {
            this.context = serviceProvider;
        }

        public void MarkDirty(Ankh.Selection.SvnProject project)
        {
            MarkDirty(new SvnProject[] { project });
        }

        public void MarkDirty(IEnumerable<Ankh.Selection.SvnProject> projects)
        {
            IVsUIShell uiShell = (IVsUIShell)context.GetService(typeof(SVsUIShell));

            // After marking the item dirty, force the SccGlyphs to be reloaded. The SvnItems know if they need refreshing at that point
            Guid commandSet = AnkhId.CommandSetGuid;
            object projectsObj = projects;
            uiShell.PostExecCommand(ref commandSet, (uint)AnkhCommand.MarkProjectDirty, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref projectsObj);
        }

        IAnkhServiceProvider context;
    }
}
