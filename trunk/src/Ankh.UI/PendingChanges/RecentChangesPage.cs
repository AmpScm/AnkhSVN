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
using System.Windows.Forms;
using SharpSvn;
using Ankh.Scc;
using Ankh.UI.PendingChanges.Synchronize;
using Ankh.UI.RepositoryExplorer;
using Ankh.Commands;
using Ankh.Configuration;

namespace Ankh.UI.PendingChanges
{
    partial class RecentChangesPage : PendingChangesPage
    {
        AnkhAction _recentChangesAction;
        bool _solutionExists;
        BusyOverlay _busyOverlay;
        private double _initialRefreshInterval = TimeSpan.FromSeconds(5).TotalMilliseconds;
        private double _refreshInterval;

        /// <summary>
        /// supported refresh intervals
        /// </summary>
        private static readonly int[] _intervals = new int[] {
            1, // 1 min
            5, // 5 mins
            10, // 10 mins
            15, // 15 mins
            30, // 30 mins
            60, // 1 hr
            120, // 2 hrs
        };

        public RecentChangesPage()
        {
            InitializeComponent();
        }

        #region PendingChangePage overrides

        protected override Type PageType
        {
            get
            {
                return typeof(RecentChangesPage);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            syncView.Context = Context;
            syncView.ColumnWidthChanged += new ColumnWidthChangedEventHandler(syncView_ColumnWidthChanged);
            IDictionary<string, int> widths = ConfigurationService.GetColumnWidths(GetType());
            syncView.SetColumnWidths(widths);

            _recentChangesAction = new AnkhAction(DoRefresh);

            // if solution is not open, don't auto-refresh
            IAnkhCommandStates commandState = Context.GetService<IAnkhCommandStates>();
            _solutionExists = (commandState != null && commandState.SolutionExists);
            RefreshIntervalConfigModified();
            HookHandlers();
        }

        protected void syncView_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            IDictionary<string, int> widths = syncView.GetColumnWidths();
            ConfigurationService.SaveColumnsWidths(GetType(), widths);
        }

        /// <summary>
        /// Populates Refresh checkbox and Refresh combo based on the settings
        /// </summary>
        private void ShowRecentChangeRefreshSettings()
        {
            checkBox1.CheckedChanged -= new EventHandler(OnRefreshIntervalModified);
            refreshCombo.SelectedIndexChanged -= new EventHandler(OnRefreshIntervalModified);
            // ensure current setting is read
            if (ReadRecentChangesRefreshInterval())
            {
                checkBox1.Checked = _refreshInterval > 0;
                if (_refreshInterval > 0)
                {
                    int ri_min = (int)TimeSpan.FromMilliseconds(_refreshInterval).TotalMinutes;
                    int index = 0;
                    int new_min = _intervals[index];
                    for (int i = 0; i < _intervals.Length; i++)
                    {
                        if (_intervals[i] <= ri_min)
                        {
                            new_min = _intervals[i];
                            index = i;
                        }
                    }
                    refreshCombo.SelectedIndex = index;

                    // if the current settings is not one of the offical settings, set it to the closest official setting
                    if (ri_min != new_min)
                    {
                        SaveRecentChangesRefreshInterval(Math.Max(new_min * 60, 0));
                    }
                }
            }
            checkBox1.CheckedChanged += new EventHandler(OnRefreshIntervalModified);
            refreshCombo.SelectedIndexChanged += new EventHandler(OnRefreshIntervalModified);
        }

        private void HookHandlers()
        {
            AnkhServiceEvents ev = Context.GetService<AnkhServiceEvents>();
            if (ev != null)
            {
                ev.SolutionClosed += new EventHandler(OnSolutionClosed);
                ev.SolutionOpened += new EventHandler(OnSolutionOpened);
            }
        }

        public override bool CanRefreshList
        {
            get { return true; }
        }

        public override void RefreshList()
        {
            try
            {
                DoRefresh(true);
            }
            catch (Exception ex)
            {
                IAnkhErrorHandler eh = Context.GetService<IAnkhErrorHandler>();
                if (eh != null && eh.IsEnabled(ex))
                    eh.OnError(ex);
                else
                    throw;
            }
        }

        #endregion

        void DoRefresh()
        {
            DoRefresh(false);
        }

