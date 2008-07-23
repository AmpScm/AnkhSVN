﻿using System.Collections.Generic;
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

namespace Ankh.UI.SvnLog
{
    public partial class LogRevisionControl : UserControl, ICurrentItemSource<ISvnLogItem>
    {
        ICollection<string> _localTargets;
        Uri _remoteTarget;
        readonly Action<SvnLogArgs> _logAction;
        readonly object _instanceLock = new object();
        readonly Queue<LogListViewItem> _logItems = new Queue<LogListViewItem>();
        readonly List<LogListViewItem> _logItemList = new List<LogListViewItem>();
        IAnkhServiceProvider _context;
        LogMode _mode;
        readonly EventHandler<SvnLogEventArgs> _logReceiver;
        readonly EventHandler<SvnMergesEligibleEventArgs> _mergesEligibleReceiver;
        readonly EventHandler<SvnMergesMergedEventArgs> _mergesMergedReceiver;
        readonly AsyncCallback _logComplete;
        readonly SynchronizationContext _syncContext;
        readonly SendOrPostCallback _sopCallback;
		BusyOverlay _busyOverlay;

        public LogRevisionControl()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
            _sopCallback = new SendOrPostCallback(SopCallback);
            _logAction = new Action<SvnLogArgs>(DoFetch);
            _logReceiver = new EventHandler<SvnLogEventArgs>(ReceiveItem);
            _mergesEligibleReceiver = new EventHandler<SvnMergesEligibleEventArgs>(ReceiveItem);
            _mergesMergedReceiver = new EventHandler<SvnMergesMergedEventArgs>(ReceiveItem);

            _logComplete = new AsyncCallback(LogComplete);

        }
        public LogRevisionControl(IContainer container)
            : this()
        {
            container.Add(this);
        }

        IAnkhUISite _site;
        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;

                IAnkhUISite site = value as IAnkhUISite;

