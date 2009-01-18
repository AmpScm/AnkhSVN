// $Id$
//
// Copyright 2008 The AnkhSVN Project
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

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using System.Threading;
using System.Runtime.Remoting.Messaging;
using System;
using Ankh.Scc;
using Ankh.UI.Services;
using Ankh.Ids;
using Ankh.UI.RepositoryExplorer;
using System.Diagnostics;
using Ankh.Commands;

namespace Ankh.UI.SvnLog
{
    /// <summary>
    /// 
    /// </summary>
    partial class LogRevisionControl : UserControl, ICurrentItemSource<ISvnLogItem>
    {
        readonly Action<SvnLogArgs> _logAction;
        readonly object _instanceLock = new object();
        readonly Queue<LogRevisionItem> _logItems = new Queue<LogRevisionItem>();
        LogMode _mode;
        BusyOverlay _busyOverlay;

        public LogRevisionControl()
        {
            InitializeComponent();
            _logAction = new Action<SvnLogArgs>(DoFetch);
        }
        public LogRevisionControl(IContainer container)
            : this()
        {
            container.Add(this);
        }

        LogDataSource _dataSource;
        public LogDataSource LogSource
        {
            get { return _dataSource; }
            set { _dataSource = value; logView.LogSource = value; }
        }

        IAnkhServiceProvider _context;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;
                OnContextChanged(EventArgs.Empty);
            }
        }

        private void OnContextChanged(EventArgs eventArgs)
        {
            logView.SelectionPublishServiceProvider = Context;
        }

        IAsyncResult _logRunner;
        public void Start(LogMode mode)
        {
            lock (_instanceLock)
            {
                _mode = mode;
                _cancel = false;
                SvnLogArgs args = new SvnLogArgs();
                args.Start = LogSource.Start;
                args.End = LogSource.End;

                // If we have EndRevision set, we want all items until End
                if (args.End == null || args.End.RevisionType == SvnRevisionType.None)
                    args.Limit = 10;

                args.StrictNodeHistory = LogSource.StrictNodeHistory;
                args.RetrieveMergedRevisions = LogSource.IncludeMergedRevisions;

                StartFetch(args);
            }
        }

        public void Stop()
        {
        }

        void StartFetch(SvnLogArgs args)
        {
            fetchCount += args.Limit;
            ShowBusyIndicator();
            _logRunner = _logAction.BeginInvoke(args, null, null);
        }

        public void Reset()
        {
            lock (_instanceLock)
            {
                if (_running)
                {
                    _cancel = true;
                    _logAction.EndInvoke(_logRunner);
                }
            }

            _logItems.Clear();
            logView.Items.Clear();
            _lastRevision = -1;
            fetchCount = 0;

        }

        int fetchCount = 0;
        bool _running;
        bool _cancel;
        void DoFetch(SvnLogArgs args)
        {
            _running = true;
            try
            {
                using (SvnClient client = _context.GetService<ISvnClientPool>().GetClient())
                {
                    SvnOrigin single = EnumTools.GetSingle(LogSource.Targets);
                    if (single != null)
                    {
                        // TODO: Use peg information
                    }
                    List<Uri> uris = new List<Uri>();
                    foreach (SvnOrigin o in LogSource.Targets)
                    {
                        uris.Add(o.Uri);
                    }

                    switch (_mode)
                    {
                        case LogMode.Log:
                            SvnLogArgs la = new SvnLogArgs();
                            la.AddExpectedError(
                                SvnErrorCode.SVN_ERR_CLIENT_UNRELATED_RESOURCES, // File not there, prevent exception
                                SvnErrorCode.SVN_ERR_UNSUPPORTED_FEATURE); // Merge info from 1.4 server
                            la.Start = args.Start;
                            la.End = args.End;
                            la.Limit = args.Limit;
                            la.StrictNodeHistory = args.StrictNodeHistory;
                            la.RetrieveMergedRevisions = args.RetrieveMergedRevisions;
                            la.Cancel += new EventHandler<SvnCancelEventArgs>(OnLogCancel);

                            client.Log(uris, la, ReceiveItem);
                            break;
                        case LogMode.MergesEligible:
                            SvnMergesEligibleArgs meArgs = new SvnMergesEligibleArgs();
                            meArgs.AddExpectedError(
                                SvnErrorCode.SVN_ERR_CLIENT_UNRELATED_RESOURCES, // File not there, prevent exception
                                SvnErrorCode.SVN_ERR_UNSUPPORTED_FEATURE); // Merge info from 1.4 server
                            meArgs.RetrieveChangedPaths = true;
                            meArgs.Cancel += new EventHandler<SvnCancelEventArgs>(OnLogCancel);
                            client.ListMergesEligible(LogSource.MergeTarget.Target, single.Target, meArgs, ReceiveItem);
                            break;
                        case LogMode.MergesMerged:
                            SvnMergesMergedArgs mmArgs = new SvnMergesMergedArgs();
                            mmArgs.AddExpectedError(
                                SvnErrorCode.SVN_ERR_CLIENT_UNRELATED_RESOURCES, // File not there, prevent exception
                                SvnErrorCode.SVN_ERR_UNSUPPORTED_FEATURE); // Merge info from 1.4 server
                            mmArgs.RetrieveChangedPaths = true;
                            mmArgs.Cancel += new EventHandler<SvnCancelEventArgs>(OnLogCancel);
                            client.ListMergesMerged(LogSource.MergeTarget.Target, single.Target, mmArgs, ReceiveItem);
                            break;
                    }
                }
            }
            finally
            {
                // Don't lock here, we can be called from within a lock
                _running = false;

                OnBatchDone();
                HideBusyIndicator();
            }
        }

        void OnLogCancel(object sender, SvnCancelEventArgs e)
        {
            if (_cancel)
                e.Cancel = true;
        }

        void ReceiveItem(object sender, SvnLogEventArgs e)
        {
            if (!_cancel)
                OnReceivedItem(e);
        }

        void ReceiveItem(object sender, SvnMergesMergedEventArgs e)
        {
            if (!_cancel)
                OnReceivedItem(e);
        }

        void ReceiveItem(object sender, SvnMergesEligibleEventArgs e)
        {
            if (!_cancel)
                OnReceivedItem(e);
        }

        void OnReceivedItem(SvnLoggingEventArgs e)
        {
            bool post = false;
            lock (_logItems)
            {
                e.Detach();

                post = (_logItems.Count == 0);

                _logItems.Enqueue(new LogRevisionItem(logView, _context, e));
            }

            if (post)
                BeginInvoke(new AnkhAction(OnShowItems));
        }

        long _lastRevision;
        void OnShowItems()
        {
            Debug.Assert(!InvokeRequired);

            lock (_logItems)
            {
                if (_logItems.Count > 0)
                {
                    LogRevisionItem[] items = _logItems.ToArray();
                    logView.Items.AddRange(items);
                    _lastRevision = items[items.Length - 1].Revision;

                    _logItems.Clear();
                }
            }
        }

        void OnBatchDone()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AnkhAction(OnBatchDone));
                return;
            }

            if (_fetchAll)
                FetchAll();

            if (BatchDone != null)
                BatchDone(this, new BatchFinishedEventArgs(11));
        }

        internal event EventHandler<BatchFinishedEventArgs> BatchDone;

        void ShowBusyIndicator()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AnkhAction(ShowBusyIndicator));
                return;
            }

            if (_busyOverlay == null)
                _busyOverlay = new BusyOverlay(logView, AnchorStyles.Bottom | AnchorStyles.Right);
            _busyOverlay.Show();
        }

        void HideBusyIndicator()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AnkhAction(HideBusyIndicator));
                return;
            }
            else if (_busyOverlay != null)
                _busyOverlay.Hide();
        }


        bool _extending;
        void ExtendList()
        {
            if (logView.VScrollPos < logView.VScrollMax - 30)
                return;

            if (!_extending)
            {
                try
                {
                    _extending = true;

                    BeginInvoke(new AnkhAction(DoExtendList));
                }
                catch
                {
                    _extending = false;
                }
            }
        }

        void DoExtendList()
        {
            try
            {
                lock (_instanceLock)
                {
                    if (!_running && logView.Items.Count == fetchCount)
                    {
                        _running = true;

                        SvnLogArgs args = new SvnLogArgs();
                        args.Start = _lastRevision - 1;
                        args.End = LogSource.End;
                        args.Limit = 20;
                        args.StrictNodeHistory = LogSource.StrictNodeHistory;
                        args.RetrieveMergedRevisions = LogSource.IncludeMergedRevisions;

                        StartFetch(args);
                    }
                }
            }
            finally
            {
                _extending = false;
            }
        }

        bool _fetchAll;
        internal void FetchAll()
        {
            lock (_instanceLock)
            {
                if (_running)
                {
                    _fetchAll = true;
                    return;
                }
                _fetchAll = false;


                SvnLogArgs args = new SvnLogArgs();
                if (_lastRevision >= 0)
                {
                    long startRev = _lastRevision - 1;
                    args.Start = startRev < 0 ? SvnRevision.Zero : startRev;
                }
                else
                {
                    lock (_logItems)
                    {
                        if (_logItems.Count > 0)
                        {
                            LogRevisionItem[] items = _logItems.ToArray();
                            long revision = items[items.Length - 1].Revision - 1;
                            // revision should not be < 0
                            args.Start = revision < 0 ? SvnRevision.Zero : revision;
                        }
                    }
                }
                args.End = LogSource.End;
                args.StrictNodeHistory = LogSource.StrictNodeHistory;
                args.RetrieveMergedRevisions = LogSource.IncludeMergedRevisions;

                StartFetch(args);
            }
        }

        #region ICurrentItemSource<ISvnLogItem> Members

        public event SelectionChangedEventHandler<ISvnLogItem> SelectionChanged;

        public event FocusChangedEventHandler<ISvnLogItem> FocusChanged;

        public ISvnLogItem FocusedItem
        {
            get
            {
                if (logView.FocusedItem == null)
                    return null;

                return new LogItem((LogRevisionItem)logView.FocusedItem, LogSource.RepositoryRoot);
            }
        }

        readonly IList<ISvnLogItem> _selectedItems = new List<ISvnLogItem>();
        public IList<ISvnLogItem> SelectedItems
        {
            get
            {
                return _selectedItems;
            }
        }

        #endregion

        private void logRevisionControl1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (FocusChanged != null)
                FocusChanged(this, FocusedItem);

            ExtendList();
        }

        void OnSelectionChanged()
        {
            _selectedItems.Clear();
            foreach (int i in logView.SelectedIndices)
                _selectedItems.Add(new LogItem((LogRevisionItem)logView.Items[i], LogSource.RepositoryRoot));

            if (SelectionChanged != null)
                SelectionChanged(this, SelectedItems);
        }

        private void logRevisionControl1_ShowContextMenu(object sender, MouseEventArgs e)
        {
            if (Context == null)
                return;

            Point p = MousePosition;
            IAnkhCommandService cs = Context.GetService<IAnkhCommandService>();
            cs.ShowContextMenu(AnkhCommandMenu.LogViewerContextMenu, p.X, p.Y);
        }

        private void logRevisionControl1_CacheVirtualItems(object sender, CacheVirtualItemsEventArgs e)
        {
            //Debug.WriteLine(string.Format("CacheVirtualItems start: {0}, end: {1}", e.StartIndex, e.EndIndex));
        }
    }

    public sealed class BatchFinishedEventArgs : EventArgs
    {
        readonly int _totalCount;
        public BatchFinishedEventArgs(int totalCount)
        {
            _totalCount = totalCount;
        }

        public int TotalCount
        {
            get { return _totalCount; }
        }
    }

    public enum LogMode
    {
        Log,
        MergesEligible,
        MergesMerged
    }
}
