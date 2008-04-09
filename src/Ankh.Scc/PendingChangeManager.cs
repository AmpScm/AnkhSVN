using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using AnkhSvn.Ids;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    partial class PendingChangeManager : AnkhService, IPendingChangesManager
    {
        bool _isActive;
        public PendingChangeManager(IAnkhServiceProvider context)
            : base(context)
        {

        }

        #region IPendingChangesManager Members

        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                if (value != _isActive)
                {
                    _isActive = value;
                    OnIsActiveChanged(new PendingChangeEventArgs(this, null));
                }
            }
        }

        readonly HybridCollection<string> _toRefresh = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
        bool _fullRefresh;
        internal void OnTickRefresh()
        {
            List<string> toRefresh;
            bool fullRefresh = true;
            lock (_toRefresh)
            {
                _refreshScheduled = false;

                if (_fullRefresh)
                {
                    fullRefresh = true;
                    toRefresh = null;
                }
                else
                {
                    fullRefresh = false;
                    toRefresh = new List<string>(_toRefresh);
                }
                _toRefresh.Clear();
                _fullRefresh = false;
            }

            if (fullRefresh)
                InnerRefresh();
            else
            {
                foreach (string path in toRefresh)
                {
                    ItemRefresh(path);
                }
            }
        }        

        bool _refreshScheduled;
        void ScheduleRefresh()
        {
            lock (_toRefresh)
            {
                if (!_refreshScheduled)
                {
                    IAnkhCommandService cmd = GetService<IAnkhCommandService>();

                    if (cmd != null && cmd.PostExecCommand(AnkhCommand.TickRefreshPendingTasks))
                        _refreshScheduled = true;
                }
            }
        }

        public void FullRefresh(bool clearStateCache)
        {
            PendingChangeEventArgs ee = new PendingChangeEventArgs(this, null);
            _pendingChanges.Clear();
            OnListFlushed(ee);

            lock (_toRefresh)
            {
                _fullRefresh = true;
                _toRefresh.Clear();

                ScheduleRefresh();
            }
        }

        public void Refresh(string path)
        {
            if (path != null && string.IsNullOrEmpty(path)) // path == ""
                throw new ArgumentNullException("path");

            lock (_toRefresh)
            {
                if (path == null)
                    _fullRefresh = true;
                else if (!_fullRefresh && !_toRefresh.Contains(path))
                    _toRefresh.Add(path);

                ScheduleRefresh();
            }
        }

        public void Refresh(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            lock (_toRefresh)
            {
                if (!_fullRefresh)
                {
                    foreach (string path in paths)
                    {
                        if (!string.IsNullOrEmpty(path) && !_toRefresh.Contains(path))
                            _toRefresh.Add(path);
                    }
                }

                ScheduleRefresh();
            }
        }

        /// <summary>
        /// Gets the <see cref="Ankh.Scc.PendingChange"/> with the specified full path.
        /// </summary>
        /// <value></value>
        public PendingChange this[string fullPath]
        {
            get
            {
                PendingChange pc;

                if (_pendingChanges.TryGetValue(fullPath, out pc))
                {
                    return pc;
                }
                return null;
            }
        }

        /// <summary>
        /// Raised when a pending change item has been added
        /// </summary>
        /// <remarks>Handlers should also hook the <see cref="FullRefresh"/> event</remarks>
        public event EventHandler<PendingChangeEventArgs> Added;

        /// <summary>
        /// Raises the <see cref="E:Added"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Scc.PendingChangeEventArgs"/> instance containing the event data.</param>
        void OnAdded(PendingChangeEventArgs e)
        {
            if (Added != null)
                Added(this, e);
        }

        /// <summary>
        /// Raised when a pending change item has been removed
        /// </summary>
        /// <remarks>Handlers should also hook the <see cref="FullRefresh"/> event</remarks>
        public event EventHandler<PendingChangeEventArgs> Removed;

        /// <summary>
        /// Raises the <see cref="E:Removed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Scc.PendingChangeEventArgs"/> instance containing the event data.</param>
        void OnRemoved(PendingChangeEventArgs e)
        {
            if (Removed != null)
                Removed(this, e);
        }

        /// <summary>
        /// Raised when the properties of a pending change have changed
        /// </summary>
        /// <remarks>Handlers should also hook the <see cref="FullRefresh"/> event</remarks>
        public event EventHandler<PendingChangeEventArgs> Changed;

        /// <summary>
        /// Raises the <see cref="E:Changed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Scc.PendingChangeEventArgs"/> instance containing the event data.</param>
        void OnChanged(PendingChangeEventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        /// <summary>
        /// Raised when the complete pending change state has been flushed; All listeners should
        /// use GetAll() to get a new initial state
        /// </summary>
        /// <remarks>Handlers should also hook the <see cref="FullRefresh"/> event</remarks>
        public event EventHandler<PendingChangeEventArgs> ListFlushed;

        /// <summary>
        /// Raises the <see cref="E:ListFlushed"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Scc.PendingChangeEventArgs"/> instance containing the event data.</param>
        void OnListFlushed(PendingChangeEventArgs e)
        {
            if (ListFlushed != null)
                ListFlushed(this, e);

        }

        /// <summary>
        /// Raised when the pending changes manager is activated or disabled
        /// </summary>
        public event EventHandler<PendingChangeEventArgs> IsActiveChanged;

        /// <summary>
        /// Raises the <see cref="E:IsActiveChanged"/> event.
        /// </summary>
        /// <param name="e">The <see cref="Ankh.Scc.PendingChangeEventArgs"/> instance containing the event data.</param>
        private void OnIsActiveChanged(PendingChangeEventArgs e)
        {
            if (IsActiveChanged != null)
                IsActiveChanged(this, e);
        }

        #endregion

        #region IPendingChangesManager Members

        // Not used at this time
        public event EventHandler<PendingChangeEventArgs> RefreshStarted;

        protected void OnRefreshStarted(PendingChangeEventArgs e)
        {
            if (RefreshStarted != null)
                RefreshStarted(this, e);
        }

        public event EventHandler<PendingChangeEventArgs> RefreshCompleted;

        protected void OnRefreshCompleted(PendingChangeEventArgs e)
        {
            if (RefreshCompleted != null)
                RefreshCompleted(this, e);
        }

        #endregion
    }
}
