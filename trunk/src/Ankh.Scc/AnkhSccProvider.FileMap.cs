using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Ankh.Scc.ProjectMap;
using System.IO;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
	partial class AnkhSccProvider
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
        /// <param name="flags">The flags.</param>
        internal void OnProjectFileRemoved(IVsSccProject2 project, string filename, VSREMOVEFILEFLAGS flags)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                return; // Not managed by us

            data.RemoveFile(filename);

            if (IsActive)
            {
                // BH: Called by both remove and delete from project??

                // Was it in subversion -> Remove
                
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
        #endregion
    }
}
