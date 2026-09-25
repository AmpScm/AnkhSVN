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
using System.Threading;
using System.Windows.Forms;
using Ankh.Commands;
using Ankh.Configuration;
using Ankh.Scc;
using Ankh.UI.VSSelectionControls;

namespace Ankh.UI.PendingChanges
{
    partial class RecentChangesPage : PendingChangesPage
    {
        static readonly int[] Intervals = { 1, 5, 10, 15, 30, 60, 120 };
        readonly NumericUpDown _historyLimit;
        readonly TextBox _details;
        readonly System.Windows.Forms.Timer _refreshTimer;
        bool _solutionExists;
        bool _updatingSettings;
        bool _loading;
        int _generation;
        AnkhServiceEvents _events;

        public RecentChangesPage()
        {
            InitializeComponent();
            label1.Text = "Commits:";
            updateTime.Visible = true;
            updateTime.Enabled = true;
            updateTime.Dock = DockStyle.Fill;
            syncView.Sorting = SortOrder.None;
            syncView.ListViewItemSorter = null;
            syncView.ShowItemToolTips = true;
            string[] names = { "Revision", "Author", "Date", "Message", "Repository" };
            int[] widths = { 85, 120, 155, 400, 250 };
            for (int i = 0; i < names.Length; i++)
                syncView.Columns.Add(new SmartColumn(syncView, names[i], widths[i], names[i]) { Sortable = false });

            var settings = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false };
            settings.Controls.Add(new Label { Text = "Commits to show:", AutoSize = true, Margin = new Padding(3, 6, 3, 3) });
            _historyLimit = new NumericUpDown { Minimum = 1, Maximum = 1000, Value = 25, Width = 75 };
            settings.Controls.Add(_historyLimit);
            Controls.Add(settings);
            splitContainer1.BringToFront();
            _historyLimit.ValueChanged += HistoryLimitChanged;

            _details = new TextBox { Dock = DockStyle.Bottom, Height = 100, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both };
            splitContainer1.Panel2.Controls.Add(_details);
            syncView.BringToFront();
            syncView.SelectedIndexChanged += delegate {
                var entry = syncView.SelectedItems.Count == 0 ? null : syncView.SelectedItems[0].Tag as CommittedHistoryEntry;
                _details.Text = entry == null ? "" : entry.Message + Environment.NewLine + Environment.NewLine + entry.ChangedPaths;
            };
            _refreshTimer = new System.Windows.Forms.Timer(components);
            _refreshTimer.Tick += delegate { RefreshList(); };
            checkBox1.CheckedChanged += OnRefreshIntervalModified;
            refreshCombo.SelectedIndexChanged += OnRefreshIntervalModified;
        }

