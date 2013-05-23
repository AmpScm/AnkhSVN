using Ankh.Commands;
using Ankh.Selection;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.Scc.Commands
{
    [SvnCommand(AnkhCommand.SccSccReCheckoutFailedProject)]
    [SvnCommand(AnkhCommand.SccEditFailedProjectLocation)]
    sealed class SccCheckoutFailedProject : ICommandHandler
    {
        static readonly object _failedProjectsKey = new object();

        public void OnUpdate(CommandUpdateEventArgs e)
        {
            object fm = e.Selection.Cache[_failedProjectsKey];
            IDictionary<string, object> map;

#if !DEBUG
            if (e.Command == AnkhCommand.SccEditFailedProjectLocation)
            {
                e.Enabled = false;
                return;
            }
#endif

            if (fm != null)
                map = (fm as IDictionary<string, object>);
            else
            {
                AnkhSccProvider scc = e.GetService<AnkhSccProvider>();
                map = scc.GetProjectsThatNeedEnlisting();
                e.Selection.Cache[_failedProjectsKey] = map ?? _failedProjectsKey;
            }
            
            if (map != null)
            {
                SvnHierarchy hier = EnumTools.GetSingle(e.Selection.GetSelectedHierarchies());

                if (hier != null && map.ContainsKey(hier.Name))
                    return;
            }

            e.Enabled = false;
        }

        public void OnExecute(CommandEventArgs e)
        {
            AnkhSccProvider scc = e.GetService<AnkhSccProvider>();
            SvnHierarchy hier = EnumTools.GetSingle(e.Selection.GetSelectedHierarchies());

            if (hier == null)
                throw new InvalidOperationException();

            switch (e.Command)
            {
                case AnkhCommand.SccSccReCheckoutFailedProject:
                    {
                        scc.EnlistAndCheckout(hier.Hierarchy, hier.Name);

                        IVsSolution s = e.GetService<IVsSolution>(typeof(SVsSolution));
                        ISccHelper helper = e.GetService<ISccHelper>();

                        if (s == null || helper == null)
                            return;

                        Guid projectGuid;
                        if (!VSErr.Succeeded(s.GetGuidOfProject(hier.Hierarchy, out projectGuid)))
                            return;

                        helper.EnsureProjectLoaded(projectGuid, false);
                        break;
                    }
                case AnkhCommand.SccEditFailedProjectLocation:
                    scc.EditEnlistment(hier.Hierarchy, hier.Name);
                    break;
            }
        }
    }
}
