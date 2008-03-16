using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using Ankh.Selection;
using Microsoft.VisualStudio;
using Ankh.Commands;
using AnkhSvn.Ids;
using Microsoft.VisualStudio.OLE.Interop;

namespace Ankh.Scc
{
    partial class AnkhSccProvider
    {
        readonly Dictionary<IVsSccProject2, SccProjectData> _projectMap = new Dictionary<IVsSccProject2, SccProjectData>();
        bool _managedSolution;
        bool _isDirty;

        public void LoadingManagedSolution(bool asPrimarySccProvider)
        {
            // Called by the package when a solution is loaded which is marked as managed by us
            _managedSolution = asPrimarySccProvider;
        }

        public bool IsProjectManaged(SvnProject project)
        {
            if (!IsActive)
                return false;

            if (project == null)
                return _managedSolution;

            return IsProjectManagedRaw(project.RawHandle);
        }

        public bool IsProjectManagedRaw(object project)
        {
            if (!IsActive)
                return false;

            if (project == null)
                return _managedSolution;

            IVsSccProject2 sccProject = project as IVsSccProject2;

            if (sccProject == null)
            {
                return (project is IVsSolution) ? _managedSolution : false;
            }

            SccProjectData data;

            if (_projectMap.TryGetValue(sccProject, out data))
                return data.IsManaged;

            return false;
        }

        public void SetProjectManaged(SvnProject project, bool managed)
        {
            if (!IsActive)
                return; // Perhaps allow clearing management settings?

            if (project == null)
                SetProjectManagedRaw(null, managed);
            else
                SetProjectManagedRaw(project.RawHandle, managed);
        }

        public void SetProjectManagedRaw(object project, bool managed)
        {
            if (!IsActive)
                return;

            if (project == null)
            {
                // We are talking about the solution

                if (managed != _managedSolution)
                {
                    _managedSolution = managed;
                    IsSolutionDirty = true;

                    foreach (SccProjectData p in _projectMap.Values)
                    {
                        if (p.IsSolutionFolder)
                        {
                            p.SetManaged(managed);

                            if (managed)
                                p.Project.SccGlyphChanged(0, null, null, null);
                        }
                    }
                }
                return;
            }

            IVsSccProject2 sccProject = project as IVsSccProject2;

            if (sccProject == null)
            {
                if (project is IVsSolution)
                    SetProjectManagedRaw(null, managed);

                return;
            }

            SccProjectData data;

            if (!_projectMap.TryGetValue(sccProject, out data))
                return;

            if (managed == data.IsManaged)
                return; // Nothing to change

            data.SetManaged(managed);
        }

        public bool IsSolutionDirty
        {
            // TODO: Only return true if the solution was not previously managed by Ankh
            get { return _isDirty; }
            set { _isDirty = value; }
        }

        /// <summary>
        /// Called by ProjectDocumentTracker when a solution is opened 
        /// </summary>
        internal void OnSolutionOpened()
        {
            if (!IsActive)
                return;

            foreach (SccProjectData data in _projectMap.Values)
            {
                if (data.IsSolutionFolder)
                {
                    // Solution folders don't save their Scc management state
                    // We let them follow the solution settings

                    if(_managedSolution)
                        data.SetManaged(true);

                    // Flush the glyph cache of solution folders
                    // (Well known VS bug: Initially clear)
                    data.Project.SccGlyphChanged(0, null, null, null);
                }
            }
        }

        /// <summary>
        /// Called by ProjectDocumentTracker when a solution is closed
        /// </summary>
        internal void OnSolutionClosed()
        {
            Debug.Assert(_projectMap.Count == 0);

            _projectMap.Clear();
            StatusCache.ClearCache();

            // Clear status for reopening solution
            _managedSolution = false;
            _isDirty = false;
        }

        /// <summary>
        /// Called by ProjectDocumentTracker when a scc-capable project is loaded
        /// </summary>
        /// <param name="project"></param>
        internal void OnProjectLoaded(IVsSccProject2 project)
        {
            if (!_projectMap.ContainsKey(project))
                _projectMap.Add(project, new SccProjectData(project));
        }

        /// <summary>
        /// Called by ProjectDocumentTracker when a scc-capable project is opened
        /// </summary>
        /// <param name="project">The loaded project</param>
        /// <param name="added">The project was added after opening</param>
        internal void OnProjectOpened(IVsSccProject2 project, bool added)
        {
            SccProjectData data;
            if (!_projectMap.TryGetValue(project, out data))
                _projectMap.Add(project, data = new SccProjectData(project));

            if (_managedSolution && data.IsSolutionFolder)
            {
                // Solution folders are projects without Scc state
                // We let them follow the solution settings (See OnSolutionOpen() for the not added case
                if (added)
                    data.SetManaged(true);

                data.Project.SccGlyphChanged(0, null, null, null);
            }
        }

        /// <summary>
        /// Called by ProjectDocumentTracker when a scc-capable project is closed
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="removed">if set to <c>true</c> the project is being removed or unloaded from the solution.</param>
        internal void OnProjectClosed(IVsSccProject2 project, bool removed)
        {
            _projectMap.Remove(project);
        }

        bool _registeredSccCleanup;
        internal void OnSccCleanup(CommandEventArgs e)
        {
            _registeredSccCleanup = false;
        }

        void RegisterForSccCleanup()
        {
            if (_registeredSccCleanup)
                return;

            IVsUIShell shell = (IVsUIShell)_context.GetService(typeof(SVsUIShell));

            if(shell != null)
            {
                Guid ankhCommands = AnkhId.CommandSetGuid;
                object nil = null;

                if(ErrorHandler.Succeeded(shell.PostExecCommand(ref ankhCommands, (uint)AnkhCommand.SccFinishTasks, (uint)OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref nil)))
                    _registeredSccCleanup = true;
            }
        }
    }
}
