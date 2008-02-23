using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using SharpSvn;
using System.Globalization;
using System.Collections.ObjectModel;

namespace Ankh.UI
{
    public partial class LogRevisionControl : UserControl, ICurrentItemSource<SvnLogEventArgs>
    {
        int scrollPosition = 0;
        const int LogBatchSize = 10;
        const int ExtraBuffer = 8 * LogBatchSize;
        List<SvnLogEventArgs> logItems = new List<SvnLogEventArgs>();
        Queue<SvnLogEventArgs> logQueue = new Queue<SvnLogEventArgs>(LogBatchSize);
        SvnLogArgs args = new SvnLogArgs();
        ProcessLogHandler processLogHandler;
        delegate void ProcessLogHandler();
        delegate void FetchEntries(SvnLogArgs args);
        bool operationStarted;
        Uri remoteTarget;
        string localTarget;
           
        [ThreadStatic]
        static SvnClient client;
        SvnClient Client
        {
            get
            {
                if (client == null)
                {
                    client = new SvnClient();
                    //SharpSvn.UI.SharpSvnUI.Bind(client);
                }
                return client;
            }
        }
        public Uri RemoteTarget
        {
            get { return remoteTarget; }
            set
            {
                if (remoteTarget != value)
                {
                    localTarget = null;
                    remoteTarget = value;
                    Reset();
                }
            }
        }
        public string LocalTarget
        {
            get { return localTarget; }
            set
            {
                if (localTarget != value)
                {
                    remoteTarget = null;
                    localTarget = value;
                    Reset();
                }
            }
        }
        public LogRevisionControl()
        {
            InitializeComponent();

             processLogHandler = new ProcessLogHandler(ProcessLog);
        }

        public LogRevisionControl(IContainer container)
            :this()
        {
            // Avoid fetching in design mode
            if(Site == null || Site.DesignMode == false)
                FetchNextEntriesAsync();
        }

        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if(e.RowIndex < logItems.Count)
                e.Value = CellValueForIndex(e.ColumnIndex, logItems[e.RowIndex]);
        }

        public Collection<SvnLogEventArgs> SelectedLogEvents
        {
            get
            {
                Collection<SvnLogEventArgs> rslt = new Collection<SvnLogEventArgs>();
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    rslt.Add(logItems[row.Index]);
                return rslt;
            }
        }
        
        private void FetchNextEntriesAsync()
        {
            if (LocalTarget == null && RemoteTarget == null)
                return; // Avoid looping

            FetchEntries fetcher = new FetchEntries(DoFetch);
            AsyncCallback c = new AsyncCallback(LogFetched);
            fetcher.BeginInvoke(args, c, null);
        }

        void LogFetched(IAsyncResult rslt)
        {
            if (InvokeRequired)
                Invoke(processLogHandler);
            else
                processLogHandler();
        }

        void ProcessLog()
        {
            lock (logQueue)
            {
                while (logQueue.Count > 0)
                {
                    logItems.Add(logQueue.Dequeue());
                }
            }

            dataGridView1.RowCount = logItems.Count;

            if (logItems.Count < ExtraBuffer + scrollPosition)
                FetchNextEntriesAsync();
            else
                operationStarted = false; // allow scroll to fire new events
        }
        
        void DoFetch(SvnLogArgs args)
        {
            if (logItems.Count > 0)
                args.Start = logItems[logItems.Count - 1].Revision;
            else
                args.Start = SvnRevision.None;
            args.Limit = LogBatchSize;

            lock (logQueue)
            {
                if (LocalTarget != null)
                {
                    Client.Log(LocalTarget, args, delegate(object sender, SvnLogEventArgs e)
                    {
                        e.Detach();
                        logQueue.Enqueue(e);
                    });
                }
                else if (RemoteTarget != null)
                {
                    Client.Log(RemoteTarget, args, delegate(object sender, SvnLogEventArgs e)
                    {
                        e.Detach();
                        logQueue.Enqueue(e);
                    });
                }
            }
        }

        private object CellValueForIndex(int columIndex, SvnLogEventArgs value)
        {
            if (value == null)
                return null;
            switch (columIndex)
            {
                case 0:
                    return value.Revision;
                case 1:
                    return value.Author;
                case 2:
                    return value.Time.ToLocalTime().ToString("F", CultureInfo.CurrentCulture);
                case 3:
                    string[] lines = value.LogMessage.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    return lines.Length > 0 ? lines[0] : "";
                default:
                    return null;
            }
        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            scrollPosition = e.NewValue;
            if (e.NewValue + ExtraBuffer > logItems.Count && !operationStarted)
            {
                FetchNextEntriesAsync();

                // Avoid starting more fetching from the scroll event. If we're scrolled down far, 
                // FetchNext is called multiple times in ProcessLog
                operationStarted = true;
            }
        }

        /// <summary>
        /// Clears state and re-schedules a fetch
        /// </summary>
        void Reset()
        {
            logItems.Clear();
            dataGridView1.RowCount = 0;
            FetchNextEntriesAsync();
        }

        /// <summary>
        /// Gets or sets a value indicating wether only the history of this exact node should be fetched (aka stop on copying)
        /// </summary>
        public bool StrictNodeHistory
        {
            get { return args.StrictNodeHistory; }
            set
            {
                if (args.StrictNodeHistory != value)
                {
                    args.StrictNodeHistory = value;
                    Reset();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating wether the merged revisions should be fetched instead of the node changes
        /// </summary>
        public bool IncludeMergedRevisions
        {
            get { return args.IncludeMergedRevisions; }
            set
            {
                if (args.IncludeMergedRevisions != value)
                {
                    args.IncludeMergedRevisions = value;
                    Reset();
                }
            }
        }

        

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, SelectedItems);
        }



        #region ICurrentItemSource<SvnLogEventArgs> Members

        public event SelectionChangedEventHandler<SvnLogEventArgs> SelectionChanged;

        public event FocusChangedEventHandler<SvnLogEventArgs> FocusChanged;

        public SvnLogEventArgs FocusedItem
        {
            get 
            {
                if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index >= logItems.Count)
                    return null;
                return logItems[dataGridView1.CurrentRow.Index]; 
            }
        }

        public IList<SvnLogEventArgs> SelectedItems
        {
            get 
            {
                List<SvnLogEventArgs> rslt = new List<SvnLogEventArgs>(dataGridView1.SelectedRows.Count);
                foreach (DataGridViewRow r in dataGridView1.SelectedRows)
                    rslt.Add(logItems[r.Index]);
                return rslt;
            }
        }

        #endregion

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (FocusChanged != null)
                FocusChanged(this, FocusedItem);
        }
    }
}
