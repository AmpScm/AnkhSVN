using System;
using System.Collections.Generic;
using System.Text;
using SharpSvn;

namespace Ankh.UI.SvnLog
{
    class LogRequest
    {
        bool _cancel;
        LogRequest(SvnClientArgs args)
        {
            args.Cancel += new EventHandler<SvnCancelEventArgs>(OnLogCancel);
        }

        public LogRequest(SvnLogArgs args, EventHandler<SvnLoggingEventArgs> receivedItem)
            : this(args)
        {
            args.Log += ReceiveItem;
            ReceivedItem += receivedItem;
        }

        public LogRequest(SvnMergesMergedArgs args, EventHandler<SvnLoggingEventArgs> receivedItem)
            : this(args)
        {
            args.MergesMerged += ReceiveItem;
            ReceivedItem += receivedItem;
        }

        public LogRequest(SvnMergesEligibleArgs args, EventHandler<SvnLoggingEventArgs> receivedItem)
            : this(args)
        {
            args.MergesEligible += ReceiveItem;
            ReceivedItem += receivedItem;
        }

        public event EventHandler<SvnLoggingEventArgs> ReceivedItem;

        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
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
            else
                e.Cancel = true;
        }

        void ReceiveItem(object sender, SvnMergesMergedEventArgs e)
        {
            if (!_cancel)
                OnReceivedItem(e);
            else
                e.Cancel = true;
        }

        void ReceiveItem(object sender, SvnMergesEligibleEventArgs e)
        {
            if (!_cancel)
                OnReceivedItem(e);
            else
                e.Cancel = true;
        }

        void OnReceivedItem(SvnLoggingEventArgs e)
        {
            if (!_cancel && ReceivedItem != null)
                ReceivedItem(this, e);
        }
    }
}
