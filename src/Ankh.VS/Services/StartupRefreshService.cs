using System;
using System.Diagnostics;
using Ankh.Commands;
using Ankh.Scc;
using Ankh.Services;
using Ankh.UI;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;

namespace Ankh.VS.Services
{
    [GlobalService(typeof(StartupRefreshService))]
    sealed class StartupRefreshService : AnkhService, IAnkhIdleProcessor, IVsBroadcastMessageEvents
    {
        bool _started;
        bool _themeRefreshed;
        bool _statusPending = true;
        AnkhServiceEvents _events;
        IAnkhPackage _package;
        IVsShell _shell;
        uint _broadcastCookie;
        bool _broadcastAdvised;
        bool _applicationInactive;
        bool _activationPending;
        long _activationDue;
        long _lastRefresh = long.MinValue;
        readonly Func<long> _milliseconds;

        // Coalesce rapid application switches without losing the final catch-up.
        internal const long ActivationDelayMilliseconds = 500;
        internal const long RefreshIntervalMilliseconds = 2000;

        public StartupRefreshService(IAnkhServiceProvider context)
            : this(context, () => (long)(Stopwatch.GetTimestamp() * (1000.0 / Stopwatch.Frequency))) { }

        internal StartupRefreshService(IAnkhServiceProvider context, Func<long> milliseconds)
            : base(context)
        {
            _milliseconds = milliseconds;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            _events = GetService<AnkhServiceEvents>();
            _events.RuntimeStarted += OnStarted;
            _events.SolutionOpened += OnStatusNeeded;
            _events.SccProviderActivated += OnStatusNeeded;
            _events.SolutionClosed += OnSolutionClosed;
            _package = GetService<IAnkhPackage>();
            _package.RegisterIdleProcessor(this);
            EnsureBroadcastEvents();
        }

        void OnStarted(object sender, EventArgs e) { _started = true; }
        void OnStatusNeeded(object sender, EventArgs e) { _statusPending = true; }

        void OnSolutionClosed(object sender, EventArgs e)
        {
            _statusPending = false;
            _activationPending = false;
        }

        void EnsureBroadcastEvents()
        {
            if (_broadcastAdvised)
                return;
            if (_shell == null)
                _shell = GetService<IVsShell>(typeof(SVsShell));
            if (_shell == null)
                return;
            uint cookie;
            if (VSErr.Succeeded(_shell.AdviseBroadcastMessages(this, out cookie)))
            {
                _broadcastCookie = cookie;
                _broadcastAdvised = true;
            }
        }

        public int OnBroadcastMessage(uint msg, IntPtr wParam, IntPtr lParam)
        {
            // Only crossing the application boundary needs a conservative catch-up:
            // an external SVN client can change properties/status without changing
            // file contents or sending shell notifications. Tool-window selection
            // inside VS must never invalidate the whole working-copy cache.
            const uint WM_ACTIVATEAPP = 0x001C;
            if (msg != WM_ACTIVATEAPP)
                return VSConstants.S_OK;

            if (wParam == IntPtr.Zero)
                _applicationInactive = true;
            else if (_applicationInactive)
            {
                _applicationInactive = false;
                _activationPending = true;
                _activationDue = _milliseconds() + ActivationDelayMilliseconds;
            }
            return VSConstants.S_OK;
        }

        public void OnIdle(AnkhIdleArgs e)
        {
            if (!_started || e.Priority)
                return;

            EnsureBroadcastEvents();

            if (_themeRefreshed && !_statusPending && !_activationPending)
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

            if (!states.SccProviderActive)
                return;

            if (!_statusPending)
            {
                if (!_activationPending || _applicationInactive)
                    return;
                if (!states.SolutionExists)
                {
                    _activationPending = false;
                    return;
                }
                long now = _milliseconds();
                if (now < _activationDue
                    || (_lastRefresh != long.MinValue && now - _lastRefresh < RefreshIntervalMilliseconds))
                    return;
            }

            IProjectFileMapper mapper = GetService<IProjectFileMapper>();
            IFileStatusMonitor monitor = GetService<IFileStatusMonitor>();
            IPendingChangesManager pending = GetService<IPendingChangesManager>();
            IAnkhOpenDocumentTracker documents = GetService<IAnkhOpenDocumentTracker>();
            if (mapper == null || monitor == null || pending == null || documents == null)
                return;

            _statusPending = false;
            _activationPending = false;
            _lastRefresh = _milliseconds();
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
                    _events.SolutionClosed -= OnSolutionClosed;
                }
                if (_broadcastAdvised && _shell != null)
                {
                    _shell.UnadviseBroadcastMessages(_broadcastCookie);
                    _broadcastAdvised = false;
                }
                _shell = null;
                if (_package != null)
                    _package.UnregisterIdleProcessor(this);
            }
            base.Dispose(disposing);
        }
    }
}