        void DoRefresh(bool showProgressDialog)
        {
            IAnkhProjectLayoutService pls = Context.GetService<IAnkhProjectLayoutService>();
            List<SvnStatusEventArgs> resultList = new List<SvnStatusEventArgs>();
            List<string> roots = new List<string>(SvnItem.GetPaths(pls.GetUpdateRoots(null)));
            Dictionary<string, string> found = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            bool refreshFromList = false;
            try
            {
                if (showProgressDialog)
                {
                    IProgressRunner pr = Context.GetService<IProgressRunner>();
                    if (pr.RunModal("Retrieving remote status",
                        delegate(object sender, ProgressWorkerArgs e)
                        {
                            SvnStatusArgs sa = new SvnStatusArgs();
                            sa.RetrieveRemoteStatus = true;
                            DoFetchRecentChanges(e.Client, sa, roots, resultList, found);
                        }).Succeeded)
                    {
                        refreshFromList = true;
                    }
                }
                else
                {
                    ShowBusyIndicator();
                    using (SvnClient client = Context.GetService<ISvnClientPool>().GetClient())
                    {
                        SvnStatusArgs sa = new SvnStatusArgs();
                        sa.RetrieveRemoteStatus = true;
                        // don't throw error
                        // list is cleared in case of an error
                        // show a label in the list view???
                        sa.ThrowOnError = false;
                        DoFetchRecentChanges(client, sa, roots, resultList, found);
                        refreshFromList = true;
                    }
                }
            }
            finally
            {
                if (refreshFromList)
                {
                    OnRecentChangesFetched(resultList);
                }
                if (!showProgressDialog)
                {
                    HideBusyIndicator();
                }
            }
        }

        private void DoFetchRecentChanges(SvnClient client,
            SvnStatusArgs sa,
            List<string> roots,
            List<SvnStatusEventArgs> resultList,
            Dictionary<string, string> found)
        {
            foreach (string path in roots)
            {
                // TODO: Find some way to get this information safely in the status cache
                // (Might not be possible because of delays in network check)
                client.Status(path, sa,
                    delegate(object s, SvnStatusEventArgs stat)
                    {
                        if (IgnoreStatus(stat))
                            return; // Not a synchronization item
                        else if (found.ContainsKey(stat.FullPath))
                            return; // Already reported

                        stat.Detach();
                        resultList.Add(stat);
                        found.Add(stat.FullPath, "");
                    });
            }
        }

