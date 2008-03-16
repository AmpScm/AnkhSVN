using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using AnkhSvn.Ids;
using Microsoft.VisualStudio;

namespace Ankh.Scc.ProjectMap
{
    class SccProjectData
    {
        readonly IAnkhServiceProvider _context;
        readonly IVsSccProject2 _project;
        readonly bool _isSolutionFolder;
        readonly SccProjectFileCollection _files;
        bool _isManaged;
        bool _isRegistered;
        bool _loaded;
        AnkhSccProvider _scc;

        public SccProjectData(IAnkhServiceProvider context, IVsSccProject2 project)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            else if (project == null)
                throw new ArgumentNullException("project");

            _context = context;
            _project = project;
            _isSolutionFolder = IsSolutionFolderProject(project);
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
            get { return _isSolutionFolder; }
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

        internal void Load()
        {
            if (_loaded)
                return;
            
            OnClose();
            _loaded = true;

            ISccProjectWalker walker = _context.GetService<ISccProjectWalker>();

            if (walker != null)
            {
                foreach (string file in walker.GetSccFiles(_project, VSConstants.VSITEMID_ROOT, ProjectWalkDepth.AllDescendants))
                {
                    AddPath(file);                    
                }
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
        static bool IsSolutionFolderProject(IVsSccProject2 project)
        {
            IPersistFileFormat pFileFormat = project as IPersistFileFormat;
            if (pFileFormat != null)
            {
                Guid guidClassID;
                if (VSConstants.S_OK == pFileFormat.GetClassID(out guidClassID)
                    && guidClassID == _solutionFolderProjectId)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion     
    }
}