                if (site != null)
                {
                    _site = site;

                    OnUISiteChanged(EventArgs.Empty);
                }
            }
        }
        [Browsable(false), CLSCompliant(false)]
        public IAnkhUISite UISite
        {
            get { return _site; }
        }

        protected void OnUISiteChanged(EventArgs e)
        {
            logRevisionControl1.SelectionPublishServiceProvider = _site;
        }

        public ICollection<string> LocalTargets
        {
            get { return _localTargets; }
            set { _localTargets = value; }
        }

        public Uri RemoteTarget
        {
            get { return _remoteTarget; }
            set { _remoteTarget = value; }
        }

        SvnRevision _startRevision;
        public SvnRevision StartRevision
        {
            get { return _startRevision; }
            set { _startRevision = value; }
        }

        SvnRevision _endRevision;
        public SvnRevision EndRevision
        {
            get { return _endRevision; }
            set { _endRevision = value; }
        }

        bool _strictNodeHistory;
        public bool StrictNodeHistory
        {
            get { return _strictNodeHistory; }
            set { _strictNodeHistory = value; }
        }

        bool _includeMergedRevisions;
        public bool IncludeMergedRevisions
        {
            get { return _includeMergedRevisions; }
            set { _includeMergedRevisions = value; }
        }

        IAsyncResult _logRunner;
        public void Start(IAnkhServiceProvider context, LogMode mode)
        {
            _mode = mode;
            _context = context;
            _cancel = false;
            SvnLogArgs args = new SvnLogArgs();
            args.Start = StartRevision;
            args.End = EndRevision;
            args.Limit = 10;
            args.StrictNodeHistory = StrictNodeHistory;
            args.RetrieveMergedRevisions = IncludeMergedRevisions;
            //args.RetrieveChangedPaths = false;

            _logRunner = _logAction.BeginInvoke(args, _logComplete, null);
			ShowBusyIndicator();
        }

        public void Stop()
        {
        }

        public void Reset()
        {
            lock (_instanceLock)
            {
                if (_running)
                {
                    _cancel = true;
                    _logAction.EndInvoke(_logRunner);

					if(_busyOverlay != null)
						_busyOverlay.Hide();
                }
            }

            _logItems.Clear();
            _logItemList.Clear();
            fetchCount = 0;
            logRevisionControl1.VirtualListSize = 0;
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
            using (SvnClient client = _context.GetService<ISvnClientPool>().GetClient())
            {
                
                args.SvnError += new EventHandler<SvnErrorEventArgs>(args_SvnError);
                switch (_mode)
                {
                    case LogMode.Local:
                        client.Log(LocalTargets, args, _logReceiver);
                        break;
                    case LogMode.Remote:
                        client.Log(RemoteTarget, args, _logReceiver);
                        break;
                    case LogMode.MergesEligible:
                        string target = null;
                        foreach(string t in LocalTargets)
                        {
                            target = t;
                            break;
                        }
                        SvnMergesEligibleArgs meArgs = new SvnMergesEligibleArgs();
                        meArgs.SvnError += new EventHandler<SvnErrorEventArgs>(args_SvnError);
                        client.ListMergesEligible(target, RemoteTarget, meArgs, _mergesEligibleReceiver);
                        break;
                    case LogMode.MergesMerged:
                        string mergedTarget = null;
                        foreach(string t in LocalTargets)
                        {
                            mergedTarget = t;
                            break;
                        }
                        SvnMergesMergedArgs mmArgs = new SvnMergesMergedArgs();
                        mmArgs.SvnError += new EventHandler<SvnErrorEventArgs>(args_SvnError);
                        client.ListMergesMerged(mergedTarget, RemoteTarget, _mergesMergedReceiver);
                        break;
                }
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

            lock (_logItems)
            {
                e.Detach();
                _logItems.Enqueue(new LogListViewItem(_context, e));
            }

            _syncContext.Post(_sopCallback, null);
        }

        void ReceiveItem(object sender, SvnMergesMergedEventArgs e)
        {
            if (_cancel)
                e.Cancel = true;

            lock (_logItems)
            {
                e.Detach();
                _logItems.Enqueue(new LogListViewItem(_context, e));
            }

            _syncContext.Post(_sopCallback, null);
        }

        void ReceiveItem(object sender, SvnMergesEligibleEventArgs e)
        {
            if (_cancel)
                e.Cancel = true;

            lock(_logItems)
            {
                e.Detach();
                _logItems.Enqueue(new LogListViewItem(_context, e));
            }

            _syncContext.Post(_sopCallback, null);
        }

        void SopCallback(object target)
        {
            lock (_logItems)
            {
                while (_logItems.Count > 0)
                {
                    _logItemList.Add(_logItems.Dequeue());

                }
            }
            logRevisionControl1.VirtualListSize = _logItemList.Count;
            //columnHeader1.Width = -3;
            //columnHeader2.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            //columnHeader3.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            //columnHeader4.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

		delegate void DoIt();
        void LogComplete(IAsyncResult result)
        {
            lock (_instanceLock)
            {
				_running = false;
            }
			HideBusyIndicator();
            OnBatchDone();
        }

        void OnBatchDone()
        {
            if (InvokeRequired)
                BeginInvoke(new DoIt(OnBatchDone));
            else
            {
                if (BatchDone != null)
                    BatchDone(this, new BatchFinishedEventArgs(_logItemList.Count));
            }
        }

        internal event EventHandler<BatchFinishedEventArgs> BatchDone;

		void ShowBusyIndicator()
		{
			if (InvokeRequired)
				BeginInvoke(new DoIt(ShowBusyIndicator));
			else
			{
				if (_busyOverlay == null)
					_busyOverlay = new BusyOverlay(logRevisionControl1, AnchorStyles.Bottom | AnchorStyles.Right);
				_busyOverlay.Show();
			}
		}
		void HideBusyIndicator()
		{
			if (InvokeRequired)
				BeginInvoke(new DoIt(HideBusyIndicator));
			else if (_busyOverlay != null)
				_busyOverlay.Hide();
		}

        IFileStatusCache StatusCache
        {
            get { return _context.GetService<IFileStatusCache>(); }
        }

        private void logRevisionControl1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex < _logItemList.Count)
            {
                LogListViewItem li = _logItemList[e.ItemIndex];
                e.Item = li;
                if (e.ItemIndex == _logItemList.Count - 1) // 10 items remaining, start new request
                {
                    lock (_instanceLock)
                    {
                        if (!_running && _logItemList.Count == fetchCount)
                        {
                            SvnLogArgs args = new SvnLogArgs();
                            args.Start = li.Revision - 1;
                            args.End = EndRevision;
                            args.Limit = 20;
                            args.StrictNodeHistory = StrictNodeHistory;
                            args.RetrieveMergedRevisions = IncludeMergedRevisions;
                            //args.RetrieveChangedPaths = false;

							ShowBusyIndicator();
                            _logRunner = _logAction.BeginInvoke(args, _logComplete, null);
                        }
                    }
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
            args.End = EndRevision;
            args.StrictNodeHistory = StrictNodeHistory;
            args.RetrieveMergedRevisions = IncludeMergedRevisions;
            //args.RetrieveChangedPaths = false;

            _logRunner = _logAction.BeginInvoke(args, _logComplete, null);
			ShowBusyIndicator();
        }

        #region ICurrentItemSource<ISvnLogItem> Members

        public event SelectionChangedEventHandler<ISvnLogItem> SelectionChanged;

        public event FocusChangedEventHandler<ISvnLogItem> FocusChanged;

        public ISvnLogItem FocusedItem
        {
            get
            {
                if (logRevisionControl1.FocusedItem == null)
                    return null;

                return new LogItem((LogListViewItem)logRevisionControl1.FocusedItem);
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
            foreach (int i in logRevisionControl1.SelectedIndices)
                _selectedItems.Add(new LogItem((LogListViewItem)logRevisionControl1.Items[i]));

            if (SelectionChanged != null)
                SelectionChanged(this, SelectedItems);
        }



        private void logRevisionControl1_ShowContextMenu(object sender, EventArgs e)
        {
            if (UISite != null)
            {
                Point p = MousePosition;
                UISite.ShowContextMenu(AnkhCommandMenu.LogViewerContextMenu, p.X, p.Y);
            }
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
        Local,
        Remote,
        MergesEligible,
        MergesMerged
    }
}
