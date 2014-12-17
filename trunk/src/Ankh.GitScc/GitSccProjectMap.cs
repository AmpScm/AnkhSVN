using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ankh.Scc;

namespace Ankh.GitScc
{
    class GitSccProjectMap : SccProjectMap
    {
        public GitSccProjectMap(IAnkhServiceProvider context)
            : base(context)
        {

        }

        protected override Selection.SccProject CreateProject(Scc.ProjectMap.SccProjectData sccProjectData)
        {
            throw new NotImplementedException();
        }

        protected override int GetSccGlyph(string[] namesArray, Microsoft.VisualStudio.Shell.Interop.VsStateIcon[] newGlyphs, uint[] sccState)
        {
            throw new NotImplementedException();
        }
    }
}
