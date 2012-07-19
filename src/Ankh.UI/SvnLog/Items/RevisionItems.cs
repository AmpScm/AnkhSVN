using System;
using System.Collections.Generic;
using System.Text;

namespace Ankh.UI.SvnLog
{
    public sealed class BatchFinishedEventArgs : EventArgs
    {
        readonly int _batchCount;
        readonly int _totalCount;
        readonly LogRequest _rq;
        internal BatchFinishedEventArgs(LogRequest rq, int totalCount, int batchCount)
        {
            _rq = rq;
            _totalCount = totalCount;
            _batchCount = batchCount;
        }

        public int TotalCount
        {
            get { return _totalCount; }
        }

        public int BatchCount
        {
            get { return _batchCount; }
        }

        internal LogRequest Request
        {
            get { return _rq; }
        }
    }

    public enum LogMode
    {
        Log,
        MergesEligible,
        MergesMerged
    }
}
