// $Id$
//
// Copyright 2008-2009 The AnkhSVN Project
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using Ankh.Commands;
using Ankh.Selection;

namespace Ankh.Scc
{
    /// <summary>
    /// 
    /// </summary>
    [GlobalService(typeof(IPendingChangesManager))]
    partial class PendingChangeManager : AnkhService, IPendingChangesManager
    {
        readonly PendingChangeCollection _pendingChanges = new PendingChangeCollection();
        readonly ReadOnlyKeyedCollectionWithNotify<string, PendingChange> _pendingChangesRo;
        bool _isActive;
        bool _solutionOpen;
        public PendingChangeManager(IAnkhServiceProvider context)
            : base(context)
        {
            _pendingChangesRo = new ReadOnlyKeyedCollectionWithNotify<string, PendingChange>(_pendingChanges);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            IsActive = true;

            AnkhServiceEvents events = GetService<AnkhServiceEvents>();

            events.SolutionOpened += new EventHandler(OnSolutionOpened);
            events.SolutionClosed += new EventHandler(OnSolutionClosed);

            _solutionOpen = !string.IsNullOrEmpty(GetService<ISelectionContext>().SolutionFilename);
        }

        void OnSolutionOpened(object sender, EventArgs e)
        {
            _solutionOpen = true;
            ScheduleRefresh();
        }

        void OnSolutionClosed(object sender, EventArgs e)
        {
            _solutionOpen = false;
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

                    if (Change != null)
                    {
                        if (value)
                            Change.SvnItemsChanged += new EventHandler<SvnItemsEventArgs>(OnSvnItemsChanged);
                        else
                            Change.SvnItemsChanged -= new EventHandler<SvnItemsEventArgs>(OnSvnItemsChanged);
                    }

                    OnIsActiveChanged(new PendingChangeEventArgs(this, null));
                }
            }
        }

        ISvnItemChange _change;
        ISvnItemChange Change
        {
            get { return _change ?? (_change = GetService<ISvnItemChange>()); }
        }

        readonly HybridCollection<string> _toRefresh = new HybridCollection<string>(StringComparer.OrdinalIgnoreCase);
        readonly List<string> _toMonitor = new List<string>();
        bool _fullRefresh;
        internal void OnTickRefresh()
        {
            List<string> toRefresh;
            bool fullRefresh;
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

                _extraFiles.UniqueAddRange(_toMonitor);
                _toMonitor.Clear();
            }

            if (fullRefresh)
                InnerRefresh();
            else
            {
                using (BatchStartedEventArgs br = SmartRefresh(toRefresh.Count))
                {
                    foreach (string path in toRefresh)
                    {
                        if (br != null)
                            br.Tick();
                        ItemRefresh(path);
                    }
                }
            }
        }

        private BatchStartedEventArgs SmartRefresh(int itemsToRefresh)
        {
            if (itemsToRefresh < 32)
                return null;

            return BatchRefresh();
        }

        Ankh.UI.IAnkhThreadedWaitService _tws;
        Ankh.UI.IAnkhThreadedWaitService ThreadedWaitService
        {
            get { return _tws ?? (_tws = GetService<Ankh.UI.IAnkhThreadedWaitService>()); }
        }

        private BatchStartedEventArgs BatchRefresh()
        {
            BatchStartedEventArgs ba = new BatchStartedEventArgs(ThreadedWaitService);
            OnBatchUpdateStarted(ba);
            ba.Disposers += _pendingChanges.BatchUpdate().Dispose;
            return ba;
        }

        void OnSvnItemsChanged(object sender, SvnItemsEventArgs e)
        {
            lock (_toRefresh)
            {
                if (_fullRefresh || !_solutionOpen)
                    return;

                foreach (SvnItem item in e.ChangedItems)
                {
                    if (!_toRefresh.Contains(item.FullPath))
                        _toRefresh.Add(item.FullPath);
                }

                ScheduleRefresh();
            }
        }

        bool _refreshScheduled;
        void ScheduleRefresh()
        {
            lock (_toRefresh)
            {
                ScheduleRefreshPreLocked();
            }
        }

        IAnkhCommandService _commandService;
        IAnkhCommandService CommandService
        {
            get { return _commandService ?? (_commandService = GetService<IAnkhCommandService>()); }
        }

        void ScheduleRefreshPreLocked()
        {
            if (!_solutionOpen)
                return;

            if (!_refreshScheduled)
                CommandService.PostTickCommand(ref _refreshScheduled, AnkhCommand.TickRefreshPendingTasks);
        }

        public void FullRefresh(bool clearStateCache)
        {
            if (clearStateCache && Cache != null)
                Cache.ClearCache();

            PendingChangeEventArgs ee = new PendingChangeEventArgs(this, null);

            lock (_toRefresh)
            {
                _fullRefresh = true;
                _toRefresh.Clear();

                ScheduleRefreshPreLocked();
            }
        }

        public void Clear()
        {
            PendingChangeEventArgs ee = new PendingChangeEventArgs(this, null);

            lock (_toRefresh)
            {
                _toRefresh.Clear();
                _fullRefresh = false;
                _pendingChanges.Clear();
                _extraFiles.Clear();
            }
        }

        public void Refresh(string path)
        {
            if (path != null && string.IsNullOrEmpty(path)) // path == ""
                throw new ArgumentNullException("path");

            if (!_isActive || !_solutionOpen)
                return;

            lock (_toRefresh)
            {
                if (path == null)
                    _fullRefresh = true;
                else if (!_fullRefresh && !_toRefresh.Contains(path))
                    _toRefresh.Add(path);

                ScheduleRefreshPreLocked();
            }
        }

        public void Refresh(IEnumerable<string> paths)
        {
            if (paths == null)
                throw new ArgumentNullException("paths");

            if (!_isActive || !_solutionOpen)
                return;

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

                ScheduleRefreshPreLocked();
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

        public event EventHandler<BatchStartedEventArgs> BatchUpdateStarted;

        void OnBatchUpdateStarted(BatchStartedEventArgs e)
        {
            if (BatchUpdateStarted != null)
                BatchUpdateStarted(this, e);
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

        internal void ScheduleMonitor(string path)
        {
            lock (_toRefresh)
            {
                _toMonitor.Add(path);

                if (!_fullRefresh && !_toRefresh.Contains(path))
                    _toRefresh.Add(path);

                if (!_isActive || !_solutionOpen)
                    return;

                ScheduleRefreshPreLocked();
            }
        }

        internal void ScheduleMonitor(IEnumerable<string> paths)
        {
            lock (_toRefresh)
            {
                _toMonitor.AddRange(paths);

                if (!_isActive || !_solutionOpen)
                    return;

                _toRefresh.UniqueAddRange(paths);

                ScheduleRefreshPreLocked();
            }
        }

        internal void StopMonitor(string path)
        {
            lock (_toRefresh)
            {
                _extraFiles.Remove(path);

                if (!_isActive || !_solutionOpen)
                    return;

                if (!_fullRefresh && !_toRefresh.Contains(path))
                    _toRefresh.Add(path);

                ScheduleRefreshPreLocked();
            }
        }

        ReadOnlyKeyedCollectionWithNotify<string, PendingChange> IPendingChangesManager.PendingChanges
        {
            get { return _pendingChangesRo; }
        }
    }
}