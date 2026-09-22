using System;
using Ankh.Commands;
using Ankh.Scc;
using Ankh.Services;
using Ankh.UI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.Services
{
    [GlobalService(typeof(StartupRefreshService))]
    sealed class StartupRefreshService : AnkhService, IAnkhIdleProcessor, IVsSelectionEvents
    {
        bool _started;
        bool _themeRefreshed;
        bool _statusPending = true;
        AnkhServiceEvents _events;
        IAnkhPackage _package;
        IVsMonitorSelection _selectionMonitor;
        uint _selectionCookie;
        bool _selectionEventsAdvised;
        static readonly Guid SolutionExplorerGuid = new Guid(ToolWindowGuids.SolutionExplorer);

        public StartupRefreshService(IAnkhServiceProvider context) : base(context) { }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _events = GetService<AnkhServiceEvents>();
            _events.RuntimeStarted += OnStarted;
            _events.SolutionOpened += OnStatusNeeded;
            _events.SccProviderActivated += OnStatusNeeded;
            _package = GetService<IAnkhPackage>();
            _package.RegisterIdleProcessor(this);
            EnsureSelectionEvents();
        }

        void OnStarted(object sender, EventArgs e) { _started = true; }
        void OnStatusNeeded(object sender, EventArgs e) { _statusPending = true; }

        void EnsureSelectionEvents()
        {
            if (_selectionEventsAdvised)
                return;

            if (_selectionMonitor == null)
                _selectionMonitor = GetService<IVsMonitorSelection>(typeof(SVsShellMonitorSelection));

            if (_selectionMonitor == null)
                return;

            uint cookie;
            if (VSErr.Succeeded(_selectionMonitor.AdviseSelectionEvents(this, out cookie)))
            {
                _selectionCookie = cookie;
                _selectionEventsAdvised = true;
            }
        }

        internal static bool IsSolutionExplorerFrame(IVsWindowFrame frame)
        {
            if (frame == null)
                return false;

            Guid persistenceSlot;
            return VSErr.Succeeded(frame.GetGuidProperty(
                       (int)__VSFPROPID.VSFPROPID_GuidPersistenceSlot,
                       out persistenceSlot))
                   && persistenceSlot == SolutionExplorerGuid;
        }

        public int OnElementValueChanged(uint elementid, object varValueOld, object varValueNew)
        {
            if (elementid == (uint)VSConstants.VSSELELEMID.SEID_WindowFrame
                && IsSolutionExplorerFrame(varValueNew as IVsWindowFrame))
            {
                // Defer the work to idle so merely activating Solution Explorer
                // never blocks the shell, and repeated activation notifications
                // before idle naturally coalesce into one refresh.
                _statusPending = true;
            }

            return VSConstants.S_OK;
        }

        public int OnSelectionChanged(
            IVsHierarchy pHierOld,
            uint itemidOld,
            IVsMultiItemSelect pMISOld,
            ISelectionContainer pSCOld,
            IVsHierarchy pHierNew,
            uint itemidNew,
            IVsMultiItemSelect pMISNew,
            ISelectionContainer pSCNew)
        {
            return VSConstants.S_OK;
        }

        public int OnCmdUIContextChanged(uint dwCmdUICookie, int fActive)
        {
            return VSConstants.S_OK;
        }

        public void OnIdle(AnkhIdleArgs e)
        {
            if (!_started || e.Priority)
                return;

            EnsureSelectionEvents();

            if (_themeRefreshed && !_statusPending)
                return;

            IAnkhCommandStates states = GetService<IAnkhCommandStates>();
            if (states == null || !states.UIShellAvailable)
                return;

            if (!_themeRefreshed)
            {
                IVsShell shell = GetService<IVsShell>(typeof(SVsShell));
                if (shell == null)
                    return;

                object initialized;
                // Older shells need only the non-zombie check above. Newer
                // shells expose a separate completion signal; query it so a
                // package loaded after the notification is handled as well.
                if (VSErr.Succeeded(shell.GetProperty(-9053, out initialized))
                    && initialized is bool && !(bool)initialized)
                    return;

                _themeRefreshed = true;
                IAnkhServiceEvents events = GetService<IAnkhServiceEvents>();
                events.OnThemeChanged(EventArgs.Empty);
                events.OnUIShellActivate(EventArgs.Empty);
            }

            if (!_statusPending || !states.SccProviderActive)
                return;

            IProjectFileMapper mapper = GetService<IProjectFileMapper>();
            IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();
            IPendingChangesManager pending = GetService<IPendingChangesManager>();
            IAnkhOpenDocumentTracker documents = GetService<IAnkhOpenDocumentTracker>();
            if (mapper == null || monitor == null || pending == null || documents == null)
                return;

            _statusPending = false;
            // Refresh the entire solution, independent of the current selection.
            // The monitor schedules status work and its dependent glyph updates.
            pending.FullRefresh(true);
            monitor.ScheduleSvnStatus(mapper.GetAllFilesOfAllProjects());
            monitor.ScheduleGlyphOnlyUpdate(mapper.GetAllSccProjects());
            documents.RefreshDirtyState();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_events != null)
                {
                    _events.RuntimeStarted -= OnStarted;
                    _events.SolutionOpened -= OnStatusNeeded;
                    _events.SccProviderActivated -= OnStatusNeeded;
                }
                if (_selectionEventsAdvised && _selectionMonitor != null)
                {
                    _selectionMonitor.UnadviseSelectionEvents(_selectionCookie);
                    _selectionEventsAdvised = false;
                }
                _selectionMonitor = null;
                if (_package != null)
                    _package.UnregisterIdleProcessor(this);
            }
            base.Dispose(disposing);
        }
    }
}
