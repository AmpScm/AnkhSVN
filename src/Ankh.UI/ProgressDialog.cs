using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SharpSvn;
using System.Collections.Generic;

namespace Ankh.UI
{
    /// <summary>
    /// A dialog used for long-running operations.
    /// </summary>
    public partial class ProgressDialog : System.Windows.Forms.Form
    {
        public event EventHandler Cancel;
        string _title;
        string _caption;
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
                Text = string.Format(_title, _caption).TrimStart().TrimStart('-',' ');
            }
        }


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        class ProgressState
        {
            public long LastCount;
        };

        delegate void DoSomething();
        List<DoSomething> _todo = new List<DoSomething>();
        DateTime _start;
        long _bytesReceived;
        SortedList<long, ProgressState> _progressCalc = new SortedList<long, ProgressState>();        

        public void OnClientProcessing(object sender, SvnProcessingEventArgs e)
        {
            e.Detach();            

            Enqueue(delegate()
            {
                _progressCalc.Clear();
                _start = DateTime.UtcNow;
                ListViewItem item = new ListViewItem("Action");
                item.SubItems.Add(e.CommandType.ToString());
                item.ForeColor = Color.Gray;

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
                    actionText = actionText.Substring(6);
                    break;
                case SvnNotifyAction.CommitSendData:
                    actionText = "Sending";
                    break;
            }

            return actionText;
        }

        public void OnClientNotify(object sender, SvnNotifyEventArgs e)
        {            
            e.Detach();

            Enqueue(delegate()
            {
                ListViewItem item = null;
                if(!string.IsNullOrEmpty(e.FullPath))
                {
                    item = new ListViewItem(GetActionText(e.Action));
                    item.SubItems.Add(e.FullPath);
                }

                if (item != null)
                {
                    actionList.Items.Add(item);
                }
            });
        }

        public void OnClientProgress(object sender, SvnProgressEventArgs e)
        {
            e.Detach();

            ProgressState state;
            if (e.TotalProgress > 0 && _progressCalc.TryGetValue(e.TotalProgress, out state))
            {
                _bytesReceived += e.Progress - state.LastCount;
                if (e.TotalProgress == e.Progress)
                    _progressCalc.Remove(e.TotalProgress);
                else
                    state.LastCount = e.Progress;
            }
            else
                _bytesReceived += e.Progress;

            TimeSpan ts = DateTime.UtcNow - _start;

            if(ts.TotalSeconds <= 0.1)
                return;

            Enqueue(delegate()
            {
                int totalSeconds= (int)ts.TotalSeconds;
                if (totalSeconds > 60)
                    progressLabel.Text = string.Format("{0} kByte transferred ({1}:{2:00})", _bytesReceived / 1024, totalSeconds/60, totalSeconds %60);
                else
                    progressLabel.Text = string.Format("{0} kByte transferred (0:{1:00})", _bytesReceived / 1024, totalSeconds);

                
                GC.KeepAlive(e);
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _canceling = true;
            base.OnClosing(e);
        }

        volatile bool _canceling; // Updated from UI thread, read from command thread
        void OnClientCancel(object sender, SvnCancelEventArgs e)
        {
            if(_canceling)
                e.Cancel = true;
        }


        /// <summary>
        /// Enqueus a task for processing in the UI thread. All tasks will run in the same order as in which they are enqueued
        /// </summary>
        /// <param name="task"></param>
        void Enqueue(DoSomething task)
        {
            if (task == null)
                return;

            bool invoke = false;
            lock(_todo)
            {
                invoke = (_todo.Count == 0);           
                _todo.Add(task);
            }

            if (invoke && IsHandleCreated)
            {
                try
                {
                    BeginInvoke(new DoSomething(RunQueue));
                }
                catch
                { 
                    // Don't kill svn on a failed begin invoke
                    _canceling = true; // Cancel at the right time
                }
            }
        }              

        void RunQueue()
        {
            DoSomething[] actions;
            lock(_todo)
            {
                actions = _todo.ToArray();
                _todo.Clear();
            }

            int n = actionList.Items.Count;

            foreach (DoSomething ds in actions)
            {
                ds();
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
            this.args.SetCancelled( true );
            this.cancelButton.Text = "Cancelling...";
            this.cancelButton.Enabled = false;
        }
        
        private ProgressStatusEventArgs args = new ProgressStatusEventArgs();        
    }

    /// <summary>
    /// An event args class used by the ProgressDialog.ProgressStatus event.
    /// </summary>
    public class ProgressStatusEventArgs : EventArgs
    {
        /// <summary>
        /// Event handlers can set this to true if the operation is finished.
        /// </summary>
        public bool Done
        {
            get{ return this.done; }
            set{ this.done = value; }
        }

        /// <summary>
        /// The dialog uses this to indicate that the user has clicked 
        /// Cancel. Event handlers should detect this and attempt to 
        /// cancel the ongoing operation.
        /// </summary>
        public bool Cancelled
        {
            get{ return this.cancelled; }
        }
        
        internal void SetCancelled( bool val )
        {
            this.cancelled = val; 
        }

        private bool done;
        private bool cancelled;
    }
}
