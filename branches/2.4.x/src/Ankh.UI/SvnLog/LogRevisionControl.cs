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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using SharpSvn;

using Ankh.Commands;
using Ankh.Scc;
using Ankh.UI.RepositoryExplorer;
using Ankh.UI.VSSelectionControls;
using System.ComponentModel.Design;
using Microsoft.VisualStudio;
using System.Text;

namespace Ankh.UI.SvnLog
{
    /// <summary>
    /// 
    /// </summary>
    partial class LogRevisionControl : ListViewWithSelection<LogRevisionItem>, ICurrentItemSource<ISvnLogItem>
    {
        readonly Action<SvnLogArgs> _logAction;
        readonly object _instanceLock = new object();
        readonly Queue<LogRevisionItem> _logItems = new Queue<LogRevisionItem>();
        LogRequest _currentRequest;
        LogMode _mode;
        BusyOverlay _busyOverlay;

        public LogRevisionControl()
            : this(null)
        {
        }

        public LogRevisionControl(IContainer container)
        {
            if (container != null)
                container.Add(this);

            Sorting = SortOrder.None;
            Init();
            _logAction = new Action<SvnLogArgs>(DoFetch);
        }

        LogDataSource _dataSource;
        public LogDataSource LogSource
        {
            get { return _dataSource; }
            set { _dataSource = value; }
        }

        IAnkhServiceProvider _context;
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IAnkhServiceProvider Context
        {
            get { return _context; }
            set
            {
                _context = value;
                OnContextChanged();
            }
        }

        bool _installed;

        private void OnContextChanged()
        {
            SelectionPublishServiceProvider = Context;

            if (Context != null && !_installed)
            {
                _installed = true;

                VSCommandHandler.Install(Context, this, new CommandID(VSConstants.GUID_VSStandardCommandSet97, (int)VSConstants.VSStd97CmdID.Copy), OnCopy, OnUpdateCopy);
            }
        }

        private void OnUpdateCopy(object sender, CommandUpdateEventArgs e)
        {
            if (SelectedIndices.Count == 0)
            {
                e.Enabled = false;
            }
        }

        private void OnCopy(object sender, CommandEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            foreach (LogRevisionItem lri in SelectedItems)
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }

