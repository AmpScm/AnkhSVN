using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using AnkhSvn.Ids;
using Microsoft.VisualStudio;
using Ankh.Selection;
using System.Diagnostics;

namespace Ankh.Scc.ProjectMap
{
    /// <summary>
    /// Enum of project types with workarounds
    /// </summary>
    enum SccProjectType
    {
        Normal,
        SolutionFolder,
        WebSite,
    }

    [DebuggerDisplay("Project={ProjectFile}, ProjectType={_projectType}")]
    class SccProjectData
    {
        readonly IAnkhServiceProvider _context;
        readonly IVsSccProject2 _project;
        readonly SccProjectType _projectType;
        readonly SccProjectFileCollection _files;
        bool _isManaged;
        bool _isRegistered;
        bool _loaded;
        string _projectFile;
        bool _checkedProjectFile;
        AnkhSccProvider _scc;
        SvnProject _svnProjectInstance;

        public SccProjectData(IAnkhServiceProvider context, IVsSccProject2 project)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (project == null)
                throw new ArgumentNullException("project");

            _context = context;
            _project = project;
            _projectType = GetProjectType(project);
            _files = new SccProjectFileCollection();
        }

        public IVsSccProject2 Project
        {
            get { return _project; }
        }

        public bool IsManaged
        {
            get { return _isManaged; }

            // Called by IVsSccManager.RegisterSccProject() when we were preregistered
            internal set { _isManaged = value; }
        }

        public string ProjectFile
        {
            get
            {
                if (!_checkedProjectFile)
                {
                    _checkedProjectFile = true;
                    ISccProjectWalker walker = _context.GetService<ISccProjectWalker>();

                    if (walker != null)
                    {
                        foreach (string file in walker.GetSccFiles(Project, VSConstants.VSITEMID_ROOT, ProjectWalkDepth.Empty))
                        {
                            _projectFile = file;
                            break;
                        }
                    }
                }

                return _projectFile;
            }
        }

        public SvnProject SvnProject
        {
            get
            {
                if (_svnProjectInstance == null)
                    _svnProjectInstance = new SvnProject(ProjectFile, Project);

                return _svnProjectInstance;
            }
        }

        /// <summary>
        /// Checks whether the project might have been renamed
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="oldName">The old name.</param>
        /// <param name="newName">The new name.</param>
        internal void CheckProjectRename(IVsSccProject2 project, string oldName, string newName)
        {
            if (_checkedProjectFile && oldName == ProjectFile)
            {
                _checkedProjectFile = false;
                _projectFile = null;
                _svnProjectInstance = null;
            }
        }

        /// <summary>
        /// Gets a boolean indicating whether the project thinks its registered on the scc provider
        /// </summary>
        /// <remarks>
        /// True if the project registered itself on the Scc provider, otherwise false
        /// </remarks>
        public bool IsRegistered
        {
            get { return _isRegistered; }

            internal set { _isRegistered = value; }
        }

        public bool IsSolutionFolder
        {
            get { return _projectType == SccProjectType.SolutionFolder; }
        }

        public bool IsWebSite
        {
            get { return _projectType == SccProjectType.WebSite; }
        }

        internal void SetManaged(bool managed)
        {
            if (managed == IsManaged)
                return;

            if (managed)
                Marshal.ThrowExceptionForHR(Project.SetSccLocation("Svn", "Svn", "Svn", AnkhId.SubversionSccName));
            else
            {
                // The managed package framework assumes empty strings for clearing; null will fail there
                Marshal.ThrowExceptionForHR(Project.SetSccLocation("", "", "", ""));
            }

            IsManaged = managed;
        }

        internal void OnClose()
        {
            while (_files.Count > 0)
            {
                SccProjectFileReference r = _files[0];
                while (r.ReferenceCount > 0)
                {
                    r.ReleaseReference();
                }
            }
            _loaded = false;
        }

        bool _inLoad;
        internal void Load()
        {
            if (_loaded)
                return;

            _inLoad = true;
            try
            {
                Debug.Assert(_files.Count == 0);
                _loaded = true;

                ISccProjectWalker walker = _context.GetService<ISccProjectWalker>();

                if (walker != null)
                {
                    foreach (string file in walker.GetSccFiles(_project, VSConstants.VSITEMID_ROOT, ProjectWalkDepth.AllDescendantsInHierarchy))
                    {
                        AddPath(file);
                    }
                }

                _project.SccGlyphChanged(0, null, null, null);
            }
            finally
            {
                _inLoad = false;
            }
        }

        internal void Reload()
        {
            OnClose();
            Load();
        }

        internal bool TrackProjectChanges()
        {
            return !_inLoad;
        }

        public IEnumerable<string> GetAllFiles()
        {
            foreach (SccProjectFileReference r in _files)
            {
                yield return r.Filename;
            }
        }

        internal void AddPath(string path)
        {
            if (_files.Contains(path))
                _files[path].AddReference();
            else
                _files.Add(new SccProjectFileReference(_context, this, Scc.GetFile(path)));
        }

        internal void RemoveFile(string path)
        {
            if (!_files.Contains(path))
                throw new ArgumentOutOfRangeException("path");

            _files[path].ReleaseReference();
        }

        #region File list management code
        internal void InvokeRemoveReference(SccProjectFileReference sccProjectFileReference)
        {
            _files.Remove(sccProjectFileReference);
        }
        #endregion


        #region Helper code

        protected AnkhSccProvider Scc
        {
            get { return _scc ?? (_scc = _context.GetService<AnkhSccProvider>()); }
        }

        /// <summary>
        /// Checks whether the specified project is a solution folder
        /// </summary>
        private static readonly Guid _solutionFolderProjectId = new Guid("2150e333-8fdc-42a3-9474-1a3956d46de8");
        private static readonly Guid _websiteProjectId = new Guid("e24c65dc-7377-472b-9aba-bc803b73c61a");
        static SccProjectType GetProjectType(IVsSccProject2 project)
        {
            IPersistFileFormat pFileFormat = project as IPersistFileFormat;
            if (pFileFormat != null)
            {
                Guid guidClassID;
                if (VSConstants.S_OK != pFileFormat.GetClassID(out guidClassID))
                    return SccProjectType.Normal;

                if (guidClassID == _solutionFolderProjectId)
                    return SccProjectType.SolutionFolder;
                else if (guidClassID == _websiteProjectId)
                    return SccProjectType.SolutionFolder;
            }

            return SccProjectType.Normal;
        }
        #endregion                
    
        /// <summary>
        /// Determines whether the project contains the specified file
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>
        /// 	<c>true</c> if the project contains the file; otherwise, <c>false</c>.
        /// </returns>
        internal bool ContainsFile(string path)
        {
            return _files.Contains(path);
        }

        internal void OnFileOpen(string file, uint itemId)
        {
            if (_files.Contains(file))
            {
                _files[file].OnFileOpen(itemId);
            }
        }

        internal void OnFileClose(string file, uint itemId)
        {
            if (_files.Contains(file))
            {
                _files[file].OnFileClose(itemId);
            }
        }        
    }
}
