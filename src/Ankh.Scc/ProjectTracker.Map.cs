using System;
using System.Collections.Generic;
using Ankh.Selection;

namespace Ankh.Scc
{
    [GlobalService(typeof(IProjectFileMapper))]
    partial class ProjectTracker : IProjectFileMapper
    {
        public IEnumerable<Selection.SccProject> GetAllProjectsContaining(string path)
        {
            return ProjectMap.GetAllProjectsContaining(path);
        }

        public IEnumerable<Selection.SccProject> GetAllProjectsContaining(IEnumerable<string> paths)
        {
            return ProjectMap.GetAllProjectsContaining(paths);
        }

        public IEnumerable<Selection.SccProject> GetAllProjects()
        {
            return ProjectMap.GetAllProjects();
        }

        public IEnumerable<string> GetAllFilesOf(SccProject project)
        {
            return ProjectMap.GetAllFilesOf(project, false);
        }

        public IEnumerable<string> GetAllFilesOf(Selection.SccProject project, bool exceptExcluded)
        {
            return ProjectMap.GetAllFilesOf(project, exceptExcluded);
        }

        public IEnumerable<string> GetAllFilesOf(ICollection<Selection.SccProject> projects)
        {
            return ProjectMap.GetAllFilesOf(projects, false);
        }

        public IEnumerable<string> GetAllFilesOf(ICollection<Selection.SccProject> projects, bool exceptExcluded)
        {
            return ProjectMap.GetAllFilesOf(projects, exceptExcluded);
        }

        public ICollection<string> GetAllFilesOfAllProjects()
        {
            return ProjectMap.GetAllFilesOfAllProjects(false);
        }

        public ICollection<string> GetAllFilesOfAllProjects(bool exceptExcluded)
        {
            return GetAllFilesOfAllProjects(exceptExcluded);
        }

        bool IProjectFileMapper.ContainsPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (ProjectMap.ContainsFile(path))
                return true;

            if (string.Equals(path, ProjectMap.SolutionFilename, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        bool IProjectFileMapper.IsSccExcluded(string path)
        {
            return ProjectMap.IsSccExcluded(path);
        }

        string IProjectFileMapper.SolutionFilename
        {
            get { return ProjectMap.SolutionFilename; }
        }

        public bool IsProjectFileOrSolution(string path)
        {
            return ProjectMap.IsProjectFileOrSolution(path);
        }

        public ISccProjectInfo GetProjectInfo(Selection.SccProject project)
        {
            return ProjectMap.GetProjectInfo(project);
        }

        public ProjectIconReference GetPathIconHandle(string path)
        {
            return ProjectMap.GetPathIconHandle(path);
        }

        public bool IgnoreEnumerationSideEffects(Microsoft.VisualStudio.Shell.Interop.IVsSccProject2 sccProject)
        {
            return ProjectMap.IgnoreEnumerationSideEffects(sccProject);
        }
    }
}
