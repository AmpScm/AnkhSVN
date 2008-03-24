using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.UI;
using Ankh.Selection;
using Ankh.Commands;
using AnkhSvn.Ids;
using Ankh.Scc.ProjectMap;
using System.Diagnostics;

namespace Ankh.Scc
{
    [Command(AnkhCommand.MarkProjectDirty)]
    [Command(AnkhCommand.MarkProjectRefresh)]
    public class MarkProjectDirty : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
            if (e.Argument == null)
                return;

            // Mark Scc Glyphs dirty, to force reload of glyphs from StatusCache
            IEnumerable<SvnProject> projects = e.Argument as IEnumerable<SvnProject>;

            if (projects != null)
            {
                MarkProjectsDirty(e, projects);
                return;
            }

            IList<string> files = e.Argument as IList<string>;

            if (files != null)
            {
                MarkFilesDirty(e, files);
                return;
            }

            Trace.WriteLine("Ignored refresh command for wrong argument type");
        }

        void MarkProjectsDirty(CommandEventArgs e, IEnumerable<SvnProject> projects)
        {
            IProjectFileMapper mapper = e.Context.GetService<IProjectFileMapper>();

            if (mapper == null)
                return;

            foreach (SvnProject p in projects)
            {
                SvnProject project = mapper.ResolveRawProject(p);                

                RefreshProject(e, project);                
            }
        }

        void MarkFilesDirty(CommandEventArgs e, IEnumerable<string> files)
        {
            IProjectFileMapper mapper = e.Context.GetService<IProjectFileMapper>();

            if (mapper == null)
                return;

            foreach (SvnProject p in mapper.GetAllProjectsContaining(files))
            {
                SvnProject project = mapper.ResolveRawProject(p);

                RefreshProject(e, project);
            }
        }

        private void RefreshProject(CommandEventArgs e, SvnProject project)
        {
            IVsSccProject2 scc = project.RawHandle;

            if (scc == null)
                return;

            if (e.Command == AnkhCommand.MarkProjectRefresh)
            {
                AnkhSccProvider provider = e.Context.GetService<AnkhSccProvider>();

                if (provider != null)
                {
                    provider.RefreshProject(project.RawHandle);
                    return;
                }
            }
            
            // Mark all glyphs cached in a project dirty
            scc.SccGlyphChanged(0, null, null, null);
        }
    }
}
