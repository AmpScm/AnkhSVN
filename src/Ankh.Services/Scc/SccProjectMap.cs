using System;
using System.Collections.Generic;
using Ankh.Scc.ProjectMap;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    [CLSCompliant(false)]
    public abstract partial class SccProjectMap : AnkhService
    {
        readonly Dictionary<IVsSccProject2, SccProjectData> _projectMap = new Dictionary<IVsSccProject2, SccProjectData>();

        protected SccProjectMap(IAnkhServiceProvider context)
            : base(context)
        {

        }

        [CLSCompliant(false)]
        public bool TryGetSccProject(IVsSccProject2 project, out SccProjectData data)
        {
            return _projectMap.TryGetValue(project, out data);
        }

        public IEnumerable<SccProjectData> AllSccProjects
        {
            get { return _projectMap.Values; }
        }

        public void Clear()
        {
            _projectMap.Clear();
            _fileMap.Clear();
        }

        public bool ContainsProject(IVsSccProject2 project)
        {
            return _projectMap.ContainsKey(project);
        }

        void AddProject(IVsSccProject2 sccProject, SccProjectData sccProjectData)
        {
            _projectMap.Add(sccProject, sccProjectData);
        }

        public int ProjectCount
        {
            get { return _projectMap.Count; }
        }

        public bool RemoveProject(IVsSccProject2 project)
        {
            return _projectMap.Remove(project);
        }

        public void EnsureSccProject(IVsSccProject2 rawProject, out SccProjectData projectData)
        {
            if (!TryGetSccProject(rawProject, out projectData))
            {
                // This method is called before the OpenProject calls
                AddProject(rawProject, projectData = new SccProjectData(this, rawProject));
            }
        }

        internal protected abstract Selection.SccProject CreateProject(SccProjectData sccProjectData);

        internal SccProjectFile GetFile(string path)
        {
            SccProjectFile projectFile;

            if (!TryGetFile(path, out projectFile))
            {
                AddFile(path, projectFile = new SccProjectFile(this, path));

                AddedToSolution(path);
            }

            return projectFile;
        }

        protected virtual void AddedToSolution(string path)
        {
        }

        protected virtual void OnRemovedFile(string fileName)
        {
        }


        internal protected abstract int GetSccGlyph(string[] namesArray, VsStateIcon[] newGlyphs, uint[] sccState);

        internal void RemoveFile(SccProjectFile sccProjectFile)
        {
            throw new NotImplementedException();
        }
    }
}
