using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Scc.ProjectMap;

namespace Ankh.Scc
{
    class SvnSccProjectMap : SccProjectMap
    {
        public SvnSccProjectMap(IAnkhServiceProvider context)
            : base(context)
        {

        }

        SvnSccProvider _scc;
        protected SvnSccProvider Scc
        {
            get { return _scc ?? (_scc = GetService<SvnSccProvider>(typeof(ITheAnkhSvnSccProvider))); }
        }

        protected override Selection.SccProject CreateProject(ProjectMap.SccProjectData sccProjectData)
        {
            return new SvnSccProject(sccProjectData);
        }

        protected override void AddedToSolution(string path)
        {
            base.AddedToSolution(path);

            Scc.AddedToSolution(path);
        }

        protected override void OnRemovedFile(string fileName)
        {
            base.OnRemovedFile(fileName);

            Scc.RemovedFromSolution(fileName);
        }

        protected override int GetSccGlyph(string[] namesArray, VsStateIcon[] newGlyphs, uint[] sccState)
        {
            return Scc.GetSccGlyph(namesArray != null ? namesArray.Length : 0, namesArray, newGlyphs, sccState);
        }
    }
}
