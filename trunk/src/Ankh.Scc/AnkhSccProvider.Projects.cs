using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;

using Ankh.Commands;
using Ankh.Scc.ProjectMap;
using Ankh.Selection;
using Ankh.Ids;
using Ankh.VS;
using System.IO;

namespace Ankh.Scc
{
    partial class AnkhSccProvider
    {
        readonly Dictionary<IVsSccProject2, SccProjectData> _projectMap = new Dictionary<IVsSccProject2, SccProjectData>();
        bool _managedSolution;
        List<string> _delayedDelete;
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
                return false;

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
                        if (p.IsSolutionFolder || p.IsWebSite)
                        {
                            p.SetManaged(managed);

                            if (managed)
                                p.SccProject.SccGlyphChanged(0, null, null, null);
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

            IAnkhSolutionExplorerWindow window = GetService<IAnkhSolutionExplorerWindow>();

            if (window != null)
                window.EnableAnkhIcons(true);

            foreach (SccProjectData data in _projectMap.Values)
            {
                if (data.IsSolutionFolder || data.IsWebSite)
                {
                    // Solution folders don't save their Scc management state
                    // We let them follow the solution settings

                    if(_managedSolution)
                        data.SetManaged(true);

                    // Flush the glyph cache of solution folders
                    // (Well known VS bug: Initially clear)
                    data.SccProject.SccGlyphChanged(0, null, null, null);
                }
            }

            IPendingChangesManager mgr = GetService<IPendingChangesManager>();
            if (mgr != null && mgr.IsActive)
                mgr.FullRefresh(false);
        }

        /// <summary>
        /// Called by ProjectDocumentTracker just before a solution is closed
        /// </summary>
        /// <remarks>At this time the closing can not be canceled.</remarks>
        internal void OnStartedSolutionClose()
        {
            if (IsActive)
            {
                IAnkhSolutionExplorerWindow window = GetService<IAnkhSolutionExplorerWindow>();

                if (window != null)
                    window.EnableAnkhIcons(false);
            }

            foreach (SccProjectData pd in _projectMap.Values)
            {
                pd.Hook(false);
            }

#if !DEBUG
            // Skip file by file cleanup of the project<-> file mapping
            // Should proably always be enabled around the release of AnkhSVN 2.0
            _projectMap.Clear();
            _fileMap.Clear();
#endif
        }

        /// <summary>
        /// Called by ProjectDocumentTracker when a solution is closed
        /// </summary>
        internal void OnSolutionClosed()
        {
            Debug.Assert(_projectMap.Count == 0);
            Debug.Assert(_fileMap.Count == 0);

            _projectMap.Clear();
            _fileMap.Clear();
            _unreloadable.Clear();
            StatusCache.ClearCache();

            // Clear status for reopening solution
            _managedSolution = false;
            _isDirty = false;

            IPendingChangesManager mgr = GetService<IPendingChangesManager>();
            if (mgr != null)
                mgr.Clear();
        }

        /// <summary>
        /// Called by ProjectDocumentTracker when a scc-capable project is loaded
        /// </summary>
        /// <param name="project"></param>
        internal void OnProjectLoaded(IVsSccProject2 project)
        {
            if (!_projectMap.ContainsKey(project))
                _projectMap.Add(project, new SccProjectData(Context, project));
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
                _projectMap.Add(project, data = new SccProjectData(Context, project));

            if(data.IsSolutionFolder || data.IsWebSite)
            {
                // Solution folders are projects without Scc state
                // Web sites are Solution-only projects with scc state
                data.SccProject.SccGlyphChanged(0, null, null, null);

                if (_managedSolution)
                {
                    // We let them follow the solution settings (See OnSolutionOpen() for the not added case
                    if (added)
                        data.SetManaged(true);

                }
            }

            _syncMap = true;
            RegisterForSccCleanup();
        }

        /// <summary>
        /// Called when a project is explicitly refreshed
        /// </summary>
        /// <param name="project"></param>
        internal void RefreshProject(IVsSccProject2 project)
        {
            SccProjectData data;
            if(_projectMap.TryGetValue(project, out data))
            {
                data.Reload();
            }
        }

        internal bool TrackProjectChanges(IVsSccProject2 project)
        {
            // We can be called with a null project
            SccProjectData data;
            if (project != null && _projectMap.TryGetValue(project, out data))
            {
                return data.TrackProjectChanges(); // Allows temporary disabling changes
            }

            return false;
        }

        /// <summary>
        /// Called by ProjectDocumentTracker when a scc-capable project is closed
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="removed">if set to <c>true</c> the project is being removed or unloaded from the solution.</param>
        internal void OnProjectClosed(IVsSccProject2 project, bool removed)
        {
            SccProjectData data;

            if(_projectMap.TryGetValue(project, out data))
            {
                data.OnClose();
                _projectMap.Remove(project);
            }
        }

        bool _registeredSccCleanup;
        internal void OnSccCleanup(CommandEventArgs e)
        {
            _registeredSccCleanup = false;

            if (_syncMap)
            {
                _syncMap = false;

                foreach (SccProjectData pd in _projectMap.Values)
                    pd.Load();
            }

            if (_delayedDelete != null)
            {
                List<string> files = _delayedDelete;
                _delayedDelete = null;

                using (SvnSccContext svn = new SvnSccContext(Context))
                {
                    foreach (string file in files)
                    {
                        if (!File.Exists(file))
                        {
                            svn.SafeDeleteFile(file);
                            MarkDirty(file, true);
                        }
                    }
                }
            }
        }

        void RegisterForSccCleanup()
        {
            if (_registeredSccCleanup)
                return;

            IAnkhCommandService cmd = Context.GetService<IAnkhCommandService>();

            if (cmd != null && cmd.PostExecCommand(AnkhCommand.SccFinishTasks))
                _registeredSccCleanup = true;
        }
    }
}
