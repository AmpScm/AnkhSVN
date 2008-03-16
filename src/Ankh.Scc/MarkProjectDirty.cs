using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio;
using Ankh.UI;
using Ankh.Selection;
using Ankh.Commands;

namespace Ankh.Scc
{
    [Command(AnkhSvn.Ids.AnkhCommand.MarkProjectDirty)]
    public class MarkProjectDirty : ICommandHandler
    {
        public void OnUpdate(CommandUpdateEventArgs e)
        {
        }

        public void OnExecute(CommandEventArgs e)
        {
            // Mark Scc Glyphs dirty, to force reload of glyphs from StatusCache
            IEnumerable<SvnProject> projects = e.Argument as IEnumerable<SvnProject>;

            IProjectFileMapper mapper = e.Context.GetService<IProjectFileMapper>();

            if (projects == null || mapper == null)
                return;

            foreach (SvnProject p in projects)
            {
                SvnProject project = mapper.ResolveRawProject(p);

                IVsSccProject2 scc = project.RawHandle as IVsSccProject2;

                if(scc != null)
                    scc.SccGlyphChanged(0, null, null, null);
            }
        }
    }
}