                sb.AppendFormat(LogStrings.LogRevision, lri.Revision);
                sb.AppendLine();
                sb.AppendFormat(LogStrings.LogAuthor, lri.Author);
                sb.AppendLine();
                sb.AppendFormat(LogStrings.LogDate, lri.Date);
                sb.AppendLine();
                sb.Append(LogStrings.LogMessagePrefix);
                sb.AppendLine();
                sb.Append(lri.LogMessage);
                sb.AppendLine();
                sb.Append(LogStrings.LogMessageSuffix);
                sb.AppendLine();
            }
            Clipboard.SetText(sb.ToString(), TextDataFormat.UnicodeText);
        }

        public void Start(LogMode mode)
        {
            lock (_instanceLock)
            {
                _mode = mode;
                SvnLogArgs args = new SvnLogArgs();
                args.Start = LogSource.Start;
                args.End = LogSource.End;

                // If we have EndRevision set, we want all items until End
                if (args.End == null || args.End.RevisionType == SvnRevisionType.None)
                    args.Limit = 10;

                args.StrictNodeHistory = LogSource.StopOnCopy;
                args.RetrieveMergedRevisions = LogSource.IncludeMergedRevisions;

                StartFetch(args);
            }
        }

        void StartFetch(SvnLogArgs args)
        {
            if (DesignMode)
                return;

            fetchCount += args.Limit;
            _logAction.BeginInvoke(args, null, null);
        }

        public void Reset()
        {
            lock (_instanceLock)
            {
                LogRequest rq = _currentRequest;
                if (rq != null)
                {
                    _currentRequest = null;
                    rq.Cancel = true;
                }
                _running = false;
            }

            _logItems.Clear();
            Items.Clear();
            _lastRevision = -1;
            fetchCount = 0;

        }

        int fetchCount;
        bool _running;
        void DoFetch(SvnLogArgs args)
        {
            LogRequest rq = _currentRequest = null;
            ShowBusyIndicator();
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

                            _currentRequest = rq = new LogRequest(la, OnReceivedItem);
                            client.Log(uris, la, null);
                            break;
                        case LogMode.MergesEligible:
                            SvnMergesEligibleArgs meArgs = new SvnMergesEligibleArgs();
                            meArgs.AddExpectedError(
                                SvnErrorCode.SVN_ERR_CLIENT_UNRELATED_RESOURCES, // File not there, prevent exception
                                SvnErrorCode.SVN_ERR_UNSUPPORTED_FEATURE); // Merge info from 1.4 server
                            meArgs.RetrieveChangedPaths = true;

                            _currentRequest = rq = new LogRequest(meArgs, OnReceivedItem);
                            client.ListMergesEligible(LogSource.MergeTarget.Target, single.Target, meArgs, null);
                            break;
                        case LogMode.MergesMerged:
                            SvnMergesMergedArgs mmArgs = new SvnMergesMergedArgs();
                            mmArgs.AddExpectedError(
                                SvnErrorCode.SVN_ERR_CLIENT_UNRELATED_RESOURCES, // File not there, prevent exception
                                SvnErrorCode.SVN_ERR_UNSUPPORTED_FEATURE); // Merge info from 1.4 server
                            mmArgs.RetrieveChangedPaths = true;
                            _currentRequest = rq = new LogRequest(mmArgs, OnReceivedItem);
                            client.ListMergesMerged(LogSource.MergeTarget.Target, single.Target, mmArgs, null);
                            break;
                    }
                }
            }
            finally
            {
                // Don't lock here, we can be called from within a lock
                if (rq == _currentRequest)
                {
                    _running = false;
                    OnBatchDone(rq);
                }
                HideBusyIndicator();
            }
        }

        void OnReceivedItem(object sender, SvnLoggingEventArgs e)
        {
            if (sender != _currentRequest)
                return;

            e.Detach();

            LogRevisionItem lri = new LogRevisionItem(this, _context, e);
            bool post;

            lock (_logItems)
            {
                post = (_logItems.Count == 0);

                _logItems.Enqueue(lri);
            }

            if (post)
            {
                if (IsHandleCreated)
                    BeginInvoke(new AnkhAction(OnShowItems));
            }
        }

        long _lastRevision;
        void OnShowItems()
        {
            Debug.Assert(!InvokeRequired);

            LogRevisionItem[] items;
            lock (_logItems)
            {
                items = _logItems.Count > 0 ? _logItems.ToArray() : null;

                _logItems.Clear();
            }

            if (items != null)
            {
                Items.AddRange(items);
                _lastRevision = items[items.Length - 1].Revision;
            }
        }

        void OnBatchDone(LogRequest rq)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<LogRequest>(OnBatchDone), rq);
                return;
            }

            if (_fetchAll)
                FetchAll();

            if (BatchDone != null)
                BatchDone(this, new BatchFinishedEventArgs(rq, Items.Count, _logItems.Count));

            ExtendList();
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
                _busyOverlay = new BusyOverlay(this, AnchorStyles.Bottom | AnchorStyles.Right);
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

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            ExtendList();
        }

        bool _extending;
        void ExtendList()
        {
            if (!IsHandleCreated || (VScrollPos < VScrollMax - 30))
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
            if (DesignMode)
                return;
            try
            {
                lock (_instanceLock)
                {
                    if (!_running && Items.Count == fetchCount)
                    {
                        _running = true;

                        SvnLogArgs args = new SvnLogArgs();
                        args.Start = _lastRevision - 1;
                        args.End = LogSource.End;
                        args.Limit = 20;
                        args.StrictNodeHistory = LogSource.StopOnCopy;
                        args.RetrieveMergedRevisions = LogSource.IncludeMergedRevisions;

                        StartFetch(args);
                    }
                }
            }
            catch { }
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
                args.StrictNodeHistory = LogSource.StopOnCopy;
                args.RetrieveMergedRevisions = LogSource.IncludeMergedRevisions;

                StartFetch(args);
            }
        }

        public event EventHandler FocusChanged;

        ISvnLogItem ICurrentItemSource<ISvnLogItem>.FocusedItem
        {
            get
            {
                if (FocusedItem == null)
                    return null;

                return new LogItem((LogRevisionItem)FocusedItem, LogSource.RepositoryRoot);
            }
        }

        readonly List<ISvnLogItem> _selectedLogItems = new List<ISvnLogItem>();
        public IList<ISvnLogItem> SelectedLogItems
        {
            get
            {
                _selectedLogItems.Clear();
                foreach (LogRevisionItem lri in SelectedItems)
                    _selectedLogItems.Add(new LogItem(lri, LogSource.RepositoryRoot));

                return _selectedLogItems.AsReadOnly();
            }
        }

        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            base.OnItemSelectionChanged(e);

            OnFocusChanged(e);

            ExtendList();
        }

        protected virtual void OnFocusChanged(EventArgs e)
        {
            if (FocusChanged != null)
                FocusChanged(this, e);
        }

        public override void OnShowContextMenu(MouseEventArgs e)
        {
            base.OnShowContextMenu(e);

            if (Context == null)
                return;

            Point screen;

            bool headerContextMenu = false;

            if (e.X == -1 && e.Y == -1)
            {
                if (SelectedItems.Count > 0)
                {
                    screen = PointToScreen(SelectedItems[SelectedItems.Count - 1].Position);
                }
                else
                {
                    headerContextMenu = true;
                    screen = PointToScreen(new Point(1, 1));
                }
            }
            else
            {
                headerContextMenu = (PointToClient(e.Location).Y < HeaderHeight);
                screen = e.Location;
            }

            IAnkhCommandService cs = Context.GetService<IAnkhCommandService>();
            cs.ShowContextMenu(headerContextMenu ? AnkhCommandMenu.ListViewHeader : AnkhCommandMenu.LogViewerContextMenu, screen);
        }

        readonly List<LogRevisionItem> _items = new List<LogRevisionItem>();

        SmartColumn _expand;
        SmartColumn _revisionColumn;
        SmartColumn _messageColumn;
        void Init()
        {
            _expand = new SmartColumn(this, "\x00A0", "&Expand Merges", 12, HorizontalAlignment.Left);
            _revisionColumn = new SmartColumn(this, "&Revision", 64, HorizontalAlignment.Right);
            SmartColumn author = new SmartColumn(this, "&Author", 73);
            SmartColumn date = new SmartColumn(this, "&Date", 118);
            SmartColumn issue = new SmartColumn(this, "&Issue", 60);
            _messageColumn = new SmartColumn(this, "&Message", 300);

            _expand.Hideable = false;
            _expand.Sortable = _revisionColumn.Sortable = author.Sortable = date.Sortable = issue.Sortable = _messageColumn.Sortable = false;

            AllColumns.Add(_expand);
            AllColumns.Add(_revisionColumn);
            AllColumns.Add(author);

            AllColumns.Add(date);
            AllColumns.Add(issue);
            AllColumns.Add(_messageColumn);

            // The listview can't align the first column right. We switch their display position
            // to work around this
            Columns.AddRange(
                new ColumnHeader[]
                {
                    _expand,
                    _revisionColumn,
                    author,
                    date,
                    _messageColumn
                });
        }

        protected override void OnRetrieveSelection(RetrieveSelectionEventArgs e)
        {
            e.SelectionItem = new LogItem(e.Item, LogSource.RepositoryRoot);
            base.OnRetrieveSelection(e);
        }

        protected override void OnResolveItem(ResolveItemEventArgs e)
        {
            e.Item = ((LogItem)e.SelectionItem).ListViewItem;
            base.OnResolveItem(e);
        }

        public List<LogRevisionItem> VirtualItems
        {
            get { return _items; }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            ExtendList();
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == 0x115) // WM_VSCROLL
                ExtendList();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (!DesignMode && _messageColumn != null)
                ResizeColumnsToFit(_messageColumn);

            ExtendList();
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            Point mp = PointToClient(MousePosition);
            ListViewHitTestInfo info = HitTest(mp);
            LogRevisionItem lvi = info.Item as LogRevisionItem;
            if (lvi != null && Context != null)
            {
                IAnkhCommandService cmdSvc = Context.GetService<IAnkhCommandService>();
                cmdSvc.PostExecCommand(AnkhCommand.LogShowChanges);
            }
        }
    }
}
