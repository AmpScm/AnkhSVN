using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using AnkhSvn.Ids;
using Microsoft.VisualStudio;

namespace Ankh.Scc
{
    class SccProjectData
    {
        readonly IVsSccProject2 _project;
        readonly bool _isSolutionFolder;
        bool _isManaged;
        bool _isRegistered;

        public SccProjectData(IVsSccProject2 project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            _project = project;
            _isSolutionFolder = IsSolutionFolderProject(project);
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


        #region Helper code
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
