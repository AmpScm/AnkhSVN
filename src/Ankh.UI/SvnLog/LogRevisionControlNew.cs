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

namespace Ankh.UI.SvnLog
{
    public partial class LogRevisionControlNew : UserControl
    {
        ICollection<string> _localTargets;
        Uri _remoteTarget;
        readonly Action<SvnLogArgs> _logAction;
        readonly object _instanceLock = new object();
        readonly Queue<LogItem> _logItems = new Queue<LogItem>();
        readonly List<LogItem> _logItemList = new List<LogItem>();
        IAnkhServiceProvider _context;
        LogMode _mode;
        readonly EventHandler<SvnLogEventArgs> _logReceiver;
        readonly AsyncCallback _logComplete;
        readonly SynchronizationContext _syncContext;
        readonly SendOrPostCallback _sopCallback;

        public LogRevisionControlNew()
        {
            InitializeComponent();
            _syncContext = SynchronizationContext.Current;
            _sopCallback = new SendOrPostCallback(SopCallback);
            _logAction = new Action<SvnLogArgs>(DoFetch);
            _logReceiver = new EventHandler<SvnLogEventArgs>(ReceiveItem);
            _logComplete = new AsyncCallback(LogComplete);

        }
        public LogRevisionControlNew(IContainer container)
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

            SvnLogArgs args = new SvnLogArgs();
            args.Start = StartRevision;
            args.End = EndRevision;
            args.Limit = 10;
            args.StrictNodeHistory = StrictNodeHistory;
            args.RetrieveMergedRevisions = IncludeMergedRevisions;
            args.RetrieveChangedPaths = false;

            _logRunner = _logAction.BeginInvoke(args, _logComplete, null);
        }

        public void Stop()
        {
        }

        public void Reset()
        {
            _logItems.Clear();
            _logItemList.Clear();
            fetchCount = 0;
            logRevisionControl1.VirtualListSize = 0;
        }

        int fetchCount = 0;
        bool _running;
        void DoFetch(SvnLogArgs args)
        {
            _running = true;
            using (SvnClient client = _context.GetService<ISvnClientPool>().GetClient())
            {
                fetchCount += args.Limit;
                args.SvnError += new EventHandler<SvnErrorEventArgs>(args_SvnError);
                switch (_mode)
                {
                    case LogMode.Local:
                        client.Log(LocalTargets, args, _logReceiver);
                        break;
                    case LogMode.Remote:
                        client.Log(RemoteTarget, args, _logReceiver);
                        break;
                }
            }
        }

        void args_SvnError(object sender, SvnErrorEventArgs e)
        {
            if (e.Exception.SvnErrorCode == SharpSvn.Implementation.SvnErrorCode.SVN_ERR_CLIENT_UNRELATED_RESOURCES)
                e.Cancel = true; // File not there, prevent exception
        }

        void ReceiveItem(object sender, SvnLogEventArgs e)
        {
            lock (_instanceLock)
            {
                e.Detach();
                _logItems.Enqueue(new LogItem(_context, e));
            }
            _syncContext.Post(_sopCallback, null);
        }

        void SopCallback(object target)
        {
            lock (_instanceLock)
            {
                while (_logItems.Count > 0)
                {
                    _logItemList.Add(_logItems.Dequeue());

                }
            }
            logRevisionControl1.VirtualListSize = _logItemList.Count;
            //columnHeader1.Width = -3;
            columnHeader2.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            columnHeader3.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
            columnHeader4.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        }

        void LogComplete(IAsyncResult result)
        {
            _running = false;
        }

        IFileStatusCache StatusCache
        {
            get { return _context.GetService<IFileStatusCache>(); }
        }

        private void logRevisionControl1_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex < _logItemList.Count)
            {
                LogItem li = _logItemList[e.ItemIndex];
                e.Item = li;
                if (e.ItemIndex == _logItemList.Count - 1) // 10 items remaining, start new request
                {
                    if (!_running && _logItemList.Count == fetchCount)
                    {
                        SvnLogArgs args = new SvnLogArgs();
                        args.Start = li.RawData.Revision - 1;
                        args.End = EndRevision;
                        args.Limit = 20;
                        args.StrictNodeHistory = StrictNodeHistory;
                        args.RetrieveMergedRevisions = IncludeMergedRevisions;
                        args.RetrieveChangedPaths = false;

                        _logRunner = _logAction.BeginInvoke(args, _logComplete, null);
                    }
                }
            }

        }
    }

    public enum LogMode
    {
        Local,
        Remote
    }
}
