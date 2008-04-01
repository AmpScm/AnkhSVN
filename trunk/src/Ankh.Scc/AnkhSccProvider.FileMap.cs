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

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
	partial class AnkhSccProvider : IProjectFileMapper
	{
        // ********************************************************
        // This file contains two very important features of the Scc provider:
        //  - The tracking of changes in the File <-> Project mapping (Many <-> Many)
        //  - The persistance of history on project add/remove/rename actions
        //
        readonly Dictionary<string, SccProjectFile> _fileMap = new Dictionary<string, SccProjectFile>(StringComparer.OrdinalIgnoreCase);
        bool _syncMap;

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

            MarkGlyphsDirty(data, filename);

            if (string.IsNullOrEmpty(fileOrigin))
                return; // Don't add new files to Subversion yet to allow case only renames, etc.

            SvnItem originItem = StatusCache[fileOrigin];

            if (!originItem.HasHistory)
                return; // The origin was new itself

            using (SvnSccContext svn = new SvnSccContext(Context))
            {
                Guid addRepos;
                Guid originRepos;
                if(!svn.TryGetRepositoryId(filename, out addRepos))
                    return; // Adding would fail, don't even try

                if (!svn.TryGetRepositoryId(fileOrigin, out originRepos))
                    return; // We can't copy it to another repository then where it came from..

                if(addRepos != originRepos)
                    return; // We can't copy it to another repository then where it came from..
                
                svn.SafeWcCopyFixup(fileOrigin, filename);
            }
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

            MarkGlyphsDirty(data, filename);
            MarkFilesDirty(filename);

            using (SvnSccContext svn = new SvnSccContext(Context))
            {
                SvnStatusEventArgs status = svn.SafeGetStatus(filename);
                
                if (File.Exists(filename))
                {
                    // The file was only removed from the project. We should not touch it

                    // Some projects delete the file before (C#) and some after (C++) calling OnProjectFileRemoved
                    if (_delayedDelete == null)
                        _delayedDelete = new List<string>();

                    if(!_delayedDelete.Contains(filename))
                        _delayedDelete.Add(filename);

                    RegisterForSccCleanup();
                    return;
                }

                if (svn.IsUnversioned(status))
                    return;

                svn.SafeDelete(filename);
            }
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

            if (IsActive)
            {
                // Should we add it to subversion now or can we wait until commit time to allow case renaming, etc.
            }
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

            // Add a directory like a folder but with an ending '\'
            data.RemovePath(Path.GetFullPath(directoryname).TrimEnd('\\') + '\\');

            if (IsActive)
            {
                // Should we add it to subversion now or can we wait until commit time to allow case renaming, etc.
            }
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

                MarkGlyphsDirty(data, newName); // Mark the glyphs dirty anyway

                if (svn.IsUnversioned(status))
                {
                    return; // Nothing to do
                }

                svn.SafeWcMoveFixup(oldName, newName);
                MarkFilesDirty(newName);
                MarkFilesDirty(oldName);
            }            
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

            if(!IsActive)
                return;

            data.RemovePath(Path.GetFullPath(oldName).TrimEnd('\\') + '\\');
            data.AddPath(Path.GetFullPath(newName).TrimEnd('\\') + '\\');
            MarkGlyphsDirty(data, newName);
        }

        /// <summary>
        /// Temporary
        /// </summary>
        /// <param name="filename"></param>
        private void MarkFilesDirty(string filename)
        {
            if(string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            if (StatusCache != null)
                StatusCache.MarkDirty(filename);
        }

        private void MarkGlyphsDirty(SccProjectData project, string filename)
        {
            List<SvnProject> projects = new List<SvnProject>();
            // TODO: We can probably do this smarter then the notifier; but for now it works
            if (project != null)
                projects.Add(project.SvnProject);

            if (filename != null)
            {
                SccProjectFile file;
                if (_fileMap.TryGetValue(filename, out file))
                {
                    foreach (SccProjectData pd in file.GetOwnerProjects())
                    {
                        projects.Add(pd.SvnProject);
                    }
                }
            }

            if(projects.Count > 0)
                Context.GetService<IProjectNotifier>().MarkDirty(projects);
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
            Debug.Assert(_fileMap[file.Filename] == file);

            _fileMap.Remove(file.Filename);
        }
        #endregion

        #region IProjectFileMapper Members

        public IEnumerable<Ankh.Selection.SvnProject> GetAllProjectsContaining(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            path = SvnTools.GetNormalizedFullPath(path);

            SccProjectFile file;
            if (!_fileMap.TryGetValue(path, out file))
                yield break;

            foreach(SccProjectData pd in file.GetOwnerProjects())
            {
                yield return pd.SvnProject;
            }
        }

        public IEnumerable<Ankh.Selection.SvnProject> GetAllProjectsContaining(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            Hashtable projects = new Hashtable();
            foreach (string path in paths)
            {
                string nPath = SvnTools.GetNormalizedFullPath(path);

                SccProjectFile file;
                if (!_fileMap.TryGetValue(nPath, out file))
                    continue;

                foreach (SccProjectData pd in file.GetOwnerProjects())
                {
                    if (projects.Contains(pd))
                        continue;

                    projects.Add(pd, pd);

                    yield return pd.SvnProject;
                }
            }            
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

            // TODO: Keep some local copy of the solution filename
            ISelectionContext sc = GetService<ISelectionContext>();

            if (string.Equals(path, sc.SolutionFilename, StringComparison.OrdinalIgnoreCase))
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

            project = ResolveRawProject(project);

            IVsSccProject2 scc = project.RawHandle;
            SccProjectData data;

            if (scc == null || !_projectMap.TryGetValue(scc, out data))
                yield break;

            foreach(string file in data.GetAllFiles())
            {
                if (file[file.Length-1] != '\\') // Don't return paths
                    yield return file;
            }
        }

        public IEnumerable<string> GetAllFilesOf(ICollection<Ankh.Selection.SvnProject> projects)
        {
            SortedList<string, string> files = new SortedList<string,string>(StringComparer.OrdinalIgnoreCase);
            Hashtable handled = new Hashtable();
            foreach (SvnProject p in projects)
            {
                SvnProject project = ResolveRawProject(p);

                IVsSccProject2 scc = project.RawHandle;
                SccProjectData data;

                if (scc == null || !_projectMap.TryGetValue(scc, out data))
                    continue;

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
            List<string> files = new List<string>(_fileMap.Count+1);

            ISelectionContext selection = Context.GetService<ISelectionContext>();

            if (selection != null && !string.IsNullOrEmpty(selection.SolutionFilename))
                files.Add(selection.SolutionFilename);

            foreach(string file in _fileMap.Keys)
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

            if (_fileMap.TryGetValue(path, out file))
            {
                foreach (SccProjectFileReference fr in file.GetAllReferences())
                {
                    ProjectIconReference icon;
                    if (fr.TryGetIcon(out icon))
                        return icon;
                }
            }

            return null;
        }

        public ISvnProjectInfo GetProjectInfo(SvnProject project)
        {
            if (project == null || project.RawHandle == null)
                return null;

            SccProjectData pd;
            if(_projectMap.TryGetValue(project.RawHandle, out pd))
            {
                return new WrapProjectInfo(pd);
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Wrapper class providing a public api to the data contained within <see cref="SccProjectData"/>
        /// </summary>
        /// <remarks>Showing the raw properties of SccProjectData has side-effects. We wrap the class to hide this problem</remarks>
        sealed class WrapProjectInfo : ISvnProjectInfo
        {
            readonly SccProjectData _data;

            public WrapProjectInfo(SccProjectData data)
            {
                if (data == null)
                    throw new ArgumentNullException("data");

                _data = data;
            }

            public string ProjectName
            {
                get { return _data.ProjectName; }
            }

            public string ProjectDirectory
            {
                get { return _data.ProjectDirectory; }
            }            
        }
    }
}
