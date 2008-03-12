using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using AnkhSvn.Ids;

namespace Ankh.Scc
{
    class SccProjectData
    {
        readonly IVsSccProject2 _project;
        bool _isManaged;
        bool _isRegistered;

        public SccProjectData(IVsSccProject2 project)
        {
            if (project == null)
                throw new ArgumentNullException("project");

            _project = project;
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
    }
}
