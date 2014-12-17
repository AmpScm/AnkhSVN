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

        internal bool TryGetValue(IVsSccProject2 project, out SccProjectData data)
        {
            return _projectMap.TryGetValue(project, out data);
        }

        public IEnumerable<SccProjectData> Values
        {
            get { return _projectMap.Values; }
        }

        public void Clear()
        {
            _projectMap.Clear();
        }

        public bool ContainsProject(IVsSccProject2 project)
        {
            return _projectMap.ContainsKey(project);
        }

        public void Add(IVsSccProject2 sccProject, SccProjectData sccProjectData)
        {
            _projectMap.Add(sccProject, sccProjectData);
        }

        public int Count
        {
            get { return _projectMap.Count; }
        }

        public void Remove(IVsSccProject2 project)
        {
            _projectMap.Remove(project);
        }
    }
}
