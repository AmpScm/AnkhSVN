// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Scc.ProjectMap;
using System.IO;
using System.Diagnostics;
using System.Collections;
using Ankh.Selection;
using SharpSvn;
using Ankh.VS;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    [GlobalService(typeof(IProjectFileMapper))]
    [GlobalService(typeof(IAnkhProjectLayoutService))]
    partial class AnkhSccProvider : IProjectFileMapper, IAnkhProjectLayoutService
    {
        // ********************************************************
        // This file contains two very important features of the Scc provider:
        //  - The tracking of changes in the File <-> Project mapping (Many <-> Many)
        //  - The persistance of history on project add/remove/rename actions
        //
        readonly Dictionary<string, SccProjectFile> _fileMap = new Dictionary<string, SccProjectFile>(StringComparer.OrdinalIgnoreCase);
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
        /// <param name="project">The project.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="fileOrigin">The file origin.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectFileAdded(IVsSccProject2 project, string filename, string fileOrigin, VSADDFILEFLAGS flags)
        {
            // First update the filemap
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us

            data.AddPath(filename);
        }

        /// <summary>
        /// Called when a file is removed from a project
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectFileRemoved(IVsSccProject2 project, string filename, VSREMOVEFILEFLAGS flags)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us

            data.RemovePath(filename);
        }

        /// <summary>
        /// Called when a directory is added to a project
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="directoryname">The directoryname.</param>
        /// <param name="origin">The origin or null.</param>
        internal void OnProjectDirectoryAdded(IVsSccProject2 project, string directoryname, string origin)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us

            // Add a directory like a folder but with an ending '\'
            string dir = directoryname.TrimEnd('\\') + '\\';
            data.AddPath(dir);

            if (!IsActive)
                return;

            if (!data.IsWebSite)
            {
                // Do nothing
            }
            else
            {
                // Websites don't contain a real file mapping; reload to load new files
                // and directories recursively
                data.Reload();
            }
        }

        /// <summary>
        /// Called when a directory is removed from a project
        /// </summary>
        /// <param name="project">The SCC project.</param>
        /// <param name="directoryname">The directoryname.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectDirectoryRemoved(IVsSccProject2 project, string directoryname, VSREMOVEDIRECTORYFLAGS flags)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us

            // a directory can be added like a folder but with an ending '\'
            string dir = directoryname.TrimEnd('\\') + '\\';
            data.RemovePath(dir);
        }

        /// <summary>
        /// Called when a file in a project is renamed
        /// </summary>
        /// <param name="project">The SCC project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectRenamedFile(IVsSccProject2 project, string oldName, string newName, VSRENAMEFILEFLAGS flags)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us
            else
                data.CheckProjectRename(project, oldName, newName); // Just to be sure (should be handled by other event)

            data.RemovePath(oldName);
            data.AddPath(newName);
        }

        internal void OnSolutionRenamedFile(string oldName, string newName)
        {
            // The solution file is renamed

            _solutionDirectory = _solutionFile = null; // Get new data after this rename
            Monitor.ScheduleGlyphUpdate(SolutionFilename);
        }

        /// <summary>
        /// Called just before a directory in a project is renamed
        /// </summary>
        /// <param name="project">The SCC project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="ok">if set to <c>true</c> [ok].</param>
        internal void OnBeforeProjectDirectoryRename(IVsSccProject2 project, string oldName, string newName, VSQUERYRENAMEDIRECTORYFLAGS flags, out bool ok)
        {
            ok = true;

            if (!_projectMap.ContainsKey(project))
                return; // Not managed by us            

            if (!IsActive)
                return;

            // TODO: Is the file managed in Subversion: Verify renaming of more than casing
            if (oldName != newName && string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
            {
                ok = false; // For now just disallow casing only changes
            }
        }

        /// <summary>
        /// Called when a directory in a project is renamed
        /// </summary>
        /// <param name="project">The SCC project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectDirectoryRenamed(IVsSccProject2 project, string oldName, string newName, VSRENAMEDIRECTORYFLAGS flags)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us

            if (!IsActive)
                return;

            Debug.Assert(!Directory.Exists(oldName));
            Debug.Assert(Directory.Exists(newName));
            GC.KeepAlive(newName);
            GC.KeepAlive(oldName);
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


        #region ProjectFile
        internal SccProjectFile GetFile(string path)
        {
            SccProjectFile projectFile;

            if (!_fileMap.TryGetValue(path, out projectFile))
            {
                _fileMap.Add(path, projectFile = new SccProjectFile(Context, path));

                // Force an initial status into the SvnItem
                StatusCache.SetSolutionContained(path, true, _sccExcluded.Contains(path));
            }

            return projectFile;
        }

        internal void RemoveFile(SccProjectFile file)
        {
            Debug.Assert(_fileMap[file.FullPath] == file);

            _fileMap.Remove(file.FullPath);
            PendingChanges.Refresh(file.FullPath);
        }
        #endregion

        #region IProjectFileMapper Members

        public IEnumerable<Ankh.Selection.SvnProject> GetAllProjectsContaining(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            path = SvnTools.GetNormalizedFullPath(path);

            SccProjectFile file;
            if (_fileMap.TryGetValue(path, out file))
            {
                foreach (SccProjectData pd in file.GetOwnerProjects())
                {
                    yield return pd.SvnProject;
                }
            }

            if (string.Equals(path, SolutionFilename, StringComparison.OrdinalIgnoreCase))
                yield return SvnProject.Solution;
        }

        public IEnumerable<SvnProject> GetAllProjectsContaining(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            Hashtable projects = new Hashtable();
            foreach (string path in paths)
            {
                string nPath = SvnTools.GetNormalizedFullPath(path);

                SccProjectFile file;
                if (_fileMap.TryGetValue(nPath, out file))
                {
                    foreach (SccProjectData pd in file.GetOwnerProjects())
                    {
                        if (projects.Contains(pd))
                            continue;

                        projects.Add(pd, pd);

                        yield return pd.SvnProject;
                    }
                }

                if (!projects.Contains(SvnProject.Solution)
                    && string.Equals(path, SolutionFilename, StringComparison.OrdinalIgnoreCase))
                {
                    projects.Add(SvnProject.Solution, SvnProject.Solution);
                    yield return SvnProject.Solution;
                }
            }
        }

        /// <summary>
        /// Gets all projects.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Ankh.Selection.SvnProject> GetAllProjects()
        {
            foreach (SccProjectData pd in _projectMap.Values)
                yield return pd.SvnProject;
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

            if (_fileMap.ContainsKey(path))
                return true;

            if (string.Equals(path, SolutionFilename, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public IEnumerable<string> GetAllFilesOf(Ankh.Selection.SvnProject project)
        {
            return GetAllFilesOf(project, false);
        }

        /// <summary>
        /// Gets a list of all files contained within <paramref name="project"/>
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllFilesOf(Ankh.Selection.SvnProject project, bool exceptExcluded)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            if (project.IsSolution)
            {
                string sf = SolutionFilename;

                if (sf != null && (!exceptExcluded || !IsSccExcluded(SolutionFilename)))
                    yield return sf;

                yield break;
            }

            project = ResolveRawProject(project);

            IVsSccProject2 scc = project.RawHandle;
            SccProjectData data;

            if (scc == null || !_projectMap.TryGetValue(scc, out data))
                yield break;

            foreach (string file in data.GetAllFiles())
            {
                if (file[file.Length - 1] != '\\') // Don't return paths
                {
                    if (exceptExcluded && IsSccExcluded(file))
                        continue;

                    yield return file;
                }
            }
        }

        public IEnumerable<string> GetAllFilesOf(ICollection<SvnProject> projects)
        {
            return GetAllFilesOf(projects, false);
        }

        public IEnumerable<string> GetAllFilesOf(ICollection<SvnProject> projects, bool exceptExcluded)
        {
            SortedList<string, string> files = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
            Hashtable handled = new Hashtable();
            foreach (SvnProject p in projects)
            {
                SvnProject project = ResolveRawProject(p);

                IVsSccProject2 scc = project.RawHandle;
                SccProjectData data;

                if (scc == null || !_projectMap.TryGetValue(scc, out data))
                {
                    if (p.IsSolution && SolutionFilename != null && !files.ContainsKey(SolutionFilename))
                    {
                        files.Add(SolutionFilename, SolutionFilename);

                        if (exceptExcluded && IsSccExcluded(SolutionFilename))
                            continue;

                        yield return SolutionFilename;
                    }

                    continue;
                }

                if (handled.Contains(data))
                    continue;

                handled.Add(data, data);

                foreach (string file in data.GetAllFiles())
                {
                    if (file[file.Length - 1] == '\\') // Don't return paths
                        continue;

                    if (files.ContainsKey(file))
                        continue;

                    files.Add(file, file);

                    if (exceptExcluded && IsSccExcluded(file))
                        continue;

                    yield return file;
                }
            }
        }

        public ICollection<string> GetAllFilesOfAllProjects()
        {
            return GetAllFilesOfAllProjects(false);
        }

        public ICollection<string> GetAllFilesOfAllProjects(bool exceptExcluded)
        {
            List<string> files = new List<string>(_fileMap.Count + 1);

            if (SolutionFilename != null && !_fileMap.ContainsKey(SolutionFilename))
                files.Add(SolutionFilename);

            foreach (string file in _fileMap.Keys)
            {
                if (file[file.Length - 1] == '\\') // Don't return paths
                    continue;

                if (exceptExcluded && IsSccExcluded(file))
                    continue;

                files.Add(file);
            }

            return files.ToArray();
        }

        public SvnProject ResolveRawProject(SvnProject project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            if (project.RawHandle == null && !project.IsSolution)
            {
                SccProjectFile file;

                if (_fileMap.TryGetValue(project.FullPath, out file))
                {
                    foreach (SccProjectData p in file.GetOwnerProjects())
                    {
                        return p.SvnProject;
                    }
                }
            }

            return project;
        }

        public ProjectIconReference GetPathIconHandle(string path)
        {
            SccProjectFile file;

            if (!_fileMap.TryGetValue(path, out file))
            {
                if (string.Equals(path, SolutionFilename, StringComparison.OrdinalIgnoreCase))
                {
                    // TODO: Fetch real solution icon
                    return null;
                }

                return null;
            }

            foreach (SccProjectFileReference fr in file.GetAllReferences())
            {
                ProjectIconReference icon;
                if (fr.TryGetIcon(out icon))
                    return icon;
            }

            return null;
        }

        public ISvnProjectInfo GetProjectInfo(SvnProject project)
        {
            if (project == null)
                return null;

            project = ResolveRawProject(project);

            if (project == null || project.RawHandle == null)
                return null;

            SccProjectData pd;
            if (_projectMap.TryGetValue(project.RawHandle, out pd))
            {
                return new WrapProjectInfo(pd);
            }

            return null;
        }

        /// <summary>
        /// Gets a boolean indicating whether the specified path is of a project or the solution
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public bool IsProjectFileOrSolution(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (string.Equals(path, SolutionFilename, StringComparison.OrdinalIgnoreCase))
                return true; // A solution file can be part of a project

            SccProjectFile file;
            if (!_fileMap.TryGetValue(path, out file))
                return false;

            return file.IsProjectFile;
        }

        #endregion

        public IEnumerable<SvnItem> GetUpdateRoots(SvnProject project)
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

            List<SccProjectData> projects = new List<SccProjectData>(_projectMap.Values);

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

        private IEnumerable<SvnItem> GetSingleProjectRoots(SvnProject project)
        {
            SccProjectData pd;
            if (project.RawHandle == null || !_projectMap.TryGetValue(project.RawHandle, out pd))
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

        bool IAnkhSccService.IgnoreEnumerationSideEffects(Microsoft.VisualStudio.Shell.Interop.IVsSccProject2 sccProject)
        {
            SccProjectData projectData;
            if (_projectMap.TryGetValue(sccProject, out projectData))
            {
                // We have to know its contents to provide SCC info
                // TODO: BH: Maybe only enable while reloading?
                return projectData.IsWebSite;
            }

            return false;
        }

        /// <summary>
        /// Wrapper class providing a public api to the data contained within <see cref="SccProjectData"/>
        /// </summary>
        /// <remarks>Showing the raw properties of SccProjectData has side-effects. We wrap the class to hide this problem</remarks>
        sealed class WrapProjectInfo : ISvnProjectInfo
        {
            readonly SccProjectData _data;

            /// <summary>
            /// Initializes a new instance of the <see cref="WrapProjectInfo"/> class.
            /// </summary>
            /// <param name="data">The data.</param>
            public WrapProjectInfo(SccProjectData data)
            {
                if (data == null)
                    throw new ArgumentNullException("data");

                _data = data;
            }

            /// <summary>
            /// Gets the name of the project.
            /// </summary>
            /// <value>The name of the project.</value>
            public string ProjectName
            {
                get { return _data.ProjectName; }
            }

            /// <summary>
            /// Gets the project directory.
            /// </summary>
            /// <value>The project directory.</value>
            public string ProjectDirectory
            {
                get { return _data.ProjectDirectory; }
            }

            #region ISvnProjectInfo Members


            /// <summary>
            /// Gets the project file.
            /// </summary>
            /// <value>The project file.</value>
            public string ProjectFile
            {
                get { return _data.ProjectFile; }
            }

            /// <summary>
            /// Gets the full name of the project (the project prefixed by the folder it is under)
            /// </summary>
            /// <value>The full name of the project.</value>
            public string UniqueProjectName
            {
                get { return _data.UniqueProjectName; }
            }

            /// <summary>
            /// Gets the SCC base directory.
            /// </summary>
            /// <value>The SCC base directory.</value>
            public string SccBaseDirectory
            {
                get { return _data.SccBaseDirectory; }
                set { throw new InvalidOperationException(); }
            }

            /// <summary>
            /// Gets or sets the SCC base URI.
            /// </summary>
            /// <value>The SCC base URI.</value>
            public Uri SccBaseUri
            {
                get { return null; }
                set { throw new InvalidOperationException(); }
            }

            public bool IsSccBindable
            {
                get { return _data.IsSccBindable; }

            }

            #endregion
        }
    }
}
