using System;
using System.Collections.Generic;
using Ankh.Scc.ProjectMap;
using Ankh.Selection;
using Ankh.VS;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    [GlobalService(typeof(IProjectFileMapper))]
    [GlobalService(typeof(IAnkhProjectLayoutService))]
    partial class SvnSccProvider : IProjectFileMapper, IAnkhProjectLayoutService
    {
        // ********************************************************
        // This file contains two very important features of the Scc provider:
        //  - The tracking of changes in the File <-> Project mapping (Many <-> Many)
        //  - The persistance of history on project add/remove/rename actions
        //
        IAnkhSolutionSettings _solutionSettings;
        IPendingChangesManager _pendingChanges;
        bool _syncMap;

        IAnkhSolutionSettings SolutionSettings
        {
            get { return _solutionSettings ?? (_solutionSettings = GetService<IAnkhSolutionSettings>()); }
        }

        IPendingChangesManager PendingChanges
        {
            get { return _pendingChanges ?? (_pendingChanges = GetService<IPendingChangesManager>()); }
        }

        /// <summary>
        /// Called when a file is added to a project
        /// </summary>
        /// <param name="data">The project.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="fileOrigin">The file origin.</param>
        /// <param name="flags">The flags.</param>
        protected override void OnProjectFileAdded(SccProjectData data, string filename)
        {
            base.OnProjectFileAdded(data, filename);
        }

        /// <summary>
        /// Called when a file is removed from a project
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="flags">The flags.</param>
        protected override void OnProjectFileRemoved(SccProjectData data, string filename)
        {
            base.OnProjectFileRemoved(data, filename);
        }

        /// <summary>
        /// Called when a directory is added to a project
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="directoryname">The directoryname.</param>
        /// <param name="origin">The origin or null.</param>
        protected override void OnProjectDirectoryAdded(SccProjectData project, string directoryname, string origin)
        {
            base.OnProjectDirectoryAdded(project, directoryname, origin);

            if (!IsActive)
                return;

            if (!project.WebLikeFileHandling)
            {
                // Do nothing
            }
            else
            {
                // Websites don't contain a real file mapping; reload to load new files
                // and directories recursively
                project.Reload();
            }
        }

        /// <summary>
        /// Called when a directory is removed from a project
        /// </summary>
        /// <param name="project">The SCC project.</param>
        /// <param name="directoryname">The directoryname.</param>
        /// <param name="flags">The flags.</param>
        protected override void OnProjectDirectoryRemoved(SccProjectData data, string directoryname)
        {
            base.OnProjectDirectoryRemoved(data, directoryname);
        }

        /// <summary>
        /// Called when a file in a project is renamed
        /// </summary>
        /// <param name="project">The SCC project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="flags">The flags.</param>
        protected override void OnProjectFileRenamed(SccProjectData data, string oldName, string newName)
        {
            base.OnProjectFileRenamed(data, oldName, newName);
            
        }

        protected override void OnSolutionRenamedFile(string oldName, string newName)
        {
            // The solution file is renamed
            base.OnSolutionRenamedFile(oldName, newName);

            Monitor.ScheduleGlyphUpdate(SolutionFilename);
        }

        /// <summary>
        /// Called when a directory in a project is renamed
        /// </summary>
        /// <param name="project">The SCC project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="flags">The flags.</param>
        protected override void OnProjectDirectoryRenamed(SccProjectData data, string oldName, string newName)
        {
            base.OnProjectDirectoryRenamed(data, oldName, newName);
        }

        IFileStatusMonitor _monitor;
        IFileStatusMonitor Monitor
        {
            get { return _monitor ?? (_monitor = GetService<IFileStatusMonitor>()); }
        }

        void MarkDirty(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Monitor.ScheduleSvnStatus(path);
        }

        void MarkDirty(IEnumerable<string> paths, bool addToMonitorList)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            if (addToMonitorList)
                Monitor.ScheduleMonitor(paths);

            Monitor.ScheduleSvnStatus(paths);
        }


        internal void RemovedFromSolution(string fullPath)
        {
            StatusCache.SetSolutionContained(fullPath, false, false);
            PendingChanges.Refresh(fullPath);
        }

        #region IProjectFileMapper Members

        public IEnumerable<Ankh.Selection.SccProject> GetAllProjectsContaining(string path)
        {
            return ProjectMap.GetAllProjectsContaining(path);
        }

        public IEnumerable<SccProject> GetAllProjectsContaining(IEnumerable<string> paths)
        {
            return ProjectMap.GetAllProjectsContaining(paths);
        }

        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Ankh.Selection.SccProject> GetAllProjects()
        {
            return ProjectMap.GetAllProjects();
        }

        /// <summary>
        /// Gets a boolean indicating whether one or more projects (or the solution) contains path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool ContainsPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (ProjectMap.ContainsFile(path))
                return true;

            if (string.Equals(path, SolutionFilename, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public IEnumerable<string> GetAllFilesOf(Ankh.Selection.SccProject project)
        {
            return GetAllFilesOf(project, false);
        }

        /// <summary>
        /// Gets a list of all files contained within <paramref name="project"/>
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllFilesOf(Ankh.Selection.SccProject project, bool exceptExcluded)
        {
            return ProjectMap.GetAllFilesOf(project, exceptExcluded);
        }

        public IEnumerable<string> GetAllFilesOf(ICollection<SccProject> projects)
        {
            return GetAllFilesOf(projects, false);
        }

        public IEnumerable<string> GetAllFilesOf(ICollection<SccProject> projects, bool exceptExcluded)
        {
            return ProjectMap.GetAllFilesOf(projects, exceptExcluded);
        }

        public ICollection<string> GetAllFilesOfAllProjects()
        {
            return GetAllFilesOfAllProjects(false);
        }

        public ICollection<string> GetAllFilesOfAllProjects(bool exceptExcluded)
        {
            return ProjectMap.GetAllFilesOfAllProjects(exceptExcluded);
        }

        public ProjectIconReference GetPathIconHandle(string path)
        {
            return ProjectMap.GetPathIconHandle(path);
        }

        public ISccProjectInfo GetProjectInfo(SccProject project)
        {
            return ProjectMap.GetProjectInfo(project);
        }

        /// <summary>
        /// Gets a boolean indicating whether the specified path is of a project or the solution
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsProjectFileOrSolution(string path)
        {
            return ProjectMap.IsProjectFileOrSolution(path);
        }

        #endregion

        public IEnumerable<SvnItem> GetUpdateRoots(SccProject project)
        {
            if (project != null)
                return GetSingleProjectRoots(project);

            return GetSolutionProjectRoots();
        }

        private IEnumerable<SvnItem> GetSolutionProjectRoots()
        {
            Dictionary<string, SvnItem> roots = new Dictionary<string, SvnItem>(StringComparer.OrdinalIgnoreCase);

            SvnItem root = SolutionSettings.ProjectRootSvnItem;
            if (root != null)
            {
                if (root.IsVersioned)
                {
                    roots.Add(root.FullPath, root);
                }
            }

            List<SccProjectData> projects = new List<SccProjectData>(ProjectMap.AllSccProjects);

            foreach (SccProjectData pb in projects)
            {
                // TODO: Move to SccProjectRoot
                string dir = pb.ProjectDirectory;
                if (dir == null || roots.ContainsKey(dir))
                    continue;

                SvnItem pItem = StatusCache[dir];
                if (pItem == null || !pItem.Exists)
                    continue;

                SvnWorkingCopy wc = pItem.WorkingCopy;

                if (wc == null || roots.ContainsKey(wc.FullPath))
                    continue;

                SvnItem wcItem = StatusCache[wc.FullPath];

                bool below = false;
                foreach (SvnItem r in roots.Values)
                {
                    if (r.WorkingCopy != wc) // same working copy?
                        continue;

                    if (r == pItem || pItem.IsBelowPath(r))
                    {
                        below = true;
                        break;
                    }
                    else if (r.IsBelowPath(pItem))
                    {
                        roots.Remove(r.FullPath);
                        break;
                    }
                }

                if (!below && !roots.ContainsKey(pItem.FullPath))
                {
                    roots.Add(pItem.FullPath, pItem);
                }
            }

            return roots.Values;
        }

        private IEnumerable<SvnItem> GetSingleProjectRoots(SccProject project)
        {
            SccProjectData pd;
            if (project.RawHandle == null || !ProjectMap.TryGetSccProject(project.RawHandle, out pd))
                yield break;

            SvnItem projectRootItem = null;
            if (!string.IsNullOrEmpty(pd.ProjectDirectory))
            {
                projectRootItem = StatusCache[pd.ProjectDirectory];

                if (projectRootItem.IsVersioned)
                    yield return projectRootItem;
            }

            string file = pd.ProjectFile;

            if (string.IsNullOrEmpty(file) || !SvnItem.IsValidPath(file))
                yield break;

            SvnItem projectFileItem = StatusCache[file];

            if (projectFileItem.IsVersioned &&
                (projectRootItem == null || !projectFileItem.IsBelowPath(projectRootItem.FullPath)))
            {
                yield return projectFileItem;
            }
        }

        bool IProjectFileMapper.IgnoreEnumerationSideEffects(Microsoft.VisualStudio.Shell.Interop.IVsSccProject2 sccProject)
        {
            return ProjectMap.IgnoreEnumerationSideEffects(sccProject);
        }

    }
}
