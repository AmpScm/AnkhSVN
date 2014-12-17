using System;
using System.Collections.Generic;
using Ankh.Scc.ProjectMap;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.Scc
{
    partial class SccProjectMap : AnkhService
    {
        readonly Dictionary<IVsSccProject2, SccProjectData> _projectMap = new Dictionary<IVsSccProject2, SccProjectData>();

        public SccProjectMap(IAnkhServiceProvider context)
            : base(context)
        {

        }

        internal bool TryGetSccProject(IVsSccProject2 project, out SccProjectData data)
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

        public void AddProject(IVsSccProject2 sccProject, SccProjectData sccProjectData)
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
    }
}
