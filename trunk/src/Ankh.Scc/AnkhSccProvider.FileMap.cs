﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Scc.ProjectMap;
using System.IO;
using System.Diagnostics;
using System.Collections;
using Ankh.Selection;

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
        Dictionary<string, SccProjectFile> _fileMap = new Dictionary<string, SccProjectFile>(StringComparer.OrdinalIgnoreCase);

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

            if (IsActive)
            {
                // Should we add it to subversion now or can we wait till commit time
                if (!string.IsNullOrEmpty(fileOrigin))
                {
                    // We have a file origin; make sure it does not get lost
                }
            }
        }

        /// <summary>
        /// Called when a file is removed from a project
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="forDelete">if set to <c>true</c> the file was deleted; if set to <c>false</c> the file was removed.</param>
        /// <param name="flags">The flags.</param>
        internal void OnProjectFileRemoved(IVsSccProject2 project, string filename, bool forDelete, VSREMOVEFILEFLAGS flags)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us

            data.RemoveFile(filename);

            if (IsActive)
            {
                if (forDelete)
                {
                    // Was it in subversion -> Remove
                }                
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
            data.RemoveFile(Path.GetFullPath(directoryname).TrimEnd('\\') + '\\');

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

            // TODO: Is the file managed in Subversion: Verify renaming of more than casing
            if (oldName != newName && string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
            {
                ok = false; // For now just disallow casing only changes
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

            data.RemoveFile(oldName);
            data.AddPath(newName);

            if (IsActive)
            {
                // Was the file in subversion?
                // Fix history by doing the action subversion style
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

            data.RemoveFile(Path.GetFullPath(oldName).TrimEnd('\\') + '\\');
            data.AddPath(Path.GetFullPath(newName).TrimEnd('\\') + '\\');
        }

        #region ProjectFie
        internal SccProjectFile GetFile(string path)
        {
            SccProjectFile projectFile;

            if (!_fileMap.TryGetValue(path, out projectFile))
                _fileMap.Add(path, projectFile = new SccProjectFile(_context, path));

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

            path = Path.GetFullPath(path);

            SccProjectFile file;
            if (!_fileMap.TryGetValue(path, out file))
                yield break;

            foreach(SccProjectData pd in file.GetOwnerProjects())
            {
                yield return pd.SvnProject;
            }
        }

        public IEnumerable<Ankh.Selection.SvnProject> GetAllProjectsContaining(ICollection<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            Hashtable projects = new Hashtable();
            foreach (string path in paths)
            {
                SccProjectFile file;
                if (!_fileMap.TryGetValue(path, out file))
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
        /// Gets a list of all files contained within <paramref name="project"/>
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public IEnumerable<string> GetAllFilesOf(Ankh.Selection.SvnProject project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            project = ResolveRawProject(project);

            IVsSccProject2 scc = project.RawHandle as IVsSccProject2;
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

                IVsSccProject2 scc = project.RawHandle as IVsSccProject2;
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

            ISelectionContext selection = _context.GetService<ISelectionContext>();

            if (selection != null && !string.IsNullOrEmpty(selection.SolutionFilename))
                files.Add(selection.SolutionFilename);

            foreach(string file in files)
            {
                if (file[file.Length - 1] == '\\') // Don't return paths
                        continue;

                files.Add(file);
            }

            return files.ToArray();
        }

        #endregion

        #region IProjectFileMapper Members


        public SvnProject ResolveRawProject(SvnProject project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            if (project.RawHandle == null)
            {
                foreach (SccProjectData pd in _projectMap.Values)
                {
                    if (pd.IsSolutionFolder && pd.ContainsFile(project.FullPath))
                        return pd.SvnProject;
                }
            }

            return project;
        }

        #endregion
    }
}
