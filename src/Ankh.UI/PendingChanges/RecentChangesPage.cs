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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using Ankh.Scc;
using Ankh.UI.PendingChanges.Synchronize;
using Ankh.VS;
using Ankh.UI.RepositoryExplorer;
using Ankh.Commands;
using Ankh.Configuration;

namespace Ankh.UI.PendingChanges
{
    partial class RecentChangesPage : PendingChangesPage
    {
        System.Timers.Timer _timer;
        AnkhAction _recentChangesAction;
        bool _solutionExists;
        BusyOverlay _busyOverlay;
        private double _initialRefreshInterval = TimeSpan.FromSeconds(5).TotalMilliseconds;
        private double _refreshInterval;

        public RecentChangesPage()
        {
            InitializeComponent();
            _timer = new System.Timers.Timer();
            _timer.Enabled = false;
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerElapsed);
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

            _recentChangesAction = new AnkhAction(DoRefresh);

            // if solution is not open, disable the timer
            IAnkhCommandStates commandState = Context.GetService<IAnkhCommandStates>();
            _solutionExists = (commandState != null && commandState.SolutionExists);
            ResetRefreshSchedule();
            HookHandlers();
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
            ScheduleRefresh(0); // disable timer
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
            switch(stat.LocalContentStatus)
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
        /// Sets the timer if a sol is open and new setting is greater than 0,
        /// Unsets otherwise.
        /// </summary>
        internal void ResetRefreshSchedule()
        {
            ReadRecentChangesRefreshInterval();
            double nextRefreshInterval = 0;
            if (_solutionExists && _refreshInterval > 0)
            {
                nextRefreshInterval = _timer.Enabled
                    ? _refreshInterval // cancel the scheduled refresh and reschedule
                    : _initialRefreshInterval; // refresh is enabled, schedule initial refresh
            }
            ScheduleRefresh(nextRefreshInterval);
        }

        /// <summary>
        /// Re-enables the timer if auto-refresh is enabled (i.e. the given <paramref name="interval"/> is greater than 0).
        /// Disable the timer otherwise.
        /// </summary>
        void ScheduleRefresh(double interval)
        {
            if (_timer.Enabled)
            {
                _timer.Enabled = false;
            }
            if (interval > 0)
            {
                _timer.Interval = interval;
                _timer.Enabled = true;
            }
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

        void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ScheduleRefresh(0); // disable timer
            BeginInvoke(_recentChangesAction);
        }

        bool ReadRecentChangesRefreshInterval()
        {
            bool result = false;
#if DEBUG
            Ankh.Configuration.AnkhConfig config = Config;
            double newInterval = config == null ? 0 : config.RecentChangesRefreshInterval;
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
#endif
            return result;
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