        protected override Type PageType { get { return typeof(RecentChangesPage); } }
        AnkhConfig Config { get { return ConfigurationService.Instance; } }
        public override bool CanRefreshList { get { return _solutionExists && !_loading; } }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            syncView.SetColumnWidths(ConfigurationService.GetColumnWidths(GetType()));
            syncView.ColumnWidthChanged += delegate {
                ConfigurationService.SaveColumnsWidths(GetType(), syncView.GetColumnWidths());
            };
            _events = Context.GetService<AnkhServiceEvents>();
            if (_events != null)
            {
                _events.SolutionOpened += OnSolutionOpened;
                _events.SolutionClosed += OnSolutionClosed;
            }
            var states = Context.GetService<IAnkhCommandStates>();
            _solutionExists = states != null && states.SolutionExists;
            RefreshIntervalConfigModified();
            RefreshList();
        }

        void OnSolutionOpened(object sender, EventArgs e)
        {
            _generation++;
            _loading = false;
            _solutionExists = true;
            syncView.Items.Clear();
            ConfigureTimer();
            RefreshList();
        }

        void OnSolutionClosed(object sender, EventArgs e)
        {
            _generation++;
            _loading = false;
            _solutionExists = false;
            _refreshTimer.Stop();
            syncView.Items.Clear();
            _details.Clear();
            updateTime.Text = "Open a versioned solution to view commits.";
        }

        internal void ReleaseHistoryResources()
        {
            _generation++;
            if (_events != null)
            {
                _events.SolutionOpened -= OnSolutionOpened;
                _events.SolutionClosed -= OnSolutionClosed;
                _events = null;
            }
        }

        public override void RefreshList()
        {
            if (!_solutionExists || _loading || IsDisposed || !IsHandleCreated)
                return;
            var layout = Context.GetService<ISvnSolutionLayout>();
            var roots = new List<string>(SvnItem.GetPaths(layout.GetUpdateRoots(null)));
            if (roots.Count == 0)
            {
                syncView.Items.Clear();
                _details.Clear();
                updateTime.Text = "No versioned paths in this solution.";
                return;
            }
            var pool = Context.GetService<ISvnClientPool>();
            pool.EnsureClient();
            int generation = _generation;
            int limit = Config.RecentChangesHistoryLimit;
            _loading = true;
            _refreshTimer.Stop();
            updateTime.Text = "Loading committed history...";
            ThreadPool.QueueUserWorkItem(delegate {
                List<CommittedHistoryEntry> entries = null;
                Exception error = null;
                try
                {
                    using (var client = pool.GetClient())
                        entries = CommittedHistory.Fetch(client, roots, limit);
                }
                catch (Exception ex) { error = ex; }
                try
                {
                    if (!IsDisposed && IsHandleCreated)
                        BeginInvoke(new Action(delegate {
                            if (IsDisposed || generation != _generation) return;
                            _loading = false;
                            if (error == null)
                            {
                                ShowEntries(entries);
                                updateTime.Text = entries.Count == 0 ? "No commits found." : "Updated " + DateTime.Now.ToShortTimeString();
                            }
                            else
                            {
                                // Keep already displayed commits if an offline refresh fails.
                                updateTime.Text = "Unable to load history; refresh to retry.";
                                _details.Text = error.Message;
                            }
                            ConfigureTimer();
                        }));
                }
                catch (InvalidOperationException) { } // Window closed while the request completed.
            });
        }

        void ShowEntries(IEnumerable<CommittedHistoryEntry> entries)
        {
            syncView.BeginUpdate();
            try
            {
                syncView.Items.Clear();
                _details.Clear();
                foreach (var entry in entries)
                {
                    var item = new SmartListViewItem(syncView) { Tag = entry, ToolTipText = entry.Message };
                    item.SetValues("r" + entry.Revision, entry.Author, entry.Time.ToLocalTime().ToString("g"),
                        entry.Message.Replace("\r", " ").Replace("\n", " "), entry.Repository);
                    syncView.Items.Add(item);
                }
            }
            finally { syncView.EndUpdate(); }
        }

        void HistoryLimitChanged(object sender, EventArgs e)
        {
            if (_updatingSettings || Context == null) return;
            var config = Config;
            config.RecentChangesHistoryLimit = (int)_historyLimit.Value;
            ConfigurationService.SaveConfig(config);
            _generation++;
            _loading = false;
            RefreshList();
        }

        internal void RefreshIntervalConfigModified()
        {
            _updatingSettings = true;
            try
            {
                _historyLimit.Value = Config.RecentChangesHistoryLimit;
                checkBox1.Checked = Config.RecentChangesRefreshInterval > 0;
                int minutes = Config.RecentChangesRefreshInterval / 60;
                int index = Array.FindLastIndex(Intervals, value => value <= minutes);
                refreshCombo.SelectedIndex = Math.Max(0, index);
                refreshCombo.Enabled = checkBox1.Checked;
            }
            finally { _updatingSettings = false; }
            ConfigureTimer();
        }

        void OnRefreshIntervalModified(object sender, EventArgs e)
        {
            if (_updatingSettings || Context == null || refreshCombo.SelectedIndex < 0) return;
            var config = Config;
            config.RecentChangesRefreshInterval = checkBox1.Checked ? Intervals[refreshCombo.SelectedIndex] * 60 : 0;
            ConfigurationService.SaveConfig(config);
            refreshCombo.Enabled = checkBox1.Checked;
            ConfigureTimer();
        }

        void ConfigureTimer()
        {
            _refreshTimer.Stop();
            int seconds = Config.RecentChangesRefreshInterval;
            if (_solutionExists && !_loading && seconds > 0)
            {
                _refreshTimer.Interval = (int)Math.Min(int.MaxValue, (long)seconds * 1000);
                _refreshTimer.Start();
            }
        }

        public override void OnThemeChanged(EventArgs e)
        {
            base.OnThemeChanged(e);
            if (VSVersion.VS2012OrLater)
            {
                syncView.BorderStyle = BorderStyle.None;
                borderPanel.BorderStyle = BorderStyle.None;
            }
        }
    }
}
