// Copyright 2003-2009 The AnkhSVN Project
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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SharpSvn;
using System.Collections.Generic;
using Ankh.VS;
using System.IO;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog used for long-running operations.
    /// </summary>
    public partial class ProgressDialog : VSDialogForm
    {
        public event EventHandler Cancel;
        string _title;
        string _caption;
        readonly object _instanceLock = new object();
        /// <summary>
        /// Loader Form
        /// </summary>
        /// <param name="inText">Text to be printed in the form.</param>
        public ProgressDialog()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            _title = Text;
        }

        public string Caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
                Text = string.Format(_title, _caption).TrimStart().TrimStart('-', ' ');
            }
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        class ProgressState
        {
            public long LastCount;
        };

        readonly List<AnkhAction> _todo = new List<AnkhAction>();
        const int _bucketCount = 16;
        readonly long[] _buckets = new long[_bucketCount];
        int _curBuck;
        bool _queued;
        DateTime _start;
        long _bytesReceived;

        readonly SortedList<long, ProgressState> _progressCalc = new SortedList<long, ProgressState>();

        public void OnClientProcessing(object sender, SvnProcessingEventArgs e)
        {
            SvnCommandType type = e.CommandType;

            _start = DateTime.UtcNow;
            _curBuck = 0;
            for (int i = 0; i < _buckets.Length; i++)
                _buckets[i] = 0;

            Enqueue(delegate()
            {
                lock (_instanceLock)
                {
                    _progressCalc.Clear();
                }
                ListViewItem item = new ListViewItem("Action");
                item.SubItems.Add(type.ToString());
                item.ForeColor = Color.Gray;

                if (actionList.Items.Count == 1 && _toAdd.Count == 0)
                    actionList.Items.Clear();
                actionList.Items.Add(item);
            });
        }

        string GetActionText(SvnNotifyAction action)
        {
            string actionText = action.ToString();

            switch (action)
            {
                case SvnNotifyAction.UpdateAdd:
                case SvnNotifyAction.UpdateDelete:
                case SvnNotifyAction.UpdateReplace:
                case SvnNotifyAction.UpdateUpdate:
                case SvnNotifyAction.UpdateCompleted:
                case SvnNotifyAction.UpdateExternal:
                case SvnNotifyAction.UpdateSkipWorkingOnly:
                case SvnNotifyAction.UpdateSkipObstruction:
                case SvnNotifyAction.UpdateSkipAccessDenied:
                case SvnNotifyAction.UpdateShadowedAdd:
                case SvnNotifyAction.UpdateShadowedDelete:
                case SvnNotifyAction.UpdateShadowedUpdate:
                case SvnNotifyAction.UpdateExternalRemoved:
                case SvnNotifyAction.UpdateBrokenLock:
                    actionText = actionText.Substring(6);
                    break;
                case SvnNotifyAction.UpgradedDirectory:
                    actionText = "Upgraded";
                    break;
                case SvnNotifyAction.CommitAdded:
                case SvnNotifyAction.CommitDeleted:
                case SvnNotifyAction.CommitModified:
                case SvnNotifyAction.CommitReplaced:
                case SvnNotifyAction.CommitAddCopy:
                case SvnNotifyAction.CommitReplacedWithCopy:
                case SvnNotifyAction.CommitFinalizing:
                    actionText = actionText.Substring(6);
                    break;
                case SvnNotifyAction.CommitSendData:
                    actionText = "Sending";
                    break;
                case SvnNotifyAction.BlameRevision:
                    actionText = "Annotating";
                    break;
                case SvnNotifyAction.UpdateStarted:
                case SvnNotifyAction.RecordMergeInfoStarted:
                case SvnNotifyAction.FollowUrlRedirect:
                case SvnNotifyAction.OperationRequiresTarget:
                    actionText = null;
                    break;
            }

            return actionText;
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            if (!DesignMode)
                ResizeToFit();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!DesignMode)
                ResizeToFit();
        }

        void ResizeToFit()
        {
            if (actionList != null && pathColumn != null)
                actionList.ResizeColumnsToFit(pathColumn);
        }

        string _splitRoot;

        string SplitRoot
        {
            get
            {
                if (_splitRoot == null && Context != null)
                {
                    IAnkhSolutionSettings ss = Context.GetService<IAnkhSolutionSettings>();

                    if (ss != null)
                        _splitRoot = ss.ProjectRoot;

                    if (string.IsNullOrEmpty(_splitRoot))
                        _splitRoot = "";
                }

                if (string.IsNullOrEmpty(_splitRoot))
                    return null;
                else
                    return _splitRoot;
            }
        }

        public void OnClientNotify(object sender, SvnNotifyEventArgs e)
        {
            //e.Detach();

            string path = e.FullPath;
            Uri uri = e.Uri;
            SvnNotifyAction action = e.Action;
            long rev = e.Revision;

            Enqueue(delegate()
            {
                ListViewItem item = null;
                string text = GetActionText(action);

                if (string.IsNullOrEmpty(text))
                    return;

                item = new ListViewItem(text);

                switch (action)
                {
                    case SvnNotifyAction.BlameRevision:
                        {
                            string file;
                            if (uri != null)
                                file = SvnTools.GetFileName(uri);
                            else
                                file = Path.GetFileName(path);

                            item.SubItems.Add(string.Format("{0} - r{1}", file, rev));
                            break;
                        }
                    default:
                        if (uri != null)
                            item.SubItems.Add(uri.ToString());
                        else if (!string.IsNullOrEmpty(path))
                        {
                            string sr = SplitRoot;
                            if (!string.IsNullOrEmpty(sr) && SvnItem.IsBelowRoot(path, sr))
                            {
                                string np = SvnItem.SubPath(path, sr, true);
                                
                                if (np.IndexOf(':') == 1)
                                    path = np; // Full path
                                else
                                    path = np.Replace(Path.DirectorySeparatorChar, '/');
                            }

                            item.SubItems.Add(path);
                        }
                        break;
                }

                if (item != null)
                    _toAdd.Add(item);
            });
        }

        public void OnClientProgress(object sender, SvnProgressEventArgs e)
        {
            ProgressState state;

            long received;
            lock (_instanceLock)
            {
                if (_progressCalc.TryGetValue(e.TotalProgress, out state))
                {
                    if (e.Progress < state.LastCount)
                        state.LastCount = 0;

                    received = e.Progress - state.LastCount;
                    if (e.TotalProgress == e.Progress)
                        _progressCalc.Remove(e.TotalProgress);
                    else
                        state.LastCount = e.Progress;
                }
                else
                {
                    state = new ProgressState();
                    state.LastCount = e.Progress;
                    _progressCalc.Add(e.TotalProgress, state);
                    received = e.Progress;
                }
            }
            _bytesReceived += received;

            TimeSpan ts = DateTime.UtcNow - _start;


            int totalSeconds = (int)ts.TotalSeconds;
            if (totalSeconds < 0)
                return;

            // Clear all buckets of previous seconds where nothing was received
            while (_curBuck < totalSeconds)
            {
                _curBuck++;
                int n = _curBuck % _bucketCount;
                _buckets[n] = 0;
            }

            // Add the amount of this second to the right bucket
            _buckets[_curBuck % _bucketCount] += received;

            int avg = -1;

            int calcBuckets;

            if (_curBuck < 3)
                calcBuckets = 0;
            else
                calcBuckets = Math.Min(5, _curBuck - 1);

            if (calcBuckets > 0)
            {
                long tot = 0;
                for (int n = _curBuck - 1; n > (_curBuck - calcBuckets - 1); n--)
                {
                    tot += _buckets[n % _bucketCount];
                }

                avg = (int)(tot / (long)calcBuckets);
            }

            Enqueue(delegate()
            {
                string text = string.Format("{0} transferred", SizeStr(_bytesReceived));

                if (avg > 0)
                    text += string.Format(" at {0}/s.", SizeStr(avg));
                else if (totalSeconds >= 1)
                    text += string.Format(" in {0} seconds.", totalSeconds);
                else
                    text += ".";

                progressLabel.Text = text;
            });
        }

        private string SizeStr(long numberOfBytes)
        {
            if (numberOfBytes == 1)
                return "1 byte";
            else if (numberOfBytes < 1024)
                return string.Format("{0} bytes", numberOfBytes);
            else if (numberOfBytes < 16384)
                return string.Format("{0:0.0} kByte", numberOfBytes / 1024.0);
            else if (numberOfBytes < 1024 * 1024)
                return string.Format("{0} kByte", numberOfBytes / 1024);
            else if (numberOfBytes < 16 * 1024 * 1024)
                return string.Format("{0:0.0} MByte", numberOfBytes / (1024.0 * 1024.0));
            else
                return string.Format("{0} MByte", numberOfBytes / 1024 / 1024);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _canceling = true;
            base.OnClosing(e);
        }

        volatile bool _canceling; // Updated from UI thread, read from command thread
        void OnClientCancel(object sender, SvnCancelEventArgs e)
        {
            if (_canceling)
                e.Cancel = true;
        }


        /// <summary>
        /// Enqueus a task for processing in the UI thread. All tasks will run in the same order as in which they are enqueued
        /// </summary>
        /// <param name="task"></param>
        void Enqueue(AnkhAction task)
        {
            if (task == null)
                return;

            lock (_todo)
            {
                _todo.Add(task);

                try
                {
                    if (!_queued && IsHandleCreated)
                    {
                        BeginInvoke(new AnkhAction(RunQueue));
                        _queued = true;
                    }
                }
                catch
                {
                    // Don't kill svn on a failed begin invoke
                }
            }
        }

        readonly List<ListViewItem> _toAdd = new List<ListViewItem>();
        void RunQueue()
        {
            AnkhAction[] actions;
            lock (_todo)
            {
                _queued = false;
                actions = _todo.ToArray();
                _todo.Clear();
            }

            int n = actionList.Items.Count;

            foreach (AnkhAction ds in actions)
            {
                ds();
            }

            if (_toAdd.Count > 0)
            {
                ListViewItem[] items = _toAdd.ToArray();
                _toAdd.Clear();
                actionList.Items.AddRange(items);
            }

            if (actionList.Items.Count != n)
            {
                actionList.Items[actionList.Items.Count - 1].EnsureVisible();
                actionList.RedrawItems(n, actionList.Items.Count - 1, false);
            }
        }

        public IDisposable Bind(SvnClient client)
        {
            if (client == null)
                throw new ArgumentNullException("client");
            client.Processing += new EventHandler<SvnProcessingEventArgs>(OnClientProcessing);
            client.Notify += new EventHandler<SvnNotifyEventArgs>(OnClientNotify);
            client.Progress += new EventHandler<SvnProgressEventArgs>(OnClientProgress);
            client.Cancel += new EventHandler<SvnCancelEventArgs>(OnClientCancel);

            return new UnbindDisposer(client, this);
        }

        class UnbindDisposer : IDisposable
        {
            SvnClient _client;
            ProgressDialog _dlg;
            public UnbindDisposer(SvnClient client, ProgressDialog dlg)
            {
                if (client == null)
                    throw new ArgumentNullException("client");
                else if (dlg == null)
                    throw new ArgumentNullException("dlg");

                _client = client;
                _dlg = dlg;
            }

            #region IDisposable Members

            public void Dispose()
            {
                _dlg.Unbind(_client);
            }

            #endregion
        }

        void Unbind(SvnClient client)
        {
            client.Notify -= new EventHandler<SvnNotifyEventArgs>(OnClientNotify);
            client.Processing -= new EventHandler<SvnProcessingEventArgs>(OnClientProcessing);
            client.Progress -= new EventHandler<SvnProgressEventArgs>(OnClientProgress);
            client.Cancel -= new EventHandler<SvnCancelEventArgs>(OnClientCancel);
        }

        private void CancelClick(object sender, System.EventArgs e)
        {
            _canceling = true;

            if (Cancel != null)
                Cancel(this, EventArgs.Empty);
            this.cancelButton.Text = "Cancelling...";
            this.cancelButton.Enabled = false;
        }
    }
}
