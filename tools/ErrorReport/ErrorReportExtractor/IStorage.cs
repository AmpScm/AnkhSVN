using System;
using System.Collections.Generic;

namespace ErrorReportExtractor
{
    public interface IStorage
    {
        void Store( IEnumerable<IErrorReport> items );

        IEnumerable<IErrorReport> GetAllReports();

        void AnswerReport( IErrorReport iErrorReport, string replyText );
    }
}
