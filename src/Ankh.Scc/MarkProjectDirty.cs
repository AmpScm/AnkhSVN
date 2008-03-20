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

namespace Ankh.Scc
{
    [Command(AnkhCommand.MarkProjectDirty)]
    public class MarkProjectDirty : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
            // Mark Scc Glyphs dirty, to force reload of glyphs from StatusCache
            IList<SvnProject> projects = e.Argument as IList<SvnProject>;

            if (projects != null)
            {
                MarkProjectsDirty(e, projects);
                return;
            }

            IList<string> files = e.Argument as IList<string>;

            if(files != null)
                MarkFilesDirty(e, files);            
        }

        void MarkProjectsDirty(CommandEventArgs e, IList<SvnProject> projects)
        {
            IProjectFileMapper mapper = e.Context.GetService<IProjectFileMapper>();

            if (mapper == null)
                return;

            foreach (SvnProject p in projects)
            {
                SvnProject project = mapper.ResolveRawProject(p);

                IVsSccProject2 scc = project.RawHandle as IVsSccProject2;

                if (scc != null)
                {
                    // Mark all glyphs cached in a project dirty
                    scc.SccGlyphChanged(0, null, null, null);
                }
            }
        }

        void MarkFilesDirty(CommandEventArgs e, IList<string> files)
        {
            IProjectFileMapper mapper = e.Context.GetService<IProjectFileMapper>();

            if (mapper == null)
                return;

            foreach (SvnProject p in mapper.GetAllProjectsContaining(files))
            {
                SvnProject project = mapper.ResolveRawProject(p);

                IVsSccProject2 scc = project.RawHandle as IVsSccProject2;

                if (scc != null)
                {
                    // Mark all glyphs cached in a project dirty
                    scc.SccGlyphChanged(0, null, null, null);
                }
            }
        }        
    }
}