        void ShowBusyIndicator()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AnkhAction(ShowBusyIndicator));
                return;
            }

            if (_busyOverlay == null)
                _busyOverlay = new BusyOverlay(syncView, AnchorStyles.Bottom | AnchorStyles.Right);
            _busyOverlay.Show();
        }

        void HideBusyIndicator()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AnkhAction(HideBusyIndicator));
                return;
            }

            if (_busyOverlay != null)
                _busyOverlay.Hide();
        }

        static bool IgnoreStatus(SvnStatusEventArgs stat)
        {
            switch (stat.LocalContentStatus)
            {
                case SvnStatus.NotVersioned:
                case SvnStatus.Ignored:
                case SvnStatus.External: // External root will be handled inside
                    return (stat.RemoteContentStatus == SvnStatus.None);
                case SvnStatus.None:
                    // Hide remote locked files
                    return (stat.RemoteContentStatus == SvnStatus.None);
                default:
                    return false;
            }
        }

        private void RefreshFromList(List<SvnStatusEventArgs> resultList)
        {
            syncView.Items.Clear();
            if (resultList != null && resultList.Count > 0)
            {
                IFileStatusCache fs = Context.GetService<IFileStatusCache>();
                List<SynchronizeListItem> items = new List<SynchronizeListItem>(resultList.Count);
                foreach (SvnStatusEventArgs s in resultList)
                {
                    SvnItem item = fs[s.FullPath];

                    if (item == null)
                        return;

                    items.Add(new SynchronizeListItem(syncView, item, s));
                }

                syncView.Items.AddRange(items.ToArray());
            }
            updateTime.Text = string.Format(PCStrings.RefreshTimeX, DateTime.Now.ToShortTimeString());
        }

        /// <summary>
        /// Re-reads the refresh interval setting,
        /// Schedules a refresh if a sol is open and new setting is greater than 0,
        /// Unschedules otherwise.
        /// </summary>
        void ResetRefreshSchedule()
        {
            ReadRecentChangesRefreshInterval();
            double nextRefreshInterval = 0;
            if (_solutionExists // if a solution is not open, don't auto-refresh
                && _refreshInterval > 0
                )
            {
                nextRefreshInterval = _scheduledActionId > 0
                    ? _refreshInterval // cancel the scheduled refresh and reschedule
                    : _initialRefreshInterval; // refresh is enabled, schedule initial refresh
            }
            ScheduleRefresh(nextRefreshInterval);
        }

        int _scheduledActionId;

        /// <summary>
        /// Reschedules the refresh if auto-refresh is enabled (i.e. the given <paramref name="interval"/> is greater than 0).
        /// </summary>
        void ScheduleRefresh(double interval)
        {
            if (_scheduledActionId > 0)
            {
                Scheduler.RemoveTask(_scheduledActionId);
                _scheduledActionId = -1;
            }
            if (interval > 0)
            {
                _scheduledActionId = Scheduler.Schedule(TimeSpan.FromMilliseconds(interval), new AnkhAction(DoDoRefresh));
            }
        }

        /// <summary>
        /// Asynch refresh action execution
        /// </summary>
        void DoDoRefresh()
        {
            _recentChangesAction.BeginInvoke(null, null);
        }

        void OnRecentChangesFetched(List<SvnStatusEventArgs> resultList)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<List<SvnStatusEventArgs>>(OnRecentChangesFetched), resultList);
                return;
            }

            try
            {
                RefreshFromList(resultList);
            }
            finally
            {
                // schedule the next refresh
                ScheduleRefresh(_refreshInterval);
            }
        }

        /// <summary>
        /// Update the <c>_solutionExists</c> flag,
        /// Schedule a refresh if there is a global setting
        /// </summary>
        void OnSolutionOpened(object sender, EventArgs e)
        {
            _solutionExists = true;
            ResetRefreshSchedule();
        }

        /// <summary>
        /// Update the <c>_solutionExists</c> flag,
        /// Unschedule refresh if it is scheduled
        /// </summary>
        void OnSolutionClosed(object sender, EventArgs e)
        {
            _solutionExists = false;

            // clear the list
            RefreshFromList(null);
            ResetRefreshSchedule();
        }

        /// <summary>
        /// Handles Refresh checkbox "checked" and Refresh combo "selection" events
        /// </summary>
        void OnRefreshIntervalModified(object sender, EventArgs e)
        {
            bool enabled = checkBox1.Checked;
            int selectedIndex = refreshCombo.SelectedIndex;
            enabled &= selectedIndex > -1;
            bool resetSchedule = false;
            if (enabled)
            {
                int selected = 60 * ((selectedIndex >= 0 && selectedIndex < _intervals.Length) ? _intervals[selectedIndex] : _intervals[_intervals.Length - 1]);
                if ((selected * 1000) != _refreshInterval)
                {
                    SaveRecentChangesRefreshInterval(selected);
                    resetSchedule = true;
                }
            }
            else
            {
                if (_refreshInterval > 0)
                {
                    SaveRecentChangesRefreshInterval(0);
                    resetSchedule = true;
                }
            }
            if (resetSchedule)
            {
                ResetRefreshSchedule();
            }
        }

        /// <summary>
        /// Updates the Refresh interval UI with the current configuration and
        /// resets the schedule
        /// </summary>
        internal void RefreshIntervalConfigModified()
        {
            ShowRecentChangeRefreshSettings();
            ResetRefreshSchedule();
        }

        /// <summary>
        /// Reads configuration setting into <code>_refreshInterval</code> member
        /// </summary>
        /// <returns>true if config is changed, false otherwise</returns>
        bool ReadRecentChangesRefreshInterval()
        {
            bool result = false;
            Ankh.Configuration.AnkhConfig config = Config;
            double newInterval = config.RecentChangesRefreshInterval * 1000.0;
            newInterval = Math.Max(0, newInterval);
            if (newInterval == _refreshInterval)
            {
                result = false;
            }
            else
            {
                _refreshInterval = newInterval;
                result = true;
            }
            return result;
        }

        /// <summary>
        /// saves the new setting into configuration
        /// </summary>
        /// <param name="seconds">new refresh interval in seconds</param>
        private void SaveRecentChangesRefreshInterval(int seconds)
        {
            AnkhConfig cfg = Config;
            cfg.RecentChangesRefreshInterval = seconds;
            ConfigSvc.SaveConfig(cfg);
        }

        private IAnkhScheduler _scheduler;
        IAnkhScheduler Scheduler
        {
            get { return _scheduler ?? (_scheduler = Context.GetService<IAnkhScheduler>()); }
        }

        private IAnkhConfigurationService _configSvc;
        IAnkhConfigurationService ConfigSvc
        {
            get { return _configSvc ?? (_configSvc = Context.GetService<IAnkhConfigurationService>()); }
        }

        Ankh.Configuration.AnkhConfig Config
        {
            get { return ConfigSvc.Instance; }
        }
    }
}
