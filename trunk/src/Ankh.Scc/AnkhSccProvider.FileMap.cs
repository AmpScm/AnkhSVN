﻿using System;
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
    partial class AnkhSccProvider : IProjectFileMapper, IAnkhProjectLayoutService
    {
        // ********************************************************
        // This file contains two very important features of the Scc provider:
        //  - The tracking of changes in the File <-> Project mapping (Many <-> Many)
        //  - The persistance of history on project add/remove/rename actions
        //
        readonly Dictionary<string, SccProjectFile> _fileMap = new Dictionary<string, SccProjectFile>(StringComparer.OrdinalIgnoreCase);
        IAnkhSolutionSettings _solutionSettings;
        bool _syncMap;

        IAnkhSolutionSettings SolutionSettings
        {
            get { return _solutionSettings ?? (_solutionSettings = GetService<IAnkhSolutionSettings>()); }
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

            if (!IsActive)
                return; // Let the other SCC package manage it

            MarkDirty(filename, true);
        }

        /// <summary>
        /// Called when a file is removed from a project
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="forDelete">if set to <c>true</c> the file was deleted; if set to <c>false</c> the file was removed.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectFileRemoved(IVsSccProject2 project, string filename, VSREMOVEFILEFLAGS flags)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us

            data.RemovePath(filename);

            if (!IsActive)
                return; // Let the other SCC package manage it

            MarkDirty(filename, true);

            using (SvnSccContext svn = new SvnSccContext(Context))
            {
                SvnStatusEventArgs status = svn.SafeGetStatus(filename);

                if (File.Exists(filename))
                {
                    // The file was only removed from the project. We should not touch it

                    // Some projects delete the file before (C#) and some after (C++) calling OnProjectFileRemoved
                    if (_delayedDelete == null)
                        _delayedDelete = new List<string>();

                    if (!_delayedDelete.Contains(filename))
                        _delayedDelete.Add(filename);

                    RegisterForSccCleanup();
                    return;
                }

                if (svn.IsUnversioned(status))
                    return;

                svn.SafeDeleteFile(filename);
            }
        }

        internal void OnBeforeRemoveDirectory(IVsSccProject2 project, string fullPath, out bool ok)
        {
            ok = true;
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us
            else if (!IsActive)
                return;

            if (_backupMap.ContainsKey(fullPath))
            {
                // Don't backup twice
                string oldBackup = _backupMap[fullPath];
                _backupMap.Remove(fullPath);
                using (SvnSccContext svn = new SvnSccContext(this))
                {
                    svn.DeleteDirectory(oldBackup);
                }
            }

            using (SvnSccContext svn = new SvnSccContext(this))
            {
                _backupMap.Add(fullPath, svn.MakeBackup(fullPath));
            }

            RegisterForSccCleanup();
        }

        /// <summary>
        /// Called when a directory is added to a project
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="directoryname">The directoryname.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectDirectoryAdded(IVsSccProject2 project, string directoryname, VSADDDIRECTORYFLAGS flags)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us

            // Add a directory like a folder but with an ending '\'
            data.AddPath(Path.GetFullPath(directoryname).TrimEnd('\\') + '\\');

            if (!IsActive)
                return;

            // Do nothing
        }

        /// <summary>
        /// Called when a directory is removed from a project
        /// </summary>
        /// <param name="sccProject">The SCC project.</param>
        /// <param name="directoryname">The directoryname.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectDirectoryRemoved(IVsSccProject2 project, string directoryname, VSREMOVEDIRECTORYFLAGS flags)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us

            // a directory can be added like a folder but with an ending '\'
            string dir = SvnTools.GetNormalizedFullPath(directoryname);
            data.RemovePath(dir);

            if (!IsActive)
                return;

            RegisterForSccCleanup();
        }

        /// <summary>
        /// Called just before a file in a project is renamed
        /// </summary>
        /// <param name="sccProject">The SCC project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="flags">The flags.</param>
        /// <param name="ok">if set to <c>true</c> [ok].</param>
        internal void OnBeforeProjectRenameFile(IVsSccProject2 project, string oldName, string newName, VSQUERYRENAMEFILEFLAGS flags, out bool ok)
        {
            ok = true;

            if (!_projectMap.ContainsKey(project))
                return; // Not managed by us

            if (!IsActive)
                return;

            using (SvnSccContext svn = new SvnSccContext(Context))
            {
                if (!svn.CouldAdd(newName, SvnNodeKind.File))
                {
                    ok = false;
                    return;
                }

                SvnStatusEventArgs status = svn.SafeGetStatus(oldName);

                if (svn.IsUnversioned(status))
                    return;
            }
        }

        internal void OnBeforeSolutionRenameFile(string oldName, string newName, VSQUERYRENAMEFILEFLAGS flags, out bool ok)
        {
            ok = true;
            if (!IsActive)
                return;

            using (SvnSccContext svn = new SvnSccContext(Context))
            {
                if (!svn.CouldAdd(newName, SvnNodeKind.File))
                {
                    ok = false;
                    return;
                }

                SvnStatusEventArgs status = svn.SafeGetStatus(oldName);

                if (svn.IsUnversioned(status))
                    return;
            }
        }

        /// <summary>
        /// Called when a file in a project is renamed
        /// </summary>
        /// <param name="sccProject">The SCC project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectRenamedFile(IVsSccProject2 project, string oldName, string newName, VSRENAMEFILEFLAGS flags)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us
            else
                data.CheckProjectRename(project, oldName, newName);

            data.RemovePath(oldName);
            data.AddPath(newName);

            if (!IsActive)
                return;

            using (SvnSccContext svn = new SvnSccContext(Context))
            {
                SvnStatusEventArgs status = svn.SafeGetStatus(oldName);

                if (!svn.IsUnversioned(status))
                {
                    if (!Directory.Exists(newName)) // Fails if the new name is a directory!
                        svn.SafeWcMoveFixup(oldName, newName);
                }

                MarkDirty(new string[] { oldName, newName }, true);
            }
        }

        internal void OnSolutionRenamedFile(string oldName, string newName, VSRENAMEFILEFLAGS flags)
        {
            if (!IsActive)
                return;

            _solutionDirectory = _solutionFile = null; // Get new data after this rename

            using (SvnSccContext svn = new SvnSccContext(Context))
            {
                SvnStatusEventArgs status = svn.SafeGetStatus(oldName);

                if (!svn.IsUnversioned(status))
                {
                    svn.SafeWcMoveFixup(oldName, newName);
                }

                MarkDirty(new string[] { oldName, newName }, true);
            }

            GetService<IFileStatusMonitor>().ScheduleGlyphUpdate(SolutionFilename);
        }

        /// <summary>
        /// Called just before a directory in a project is renamed
        /// </summary>
        /// <param name="sccProject">The SCC project.</param>
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
        /// <param name="sccProject">The SCC project.</param>
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


        void MarkDirty(string path, bool rescan)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();

            if (rescan)
                monitor.ScheduleSvnStatus(path);
            else
                monitor.ScheduleGlyphUpdate(path);
        }

        void MarkDirty(IEnumerable<string> paths, bool rescan)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();

            if (rescan)
                monitor.ScheduleSvnStatus(paths);
            else
                monitor.ScheduleGlyphUpdate(paths);

            monitor.ScheduleMonitor(paths);
        }


        #region ProjectFile
        internal SccProjectFile GetFile(string path)
        {
            SccProjectFile projectFile;

            if (!_fileMap.TryGetValue(path, out projectFile))
                _fileMap.Add(path, projectFile = new SccProjectFile(Context, path));

            return projectFile;
        }

        internal void RemoveFile(SccProjectFile file)
        {
            Debug.Assert(_fileMap[file.FullPath] == file);

            _fileMap.Remove(file.FullPath);
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

        /// <summary>
        /// Gets a list of all files contained within <paramref name="project"/>
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllFilesOf(Ankh.Selection.SvnProject project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            if (project.IsSolution)
            {
                string sf = SolutionFilename;

                if (sf != null)
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
                    yield return file;
            }
        }

        public IEnumerable<string> GetAllFilesOf(ICollection<SvnProject> projects)
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
                        files.Add(SolutionFilename, SolutionFilename);

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
                    yield return file;
                }
            }
        }

        public ICollection<string> GetAllFilesOfAllProjects()
        {
            List<string> files = new List<string>(_fileMap.Count + 1);

            if (SolutionFilename != null && !_fileMap.ContainsKey(SolutionFilename))
                files.Add(SolutionFilename);

            foreach (string file in _fileMap.Keys)
            {
                if (file[file.Length - 1] == '\\') // Don't return paths
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

            SccProjectFile file;
            if (!_fileMap.TryGetValue(path, out file))
            {
                return string.Equals(SolutionFilename, path, StringComparison.OrdinalIgnoreCase);
            }

            foreach (SccProjectData pd in file.GetOwnerProjects())
            {
                if (string.Equals(pd.ProjectFile, path, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
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

            throw new NotImplementedException();
        }

        private IEnumerable<SvnItem> GetSingleProjectRoots(SvnProject project)
        {
            SccProjectData pd;
            if(project.RawHandle == null || !_projectMap.TryGetValue(project.RawHandle, out pd))
                yield break;

            SvnItem projectRootItem = null;
            if(!string.IsNullOrEmpty(pd.ProjectDirectory))
            {
                projectRootItem = StatusCache[pd.ProjectDirectory];

                if(projectRootItem.IsVersioned)
                    yield return projectRootItem;
            }

            string file = pd.ProjectFile;

            if(string.IsNullOrEmpty(file) || !SvnItem.IsValidPath(file))
                yield break;

            SvnItem projectFileItem = StatusCache[file];

            if(projectFileItem.IsVersioned &&
                (projectRootItem == null || !projectFileItem.IsBelowPath(projectRootItem.FullPath)))
            {
                yield return projectFileItem;
            }
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

            #endregion
        }        
    }
}
