using System;
using System.Collections.Generic;

namespace ErrorReportExtractor
{
    public interface IStorage : IService
    {
        void Store( IEnumerable<IErrorReport> items );

        IEnumerable<IErrorReport> GetAllReports();

        void AnswerReport( IMailItem iErrorReport, string replyText );

        void StorePotentialReplies( IEnumerable<IMailItem> items );

        void GetReplies( IErrorReport report );

        void UpdateMailItem( IMailItem item );

        IEnumerable<IMailItem> GetAllItems();
    }
}
