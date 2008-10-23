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
        readonly Queue<LogListViewItem> _logItems = new Queue<LogListViewItem>();
        readonly List<LogListViewItem> _logItemList = new List<LogListViewItem>();
        IAnkhServiceProvider _context;
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

        IAnkhServiceProvider _qcontext;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAnkhServiceProvider Context
        {
            get { return _qcontext; }
            set
            {
                _qcontext = value;
                logView.SelectionPublishServiceProvider = value;
            }
        }

        IAsyncResult _logRunner;
        public void Start(IAnkhServiceProvider context, LogMode mode)
        {
            _mode = mode;
            _context = context;
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

        public void Stop()
        {
        }

        void StartFetch(SvnLogArgs args)
        {
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
            _logItemList.Clear();
            fetchCount = 0;
            logView.VirtualListSize = 0;
        }

        int fetchCount = 0;
        bool _running;
        bool _cancel;
        void DoFetch(SvnLogArgs args)
        {
            lock (_instanceLock)
            {
                _running = true;
                fetchCount += args.Limit;
            }
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
                            la.SvnError += new EventHandler<SvnErrorEventArgs>(args_SvnError);
                            la.Start = args.Start;
                            la.End = args.End;
                            la.Limit = args.Limit;
                            la.StrictNodeHistory = args.StrictNodeHistory;
                            la.RetrieveMergedRevisions = args.RetrieveMergedRevisions;

                            client.Log(uris, la, ReceiveItem);
                            break;
                        case LogMode.MergesEligible:
                            SvnMergesEligibleArgs meArgs = new SvnMergesEligibleArgs();
                            meArgs.SvnError += new EventHandler<SvnErrorEventArgs>(args_SvnError);
                            meArgs.RetrieveChangedPaths = true;
                            client.ListMergesEligible(LogSource.MergeTarget.Target, single.Target, meArgs, ReceiveItem);
                            break;
                        case LogMode.MergesMerged:
                            SvnMergesMergedArgs mmArgs = new SvnMergesMergedArgs();
                            mmArgs.SvnError += new EventHandler<SvnErrorEventArgs>(args_SvnError);
                            mmArgs.RetrieveChangedPaths = true;
                            client.ListMergesMerged(LogSource.MergeTarget.Target, single.Target, mmArgs, ReceiveItem);
                            break;
                    }
                }
            }
            finally
            {
                lock (_instanceLock)
                    _running = false;

                OnBatchDone();
                HideBusyIndicator();
            }
        }

        void args_SvnError(object sender, SvnErrorEventArgs e)
        {
            // TODO: replace with specific SvnExceptions when available
            if (e.Exception.SvnErrorCode == SvnErrorCode.SVN_ERR_CLIENT_UNRELATED_RESOURCES)
                e.Cancel = true; // File not there, prevent exception
            else if (e.Exception.SvnErrorCode == SvnErrorCode.SVN_ERR_UNSUPPORTED_FEATURE)
                e.Cancel = true; // Probably requesting merge against 1.4 server
        }

        void ReceiveItem(object sender, SvnLogEventArgs e)
        {
            if (_cancel)
                e.Cancel = true;
            else
                OnReceivedItem(e);
        }

        void ReceiveItem(object sender, SvnMergesMergedEventArgs e)
        {
            if (_cancel)
                e.Cancel = true;
            else
                OnReceivedItem(e);
        }

        void ReceiveItem(object sender, SvnMergesEligibleEventArgs e)
        {
            if (_cancel)
                e.Cancel = true;
            else
                OnReceivedItem(e);
        }

        void OnReceivedItem(SvnLoggingEventArgs e)
        {
            bool post = false;
            lock (_logItems)
            {
                e.Detach();

                post = (_logItems.Count == 0);

                _logItems.Enqueue(new LogListViewItem(logView, _context, e));
            }

            if (post)
                BeginInvoke(new AnkhAction(OnShowItems));
        }

        void OnShowItems()
        {
            Debug.Assert(!InvokeRequired);

            lock (_logItems)
            {
                if (_logItems.Count > 0)
                    _logItemList.AddRange(_logItems);

                _logItems.Clear();
            }

            logView.VirtualListSize = _logItemList.Count;
        }

        void OnBatchDone()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new AnkhAction(OnBatchDone));
                return;
            }

            if (BatchDone != null)
                BatchDone(this, new BatchFinishedEventArgs(_logItemList.Count));
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

        private void logRevisionControl1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            try
            {
                if (e.ItemIndex >= _logItemList.Count)
                    return;

                LogListViewItem li = _logItemList[e.ItemIndex];
                e.Item = li;
            }
            finally
            {
                if (e.ItemIndex > _logItemList.Count - 10)
                    ExtendList();
            }
        }

        void ExtendList()
        {
            lock (_instanceLock)
            {
                if (!_running && _logItemList.Count == fetchCount)
                {
                    if (logView.VScrollPos < logView.VScrollMax - 30)
                        return;

                    _running = true;

                    SvnLogArgs args = new SvnLogArgs();
                    args.Start = _logItemList[_logItemList.Count - 1].Revision - 1;
                    args.End = LogSource.End;
                    args.Limit = 20;
                    args.StrictNodeHistory = LogSource.StrictNodeHistory;
                    args.RetrieveMergedRevisions = LogSource.IncludeMergedRevisions;

                    StartFetch(args);
                }
            }
        }

        internal void FetchAll()
        {
            lock (_instanceLock)
            {
                if (_running)
                {
                    _logAction.EndInvoke(_logRunner);
                }
            }

            SvnLogArgs args = new SvnLogArgs();
            if (_logItemList.Count > 0)
            {
                long startRev = _logItemList[_logItemList.Count - 1].Revision - 1;
                args.Start = startRev < 0 ? SvnRevision.Zero : startRev;
            }
            else
            {
                lock (_logItems)
                {
                    if (_logItems.Count > 0)
                    {
                        LogListViewItem[] items = _logItems.ToArray();
                        long revision = items[items.Length - 1].Revision - 1;
                        // revision should not be < 0
                        args.Start = revision < 0 ? SvnRevision.Zero : revision;
                    }
                }
            }
            args.End = LogSource.End;
            args.StrictNodeHistory = LogSource.StrictNodeHistory;
            args.RetrieveMergedRevisions = LogSource.IncludeMergedRevisions;
            //args.RetrieveChangedPaths = false;

            StartFetch(args);
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

                return new LogItem((LogListViewItem)logView.FocusedItem, LogSource.RepositoryRoot);
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
        }

        /* SR:  Weird stuff is going on with ListViews in virtual mode, modify with care
         *      -   normal clicking -> SelectedIndexChange is 'right' about the number of selected indices
         *      -   ctrl clicking ->  SelectedIndexChange is 'right' about the number of selected indices
         *      -   shift clicking -> VirtualItemsSelectionRangeChanges is 'right' about the number of selected indices
        */
        private void logRevisionControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Debug.WriteLine("NormalChanged: " + logRevisionControl1.SelectedIndices.Count);
            OnSelectionChanged();
        }

        private void logRevisionControl1_VirtualItemsSelectionRangeChanged(object sender, ListViewVirtualItemsSelectionRangeChangedEventArgs e)
        {
            //Debug.WriteLine("RangeChanged:  " + logRevisionControl1.SelectedIndices.Count);
            OnSelectionChanged();
        }

        void OnSelectionChanged()
        {
            _selectedItems.Clear();
            foreach (int i in logView.SelectedIndices)
                _selectedItems.Add(new LogItem((LogListViewItem)logView.Items[i], LogSource.RepositoryRoot));

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
